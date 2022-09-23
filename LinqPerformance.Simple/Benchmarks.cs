using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace LinqPerformance.Simple
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private static readonly Samples samples = new Samples();

        //[Benchmark]
        //public int AlcoholicDrinksCountLinqWhere() => samples.AlcoholicDrinksCountLinqWhere();

        //[Benchmark]
        //public int AlcoholicDrinksCountLinqCount() => samples.AlcoholicDrinksCountLinqCount();

        //[Benchmark]
        //public int AlcoholicDrinkCountForLoop() => samples.AlcoholicDrinkCountForLoop();
        
        //[Benchmark]
        //public int AlcoholicDrinkCountForeachLoop() => samples.AlcoholicDrinkCountForeachLoop();

        [Benchmark]
        public List<string> NonAlcoholicDrinkNamesForLoop() => samples.NonAlcoholicDrinkNamesForLoop();

        [Benchmark]
        public List<string> NonAlcoholicDrinkNamesLinq() => samples.NonAlcoholicDrinkNamesLinq();
    }
}