namespace BD.Common8.Primitives.Extensions;

#pragma warning disable SA1600 // Elements should be documented

public static partial class ExplicitHasValueExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasValue([NotNullWhen(true)] this IExplicitHasValue? obj)
    {
        return obj != null && obj.ExplicitHasValue();
    }
}
