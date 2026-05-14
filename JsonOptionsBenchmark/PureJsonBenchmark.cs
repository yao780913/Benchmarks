using BenchmarkDotNet.Attributes;
using System.Text.Json;
using NewtonsoftJson = Newtonsoft.Json.JsonConvert;

namespace JsonOptionsBenchmark;

/// <summary>
/// Group 1 — 純 JSON round-trip。對同一個 DTO 做 serialize + deserialize，
/// 比較不同 options 策略的 throughput。
/// </summary>
[MemoryDiagnoser]
[HideColumns("Error", "StdDev")]
public class PureJsonBenchmark
{
    private static readonly CandidateSnapshot Payload = TestData.SampleSnapshot();

    // 重用共享單例（最佳 pattern）
    [Benchmark(Baseline = true)]
    public CandidateSnapshot Shared_Options()
    {
        var json = JsonSerializer.Serialize(Payload, JsonOptionsFactory.Shared);
        return JsonSerializer.Deserialize<CandidateSnapshot>(json, JsonOptionsFactory.Shared)!;
    }

    // 不傳 options，STJ 內部有 cached default options
    [Benchmark]
    public CandidateSnapshot Default_NoOptions()
    {
        var json = JsonSerializer.Serialize(Payload);
        return JsonSerializer.Deserialize<CandidateSnapshot>(json)!;
    }

    // 每次 call 都 new 一份 options（無 converter）
    [Benchmark]
    public CandidateSnapshot New_PerCall_Plain()
    {
        var json = JsonSerializer.Serialize(Payload, JsonOptionsFactory.FreshPerCall_NoConverter());
        return JsonSerializer.Deserialize<CandidateSnapshot>(
            json, JsonOptionsFactory.FreshPerCall_NoConverter())!;
    }

    // 每次 call 都 new 一份 options + enum converter（最壞情境）
    [Benchmark]
    public CandidateSnapshot New_PerCall_WithConverter()
    {
        var json = JsonSerializer.Serialize(Payload, JsonOptionsFactory.FreshPerCall());
        return JsonSerializer.Deserialize<CandidateSnapshot>(
            json, JsonOptionsFactory.FreshPerCall())!;
    }

    // Newtonsoft 對照
    [Benchmark]
    public CandidateSnapshot Newtonsoft_Baseline()
    {
        var json = NewtonsoftJson.SerializeObject(Payload);
        return NewtonsoftJson.DeserializeObject<CandidateSnapshot>(json)!;
    }
}
