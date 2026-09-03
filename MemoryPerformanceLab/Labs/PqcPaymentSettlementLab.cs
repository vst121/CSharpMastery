using CSharpMastery.FraudEngine.Services;

namespace CSharpMastery.FraudEngine.Labs;

public static class PqcPaymentSettlementLab
{
    public static void Run()
    {
        Console.WriteLine("--- [LAB 14] Post-Quantum Tokenized Payment Settlement ---");

        ReadOnlySpan<byte> pan = "4532015589001234"u8; // Sensitive Card Number
        decimal settlementAmount = 245000.75m;

        // Buffer allocated entirely on the CPU stack - 0 Bytes heap garbage
        Span<byte> pqcTokenBuffer = stackalloc byte[64];

        bool tokenized = PqcTokenizationEngine.TryProcessPqcToken(
            pan,
            settlementAmount,
            pqcTokenBuffer,
            out int bytesWritten);

        Console.WriteLine($"Tokenization Execution Success: {tokenized}");
        Console.WriteLine($"Token Byte Length:             {bytesWritten} bytes");
        Console.WriteLine($"Stack Token Hash (Hex):        {Convert.ToHexString(pqcTokenBuffer[..bytesWritten])}");
        Console.WriteLine("  └─ Status: Zero-Heap Allocation | Post-Quantum Safe | ISO 20022 Ready");
    }
}