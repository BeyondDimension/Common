namespace System;

/// <summary>
/// 可释放对象持有者接口
/// </summary>
public interface IDisposableHolder : IDisposable
{
    /// <summary>
    /// 可释放对象的集合
    /// </summary>
    ICollection<IDisposable> CompositeDisposable { get; }
}