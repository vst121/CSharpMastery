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
    Console.WriteLine("  0. Exit\n");
    Console.Write("Enter choice [0-8]: ");

    var choice = Console.ReadLine()?.Trim();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            ZeroAllocationParsingLab.Run();
            break;
        case "2":
            VectorizedSimdLab.Run();
            break;
        case "3":
            await ConcurrencyPipelineLab.RunAsync();
            break;
        case "4":
            MemoryProfilingLab.Run();
            break;
        case "5":
            CompileTimeValidationLab.Run();
            break;
        case "6":
            await StreamAlertsSseLab.RunAsync();
            break;
        case "7":
            await ResilientApiLab.RunAsync();
            break;
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