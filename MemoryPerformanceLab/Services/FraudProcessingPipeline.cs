using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ModernCSharpMastery.FraudEngine.Models;

namespace ModernCSharpMastery.FraudEngine.Services;

public sealed class FraudProcessingPipeline<TAmount> where TAmount : struct, INumber<TAmount>
{
    private readonly Channel<Transaction<TAmount>> _channel;
    private readonly ConcurrentDictionary<Guid, byte> _seenTransactions = new();

    public FraudProcessingPipeline(int capacity = 1000)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            SingleWriter = false,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<Transaction<TAmount>>(options);
    }

    public ValueTask<bool> PublishAsync(Transaction<TAmount> transaction, CancellationToken ct = default)
    {
        if (_seenTransactions.ContainsKey(transaction.TransactionId))
        {
            return ValueTask.FromResult(false);
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
        await foreach (var tx in reader.ReadAllAsync(ct))
        {
            if (tx.Amount > TAmount.CreateChecked(10000))
            {
                await onFraudDetected(tx);
            }
        }
    }

    public void CompleteIngress() => _channel.Writer.Complete();
}