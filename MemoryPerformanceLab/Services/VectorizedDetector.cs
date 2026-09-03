using System;
using System.Runtime.Intrinsics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ModernCSharpMastery.FraudEngine.Services;

public static class VectorizedDetector
{
    /// <summary>
    /// Uses SIMD Vectorization (AVX2/NEON) to scan an array of transaction hashes 
    /// for a target hash in parallel hardware registers.
    /// </summary>
    public static bool ContainsHashVectorized(ReadOnlySpan<long> hashArray, long targetHash)
    {
        int i = 0;

        if (Vector128.IsHardwareAccelerated && hashArray.Length >= Vector128<long>.Count)
        {
            var targetVector = Vector128.Create(targetHash);

            for (; i <= hashArray.Length - Vector128<long>.Count; i += Vector128<long>.Count)
            {
                var currentVector = Vector128.LoadUnsafe(ref Unsafe.Add(ref MemoryMarshal.GetReference(hashArray), i));

                var comparison = Vector128.Equals(currentVector, targetVector);
                if (comparison != Vector128<long>.Zero)
                {
                    return true;
                }
            }
        }

        for (; i < hashArray.Length; i++)
        {
            if (hashArray[i] == targetHash) return true;
        }

        return false;
    }
}