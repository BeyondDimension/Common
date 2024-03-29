#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)

namespace System.Text.Json;

/// <summary>
/// 提供兼容性选项，用于配置  JSON 序列化器
/// </summary>
public static partial class JsonSerializerCompatOptions
{
    /// <summary>
    /// 默认的序列化选项
    /// </summary>
    static SystemTextJsonSerializerOptions? _Default;

    /// <summary>
    /// 获取默认序列化选项
    /// </summary>
    public static SystemTextJsonSerializerOptions Default => _Default ??= new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// 缩进格式的序列化选项
    /// </summary>
    static SystemTextJsonSerializerOptions? _WriteIndented;

    /// <summary>
    /// 获取缩进格式的序列化选项
    /// </summary>
    public static SystemTextJsonSerializerOptions WriteIndented => _WriteIndented ??= new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    /// <summary>
    /// JsonWriter 默认选项
    /// </summary>
    public static partial class Writer
    {
        /// <summary>
        /// 获取默认的 JsonWriter 选项
        /// </summary>
        public static JsonWriterOptions Default => new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// 获取启用缩进的 JsonWriter 选项
        /// </summary>
        public static JsonWriterOptions WriteIndented => new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = true,
        };
    }
}

#endif