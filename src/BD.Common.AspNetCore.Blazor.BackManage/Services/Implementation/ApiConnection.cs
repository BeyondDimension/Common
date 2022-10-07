namespace BD.Common.Services.Implementation;

public sealed class ApiConnection : HttpClientWrapper, IApiConnection
{
    readonly IAuthService auth;
    readonly MessageService message;
    static readonly JsonSerializerOptions options;

    static ApiConnection()
    {
        options = new(JsonSerializerDefaults.Web);
        options.Converters.Add(new Utc8DateTimeOffsetConverter());
    }

    public ApiConnection(HttpClient client, IAuthService auth, MessageService message) : base(client)
    {
        this.auth = auth;
        this.message = message;
    }

    const string ResponseDataIsNull = "错误：响应正文为空";
    const string ResponseDataDeserializationFail = "错误：响应正文反序列化失败";

    async ValueTask SetAuthorizationAsync(CancellationToken cancellationToken)
    {
        var token = await auth.GetTokenAsync(cancellationToken);
        SetAuthorization(token);
    }

    public static TApiResponse Check<TApiResponse>(TApiResponse? data, bool allowNull = false) where TApiResponse : ApiResponse, new()
    {
        if (data == null)
        {
            return new TApiResponse
            {
                IsSuccess = false,
                Messages = new[] { ResponseDataDeserializationFail },
            };
        }
        if (data.IsSuccess && !allowNull && data is IApiResponse<object?> dataT && dataT.Data == null)
        {
            return new TApiResponse
            {
                IsSuccess = false,
                Messages = new[] { ResponseDataIsNull },
            };
        }
        return data;
    }

    /// <summary>
    /// 读取响应的 JSON 并反序列化
    /// </summary>
    /// <typeparam name="TApiResponse"></typeparam>
    /// <param name="allowNull"></param>
    /// <param name="rsp"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    static async Task<TApiResponse> ReadFromJsonCoreAsync<TApiResponse>(bool allowNull, HttpResponseMessage rsp, CancellationToken cancellationToken = default) where TApiResponse : ApiResponse, new()
    {
        if (rsp.IsSuccessStatusCode)
        {
            if (rsp.Content == null || !rsp.Content.Headers.ContentLength.HasValue || rsp.Content.Headers.ContentLength <= 0)
            {
                return new TApiResponse
                {
                    IsSuccess = allowNull,
                    Messages = allowNull ? Array.Empty<string>() : new[] { ResponseDataIsNull },
                };
            }
            var data = await rsp.Content.ReadFromJsonAsync<TApiResponse>(options, cancellationToken);
            return Check(data, allowNull);
        }
        else
        {
            return new TApiResponse
            {
                IsSuccess = false,
                Messages = new[] { $"服务端错误：{rsp.StatusCode}" },
            };
        }
    }

    /// <summary>
    /// 读取响应的 JSON 并反序列化，<see cref="ApiResponse{T}"/>
    /// </summary>
    /// <typeparam name="TResponseModel"></typeparam>
    /// <param name="allowNull"></param>
    /// <param name="rsp"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static Task<ApiResponse<TResponseModel>> ReadFromJsonAsync<TResponseModel>(bool allowNull, HttpResponseMessage rsp, CancellationToken cancellationToken = default)
        => ReadFromJsonCoreAsync<ApiResponse<TResponseModel>>(allowNull, rsp, cancellationToken);

    /// <summary>
    /// 读取响应的 JSON 并反序列化，<see cref="ApiResponse"/>
    /// </summary>
    /// <param name="rsp"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static Task<ApiResponse> ReadFromJsonAsync(HttpResponseMessage rsp, CancellationToken cancellationToken = default)
        => ReadFromJsonCoreAsync<ApiResponse>(false, rsp, cancellationToken);

    /// <summary>
    /// 处理 401 未授权，刷新 Token 并重试一次
    /// </summary>
    /// <param name="rsp"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async Task<bool> HandleUnauthorized(HttpResponseMessage rsp, CancellationToken cancellationToken = default)
    {
        if (rsp.StatusCode == HttpStatusCode.Unauthorized)
        {
            var token = await auth.GetTokenAsync(cancellationToken);
            var isSuccess = await auth.RefreshTokenAsync(token, cancellationToken);
            if (isSuccess) return true;
            auth.NavigateToLogin(isUnauthorized: true, setRedirectUrl: true);
        }
        return false;
    }

    /// <summary>
    /// 处理错误消息
    /// </summary>
    /// <param name="rsp"></param>
    public static async void HandleErrorMessage(IApiResponse rsp, MessageService message)
    {
        if (!rsp.IsSuccess)
        {
            // https://antblazor.com/zh-CN/components/message
            await message.Error(rsp.GetErrorMessage());
        }
    }

    void HandleErrorMessage(IApiResponse rsp) => HandleErrorMessage(rsp, message);

    static string[] GetMessages(Exception ex) => new[] { ex.ToString(), };

    async Task<ApiResponse<TResponseModel>> ApiPostCoreAsync<TRequestModel, TResponseModel>(bool isFirst, string apiRelativeUrl, TRequestModel req, bool allowNull, CancellationToken cancellationToken)
    {
        try
        {
            await SetAuthorizationAsync(cancellationToken);
            var rsp = await client.PostAsJsonAsync(apiRelativeUrl, req, cancellationToken: cancellationToken);
            var isRepeat = isFirst && await HandleUnauthorized(rsp, cancellationToken);
            if (isRepeat) // 重试一次
            {
                return await ApiPostCoreAsync<TRequestModel, TResponseModel>(false, apiRelativeUrl, req, allowNull, cancellationToken);
            }
            var apiRsp = await ReadFromJsonAsync<TResponseModel>(allowNull, rsp, cancellationToken: cancellationToken);
            HandleErrorMessage(apiRsp);
            return apiRsp;
        }
        catch (Exception ex)
        {
            return new ApiResponse<TResponseModel>
            {
                IsSuccess = false,
                Messages = GetMessages(ex),
            };
        }
    }

    public Task<ApiResponse<TResponseModel>> ApiPostAsync<TRequestModel, TResponseModel>(string apiRelativeUrl, TRequestModel req, bool allowNull = false, CancellationToken cancellationToken = default)
        => ApiPostCoreAsync<TRequestModel, TResponseModel>(true, apiRelativeUrl, req, allowNull, cancellationToken);

    async Task<ApiResponse<TResponseModel>> ApiGetCoreAsync<TResponseModel>(bool isFirst, string apiRelativeUrl, bool allowNull, CancellationToken cancellationToken)
    {
        try
        {
            await SetAuthorizationAsync(cancellationToken);
            var rsp = await client.GetAsync(apiRelativeUrl, cancellationToken: cancellationToken);
            var isRepeat = isFirst && await HandleUnauthorized(rsp, cancellationToken);
            if (isRepeat) // 重试一次
            {
                return await ApiGetCoreAsync<TResponseModel>(false, apiRelativeUrl, allowNull, cancellationToken);
            }
            var apiRsp = await ReadFromJsonAsync<TResponseModel>(allowNull, rsp, cancellationToken: cancellationToken);
            HandleErrorMessage(apiRsp);
            return apiRsp;
        }
        catch (Exception ex)
        {
            return new ApiResponse<TResponseModel>
            {
                IsSuccess = false,
                Messages = GetMessages(ex),
            };
        }
    }

    public Task<ApiResponse<TResponseModel>> ApiGetAsync<TResponseModel>(string apiRelativeUrl, bool allowNull = false, CancellationToken cancellationToken = default)
        => ApiGetCoreAsync<TResponseModel>(true, apiRelativeUrl, allowNull, cancellationToken);

    async Task<ApiResponse> ApiDeleteCoreAsync(bool isFirst, string apiRelativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            await SetAuthorizationAsync(cancellationToken);
            var rsp = await client.DeleteAsync(apiRelativeUrl, cancellationToken: cancellationToken);
            var isRepeat = isFirst && await HandleUnauthorized(rsp, cancellationToken);
            if (isRepeat) // 重试一次
            {
                return await ApiDeleteCoreAsync(false, apiRelativeUrl, cancellationToken);
            }
            var apiRsp = await ReadFromJsonAsync(rsp, cancellationToken: cancellationToken);
            HandleErrorMessage(apiRsp);
            return apiRsp;
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Messages = GetMessages(ex),
            };
        }
    }

    public Task<ApiResponse> ApiDeleteAsync(string apiRelativeUrl, CancellationToken cancellationToken = default)
        => ApiDeleteCoreAsync(true, apiRelativeUrl, cancellationToken);

    async Task<ApiResponse> ApiPutCoreAsync(bool isFirst, string apiRelativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            await SetAuthorizationAsync(cancellationToken);
            var rsp = await client.PutAsync(apiRelativeUrl, content: null, cancellationToken: cancellationToken);
            var isRepeat = isFirst && await HandleUnauthorized(rsp, cancellationToken);
            if (isRepeat) // 重试一次
            {
                return await ApiPutCoreAsync(false, apiRelativeUrl, cancellationToken);
            }
            var apiRsp = await ReadFromJsonAsync(rsp, cancellationToken: cancellationToken);
            HandleErrorMessage(apiRsp);
            return apiRsp;
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Messages = GetMessages(ex),
            };
        }
    }

    public Task<ApiResponse> ApiPutAsync(string apiRelativeUrl, CancellationToken cancellationToken = default)
        => ApiPutCoreAsync(true, apiRelativeUrl, cancellationToken);

    async Task<ApiResponse> ApiPostCoreAsync<TRequestModel>(bool isFirst, string apiRelativeUrl, TRequestModel req, CancellationToken cancellationToken)
    {
        try
        {
            await SetAuthorizationAsync(cancellationToken);
            var rsp = await client.PostAsJsonAsync(apiRelativeUrl, req, cancellationToken: cancellationToken);
            var isRepeat = isFirst && await HandleUnauthorized(rsp, cancellationToken);
            if (isRepeat) // 重试一次
            {
                return await ApiPostCoreAsync(false, apiRelativeUrl, req, cancellationToken);
            }
            var apiRsp = await ReadFromJsonAsync(rsp, cancellationToken: cancellationToken);
            HandleErrorMessage(apiRsp);
            return apiRsp;
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Messages = GetMessages(ex),
            };
        }
    }

    public Task<ApiResponse> ApiPostAsync<TRequestModel>(string apiRelativeUrl, TRequestModel req, CancellationToken cancellationToken = default)
        => ApiPostCoreAsync(true, apiRelativeUrl, req, cancellationToken);

    async Task<ApiResponse> ApiPutCoreAsync<TRequestModel>(bool isFirst, string apiRelativeUrl, TRequestModel req, CancellationToken cancellationToken)
    {
        try
        {
            await SetAuthorizationAsync(cancellationToken);
            var rsp = await client.PutAsJsonAsync(apiRelativeUrl, req, cancellationToken: cancellationToken);
            var isRepeat = isFirst && await HandleUnauthorized(rsp, cancellationToken);
            if (isRepeat) // 重试一次
            {
                return await ApiPutCoreAsync(false, apiRelativeUrl, req, cancellationToken);
            }
            var apiRsp = await ReadFromJsonAsync(rsp, cancellationToken: cancellationToken);
            HandleErrorMessage(apiRsp);
            return apiRsp;
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Messages = GetMessages(ex),
            };
        }
    }

    public Task<ApiResponse> ApiPutAsync<TRequestModel>(string apiRelativeUrl, TRequestModel req, CancellationToken cancellationToken = default)
        => ApiPutCoreAsync(true, apiRelativeUrl, req, cancellationToken);

    public Task<ApiResponse> ApiPostAsync(string apiRelativeUrl, CancellationToken cancellationToken = default)
        => ApiPostCoreAsync(true, apiRelativeUrl, cancellationToken);

    async Task<ApiResponse> ApiPostCoreAsync(bool isFirst, string apiRelativeUrl, CancellationToken cancellationToken)
    {
        try
        {
            await SetAuthorizationAsync(cancellationToken);
            var rsp = await client.PostAsync(apiRelativeUrl, content: null, cancellationToken: cancellationToken);
            var isRepeat = isFirst && await HandleUnauthorized(rsp, cancellationToken);
            if (isRepeat) // 重试一次
            {
                return await ApiPostCoreAsync(false, apiRelativeUrl, cancellationToken);
            }
            var apiRsp = await ReadFromJsonAsync(rsp, cancellationToken: cancellationToken);
            HandleErrorMessage(apiRsp);
            return apiRsp;
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Messages = GetMessages(ex),
            };
        }
    }
}
