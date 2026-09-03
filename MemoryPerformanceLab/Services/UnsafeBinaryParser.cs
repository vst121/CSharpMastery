using CSharpMastery.FraudEngine.Models;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CSharpMastery.FraudEngine.Services;

public static class UnsafeBinaryParser
{
    // Binary Layout: [16 bytes Guid][8 bytes AccountId][16 bytes Decimal Amount] = 40 Bytes total
    public static bool TryParseBinaryPayload(ReadOnlySpan<byte> rawBytes, out Transaction<decimal> transaction)
    {
        transaction = default;
        if (rawBytes.Length < 40) return false;

        ref byte byteRef = ref MemoryMarshal.GetReference(rawBytes);

        // Read Guid (16 bytes) directly from pointer
        Guid id = Unsafe.ReadUnaligned<Guid>(ref byteRef);

        // Read AccountId (8 bytes) at offset 16
        long accountId = Unsafe.ReadUnaligned<long>(ref Unsafe.Add(ref byteRef, 16));

        // Read Amount (16 bytes decimal) at offset 24
        decimal amount = Unsafe.ReadUnaligned<decimal>(ref Unsafe.Add(ref byteRef, 24));

        transaction = new Transaction<decimal>(id, accountId, amount, DateTime.UtcNow.Ticks);
        return true;
    }
}