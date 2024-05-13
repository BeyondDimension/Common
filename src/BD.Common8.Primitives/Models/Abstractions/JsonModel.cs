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

/// <summary>
/// 抽象的 JSON 模型类，实现了 <see cref="IJsonModel"/> 接口
/// </summary>
public abstract class JsonModel : IJsonModel
{
    /// <inheritdoc/>
    public virtual string GetJsonString(bool writeIndented = false)
    {
        var result = SystemTextJsonSerializer.Serialize(this, GetType(), options: writeIndented ? JsonSerializerCompatOptions.WriteIndented : JsonSerializerCompatOptions.Default);
        return result;
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        try
        {
            return GetJsonString(true);
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
    public override string GetJsonString(bool writeIndented = false)
    {
        var result = SystemTextJsonSerializer.Serialize((T)this, options: writeIndented ? JsonSerializerCompatOptions.WriteIndented : JsonSerializerCompatOptions.Default);
        return result;
    }
}

/// <summary>
/// 抽象的 JSON 模型类，实现了 <see cref="IJsonModel"/> 接口
/// </summary>
public abstract record class JsonRecordModel : IJsonModel
{
    /// <inheritdoc/>
    public virtual string GetJsonString(bool writeIndented = false)
    {
        var result = SystemTextJsonSerializer.Serialize(this, GetType(), options: writeIndented ? JsonSerializerCompatOptions.WriteIndented : JsonSerializerCompatOptions.Default);
        return result;
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        try
        {
            return GetJsonString(true);
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
    public override string GetJsonString(bool writeIndented = false)
    {
        var result = SystemTextJsonSerializer.Serialize((T)this, options: writeIndented ? JsonSerializerCompatOptions.WriteIndented : JsonSerializerCompatOptions.Default);
        return result;
    }
}