using System;
using System.Threading;
using System.Threading.Tasks;
using ModernCSharpMastery.FraudEngine;

Console.WriteLine("=== High-Performance Fraud & Duplication Engine ===\n");

var pipeline = new FraudProcessingPipeline<decimal>(capacity: 500);
using var cts = new CancellationTokenSource();

// 1. Spin up background workers
var processingTask = pipeline.StartProcessingWorkersAsync(
    workerCount: Environment.ProcessorCount,
    onFraudDetected: async (tx) =>
    {
        Console.WriteLine($"[ALERT - FRAUD DETECTED] High Value Tx: {tx.TransactionId} | Amount: ${tx.Amount}");
        await ValueTask.CompletedTask;
    },
    cts.Token
);

// 2. Simulating incoming payload streaming
var txId = Guid.NewGuid();
var tx1 = new Transaction<decimal>(txId, AccountId: 1001, Amount: 15000.50m, TimestampTicks: DateTime.UtcNow.Ticks);
var txDuplicate = new Transaction<decimal>(txId, AccountId: 1001, Amount: 15000.50m, TimestampTicks: DateTime.UtcNow.Ticks);
var txNormal = new Transaction<decimal>(Guid.NewGuid(), AccountId: 1002, Amount: 250.00m, TimestampTicks: DateTime.UtcNow.Ticks);

// 3. Publish transactions
Console.WriteLine($"Publishing Tx 1: {await pipeline.PublishAsync(tx1)}");
Console.WriteLine($"Publishing Tx 1 (Duplicate Check): {await pipeline.PublishAsync(txDuplicate)}"); // Returns false
Console.WriteLine($"Publishing Tx 2 (Normal): {await pipeline.PublishAsync(txNormal)}");

// Complete channel and wait for processing
pipeline.CompleteIngress();
await processingTask;

Console.WriteLine("\n=== Processing Completed Safely ===");