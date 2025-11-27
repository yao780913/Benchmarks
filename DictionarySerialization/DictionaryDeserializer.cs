using System.Text.Json;
using System.Text.Json.Serialization;

namespace DictionarySerialization;

public class DictionaryDeserializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// 最原始寫法, 待驗證效能
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<T> DeserializeJsonElement<T>(object? value, JsonSerializerOptions options)
    {
        if (value == null)
            return [];

        // 如果已經是 List<T>，直接返回
        if (value is List<T> list)
            return list;

        // 如果是 JsonElement，將其反序列化為 List<T>
        if (value is JsonElement jsonElement)
        {
            var deserializedList = JsonSerializer.Deserialize<List<T>>(jsonElement.GetRawText(), options);
            return deserializedList ?? [];
        }

        // 其他情況，嘗試序列化後反序列化
        var json = JsonSerializer.Serialize(value, options);
        var result = JsonSerializer.Deserialize<List<T>>(json, options);
        return result ?? [];
    }

    /// <summary>
    /// 從 Dictionary 值反序列化為 List<T>
    /// </summary>
    /// <typeparam name="T">目標類型</typeparam>
    /// <param name="value">Dictionary 中的值</param>
    /// <param name="options">JSON 序列化選項 (可選)</param>
    /// <returns>反序列化後的列表，若失敗則返回空列表</returns>
    public List<T> DeserializeToList<T>(object? value, JsonSerializerOptions? options = null)
    {
        if (value == null)
            return [];

        options ??= DefaultOptions;

        // 如果已經是 List<T>，直接返回
        if (value is List<T> list)
            return list;

        // 如果是 JsonElement，將其反序列化為 List<T>
        if (value is JsonElement jsonElement)
        {
            var deserializedList = JsonSerializer.Deserialize<List<T>>(jsonElement.GetRawText(), options);
            return deserializedList ?? [];
        }

        // 其他情況，嘗試序列化後反序列化
        var json = JsonSerializer.Serialize(value, options);
        var result = JsonSerializer.Deserialize<List<T>>(json, options);
        return result ?? [];
    }

    /// <summary>
    /// 從 Dictionary 值反序列化為強型別物件
    /// </summary>
    /// <typeparam name="T">目標類型</typeparam>
    /// <param name="value">Dictionary 中的值</param>
    /// <param name="options">JSON 序列化選項 (可選)</param>
    /// <returns>反序列化後的物件，若失敗則返回 null</returns>
    public T? DeserializeToObject<T>(object? value, JsonSerializerOptions? options = null)
    {
        if (value == null)
            return default;

        options ??= DefaultOptions;

        // 如果已經是 T 類型，直接返回
        if (value is T typedValue)
            return typedValue;

        // 如果是 JsonElement，將其反序列化為 T
        if (value is JsonElement jsonElement)
        {
            return JsonSerializer.Deserialize<T>(jsonElement.GetRawText(), options);
        }

        // 其他情況，嘗試序列化後反序列化
        var json = JsonSerializer.Serialize(value, options);
        return JsonSerializer.Deserialize<T>(json, options);
    }

    /// <summary>
    /// 從 Dictionary 提取並轉換為 GUID
    /// </summary>
    /// <param name="value">Dictionary 中的值</param>
    /// <returns>轉換後的 GUID，若失敗則返回 Guid.Empty</returns>
    public Guid DeserializeToGuid(object? value)
    {
        if (value == null)
            return Guid.Empty;

        if (Guid.TryParse(value.ToString(), out var guid))
            return guid;

        return Guid.Empty;
    }

    /// <summary>
    /// 從 Dictionary 提取值並反序列化為 List<T>
    /// </summary>
    /// <typeparam name="T">列表項目類型</typeparam>
    /// <param name="dictionary">來源字典</param>
    /// <param name="key">字鍵</param>
    /// <param name="options">JSON 序列化選項 (可選)</param>
    /// <returns>反序列化後的列表或空列表</returns>
    public List<T> GetValueAsList<T>(Dictionary<string, object>? dictionary, string key, JsonSerializerOptions? options = null)
    {
        if (dictionary == null || !dictionary.TryGetValue(key, out var value))
            return [];

        return DeserializeToList<T>(value, options);
    }

    /// <summary>
    /// 從 Dictionary 提取值並反序列化為物件
    /// </summary>
    /// <typeparam name="T">目標類型</typeparam>
    /// <param name="dictionary">來源字典</param>
    /// <param name="key">字鍵</param>
    /// <param name="defaultValue">默認值</param>
    /// <param name="options">JSON 序列化選項 (可選)</param>
    /// <returns>反序列化後的值或默認值</returns>
    public T? GetValueAsObject<T>(Dictionary<string, object>? dictionary, string key, T? defaultValue = default, JsonSerializerOptions? options = null)
    {
        if (dictionary == null || !dictionary.TryGetValue(key, out var value))
            return defaultValue;

        return DeserializeToObject<T>(value, options) ?? defaultValue;
    }
}
