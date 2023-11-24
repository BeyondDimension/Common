namespace BD.Common8.Primitives.ApiRsp.Models;

/// <summary>
/// <see cref="IApiRsp{TContent}"/> 的默认实现类
/// </summary>
/// <typeparam name="TContent"></typeparam>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
[MPObj]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
[MP2Obj(MP2SerializeLayout.Explicit)]
#endif
public sealed partial class ApiRspImpl<TContent> : ApiRspBase, IApiRsp<TContent?>
{
    /// <inheritdoc/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(LastMKeyIndex + 1)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(LastMKeyIndex + 1)]
#endif
    [NewtonsoftJsonProperty(JsonPropertyName_Content)]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonProperty(JsonPropertyName_Content)]
#endif
    public TContent? Content { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiRspImpl<TContent?>(TContent content) => ApiRspHelper.Ok(content);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiRspImpl<TContent?>(ApiRspCode code) => ApiRspHelper.Code<TContent>(code);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiRspImpl<TContent?>((ApiRspCode code, string? message) args) => ApiRspHelper.Code<TContent>(args.code, args.message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiRspImpl<TContent?>(string? message) => ApiRspHelper.Fail<TContent>(message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiRspImpl<TContent?>(Exception exception) => ApiRspHelper.Exception<TContent>(exception);
}