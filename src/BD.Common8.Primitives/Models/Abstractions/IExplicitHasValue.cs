namespace BD.Common8.Models.Abstractions;

/// <summary>
/// HasValue 接口定义，通常显示实现此接口
/// </summary>
public interface IExplicitHasValue
{
    /// <summary>
    /// 判断 <see cref="IExplicitHasValue"/> 实例是否具有值
    /// </summary>
    bool ExplicitHasValue();
}