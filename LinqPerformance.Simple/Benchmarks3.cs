
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace LinqPerformance.Simple;

[MemoryDiagnoser]
public class Benchmarks3
{
    [Params(1_000)]
    public int Size { get; set; }

    private List<int> _list;
    private IEnumerable<int> _enumerable;
    
    [GlobalSetup]
    public void Setup()
    {
        _list = [];
        _enumerable = [];
    }
    
    [Benchmark]
    public void ListSeparateWhere()
    {
        _list = Enumerable.Range(1, Size).ToList();
        var result = _list
            .Where(x => x % 2 == 0)
            .Where(x => x % 3 == 0)
            .Where(x => x % 5 == 0)
            .Where(x => x % 7 == 0).ToList();

    }
    
    [Benchmark]
    public void ListCombineWhere()
    {
        _list = Enumerable.Range(1, Size).ToList();
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
        _enumerable = Enumerable.Range(1, Size);
        var result = _enumerable
            .Where(x => x % 2 == 0)
            .Where(x => x % 3 == 0)
            .Where(x => x % 5 == 0)
            .Where(x => x % 7 == 0).ToList();

    }
    
    [Benchmark]
    public void EnumerableCombineWhere()
    {
        _enumerable = Enumerable.Range(1, Size);
        var result = _enumerable
            .Where(x => 
                x % 2 == 0
                && x % 3 == 0
                && x % 5 == 0
                && x % 7 == 0).ToList();
    }
}