using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ModernCSharpMastery.FraudEngine;

public sealed class FraudProcessingPipeline<TAmount> where TAmount : struct, INumber<TAmount>
{
    private readonly Channel<Transaction<TAmount>> _channel;
    private readonly ConcurrentDictionary<Guid, byte> _seenTransactions = new();

    public FraudProcessingPipeline(int capacity = 1000)
    {
        // Bounded channel enforces backpressure when ingress exceeds processing capacity
        var options = new BoundedChannelOptions(capacity)
        {
            SingleWriter = false,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<Transaction<TAmount>>(options);
    }

    // High-performance ingress method using ValueTask to eliminate task allocations
    public ValueTask<bool> PublishAsync(Transaction<TAmount> transaction, CancellationToken ct = default)
    {
        // Fast-path inline duplication check
        if (_seenTransactions.ContainsKey(transaction.TransactionId))
        {
            return ValueTask.FromResult(false); // Duplicate detected before queueing
        }

        if (_channel.Writer.TryWrite(transaction))
        {
            _seenTransactions.TryAdd(transaction.TransactionId, 0);
            return ValueTask.FromResult(true);
        }

        return PublishSlowAsync(transaction, ct);
    }

    private async ValueTask<bool> PublishSlowAsync(Transaction<TAmount> transaction, CancellationToken ct)
    {
        await _channel.Writer.WriteAsync(transaction, ct);
        _seenTransactions.TryAdd(transaction.TransactionId, 0);
        return true;
    }

    // Worker Engine: Concurrent Consumers pulling from Channel
    public async Task StartProcessingWorkersAsync(int workerCount, Func<Transaction<TAmount>, ValueTask> onFraudDetected, CancellationToken ct)
    {
        var workers = new Task[workerCount];
        for (int i = 0; i < workerCount; i++)
        {
            workers[i] = Task.Run(() => ConsumeAsync(_channel.Reader, onFraudDetected, ct), ct);
        }

        await Task.WhenAll(workers);
    }

    private static async Task ConsumeAsync(
        ChannelReader<Transaction<TAmount>> reader,
        Func<Transaction<TAmount>, ValueTask> onFraudDetected,
        CancellationToken ct)
    {
        // ReadAllAsync natively optimizes enumerator allocations in modern .NET
        await foreach (var tx in reader.ReadAllAsync(ct))
        {
            // Evaluate Fraud Rule: High Value Threshold
            if (tx.Amount > TAmount.CreateChecked(10000))
            {
                await onFraudDetected(tx);
            }
        }
    }

    public void CompleteIngress() => _channel.Writer.Complete();
}