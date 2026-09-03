using CSharpMastery.FraudEngine.Services;

namespace CSharpMastery.FraudEngine.Labs;

public static class VectorizedSimdLab
{
    public static void Run()
    {
        Console.WriteLine("--- [LAB 2] SIMD Hardware-Accelerated Duplicate Scan ---");

        ReadOnlySpan<long> activeBatchHashes = [100234L, 500123L, 987654321L, 112233L, 445566L, 778899L];
        long targetAccountId = 987654321L;

        bool found = VectorizedDetector.ContainsHashVectorized(activeBatchHashes, targetAccountId);

        Console.WriteLine($"Scanning batch of {activeBatchHashes.Length} items using Vector128 registers...");
        Console.WriteLine($" -> Target Account {targetAccountId} Duplicate Status: {(found ? "MATCH FOUND" : "CLEAR")}");
    }
}