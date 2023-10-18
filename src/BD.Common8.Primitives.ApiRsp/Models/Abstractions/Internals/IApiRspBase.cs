namespace BD.Common8.Primitives.ApiRsp.Models.Abstractions.Internals;

/// <summary>
/// Api 返回基类接口
/// </summary>
public partial interface IApiRspBase
{
    /// <inheritdoc cref="IApiRsp.Code"/>
    ApiRspCode Code { get; }

    /// <inheritdoc cref="IApiRsp.Message"/>
    string Message { get; }

    /// <inheritdoc cref="IApiRsp.IsSuccess"/>
    bool IsSuccess { get; }

    /// <inheritdoc cref="IApiRsp{TContent}.Content"/>
    object? Content { get; }

    /// <summary>
    /// 用于在客户端上纪录本次请求中出现的异常
    /// </summary>
    Exception? ClientException { get; }

    /// <summary>
    /// 用于在客户端上纪录本次请求的 Url
    /// </summary>
    string? Url { get; }
}
