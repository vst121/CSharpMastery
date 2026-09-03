using CSharpMastery.FraudEngine.Models;
using System.Numerics;
using System.Text.Json;

namespace CSharpMastery.FraudEngine.Services;

public static class Utf8TransactionParser
{
    /// <summary>
    /// Parses a JSON payload directly from a UTF-8 ReadOnlySpan<byte> with ZERO heap allocations.
    /// Expected format: {"id":"00000000-0000-0000-0000-000000000000","account":1001,"amount":15000.50}
    /// </summary>
    public static bool TryParseUtf8Payload<TAmount>(
        ReadOnlySpan<byte> utf8Json,
        ref ValidationContext<TAmount> context)
        where TAmount : struct, INumber<TAmount>
    {
        var reader = new Utf8JsonReader(utf8Json);

        Guid transactionId = Guid.Empty;
        long accountId = 0;
        TAmount amount = TAmount.Zero;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                if (reader.ValueTextEquals("id"u8))
                {
                    reader.Read();
                    if (!reader.TryGetGuid(out transactionId)) return false;
                }
                else if (reader.ValueTextEquals("account"u8))
                {
                    reader.Read();
                    if (!reader.TryGetInt64(out accountId)) return false;
                }
                else if (reader.ValueTextEquals("amount"u8))
                {
                    reader.Read();
                    ReadOnlySpan<byte> rawValue = reader.ValueSpan;
                    if (!TAmount.TryParse(rawValue, provider: null, out amount))
                    {
                        return false;
                    }
                }
            }
        }

        if (transactionId == Guid.Empty || accountId == 0)
        {
            return false;
        }

        context.SetParsed(new Transaction<TAmount>(
            transactionId,
            accountId,
            amount,
            DateTime.UtcNow.Ticks
        ));

        return true;
    }
}