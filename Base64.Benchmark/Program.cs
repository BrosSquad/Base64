using System;
using BenchmarkDotNet.Running;

namespace Base64.Benchmark
{
    public static class Program
    {
        public static void Main()
        {
            var summary = BenchmarkRunner.Run<Base64Benchmark>();
            
            Console.WriteLine(summary);
        }
    }
}