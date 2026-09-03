(# Modern C# Mastery — .NET 10 High-Performance Fraud Engine Labs)

A collection of focused labs that demonstrate high-performance patterns in modern C# and .NET 10, built around a small fraud-detection engine. Each lab is a self-contained experiment showcasing techniques such as zero-allocation parsing, SIMD/vectorization, lock-free data structures, zero-copy binary parsing, high-throughput pipelines, and distributed messaging patterns.

## Features

- Small, focused labs exercised from a single launcher in [MemoryPerformanceLab/Program.cs](MemoryPerformanceLab/Program.cs).
- Zero-heap JSON parsing using `Utf8JsonReader` and `ReadOnlySpan<byte>`.
- SIMD-accelerated scanning via `Vector128<T>` (`VectorizedDetector`).
- Lock-free Bloom filter deduplication using `XxHash3` and atomic operations (`LockFreeBloomFilter`).
- Unsafe, zero-copy binary parsing with `MemoryMarshal` and `Unsafe` (`UnsafeBinaryParser`).
- High-throughput producer/consumer pipelines using `System.Threading.Channels` (`FraudProcessingPipeline`).
- Simple distributed patterns: transactional outbox and simulated gRPC streaming (`DistributedOutbox`, `GrpcFraudStreamService`).

## Prerequisites

- .NET 10 SDK (target framework: `net10.0`).

## Build

From the repository root:

```bash
dotnet build MemoryPerformanceLab/MemoryPerformanceLab.csproj -c Release
```

## Run

Run the interactive launcher which presents the available labs:

```bash
dotnet run --project MemoryPerformanceLab/MemoryPerformanceLab.csproj
```

The console app displays a numbered menu. Choose a lab to run (for example, `1` for zero-allocation UTF-8 parsing).

## Labs (quick summary)

The main launcher enumerates these experiments (see [MemoryPerformanceLab/Program.cs](MemoryPerformanceLab/Program.cs)):

- `Zero-Allocation UTF-8 Parsing` — Parse JSON directly from UTF-8 spans with zero heap allocations (`Utf8TransactionParser`).
- `SIMD Vectorized Batch Duplicate Scan` — Use hardware vector intrinsics to scan arrays in parallel (`VectorizedDetector`).
- `High-Throughput Concurrent Pipeline` — Producer/consumer channels with `ValueTask` workers (`FraudProcessingPipeline`).
- `Memory Allocation Profiling` — Compare allocation characteristics of different parsers/serializers.
- `Compile-Time Fast Validation` — Generated/fast validation patterns.
- `Live Fraud Alert Streaming` — Server-Sent Events / IAsyncEnumerable streaming example.
- `Resilient External Risk API Calls` — Retry and backoff strategies for external calls.
- `Unsafe Zero-Copy Binary Field Parsing` — Read binary payloads without copies using `Unsafe` (`UnsafeBinaryParser`).
- `Lock-Free Bloom Filter Deduplication` — Fast, concurrent deduplication using atomic bit ops (`LockFreeBloomFilter`).
- `Native AOT & Zero-JIT Inspection` — Native AOT related checks.
- `Distributed gRPC Network Stream Processing` — Simulated inbound gRPC stream handling (`GrpcFraudStreamService`).
- `Transactional Outbox Pattern` — Enqueue and dispatch reliably to a broker (`DistributedOutbox`).
- `Post-Quantum Tokenized Settlement` — Stack-allocated PQC HMAC example.

## Key files & components

- Launcher: [MemoryPerformanceLab/Program.cs](MemoryPerformanceLab/Program.cs)
- Project file: [MemoryPerformanceLab/MemoryPerformanceLab.csproj](MemoryPerformanceLab/MemoryPerformanceLab.csproj)
- Services: [MemoryPerformanceLab/Services](MemoryPerformanceLab/Services)
  - [LockFreeBloomFilter.cs](MemoryPerformanceLab/Services/LockFreeBloomFilter.cs)
  - [Utf8TransactionParser.cs](MemoryPerformanceLab/Services/Utf8TransactionParser.cs)
  - [VectorizedDetector.cs](MemoryPerformanceLab/Services/VectorizedDetector.cs)
  - [UnsafeBinaryParser.cs](MemoryPerformanceLab/Services/UnsafeBinaryParser.cs)
  - [FraudProcessingPipeline.cs](MemoryPerformanceLab/Services/FraudProcessingPipeline.cs)
  - [DistributedOutbox.cs](MemoryPerformanceLab/Services/DistributedOutbox.cs)
  - [GrpcFraudStreamService.cs](MemoryPerformanceLab/Services/GrpcFraudStreamService.cs)

## Design notes

- The code is intentionally small and educational — focus is on demonstrating micro-optimizations and concurrency patterns rather than production-ready glue code.
- Many examples use low-level APIs (`Unsafe`, `MemoryMarshal`, intrinsics) to illustrate trade-offs; treat these as learning artifacts and verify safety for production use.

## Suggested next steps

- Run individual labs and profile with the dotnet-trace / dotnet-dump tools to see allocations and hotspots.
- Add unit tests that validate parsing correctness and pipeline behavior under concurrency.
- Expand README with per-lab guidance, inputs, and expected outputs if you want reproducible experiments.

## Contribution

Contributions and improvements are welcome. Open an issue or PR describing the change and which lab it affects.

---

_Generated README based on the launcher and service implementations in `MemoryPerformanceLab`._
