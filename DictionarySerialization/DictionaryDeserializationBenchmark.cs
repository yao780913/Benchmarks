using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace DictionarySerialization;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, targetCount: 5)]
public class DictionaryDeserializationBenchmark
{
    private Dictionary<string, object> _testData = null!;
    private readonly DictionaryDeserializer _deserializer = new();

    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    [GlobalSetup]
    public void Setup()
    {
        _testData = GenerateTestData();
    }

    /// <summary>
    /// 生成測試資料 (4 種情境)
    /// </summary>
    private Dictionary<string, object> GenerateTestData()
    {
        var faker = new Faker();
        var guidId = Guid.NewGuid();

        // 生成 SimpleObject
        var simpleObject = new SimpleObject
        {
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
            Email = faker.Internet.Email(),
            IsActive = faker.Random.Bool()
        };

        // 生成 List<string>
        var stringList = new List<string>
        {
            faker.Random.Word(),
            faker.Random.Word(),
            faker.Random.Word(),
            faker.Random.Word(),
            faker.Random.Word()
        };

        // 生成 List<ComplexObject>
        var complexObjectList = new List<ComplexObject>();
        for (int i = 0; i < 5; i++)
        {
            complexObjectList.Add(new ComplexObject
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Tags = new List<string>
                {
                    faker.Random.Word(),
                    faker.Random.Word()
                },
                Metadata = new Dictionary<string, object>
                {
                    { "created", DateTime.UtcNow },
                    { "version", faker.Random.Int(1, 10) }
                }
            });
        }

        return new Dictionary<string, object>
        {
            // 情境 1: Guid
            { "propertyA", guidId },
            // 情境 2: Object (SimpleObject)
            { "propertyB", simpleObject },
            // 情境 3: List<string>
            { "propertyC", stringList },
            // 情境 4: List<ComplexObject>
            { "propertyD", complexObjectList }
        };
    }

    /// <summary>
    /// 基準線：直接暴力序列化後反序列化 Guid
    /// </summary>
    [Benchmark(Baseline = true)]
    public Guid Benchmark_BruteForce_Guid()
    {
        return _deserializer.DeserializeToGuid(_testData["propertyA"]);
    }

    /// <summary>
    /// 優化版本：直接指定 Guid 型別
    /// </summary>
    [Benchmark]
    public Guid Benchmark_Optimized_Guid()
    {
        var value = _testData["propertyA"];
        if (value is Guid guid)
            return guid;
        return Guid.Empty;
    }

    /// <summary>
    /// 暴力序列化後反序列化 SimpleObject
    /// </summary>
    [Benchmark]
    public SimpleObject? Benchmark_BruteForce_SimpleObject()
    {
        return _deserializer.DeserializeToObject<SimpleObject>(_testData["propertyB"]);
    }

    /// <summary>
    /// 優化版本：直接型別檢查 SimpleObject
    /// </summary>
    [Benchmark]
    public SimpleObject? Benchmark_Optimized_SimpleObject()
    {
        var value = _testData["propertyB"];
        if (value is SimpleObject obj)
            return obj;
        return null;
    }

    /// <summary>
    /// 暴力序列化後反序列化 List<string>
    /// </summary>
    [Benchmark]
    public List<string> Benchmark_BruteForce_ListString()
    {
        return _deserializer.DeserializeToList<string>(_testData["propertyC"]);
    }

    /// <summary>
    /// 優化版本：直接型別檢查 List<string>
    /// </summary>
    [Benchmark]
    public List<string> Benchmark_Optimized_ListString()
    {
        var value = _testData["propertyC"];
        if (value is List<string> list)
            return list;
        return [];
    }

    /// <summary>
    /// 暴力序列化後反序列化 List<ComplexObject>
    /// </summary>
    [Benchmark]
    public List<ComplexObject> Benchmark_BruteForce_ListComplexObject()
    {
        return _deserializer.DeserializeToList<ComplexObject>(_testData["propertyD"]);
    }

    /// <summary>
    /// 優化版本：直接型別檢查 List<ComplexObject>
    /// </summary>
    [Benchmark]
    public List<ComplexObject> Benchmark_Optimized_ListComplexObject()
    {
        var value = _testData["propertyD"];
        if (value is List<ComplexObject> list)
            return list;
        return [];
    }

    /// <summary>
    /// 混合場景：暴力反序列化所有情境
    /// </summary>
    [Benchmark]
    public void Benchmark_BruteForce_AllScenarios()
    {
        var guidResult = _deserializer.DeserializeToGuid(_testData["propertyA"]);
        var objResult = _deserializer.DeserializeToObject<SimpleObject>(_testData["propertyB"]);
        var listStringResult = _deserializer.DeserializeToList<string>(_testData["propertyC"]);
        var listComplexResult = _deserializer.DeserializeToList<ComplexObject>(_testData["propertyD"]);
    }

    /// <summary>
    /// 混合場景：優化版本反序列化所有情境
    /// </summary>
    [Benchmark]
    public void Benchmark_Optimized_AllScenarios()
    {
        var guidResult = _testData["propertyA"] is Guid g ? g : Guid.Empty;
        var objResult = _testData["propertyB"] is SimpleObject o ? o : null;
        var listStringResult = _testData["propertyC"] is List<string> ls ? ls : [];
        var listComplexResult = _testData["propertyD"] is List<ComplexObject> lc ? lc : [];
    }
}
