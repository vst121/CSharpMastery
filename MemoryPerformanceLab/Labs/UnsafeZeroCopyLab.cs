using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ModernCSharpMastery.FraudEngine.Services;

namespace ModernCSharpMastery.FraudEngine.Labs;

public static class UnsafeZeroCopyLab
{
    public static void Run()
    {
        Console.WriteLine("--- [LAB 8] Unsafe Zero-Copy Binary Field Extraction ---");

        Span<byte> binaryBuffer = stackalloc byte[40];
        Guid sampleId = Guid.NewGuid();
        long accountId = 987654321L;
        decimal amount = 85000.50m;

        // Write binary memory directly
        MemoryMarshal.Write(binaryBuffer[..16], in sampleId);
        MemoryMarshal.Write(binaryBuffer[16..24], in accountId);
        MemoryMarshal.Write(binaryBuffer[24..40], in amount);

        bool parsed = UnsafeBinaryParser.TryParseBinaryPayload(binaryBuffer, out var tx);

        Console.WriteLine($"[Zero-Copy Parse] Success: {parsed}");
        Console.WriteLine($"  ├─ Transaction ID: {tx.TransactionId}");
        Console.WriteLine($"  ├─ Account ID:     {tx.AccountId}");
        Console.WriteLine($"  └─ Amount:         ${tx.Amount:N2}");
    }
}