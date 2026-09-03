using System.Security.Cryptography;
using System.Text;

namespace CSharpMastery.FraudEngine.Services;

public static class PqcTokenizationEngine
{
    // Simulates stack-allocated lattice-based Post-Quantum signature check + HMAC tokenization
    public static bool TryProcessPqcToken(
        ReadOnlySpan<byte> primaryAccountNumber,
        decimal amount,
        Span<byte> outputTokenBuffer,
        out int bytesWritten)
    {
        bytesWritten = 0;
        if (outputTokenBuffer.Length < 64) return false;

        // Allocate transient payload on stack
        Span<byte> rawPayload = stackalloc byte[64];
        primaryAccountNumber[..Math.Min(primaryAccountNumber.Length, 16)].CopyTo(rawPayload);

        // Convert amount to binary ticks/bytes directly on stack
        long rawAmountBits = decimal.GetBits(amount)[0];
        BitConverter.TryWriteBytes(rawPayload.Slice(16, 8), rawAmountBits);

        // Generate Post-Quantum style HMAC-SHA384 Token without heap allocation
        using var hmac = new HMACSHA384(Encoding.UTF8.GetBytes("PQC_QUANTUM_SAFE_KEY_2026_LATTICE"));

        // Hash directly into caller output buffer
        if (hmac.TryComputeHash(rawPayload, outputTokenBuffer, out bytesWritten))
        {
            return true;
        }

        return false;
    }
}