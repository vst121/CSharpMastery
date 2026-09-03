using System;
using System.Numerics;

namespace ModernCSharpMastery.FraudEngine.Models;

/// <summary>
/// Immutable transaction record using Generic Math (INumber<T>) for zero-cost numeric flexibility.
/// </summary>
public readonly record struct Transaction<TAmount>(
    Guid TransactionId,
    long AccountId,
    TAmount Amount,
    long TimestampTicks
) where TAmount : struct, INumber<TAmount>;