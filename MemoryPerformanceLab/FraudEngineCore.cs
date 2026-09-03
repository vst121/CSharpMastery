using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ModernCSharpMastery.FraudEngine;

// 1. Transaction Record using Generic Math for flexible currency/amount types
public readonly record struct Transaction<TAmount>(
    Guid TransactionId,
    long AccountId,
    TAmount Amount,
    long TimestampTicks
) where TAmount : struct, INumber<TAmount>;

// 2. Transient Validation Context (Stack-Only Ref Struct)
// Guarantees zero heap allocation during parsing and fraud-check preparation
public ref struct TransactionValidationContext<TAmount>
    where TAmount : struct, INumber<TAmount>
{
    public ReadOnlySpan<byte> RawPayload { get; }
    public Transaction<TAmount> ParsedTransaction { get; private set; }
    public bool IsValid { get; private set; }

    public TransactionValidationContext(ReadOnlySpan<byte> rawPayload)
    {
        RawPayload = rawPayload;
        ParsedTransaction = default;
        IsValid = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetParsed(Transaction<TAmount> transaction)
    {
        ParsedTransaction = transaction;
        IsValid = true;
    }
}

// 3. High-Performance Metric Aggregators using Generic Math & Spans
public static class TransactionAnalytics
{
    // Sums transaction amounts directly over a Span without boxing or heap allocations
    public static TAmount CalculateTotalVolume<TAmount>(ReadOnlySpan<Transaction<TAmount>> transactions)
        where TAmount : struct, INumber<TAmount>
    {
        TAmount total = TAmount.Zero;
        foreach (ref readonly var tx in transactions)
        {
            total += tx.Amount;
        }
        return total;
    }
}