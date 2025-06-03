
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace LinqPerformance.Simple;

[MemoryDiagnoser]
public class Benchmarks2
{
    [Params(1_000)]
    public int Size { get; set; }

    private List<int> _list;
    private IEnumerable<int> _enumerable;
    
    [GlobalSetup]
    public void Setup()
    {
        _list = Enumerable.Range(1, Size).ToList();
        _enumerable = Enumerable.Range(1, Size);
    }
    
    [Benchmark]
    public void ListSeparateWhere()
    {
        var result = _list
            .Where(x => x % 2 == 0)
            .Where(x => x % 3 == 0)
            .Where(x => x % 5 == 0)
            .Where(x => x % 7 == 0).ToList();

    }
    
    [Benchmark]
    public void ListCombineWhere()
    {
        var result = _list
            .Where(x => 
                x % 2 == 0
                && x % 3 == 0
                && x % 5 == 0
                && x % 7 == 0).ToList();
    }
    
    [Benchmark]
    public void EnumerableSeparateWhere()
    {
        var result = _enumerable
            .Where(x => x % 2 == 0)
            .Where(x => x % 3 == 0)
            .Where(x => x % 5 == 0)
            .Where(x => x % 7 == 0).ToList();

    }
    
    [Benchmark]
    public void EnumerableCombineWhere()
    {
        var result = _enumerable
            .Where(x => 
                x % 2 == 0
                && x % 3 == 0
                && x % 5 == 0
                && x % 7 == 0).ToList();
    }
}