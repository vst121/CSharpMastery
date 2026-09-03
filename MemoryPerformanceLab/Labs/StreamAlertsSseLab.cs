using CSharpMastery.FraudEngine.Models;
using System.Threading.Channels;

namespace CSharpMastery.FraudEngine.Labs;

public static class StreamAlertsSseLab
{
    public static async Task RunAsync()
    {
        Console.WriteLine("--- [LAB 6] Native Server-Sent Event (SSE) Stream ---");

        var channel = Channel.CreateUnbounded<Transaction<decimal>>();
        using var cts = new CancellationTokenSource();

        // Producer: Publish high-risk fraud alerts
        _ = Task.Run(async () =>
        {
            await channel.Writer.WriteAsync(new Transaction<decimal>(Guid.NewGuid(), 9001, 45000.00m, DateTime.UtcNow.Ticks));
            await Task.Delay(100);
            await channel.Writer.WriteAsync(new Transaction<decimal>(Guid.NewGuid(), 9002, 120000.00m, DateTime.UtcNow.Ticks));
            channel.Writer.Complete();
        });

        // Consumer: Stream via IAsyncEnumerable (matching TypedResults.ServerSentEvents pattern)
        Console.WriteLine("Streaming incoming SSE events to client:");
        await foreach (var sseMessage in StreamSseEventsAsync(channel.Reader, cts.Token))
        {
            Console.WriteLine($"  {sseMessage}");
        }
    }

    private static async IAsyncEnumerable<string> StreamSseEventsAsync(
        ChannelReader<Transaction<decimal>> reader,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (var alert in reader.ReadAllAsync(ct))
        {
            yield return $"event: fraud-alert | id: {alert.TransactionId} | data: {{\"account\":{alert.AccountId},\"amount\":{alert.Amount}}}";
        }
    }
}