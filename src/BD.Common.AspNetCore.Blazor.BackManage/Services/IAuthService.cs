namespace BD.Common.Services;

/// <summary>
/// 授权服务
/// </summary>
/// <typeparam name="TBMMeInfoDTO"></typeparam>
public interface IAuthService<TBMMeInfoDTO> where TBMMeInfoDTO : BMMeInfoDTO
{
    static event Action<TBMMeInfoDTO>? OnUserInfoChanged;

    static void UserInfoChanged(TBMMeInfoDTO userInfo) => OnUserInfoChanged?.Invoke(userInfo);

    /// <summary>
    /// 获取当前登录的后台用户资料
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TBMMeInfoDTO> GetUserInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="redirectUrl"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse<JWTEntity>> LoginAsync(string userName, string password, string? redirectUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前是否已登录
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<NullableBoolean> IsAuthenticatedAsync(JWTEntity? token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前 Token
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<JWTEntity?> GetTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 跳转到登录页面
    /// </summary>
    /// <param name="isUnauthorized"></param>
    /// <param name="setRedirectUrl"></param>
    void NavigateToLogin(bool isUnauthorized = false, bool setRedirectUrl = false);

    /// <summary>
    /// 跳转到根目录
    /// </summary>
    void NavigateToRoot();

    /// <summary>
    /// 刷新 Token
    /// </summary>
    /// <param name="refresh_token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiResponse<JWTEntity?>> RefreshTokenAsync(string refresh_token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将 Token 保存到本地存储中
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask SetTokenAsync(JWTEntity token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刷新 Token
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> RefreshTokenAsync(JWTEntity? token, CancellationToken cancellationToken);
}
