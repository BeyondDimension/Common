namespace BD.Common.Services.Implementation;

public sealed class AuthService : HttpClientWrapper, IAuthService
{
    readonly ILocalStorageService storage;
    readonly NavigationManager nav;
    readonly MessageService message;

    /// <summary>
    /// 用于存储 Token 的键
    /// </summary>
    const string key_token = nameof(JWTEntity);

    const string loginUrl = "/user/login";

    public void NavigateToLogin(bool isUnauthorized, bool setRedirectUrl)
    {
        var redirectUrl = new Uri(nav.Uri).PathAndQuery;
        if (redirectUrl.StartsWith(loginUrl, StringComparison.OrdinalIgnoreCase)) return;
        string url;
        if (isUnauthorized)
        {
            if (setRedirectUrl && GetRedirectUrl(redirectUrl, out var u))
            {
                url = $"{loginUrl}?e=401&u={u}";
            }
            else
            {
                url = $"{loginUrl}?e=401";
            }
        }
        else
        {
            if (setRedirectUrl && GetRedirectUrl(redirectUrl, out var u))
            {
                url = $"{loginUrl}?u={u}";
            }
            else
            {
                url = loginUrl;
            }
        }
        nav.NavigateTo(url, forceLoad: true);

        static bool GetRedirectUrl(string redirectUrl, [NotNullWhen(true)] out string? value)
        {
            value = null;
            if (redirectUrl == "/") return false;
            value = WebUtility.UrlEncode(redirectUrl);
            return true;
        }
    }

    public void NavigateToRoot() => nav.NavigateTo("/", forceLoad: true);

    public AuthService(HttpClient client, ILocalStorageService storage, NavigationManager nav, MessageService message) : base(client)
    {
        this.storage = storage;
        this.nav = nav;
        this.message = message;
    }

    async ValueTask<bool> SetAuthorizationReturnIsAuthenticatedAsync(CancellationToken cancellationToken)
    {
        var token = await GetTokenAsync(cancellationToken);
        SetAuthorization(token);
        return token != null;
    }

    static Task<CurrentUserInfoDTO>? mUserInfo;
    static readonly object lock_userInfo = new();

    public Task<CurrentUserInfoDTO> GetUserInfoAsync(CancellationToken cancellationToken)
    {
        lock (lock_userInfo)
        {
            mUserInfo ??= GetUserInfoCoreAsync(cancellationToken);
        }
        return mUserInfo;
    }

    async void ShowErrorMessage(string errorMessage)
    {
        // https://antblazor.com/zh-CN/components/message
        await message.Error(errorMessage);
    }

    async Task<CurrentUserInfoDTO> GetUserInfoCoreAsync(CancellationToken cancellationToken)
    {
        var isAuthenticated = await SetAuthorizationReturnIsAuthenticatedAsync(cancellationToken);
        if (isAuthenticated)
        {
            const string apiRelativeUrl = "/api/user";
            var rsp = await client.GetAsync(apiRelativeUrl, cancellationToken);
            if (rsp.IsSuccessStatusCode)
            {
                var data = await rsp.Content.ReadFromJsonAsync<ApiResponse<CurrentUserInfoDTO>>(cancellationToken: cancellationToken);
                if (data != null && data.IsSuccess && data.Data != null)
                {
                    return data.Data;
                }
                else
                {
                    ShowErrorMessage($"获取当前登录用户信息失败，错误：{data?.GetErrorMessage()}");
                }
            }
            else
            {
                ShowErrorMessage($"获取当前登录用户信息失败，状态码：{rsp.StatusCode}");
            }
        }
        return new();
    }

    async Task<ApiResponse<TResponseModel>> ApiPostAsync<TRequestModel, TResponseModel>(string apiRelativeUrl, TRequestModel req, bool allowNull = false, CancellationToken cancellationToken = default)
    {
        var rsp = await client.PostAsJsonAsync(apiRelativeUrl, req, cancellationToken: cancellationToken);
        return await ApiConnection.ReadFromJsonAsync<TResponseModel>(allowNull, rsp, cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<JWTEntity>> LoginAsync(string userName, string password, string? redirectUrl, CancellationToken cancellationToken)
    {
        const string apiRelativeUrl = "/api/login";
        var req = new[] { userName, password };
        var rsp = await ApiPostAsync<string[], JWTEntity>(apiRelativeUrl, req, cancellationToken: cancellationToken);
        if (rsp.IsSuccess && rsp.Data != null)
        {
            await SetTokenAsync(rsp.Data, cancellationToken);
            if (redirectUrl != null && redirectUrl.StartsWith('/'))
            {
                nav.NavigateTo(redirectUrl);
            }
            else
            {
                NavigateToRoot();
            }
        }
        return rsp;
    }

    public async ValueTask LogoutAsync(CancellationToken cancellationToken)
    {
        await storage.RemoveItemAsync(key_token, cancellationToken);
        mUserInfo = null;
        NavigateToLogin(false, false);
    }

    public async ValueTask<JWTEntity?> GetTokenAsync(CancellationToken cancellationToken)
        => await storage.GetItemAsync<JWTEntity?>(key_token, cancellationToken);

    public async ValueTask<NullableBoolean> IsAuthenticatedAsync(JWTEntity? token, CancellationToken cancellationToken)
    {
        if (token == null) return NullableBoolean.Null;
        if (token.ExpiresIn > DateTimeOffset.UtcNow) return NullableBoolean.True;
        var isSuccess = await RefreshTokenAsync(token, cancellationToken);
        return isSuccess ? NullableBoolean.True : NullableBoolean.False;
    }

    public async Task<bool> RefreshTokenAsync(JWTEntity? token, CancellationToken cancellationToken)
    {
        var refreshToken = token?.RefreshToken;
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var rsp_token = await RefreshTokenAsync(refreshToken, cancellationToken);
            if (rsp_token.IsSuccess && rsp_token.Data != null)
            {
                await SetTokenAsync(rsp_token.Data, cancellationToken);
                return true;
            }
        }
        return false;
    }

    public async Task<ApiResponse<JWTEntity?>> RefreshTokenAsync(string refresh_token, CancellationToken cancellationToken)
    {
        string apiRelativeUrl = $"/api/login/{refresh_token}";
        var rsp = await client.PutAsync(apiRelativeUrl, content: null, cancellationToken: cancellationToken);
        var token = await ApiConnection.ReadFromJsonAsync<JWTEntity?>(allowNull: false, rsp: rsp, cancellationToken: cancellationToken);
        return token;
    }

    public ValueTask SetTokenAsync(JWTEntity token, CancellationToken cancellationToken = default)
    {
        return storage.SetItemAsync(key_token, token, cancellationToken);
    }
}
