using BD.Common8.Enums;
using BD.Common8.Helpers;
using BD.Common8.Models.Abstractions;
using BD.Common8.Models.Abstractions.Internals;
using System.Runtime.CompilerServices;

namespace BD.Common8.Models;

/// <summary>
/// <see cref="IApiRsp{TContent}"/> 的默认实现类
/// </summary>
/// <typeparam name="TContent"></typeparam>
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
[global::MessagePack.MessagePackObject]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
[global::MemoryPack.MemoryPackable(global::MemoryPack.SerializeLayout.Explicit)]
#endif
public sealed partial class ApiRspImpl<TContent> : ApiRspBase, IApiRsp<TContent?>
{
    /// <inheritdoc/>
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::MessagePack.Key(LastMKeyIndex + 1)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [global::MemoryPack.MemoryPackOrder(LastMKeyIndex + 1)]
#endif
#if !NO_NEWTONSOFT_JSON
    [global::Newtonsoft.Json.JsonProperty(JsonPropertyName_Content)]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::System.Text.Json.Serialization.JsonPropertyName(JsonPropertyName_Content)]
#endif
    public TContent? Content { get; set; }

    protected override object? GetContent() => Content;

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