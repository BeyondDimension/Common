namespace BD.Common8.Models.Abstractions.Internals;

/// <summary>
/// <see cref="IApiRsp"/> 的默认实现基类
/// </summary>
[DebuggerDisplay("{Code}, M={Message}, C={GetContent()}, U={Url}, D={IsDisplayed}, E={ClientException}")]
public abstract partial class ApiRspBase : IApiRspBase, IApiRsp
{
    ApiRspCode mCode;
    bool mIsSuccess;

    /// <summary>
    /// 表示 Josn 数据中 Code 字段属性
    /// </summary>
    public const string JsonPropertyName_Code = "🦄";

    /// <summary>
    /// 表示 Josn 数据中 Message 字段属性
    /// </summary>
    public const string JsonPropertyName_Message = "🐴";

    /// <summary>
    /// 表示 Josn 数据中 Content 字段属性
    /// </summary>
    public const string JsonPropertyName_Content = "🦓";

    /// <inheritdoc/>
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(0)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(0)]
#endif
#if !NO_NEWTONSOFT_JSON
    [NewtonsoftJsonProperty(JsonPropertyName_Code)]
#endif
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

    string? internalMessage;

    /// <summary>
    /// <see cref="IApiRspBase.Message"/> 的内部值
    /// </summary>
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(LastMKeyIndex)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(LastMKeyIndex)]
#endif
#if !NO_NEWTONSOFT_JSON
    [NewtonsoftJsonProperty(JsonPropertyName_Message)]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonProperty(JsonPropertyName_Message)]
#endif
    public string? InternalMessage { get => internalMessage ??= this.CreateMessage(); set => internalMessage = value; }

    /// <inheritdoc/>
    [IgnoreDataMember]
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
#if !NO_NEWTONSOFT_JSON
    [NewtonsoftJsonIgnore]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public string Message => InternalMessage ?? string.Empty;

    /// <inheritdoc/>
    [IgnoreDataMember]
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
#if !NO_NEWTONSOFT_JSON
    [NewtonsoftJsonIgnore]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public bool IsSuccess => mIsSuccess;

    /// <summary>
    /// 获取内容对象
    /// </summary>
    /// <returns></returns>
    protected virtual object? GetContent() => null;

    /// <inheritdoc/>
    object? IApiRspBase.Content => GetContent();

    /// <summary>
    /// 最后一个 MessagePack 序列化 下标，继承自此类，新增需要序列化的字段/属性，标记此值+1，+2
    /// </summary>
    protected const int LastMKeyIndex = 1;

    /// <inheritdoc/>
    [IgnoreDataMember]
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
#if !NO_NEWTONSOFT_JSON
    [NewtonsoftJsonIgnore]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public Exception? ClientException { get; set; }

    /// <inheritdoc/>
    [IgnoreDataMember]
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
#if !NO_NEWTONSOFT_JSON
    [NewtonsoftJsonIgnore]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public string? Url { get; set; }

    /// <inheritdoc/>
    [IgnoreDataMember]
#if !NO_MESSAGEPACK && !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPIgnore]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Ignore]
#endif
#if !NO_NEWTONSOFT_JSON
    [NewtonsoftJsonIgnore]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [SystemTextJsonIgnore]
#endif
    public bool IsDisplayed { get; set; }
}
