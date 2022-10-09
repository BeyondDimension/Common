namespace BD.Common.Services;

/// <summary>
/// 与 Web API 的调用
/// </summary>
public interface IApiConnection
{
    /// <summary>
    /// 使用 POST 调用 API
    /// </summary>
    /// <typeparam name="TRequestModel">请求模型</typeparam>
    /// <typeparam name="TResponseModel">响应模型</typeparam>
    /// <param name="apiRelativeUrl">API 相对地址</param>
    /// <param name="req">请求模型值</param>
    /// <param name="allowNull">是否允许响应正文为空，默认不允许</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse<TResponseModel>> ApiPostAsync<TRequestModel, TResponseModel>(string apiRelativeUrl, TRequestModel req, bool allowNull = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 GET 调用 API
    /// </summary>
    /// <typeparam name="TResponseModel">响应模型</typeparam>
    /// <param name="apiRelativeUrl">API 相对地址</param>
    /// <param name="allowNull">是否允许响应正文为空，默认不允许</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse<TResponseModel>> ApiGetAsync<TResponseModel>(string apiRelativeUrl, bool allowNull = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 DELETE 调用 API
    /// </summary>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse> ApiDeleteAsync(string apiRelativeUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 PUT 调用 API
    /// </summary>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse> ApiPutAsync(string apiRelativeUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 POST 调用 API
    /// </summary>
    /// <typeparam name="TRequestModel"></typeparam>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse> ApiPostAsync<TRequestModel>(string apiRelativeUrl, TRequestModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 PUT 调用 API
    /// </summary>
    /// <typeparam name="TRequestModel"></typeparam>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse> ApiPutAsync<TRequestModel>(string apiRelativeUrl, TRequestModel req, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 POST 调用 API
    /// </summary>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse> ApiPostAsync(string apiRelativeUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 PUT 调用 API
    /// </summary>
    /// <typeparam name="TResponseModel"></typeparam>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="allowNull"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse<TResponseModel>> ApiPutAsync<TResponseModel>(string apiRelativeUrl, bool allowNull = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 DELETE 调用 API
    /// </summary>
    /// <typeparam name="TResponseModel"></typeparam>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="allowNull"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse<TResponseModel>> ApiDeleteAsync<TResponseModel>(string apiRelativeUrl, bool allowNull = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 POST 调用 API
    /// </summary>
    /// <typeparam name="TResponseModel"></typeparam>
    /// <param name="apiRelativeUrl"></param>
    /// <param name="allowNull"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse<TResponseModel>> ApiPostAsync<TResponseModel>(string apiRelativeUrl, bool allowNull = false, CancellationToken cancellationToken = default);
}
