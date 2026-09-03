using System;
using System.Threading.Tasks;
using ModernCSharpMastery.FraudEngine.Labs;

Console.WriteLine("==================================================");
Console.WriteLine("    .NET 10 HIGH-PERFORMANCE FRAUD ENGINE LABS    ");
Console.WriteLine("==================================================\n");

while (true)
{
    Console.WriteLine("Select a lab to execute:");
    Console.WriteLine("  1. Zero-Allocation UTF-8 Parsing (Span<byte> / Utf8JsonReader)");
    Console.WriteLine("  2. SIMD Vectorized Batch Duplicate Scan (Vector128<T>)");
    Console.WriteLine("  3. High-Throughput Concurrent Pipeline (Channels / ValueTask)");
    Console.WriteLine("  4. Memory Allocation Profiling (Utf8Parser vs JsonSerializer)");
    Console.WriteLine("  5. Compile-Time Fast Validation (Generated Validator)");
    Console.WriteLine("  6. Live Fraud Alert Streaming (Server-Sent Events / IAsyncEnumerable)");
    Console.WriteLine("  7. Resilient External Risk API Calls (Retry & Backoff)");
    Console.WriteLine("  8. Unsafe Zero-Copy Binary Field Parsing (MemoryMarshal)");
    Console.WriteLine("  9. Lock-Free Bloom Filter Deduplication (XxHash3)");
    Console.WriteLine(" 10. Native AOT & Zero-JIT Inspection");
    Console.WriteLine(" 11. Distributed gRPC Network Stream Processing");
    Console.WriteLine(" 12. Transactional Outbox Pattern for Distributed Messaging");
    Console.WriteLine(" 13. Post-Quantum Tokenized Settlement (Stack-Allocated PQC HMAC)");
    Console.WriteLine("  0. Exit\n");
    Console.Write("Enter choice [0-12]: ");

    var choice = Console.ReadLine()?.Trim();
    Console.WriteLine();

    switch (choice)
    {
        case "1": ZeroAllocationParsingLab.Run(); break;
        case "2": VectorizedSimdLab.Run(); break;
        case "3": await ConcurrencyPipelineLab.RunAsync(); break;
        case "4": MemoryProfilingLab.Run(); break;
        case "5": CompileTimeValidationLab.Run(); break;
        case "6": await StreamAlertsSseLab.RunAsync(); break;
        case "7": await ResilientApiLab.RunAsync(); break;
        case "8": UnsafeZeroCopyLab.Run(); break;
        case "9": BloomFilterDeduplicationLab.Run(); break;
        case "10": NativeAotVerificationLab.Run(); break;
        case "11": await GrpcStreamingLab.RunAsync(); break;
        case "12": await DistributedOutboxLab.RunAsync(); break;
        case "13": PqcPaymentSettlementLab.Run(); break;

        case "0":
            Console.WriteLine("Exiting engine. Goodbye!");
            return;
        default:
            Console.WriteLine("Invalid option. Try again.");
            break;
    }

    Console.WriteLine("\nPress Enter to return to menu...");
    Console.ReadLine();
    Console.Clear();
}