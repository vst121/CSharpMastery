using CSharpMastery.FraudEngine.Distributed;
using CSharpMastery.FraudEngine.Services;
using System.Runtime.InteropServices;

namespace CSharpMastery.FraudEngine.Labs;

public static class GrpcStreamingLab
{
    public static async Task RunAsync()
    {
        Console.WriteLine("--- [LAB 12] Distributed gRPC High-Throughput Stream ---");

        var bloomFilter = new LockFreeBloomFilter(capacity: 10_000);
        var grpcService = new GrpcFraudStreamService(bloomFilter);

        // Generate synthetic network payload stream
        static async IAsyncEnumerable<ReadOnlyMemory<byte>> GenerateNetworkPackets()
        {
            byte[] buffer = new byte[40];
            Guid id = Guid.NewGuid();

            // Packet 1
            id.TryWriteBytes(buffer.AsSpan(0, 16));
            BitConverter.TryWriteBytes(buffer.AsSpan(16, 8), 987654321L);
            decimal amount = 15000.00m;
            decimal.GetBits(amount).AsSpan().CopyTo(MemoryMarshal.Cast<byte, int>(buffer.AsSpan(24, 16)));
            yield return buffer;

            await Task.Delay(10);

            // Packet 2 (Duplicate ID)
            yield return buffer;
        }

        await grpcService.ProcessDistributedStreamAsync(GenerateNetworkPackets());
    }
}