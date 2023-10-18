namespace BD.Common8.Primitives.ApiRsp.Models.Abstractions.Internals;

/// <summary>
/// <see cref="IApiRsp"/> 的默认实现基类
/// </summary>
[DebuggerDisplay("Code={Code}, Message={Message}, Content={GetContent()}, Url={Url}")]
public abstract partial class ApiRspBase : IApiRspBase, IApiRsp
{
    ApiRspCode mCode;
    bool mIsSuccess;

#pragma warning disable SA1600 // Elements should be documented

    public const string JsonPropertyName_Code = "🦄";
    public const string JsonPropertyName_Message = "🐴";
    public const string JsonPropertyName_Content = "🦓";

#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(0)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(0)]
#endif
    [NewtonsoftJsonProperty(JsonPropertyName_Code)]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonProperty(JsonPropertyName_Code)]
#endif
    public ApiRspCode Code
    {
        get => mCode;
        set
        {
            mCode = value;
            // https://github.com/dotnet/corefx/blob/v3.1.6/src/System.Net.Http/src/System/Net/Http/HttpResponseMessage.cs#L143
            mIsSuccess = mCode >= ApiRspCode.OK && mCode <= (ApiRspCode)299;
        }
    }

    /// <summary>
    /// <see cref="IApiRspBase.Message"/> 的内部值
    /// </summary>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(LastMKeyIndex)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(LastMKeyIndex)]
#endif
    [NewtonsoftJsonProperty(JsonPropertyName_Message)]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonProperty(JsonPropertyName_Message)]
#endif
    public string? InternalMessage { get; set; }

    [IgnoreDataMember]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
    [NewtonsoftJsonIgnore]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public string Message => InternalMessage ?? string.Empty;

    [IgnoreDataMember]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
    [NewtonsoftJsonIgnore]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public bool IsSuccess => mIsSuccess;

    protected virtual object? GetContent() => null;

    object? IApiRspBase.Content => GetContent();

    /// <summary>
    /// 最后一个 MessagePack 序列化 下标，继承自此类，新增需要序列化的字段/属性，标记此值+1，+2
    /// </summary>
    protected const int LastMKeyIndex = 1;

    [IgnoreDataMember]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
    [NewtonsoftJsonIgnore]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public Exception? ClientException { get; set; }

    [IgnoreDataMember]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
    [NewtonsoftJsonIgnore]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public string? Url { get; set; }

#pragma warning restore SA1600 // Elements should be documented
}
