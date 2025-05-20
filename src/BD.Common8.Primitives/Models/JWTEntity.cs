using BD.Common8.Models.Abstractions;

namespace BD.Common8.Models;

/// <summary>
/// JWT 值
/// </summary>
[global::MessagePack.MessagePackObject]
[global::MemoryPack.MemoryPackable(global::MemoryPack.SerializeLayout.Explicit)]
[Serializable]
public sealed partial class JWTEntity : IExplicitHasValue
{
    /// <summary>
    /// 凭证有效期
    /// </summary>
    [global::MessagePack.Key(0)]
    [global::MemoryPack.MemoryPackOrder(0)]
    public DateTimeOffset ExpiresIn { get; set; }

    /// <summary>
    /// 当前凭证
    /// </summary>
    [global::MessagePack.Key(1)]
    [global::MemoryPack.MemoryPackOrder(1)]
    public string? AccessToken { get; set; }

    /// <summary>
    /// 刷新凭证
    /// </summary>
    [global::MessagePack.Key(2)]
    [global::MemoryPack.MemoryPackOrder(2)]
    public string? RefreshToken { get; set; }

    /// <inheritdoc/>
    bool IExplicitHasValue.ExplicitHasValue()
    {
        // 仅数据格式是否正确，不验证时间有效期等业务逻辑
        return !string.IsNullOrEmpty(AccessToken) &&
            !string.IsNullOrEmpty(RefreshToken);
    }

    /// <summary>
    /// 适用于 URL QueryString 传参的 Token
    /// </summary>
    public sealed class QueryString : IExplicitHasValue
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        public long ExpiresIn { get; set; }

        /// <summary>
        /// 请求头中的授权值
        /// </summary>
        public string? Authorization { get; set; }

        /// <inheritdoc/>
        bool IExplicitHasValue.ExplicitHasValue()
        {
            var now = DateTimeOffset.Now;
            var expiresIn = new DateTimeOffset(ExpiresIn, TimeSpan.Zero);
            return now < expiresIn && !string.IsNullOrEmpty(Authorization);
        }

        ///// <summary>
        ///// 将请求头中的 JWT 值转换为 QueryString 值
        ///// </summary>
        ///// <param name="js"></param>
        ///// <param name="authorizationValue"></param>
        ///// <returns></returns>
        //public static async ValueTask<string> ToString(IJSRuntime js, string authorizationValue)
        //{
        //    var token = new QueryString
        //    {
        //        Authorization = authorizationValue,
        //        ExpiresIn = DateTimeOffset.UtcNow.AddMinutes(10D).Ticks,
        //    };
        //    var value = JsonSerializer.Serialize(token);
        //    value = await ServerSecurity.Encrypt(js, value);
        //    return value;
        //}
    }
}