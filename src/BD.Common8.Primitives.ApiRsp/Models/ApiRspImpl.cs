namespace BD.Common8.Primitives.ApiRsp.Models;

/// <summary>
/// <see cref="IApiRsp"/> 的默认实现类
/// </summary>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
[MPObj]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
[MP2Obj(MP2SerializeLayout.Explicit)]
#endif
public sealed partial class ApiRspImpl : ApiRspBase, IApiRsp<object?>
{
    /// <inheritdoc/>
    object? IApiRsp<object?>.Content => null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiRspImpl(ApiRspCode code) => ApiRspHelper.Code(code);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiRspImpl((ApiRspCode code, string? message) args) => ApiRspHelper.Code(args.code, args.message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiRspImpl(string? message) => ApiRspHelper.Fail(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiRspImpl(Exception exception) => ApiRspHelper.Exception(exception);
}