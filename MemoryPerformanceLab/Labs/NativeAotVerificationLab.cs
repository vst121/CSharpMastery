using System;
using System.Runtime.CompilerServices;

namespace ModernCSharpMastery.FraudEngine.Labs;

public static class NativeAotVerificationLab
{
    public static void Run()
    {
        Console.WriteLine("--- [LAB 10] Native AOT & Zero-JIT Inspection ---");

        Console.WriteLine($"Dynamic Code Supported:       {RuntimeFeature.IsDynamicCodeSupported}");
        Console.WriteLine($"Dynamic Code Compiled:        {RuntimeFeature.IsDynamicCodeCompiled}");

        Console.WriteLine();

        Console.WriteLine($"ByRef Fields:                  {RuntimeFeature.IsSupported(RuntimeFeature.ByRefFields)}");
        Console.WriteLine($"ByRef-Like Generics:           {RuntimeFeature.IsSupported(RuntimeFeature.ByRefLikeGenerics)}");
        Console.WriteLine($"Covariant Class Returns:       {RuntimeFeature.IsSupported(RuntimeFeature.CovariantReturnsOfClasses)}");
        Console.WriteLine($"Default Interface Methods:     {RuntimeFeature.IsSupported(RuntimeFeature.DefaultImplementationsOfInterfaces)}");
        Console.WriteLine($"Numeric IntPtr:                {RuntimeFeature.IsSupported(RuntimeFeature.NumericIntPtr)}");
        Console.WriteLine($"Portable PDB:                  {RuntimeFeature.IsSupported(RuntimeFeature.PortablePdb)}");
        Console.WriteLine($"Unmanaged Calling Convention:  {RuntimeFeature.IsSupported(RuntimeFeature.UnmanagedSignatureCallingConvention)}");
        Console.WriteLine($"Virtual Static Interface:      {RuntimeFeature.IsSupported(RuntimeFeature.VirtualStaticsInInterfaces)}");

        Console.WriteLine("\n[AOT Optimization Tip]: Zero trim warnings guarantee clean native code generation!");
    }
}