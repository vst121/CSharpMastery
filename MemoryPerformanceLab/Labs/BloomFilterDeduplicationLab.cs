using System;
using ModernCSharpMastery.FraudEngine.Services;

namespace ModernCSharpMastery.FraudEngine.Labs;

public static class BloomFilterDeduplicationLab
{
    public static void Run()
    {
        Console.WriteLine("--- [LAB 9] Lock-Free Bloom Filter Deduplication ---");

        var bloomFilter = new LockFreeBloomFilter(capacity: 50_000);
        ReadOnlySpan<byte> account1 = "ACCOUNT_987654"u8;
        ReadOnlySpan<byte> account2 = "ACCOUNT_123456"u8;

        bool isDup1 = bloomFilter.AddAndCheck(account1);
        bool isDup1Repeat = bloomFilter.AddAndCheck(account1); // Re-insert
        bool isDup2 = bloomFilter.AddAndCheck(account2);

        Console.WriteLine($"Account 987654 First Check:  {(isDup1 ? "DUPLICATE" : "NEW")}");
        Console.WriteLine($"Account 987654 Second Check: {(isDup1Repeat ? "DUPLICATE (Bloom Match)" : "NEW")}");
        Console.WriteLine($"Account 123456 First Check:  {(isDup2 ? "DUPLICATE" : "NEW")}");
    }
}