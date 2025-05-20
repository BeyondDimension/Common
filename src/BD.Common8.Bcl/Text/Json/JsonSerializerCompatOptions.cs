#if !NO_SYSTEM_TEXT_JSON
#if NET7_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Text.Encodings.Web;

namespace System.Text.Json;

/// <summary>
/// 提供兼容性选项，用于配置 JSON 序列化器
/// </summary>
#if NET7_0_OR_GREATER
[RequiresDynamicCode(Serializable.CreateOptions_RequiresXCodeMessage)]
[RequiresUnreferencedCode(Serializable.CreateOptions_RequiresXCodeMessage)]
#endif
public static partial class JsonSerializerCompatOptions
{
    /// <summary>
    /// 默认的序列化选项
    /// </summary>
    static JsonSerializerOptions? _Default;

    /// <summary>
    /// 获取默认序列化选项
    /// </summary>
    public static JsonSerializerOptions Default
    {
        get => _Default ??= Serializable.CreateOptions(new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });
        set => _Default = value;
    }

    /// <summary>
    /// 缩进格式的序列化选项
    /// </summary>
    static JsonSerializerOptions? _WriteIndented;

    /// <summary>
    /// 获取缩进格式的序列化选项
    /// </summary>
    public static JsonSerializerOptions WriteIndented
    {
        get => _WriteIndented ??= Serializable.CreateOptions(new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
        });
        set => _WriteIndented = value;
    }

    /// <inheritdoc cref="JsonWriterCompatOptions"/>
    [Obsolete("use JsonWriterCompatOptions")]
    public static partial class Writer
    {
        /// <inheritdoc cref="JsonWriterCompatOptions.Default"/>
        public static JsonWriterOptions Default
        {
            get => JsonWriterCompatOptions.Default;
            set => JsonWriterCompatOptions.Default = value;
        }

        /// <inheritdoc cref="JsonWriterCompatOptions.WriteIndented"/>
        public static JsonWriterOptions WriteIndented
        {
            get => JsonWriterCompatOptions.WriteIndented;
            set => JsonWriterCompatOptions.WriteIndented = value;
        }
    }
}

/// <summary>
/// JsonWriter 默认选项
/// </summary>
public static partial class JsonWriterCompatOptions
{
    static JsonWriterOptions? _Default;

    /// <summary>
    /// 获取默认的 JsonWriter 选项
    /// </summary>
    public static JsonWriterOptions Default
    {
        get => _Default ??= new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        set => _Default = value;
    }

    static JsonWriterOptions? _WriteIndented;

    /// <summary>
    /// 获取启用缩进的 JsonWriter 选项
    /// </summary>
    public static JsonWriterOptions WriteIndented
    {
        get => _WriteIndented ??= new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = true,
        };
        set => _WriteIndented = value;
    }
}
#endif