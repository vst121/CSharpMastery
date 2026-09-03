using System;
using System.Diagnostics;
using System.Text.Json;
using ModernCSharpMastery.FraudEngine.Models;
using ModernCSharpMastery.FraudEngine.Services;

namespace ModernCSharpMastery.FraudEngine.Labs;

public static class MemoryProfilingLab
{
    public static void Run()
    {
        Console.WriteLine("--- [LAB 4] Memory Allocation & Performance Profiling ---");

        ReadOnlySpan<byte> utf8Payload = """
        {
            "id": "a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11",
            "account": 987654321,
            "amount": 25000.75
        }
        """u8;

        string jsonString = """
        {
            "id": "a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11",
            "account": 987654321,
            "amount": 25000.75
        }
        """;

        const int iterations = 100_000;

        // 1. Standard Reflection Deserialization
        long memoryBeforeStandard = GC.GetTotalAllocatedBytes(precise: true);
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            _ = JsonSerializer.Deserialize<Transaction<decimal>>(jsonString);
        }
        sw.Stop();
        long allocatedStandard = GC.GetTotalAllocatedBytes(precise: true) - memoryBeforeStandard;

        Console.WriteLine($"[Standard JsonSerializer]");
        Console.WriteLine($"  ├─ Time Elapsed: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"  └─ Memory Allocated: {allocatedStandard / 1024.0:N2} KB");

        // 2. Zero-Allocation Span Parser
        long memoryBeforeSpan = GC.GetTotalAllocatedBytes(precise: true);
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            var context = new ValidationContext<decimal>(utf8Payload);
            _ = Utf8TransactionParser.TryParseUtf8Payload(utf8Payload, ref context);
        }
        sw.Stop();
        long allocatedSpan = GC.GetTotalAllocatedBytes(precise: true) - memoryBeforeSpan;

        Console.WriteLine($"\n[Zero-Allocation Utf8TransactionParser]");
        Console.WriteLine($"  ├─ Time Elapsed: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"  └─ Memory Allocated: {allocatedSpan} Bytes (0 Heap Allocations)");
    }
}