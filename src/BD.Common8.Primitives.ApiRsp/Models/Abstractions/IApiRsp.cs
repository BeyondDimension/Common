namespace BD.Common8.Primitives.ApiRsp.Models.Abstractions;

/// <summary>
/// Api 返回接口
/// </summary>
public partial interface IApiRsp
{
    /// <summary>
    /// 状态码
    /// </summary>
    ApiRspCode Code { get; }

    /// <summary>
    /// 显示消息
    /// </summary>
    string Message { get; }

    /// <summary>
    /// 是否成功
    /// </summary>
    bool IsSuccess { get; }
}

/// <summary>
/// Api 返回接口，带附加内容
/// </summary>
/// <typeparam name="TContent">附加内容泛型</typeparam>
public partial interface IApiRsp<out TContent> : IApiRsp
{
    /// <summary>
    /// 附加内容
    /// </summary>
    TContent? Content { get; }
}