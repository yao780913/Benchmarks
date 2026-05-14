using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace JsonOptionsBenchmark;

/// <summary>
/// 統一的 options 建構邏輯。Shared 是已 MakeReadOnly 的共享單例；
/// FreshPerCall 每次都吐一份新的（模擬「壞 pattern」）。
/// </summary>
public static class JsonOptionsFactory
{
    public static JsonSerializerOptions Shared { get; } = BuildShared();

    public static JsonSerializerOptions FreshPerCall()
    {
        var o = new JsonSerializerOptions
        {
            PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Encoder                     = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        o.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        return o;
    }

    public static JsonSerializerOptions FreshPerCall_NoConverter()
    {
        var o = new JsonSerializerOptions
        {
            PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Encoder                     = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        return o;
    }

    private static JsonSerializerOptions BuildShared()
    {
        var o = FreshPerCall();
        o.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();
        o.MakeReadOnly();
        return o;
    }
}
