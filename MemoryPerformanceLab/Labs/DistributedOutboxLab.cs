using System;
using System.Threading.Tasks;
using ModernCSharpMastery.FraudEngine.Distributed;
using ModernCSharpMastery.FraudEngine.Models;

namespace ModernCSharpMastery.FraudEngine.Labs;

public static class DistributedOutboxLab
{
    public static async Task RunAsync()
    {
        Console.WriteLine("--- [LAB 13] Transactional Outbox Pattern for Distributed Messaging ---");

        var outbox = new DistributedOutbox();
        var tx = new Transaction<decimal>(Guid.NewGuid(), 880011, 35000.00m, DateTime.UtcNow.Ticks);

        // 1. Stage locally in outbox
        outbox.SaveToOutbox(tx, topic: "fraud.alerts.high-risk");
        Console.WriteLine($"Staged transaction {tx.TransactionId} into Outbox queue.");

        // 2. Background worker flushes to network broker
        Console.WriteLine("Dispatching outbox messages to remote message broker:");
        await outbox.DispatchPendingMessagesAsync(async (msg) =>
        {
            Console.WriteLine($"  [PUBLISHED to {msg.Topic}] Id: {msg.MessageId} | Payload: {msg.PayloadJson}");
            await ValueTask.CompletedTask;
        });
    }
}