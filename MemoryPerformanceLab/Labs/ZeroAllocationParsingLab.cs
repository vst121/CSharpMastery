using CSharpMastery.FraudEngine.Models;
using CSharpMastery.FraudEngine.Services;

namespace CSharpMastery.FraudEngine.Labs;

public static class ZeroAllocationParsingLab
{
    public static void Run()
    {
        Console.WriteLine("--- [LAB 1] Zero-Allocation UTF-8 Payload Parsing ---");

        ReadOnlySpan<byte> rawJsonPayload = """
        {
            "id": "a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11",
            "account": 987654321,
            "amount": 25000.75
        }
        """u8;

        var context = new ValidationContext<decimal>(rawJsonPayload);
        bool success = Utf8TransactionParser.TryParseUtf8Payload(rawJsonPayload, ref context);

        if (success && context.IsValid)
        {
            var tx = context.ParsedTransaction;
            Console.WriteLine($"[SUCCESS] Parsed Transaction:");
            Console.WriteLine($"  ├─ ID:      {tx.TransactionId}");
            Console.WriteLine($"  ├─ Account: {tx.AccountId}");
            Console.WriteLine($"  └─ Amount:  ${tx.Amount:N2}");
        }
        else
        {
            Console.WriteLine("[FAILED] Payload parsing failed.");
        }
    }
}