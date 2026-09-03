using CSharpMastery.FraudEngine.Services;

namespace CSharpMastery.FraudEngine.Distributed;

public sealed class GrpcFraudStreamService
{
    private readonly LockFreeBloomFilter _bloomFilter;

    public GrpcFraudStreamService(LockFreeBloomFilter bloomFilter)
    {
        _bloomFilter = bloomFilter;
    }

    // Simulates an inbound streaming RPC endpoint receiving network packets
    public async ValueTask ProcessDistributedStreamAsync(IAsyncEnumerable<ReadOnlyMemory<byte>> stream)
    {
        await foreach (var packet in stream)
        {
            ReadOnlySpan<byte> span = packet.Span;

            // Zero-copy binary field parsing directly off network buffer
            if (UnsafeBinaryParser.TryParseBinaryPayload(span, out var tx))
            {
                // Instant local filter check before hitting distributed network storage
                ReadOnlySpan<byte> txIdBytes = tx.TransactionId.ToByteArray();
                bool potentialDuplicate = _bloomFilter.AddAndCheck(txIdBytes);

                if (potentialDuplicate)
                {
                    Console.WriteLine($"  [gRPC Stream] Node Flagged Potential Duplicate: {tx.TransactionId}");
                }
            }
        }
    }
}