using CSharpMastery.FraudEngine.Models;
using System.Collections.Concurrent;

namespace CSharpMastery.FraudEngine.Distributed;

public readonly record struct OutboxMessage(
    Guid MessageId,
    string Topic,
    string PayloadJson,
    DateTime CreatedAtUtc
);

public sealed class DistributedOutbox
{
    private readonly ConcurrentQueue<OutboxMessage> _pendingMessages = new();

    public void SaveToOutbox(Transaction<decimal> tx, string topic)
    {
        var message = new OutboxMessage(
            Guid.NewGuid(),
            topic,
            $"{{\"id\":\"{tx.TransactionId}\",\"account\":{tx.AccountId},\"amount\":{tx.Amount}}}",
            DateTime.UtcNow
        );

        _pendingMessages.Enqueue(message);
    }

    public async ValueTask DispatchPendingMessagesAsync(Func<OutboxMessage, ValueTask> brokerPublisher)
    {
        while (_pendingMessages.TryDequeue(out var message))
        {
            // Reliable delivery to network broker
            await brokerPublisher(message);
        }
    }
}