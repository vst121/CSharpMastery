using System.IO.Hashing;

namespace CSharpMastery.FraudEngine.Services;

public sealed class LockFreeBloomFilter
{
    private readonly long[] _bitArray;
    private readonly int _bitSize;

    public LockFreeBloomFilter(int capacity = 100_000)
    {
        _bitSize = capacity * 10; // 10 bits per item (~1% false positive rate)
        _bitArray = new long[(_bitSize + 63) / 64];
    }

    public bool AddAndCheck(ReadOnlySpan<byte> data)
    {
        ulong hash1 = XxHash3.HashToUInt64(data, seed: 0x12345678);
        ulong hash2 = XxHash3.HashToUInt64(data, seed: 0x87654321);

        bool allBitsWereSet = true;

        for (int i = 0; i < 3; i++)
        {
            int bitIndex = (int)((hash1 + (ulong)i * hash2) % (ulong)_bitSize);
            int wordIndex = bitIndex >> 6;
            long bitMask = 1L << (bitIndex & 63);

            // Atomic bitwise OR operation across threads
            long initialWord = Interlocked.Or(ref _bitArray[wordIndex], bitMask);

            if ((initialWord & bitMask) == 0)
            {
                allBitsWereSet = false; // At least one bit was newly set
            }
        }

        return allBitsWereSet; // If all bits were already set, it's a potential duplicate
    }
}