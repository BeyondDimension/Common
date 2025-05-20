using BD.Common8.Enums;
using BD.Common8.Helpers;
using BD.Common8.Models.Abstractions;
using BD.Common8.Models.Abstractions.Internals;
using System.Runtime.CompilerServices;

namespace BD.Common8.Models;

/// <summary>
/// <see cref="IApiRsp"/> 的默认实现类
/// </summary>
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
[global::MessagePack.MessagePackObject]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
[global::MemoryPack.MemoryPackable(global::MemoryPack.SerializeLayout.Explicit)]
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