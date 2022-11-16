using BD.Common;

// ReSharper disable once CheckNamespace
namespace System;

public static partial class DisposableExtensions
{
    /// <summary>
    /// 将 <see cref="IDisposable"/> 对象添加 <see cref="IDisposableHolder.CompositeDisposable"/> 中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="disposable"></param>
    /// <param name="vm"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T AddTo<T>(this T disposable, IDisposableHolder vm) where T : IDisposable
    {
        if (vm == null)
        {
            disposable.Dispose();
            return disposable;
        }

        if (vm.CompositeDisposable == null)
        {
            throw new ArgumentNullException(nameof(IDisposableHolder.CompositeDisposable));
        }

        vm.CompositeDisposable.Add(disposable);
        return disposable;
    }

    /// <summary>
    /// 将 <see cref="IDisposable"/> 对象从 <see cref="IDisposableHolder.CompositeDisposable"/> 中移除
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="disposable"></param>
    /// <param name="vm"></param>
    /// <returns></returns>
    public static bool RemoveTo<T>(this T? disposable, IDisposableHolder vm) where T : IDisposable
    {
        if (disposable == null) return true;

        if (vm?.CompositeDisposable == null)
        {
            disposable.Dispose();
            return true;
        }

        var r = vm.CompositeDisposable.Remove(disposable);
        disposable.Dispose();
        return r;
    }
}
