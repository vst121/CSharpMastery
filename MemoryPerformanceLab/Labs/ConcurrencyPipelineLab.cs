using CSharpMastery.FraudEngine.Models;
using CSharpMastery.FraudEngine.Services;

namespace CSharpMastery.FraudEngine.Labs;

public static class ConcurrencyPipelineLab
{
    public static async Task RunAsync()
    {
        Console.WriteLine("--- [LAB 3] Bounded Channels & ValueTask Pipeline ---");

        var pipeline = new FraudProcessingPipeline<decimal>(capacity: 100);
        using var cts = new CancellationTokenSource();

        // Start processing engine
        var workerTask = pipeline.StartProcessingWorkersAsync(
            workerCount: Environment.ProcessorCount,
            onFraudDetected: async (tx) =>
            {
                Console.WriteLine($"  [ALERT] High-Value Transaction: {tx.TransactionId} | ${tx.Amount:N2}");
                await ValueTask.CompletedTask;
            },
            cts.Token
        );

        // Publish events
        var sharedId = Guid.NewGuid();
        var tx1 = new Transaction<decimal>(sharedId, 1001, 15000.50m, DateTime.UtcNow.Ticks);
        var txDuplicate = new Transaction<decimal>(sharedId, 1001, 15000.50m, DateTime.UtcNow.Ticks);

        Console.WriteLine($"Publishing Tx 1:           Published = {await pipeline.PublishAsync(tx1)}");
        Console.WriteLine($"Publishing Duplicate Tx:   Published = {await pipeline.PublishAsync(txDuplicate)} (Deduplicated)");

        pipeline.CompleteIngress();
        await workerTask;
    }
}