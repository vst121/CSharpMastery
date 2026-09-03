using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ModernCSharpMastery.FraudEngine.Models;

/// <summary>
/// Stack-only ref struct ensuring zero heap allocations during payload parsing and validation.
/// </summary>
public ref struct ValidationContext<TAmount> where TAmount : struct, INumber<TAmount>
{
    public ReadOnlySpan<byte> RawPayload { get; }
    public Transaction<TAmount> ParsedTransaction { get; private set; }
    public bool IsValid { get; private set; }

    public ValidationContext(ReadOnlySpan<byte> rawPayload)
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