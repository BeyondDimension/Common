using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    string GetJsonString(bool writeIndented = false);
}

/// <inheritdoc cref="IJsonModel"/>
public interface IJsonModel<T> : IJsonModel
    where T : IJsonModel<T>, IJsonSerializerContext
{
    string IJsonModel.GetJsonString(bool writeIndented) => GetJsonString(writeIndented);

    new string GetJsonString(bool writeIndented = false)
    {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        var result = JsonSerializer.Serialize(this, GetType(), options: IJsonSerializerContext.GetJsonSerializerOptions<T>(writeIndented));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return result;
    }
}

/// <inheritdoc cref="IJsonSerializerContext"/>
public interface IJsonSerializerContext
{
    /// <summary>
    /// 返回 Json 源生成的 <see cref="JsonSerializerContext"/> 默认实例
    /// </summary>
    static abstract JsonSerializerContext Default { get; }

    internal static JsonSerializerOptions GetJsonSerializerOptions(bool writeIndented = false)
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        => writeIndented ? JsonSerializerCompatOptions.WriteIndented : JsonSerializerCompatOptions.Default;
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    public static JsonSerializerOptions GetJsonSerializerOptions<T>(bool writeIndented = false) where T : IJsonSerializerContext
    {
        try
        {
            var opt = T.Default.Options;
            if (writeIndented)
            {
                if (opt.WriteIndented)
                {
                    return opt;
                }
                else
                {
                    opt = new(opt) // 重新创建一份，不修改原值
                    {
                        WriteIndented = true,
                    };
                    return opt;
                }
            }
            else
            {
                if (opt.WriteIndented)
                {
                    opt = new(opt) // 重新创建一份，不修改原值
                    {
                        WriteIndented = false,
                    };
                    return opt;
                }
                else
                {
                    return opt;
                }
            }
        }
        catch
        {
            return GetJsonSerializerOptions(writeIndented);
        }
    }
}

/// <summary>
/// 抽象的 JSON 模型类，实现了 <see cref="IJsonModel"/> 接口
/// </summary>
[Obsolete("use JsonModel<T>", true)]
public abstract class JsonModel : IJsonModel
{
    /// <inheritdoc/>
    public virtual string GetJsonString(bool writeIndented = false)
    {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        var result = JsonSerializer.Serialize(this, GetType(), options: IJsonSerializerContext.GetJsonSerializerOptions(writeIndented));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return result;
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        try
        {
            var json = GetJsonString(true);
            return json;
        }
        catch
        {
            var str = base.ToString();
            return str;
        }
    }
}

/// <summary>
/// 泛型的 JSON 模型类，继承此类并将泛型 T 设为子类型，且实现 <see cref="IJsonSerializerContext"/> 接口，已提供 <see cref="IJsonModel.GetJsonString(bool)"/> 函数与 <see cref="object.ToString"/> 重写为输出 Json 字符串
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class JsonModel<T> : IJsonModel, IJsonModel<T> where T : JsonModel<T>, IJsonSerializerContext
{
    /// <inheritdoc/>
    public virtual string GetJsonString(bool writeIndented = false)
    {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        var result = JsonSerializer.Serialize((T)this, options: IJsonSerializerContext.GetJsonSerializerOptions<T>(writeIndented));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return result;
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        try
        {
            var json = GetJsonString(true);
            return json;
        }
        catch
        {
            var str = base.ToString();
            return str;
        }
    }
}

/// <summary>
/// 抽象的 JSON 模型类，实现了 <see cref="IJsonModel"/> 接口
/// </summary>
[Obsolete("use JsonRecordModel<T>", true)]
public abstract record class JsonRecordModel : IJsonModel
{
    /// <inheritdoc/>
    public virtual string GetJsonString(bool writeIndented = false)
    {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        var result = JsonSerializer.Serialize(this, GetType(), options: IJsonSerializerContext.GetJsonSerializerOptions(writeIndented));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return result;
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        try
        {
            var json = GetJsonString(true);
            return json;
        }
        catch
        {
            var str = base.ToString();
            return str;
        }
    }
}

/// <summary>
/// 泛型的 JSON 模型类，继承此类并将泛型 T 设为子类型，且实现 <see cref="IJsonSerializerContext"/> 接口，已提供 <see cref="IJsonModel.GetJsonString(bool)"/> 函数与 <see cref="object.ToString"/> 重写为输出 Json 字符串
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract record class JsonRecordModel<T> : IJsonModel, IJsonModel<T> where T : JsonRecordModel<T>, IJsonSerializerContext
{
    /// <inheritdoc/>
    public virtual string GetJsonString(bool writeIndented = false)
    {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        var result = JsonSerializer.Serialize((T)this, options: IJsonSerializerContext.GetJsonSerializerOptions<T>(writeIndented));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return result;
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        try
        {
            var json = GetJsonString(true);
            return json;
        }
        catch
        {
            var str = base.ToString();
            return str;
        }
    }
}