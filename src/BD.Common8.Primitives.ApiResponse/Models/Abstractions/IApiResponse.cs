namespace BD.Common8.Models.Abstractions;

/// <summary>
/// Api 返回接口 (BackManage 后台管理)
/// </summary>
public partial interface IApiResponse : IApiRsp
{
    /// <summary>
    /// 是否成功
    /// </summary>
    new bool IsSuccess { get; set; }

    /// <summary>
    /// 错误消息数组，用换行拼接显示
    /// </summary>
    string[] Messages { get; set; }

    /// <inheritdoc/>
    bool IApiRsp.IsSuccess => IsSuccess;

    /// <inheritdoc/>
    ApiRspCode IApiRsp.Code => IsSuccess ? ApiRspCode.OK : ApiRspCode.Fail;

    /// <inheritdoc/>
    string IApiRsp.Message => this.GetMessages();
}

/// <summary>
/// Api 返回接口 (BackManage 后台管理)，带附加内容
/// </summary>
/// <typeparam name="T"></typeparam>
public partial interface IApiResponse<out T> : IApiResponse, IApiRsp<T>
{
    /// <summary>
    /// 主数据
    /// </summary>
    T? Data { get; }

    /// <inheritdoc/>
    T? IApiRsp<T>.Content => Data;
}