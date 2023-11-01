namespace BD.Common8.Primitives.Columns;

/// <summary>
/// 某个操作的返回结果
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResult<T>
{
    /// <summary>
    /// 结果值
    /// </summary>
    T Result { get; set; }
}