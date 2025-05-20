using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace BD.Common8.Models.Abstractions;

/// <summary>
/// JSON 模型接口，用于表示具有 JSON 序列化功能的模型
/// </summary>
public interface IJsonModel
{
    /// <summary>
    /// 将当前对象序列化为 JSON 字符串
    /// </summary>
    /// <param name="writeIndented"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
    string GetJsonString(bool writeIndented = false);
}

/// <summary>
/// 抽象的 JSON 模型类，实现了 <see cref="IJsonModel"/> 接口
/// </summary>
public abstract class JsonModel : IJsonModel
{
    /// <inheritdoc/>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
    public virtual string GetJsonString(bool writeIndented = false)
    {
        var result = JsonSerializer.Serialize(this, GetType(), options: writeIndented ? JsonSerializerCompatOptions.WriteIndented : JsonSerializerCompatOptions.Default);
        return result;
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        try
        {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            return GetJsonString(true);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        }
        catch
        {
            return base.ToString();
        }
    }
}

/// <summary>
/// 泛型的 JSON 模型类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class JsonModel<T> : JsonModel, IJsonModel where T : JsonModel<T>
{
    /// <inheritdoc/>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
    public override string GetJsonString(bool writeIndented = false)
    {
        var result = JsonSerializer.Serialize((T)this, options: writeIndented ? JsonSerializerCompatOptions.WriteIndented : JsonSerializerCompatOptions.Default);
        return result;
    }
}

/// <summary>
/// 抽象的 JSON 模型类，实现了 <see cref="IJsonModel"/> 接口
/// </summary>
public abstract record class JsonRecordModel : IJsonModel
{
    /// <inheritdoc/>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
    public virtual string GetJsonString(bool writeIndented = false)
    {
        var result = JsonSerializer.Serialize(this, GetType(), options: writeIndented ? JsonSerializerCompatOptions.WriteIndented : JsonSerializerCompatOptions.Default);
        return result;
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        try
        {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            return GetJsonString(true);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        }
        catch
        {
            return base.ToString();
        }
    }
}

/// <summary>
/// 泛型的 JSON 模型类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract record class JsonRecordModel<T> : JsonRecordModel, IJsonModel where T : JsonRecordModel<T>
{
    /// <inheritdoc/>
    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
    [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
    public override string GetJsonString(bool writeIndented = false)
    {
        var result = JsonSerializer.Serialize((T)this, options: writeIndented ? JsonSerializerCompatOptions.WriteIndented : JsonSerializerCompatOptions.Default);
        return result;
    }
}