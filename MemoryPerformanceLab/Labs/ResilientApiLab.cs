namespace CSharpMastery.FraudEngine.Labs;

public static class ResilientApiLab
{
    public static async Task RunAsync()
    {
        Console.WriteLine("--- [LAB 7] Resilient Downstream API Calling (Retry Pattern) ---");

        int attempts = 0;

        // Executing transient operation with retry strategy
        bool success = await ExecuteWithRetryAsync(async () =>
        {
            attempts++;
            Console.WriteLine($"  [CALL] Attempting External Credit Risk Check (Attempt {attempts})...");

            if (attempts < 3)
            {
                throw new InvalidOperationException("503 Service Unavailable (Transient Error)");
            }

            return await ValueTask.FromResult(true);
        }, maxRetries: 3, delayMs: 150);

        Console.WriteLine($"[RESULT] Downstream Check Succeeded: {success}");
    }

    private static async ValueTask<T> ExecuteWithRetryAsync<T>(Func<ValueTask<T>> operation, int maxRetries, int delayMs)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"  [RETRY] Caught: '{ex.Message}'. Backing off {delayMs}ms...");
                await Task.Delay(delayMs);
            }
        }
        return await operation();
    }
}