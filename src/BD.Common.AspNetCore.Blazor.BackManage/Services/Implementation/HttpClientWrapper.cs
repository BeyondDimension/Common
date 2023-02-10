namespace BD.Common.Services.Implementation;

public abstract class HttpClientWrapper : HttpClientServiceBaseImpl
{
    protected HttpClientWrapper(HttpClient client) : base(client)
    {
    }

    /// <summary>
    /// 根据 JWT 值获取请求头值
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static AuthenticationHeaderValue? GetAuthorizationValue(string? accessToken)
    {
        AuthenticationHeaderValue? authorization = accessToken == null ? null :
         new(JwtBearerDefaults.AuthenticationScheme, accessToken);
        return authorization;
    }

    /// <summary>
    /// 根据 JWT 模型获取请求头值
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static AuthenticationHeaderValue? GetAuthorizationValue(JWTEntity? token)
    {
        AuthenticationHeaderValue? authorization = token == null ? null : GetAuthorizationValue(token.AccessToken);
        return authorization;
    }

    /// <summary>
    /// 根据 JWT 模型设置请求头
    /// </summary>
    /// <param name="headers"></param>
    /// <param name="token"></param>
    public static void SetAuthorization(IDictionary<string, string> headers, JWTEntity? token)
    {
        var authorization = GetAuthorizationValue(token)?.ToString();
        SetAuthorization(headers, authorization);
    }

    /// <summary>
    /// 根据 JWT 值设置请求头
    /// </summary>
    /// <param name="headers"></param>
    /// <param name="authorization"></param>
    public static void SetAuthorization(IDictionary<string, string> headers, string? authorization)
    {
        const string Authorization = nameof(HttpRequestHeaders.Authorization);
        if (authorization != null)
        {
            if (headers.ContainsKey(Authorization))
            {
                headers[Authorization] = authorization.ToString();
            }
            else
            {
                headers.Add(Authorization, authorization.ToString());
            }
        }
        else
        {
            if (headers.ContainsKey(Authorization))
            {
                headers.Remove(Authorization);
            }
        }
    }

    protected void SetAuthorization(JWTEntity? token)
    {
        var authorization = GetAuthorizationValue(token);
        if (client.DefaultRequestHeaders.Authorization != authorization)
            client.DefaultRequestHeaders.Authorization = authorization;
    }
}
