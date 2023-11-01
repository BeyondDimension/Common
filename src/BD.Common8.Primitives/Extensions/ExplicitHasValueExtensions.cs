namespace BD.Common8.Primitives.Extensions;

#pragma warning disable SA1600 // Elements should be documented

public static partial class ExplicitHasValueExtensions
{
    /// <summary>
    /// 判断 <see cref="IExplicitHasValue"/> 实例是否具有值
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasValue([NotNullWhen(true)] this IExplicitHasValue? obj)
    {
        return obj != null && obj.ExplicitHasValue();
    }
}
