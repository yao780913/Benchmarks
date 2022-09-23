using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace FastListIteration;

[MemoryDiagnoser(false)]
public class Benchmarks
{
    private static readonly Random Rng = new (80085);

    [Params(100, 1_000, 10_000)]
    public int Size { get; set; }

    private List<int> _items;

    [GlobalSetup]
    public void Setup () => _items = Enumerable.Range(1, Size).Select(i => Rng.Next()).ToList();

    [Benchmark]
    public void For ()
    {
        for (var i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
        }
    }

    [Benchmark]
    public void Foreach ()
    {
        foreach (var item in _items) { }
    }
    
    [Benchmark]
    public void Foreach_Linq ()
    {
        _items.ForEach(item => { });
    }
    
    [Benchmark]
    public void Parallel_Foreach ()
    {
        Parallel.ForEach(_items, item => { });
    }
    
    [Benchmark]
    public void Parallel_Linq ()
    {
        _items.AsParallel().ForAll(item => { });
    }
    
    [Benchmark]
    public void ForEach_Span ()
    {
        foreach (var item in CollectionsMarshal.AsSpan(_items)) { }
    }
    
    [Benchmark]
    public void For_Span ()
    {
        var asSpan = CollectionsMarshal.AsSpan(_items);
        for (var i = 0; i < _items.Count; i++)
        {
            var item = asSpan[i];
        }
    }
}