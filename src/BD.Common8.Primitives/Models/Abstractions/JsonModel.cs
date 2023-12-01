namespace BD.Common8.Models.Abstractions;

/// <summary>
/// JSON 模型接口，用于表示具有 JSON 序列化功能的模型
/// </summary>
public interface IJsonModel
{
}

/// <summary>
/// 抽象的 JSON 模型类，实现了 <see cref="IJsonModel"/> 接口
/// </summary>
public abstract class JsonModel : IJsonModel
{
    /// <summary>
    /// 将当前对象序列化为 JSON 字符串并返回
    /// </summary>
    public override string? ToString()
    {
        try
        {
            return SystemTextJsonSerializer.Serialize(this, GetType(), options: JsonSerializerCompatOptions.WriteIndented);
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
public abstract class JsonModel<T> : IJsonModel where T : JsonModel<T>
{
    /// <summary>
    /// 将当前对象序列化为 JSON 字符串并返回
    /// </summary>
    public override string? ToString()
    {
        try
        {
            return SystemTextJsonSerializer.Serialize<T>((T)this, options: JsonSerializerCompatOptions.WriteIndented);
        }
        catch
        {
            return base.ToString();
        }
    }
}