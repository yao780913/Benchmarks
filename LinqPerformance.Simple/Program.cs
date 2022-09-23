using BenchmarkDotNet.Running;
using Bogus;
using System;

namespace LinqPerformance.Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            Randomizer.Seed = new Random(420);
            BenchmarkRunner.Run<Benchmarks>(); 
        }
    }
}
