using System;
using ModernCSharpMastery.FraudEngine.Models;

namespace ModernCSharpMastery.FraudEngine.Labs;

public static class CompileTimeValidationLab
{
    public static void Run()
    {
        Console.WriteLine("--- [LAB 5] Fast Inlined Fraud Rule Validation ---");

        var highRiskTx = new Transaction<decimal>(Guid.NewGuid(), 1001, 50000.00m, DateTime.UtcNow.Ticks);
        var lowRiskTx = new Transaction<decimal>(Guid.NewGuid(), 1002, 150.00m, DateTime.UtcNow.Ticks);

        bool isHighRisk = FastFraudValidator.IsHighRisk(highRiskTx);
        bool isLowRisk = FastFraudValidator.IsHighRisk(lowRiskTx);

        Console.WriteLine($"Transaction ${highRiskTx.Amount:N2} -> High Risk Flagged: {isHighRisk}");
        Console.WriteLine($"Transaction ${lowRiskTx.Amount:N2}  -> High Risk Flagged: {isLowRisk}");
    }
}

// Inlined zero-reflection validation rules
public static class FastFraudValidator
{
    private static readonly decimal Threshold = 10000.00m;

    public static bool IsHighRisk(in Transaction<decimal> tx) => tx.Amount > Threshold;
}