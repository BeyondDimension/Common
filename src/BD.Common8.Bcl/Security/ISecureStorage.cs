namespace System.Security;

/// <summary>
/// 为键/值对提供简单的安全存储(值类型泛型与非泛型重载方法不兼容)
/// <para>仅客户端：如果需要存储明文数据或需要同步方法，则可使用首选项(Preferences2)</para>
/// </summary>
/// <remarks>
/// <para>每个平台使用平台提供的本机 API 来安全存储数据：</para>
/// <list type="bullet">
///   <item>
///     <term>iOS：数据存储在 KeyChain 中。有关 SecAccessible 的其他信息</term>
///   </item>
///   <item>
///     <term>Android：加密密钥存储在密钥库中，加密数据存储在命名的共享首选项容器中</term>
///   </item>
///   <item>
///     <term>UWP：数据使用 DataProtectionProvider 加密并存储在命名的 ApplicationDataContainer</term>
///   </item>
///   <item>
///     <term>Windows 10 Desktop：数据使用 DataProtectionProvider 加密并存储在仓储中</term>
///   </item>
///   <item>
///     <term>Other Desktop：数据使用 AES 加密并存储在仓储中</term>
///   </item>
///   <item>
///     <term>AspNetCore：数据存储在仓储中</term>
///   </item>
/// </list>
/// <para>注意：在运行 Android 23（6.0/M）以下的 Android 设备上，KeyStore 中没有 AES 可用。作为最佳实践，此 API 将生成存储在 KeyStore 中的 RSA/ECB/PKCS7Padding 密钥对（KeyStore 中这些较低的API级别支持的唯一类型），用于包装运行时生成的 AES 密钥。这个包装好的密钥存储在首选项中</para>
/// </remarks>
public partial interface ISecureStorage
{
    #region Public API

    /// <summary>
    /// 获取给定键的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>如果键不存在，则返回null</returns>
    async Task<string?> GetAsync(string key)
    {
        if (IsNativeSupportedBytes)
        {
            var bytesValue = await GetBytesAsync(key);
            if (bytesValue == null) return null;
            try
            {
                return Encoding.UTF8.GetString(bytesValue);
            }
            catch
            {
                return null;
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 存储给定键的值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    Task SetAsync(string key, string? value)
    {
        if (IsNativeSupportedBytes)
        {
            var bytesValue = value == null ? null : Encoding.UTF8.GetBytes(value);
            return SetAsync(key, bytesValue);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 移除给定键的键/值对
    /// </summary>
    /// <param name="key">要移除的键</param>
    /// <returns>如果删除了键值对，则返回true</returns>
    Task<bool> RemoveAsync(string key);

    /// <inheritdoc cref="GetAsync(string)"/>
    async Task<TValue?> GetAsync<TValue>(string key)
    {
        try
        {
            if (IsNativeSupportedBytes)
            {
                var bytesValue = await GetBytesAsync(key);
                if (bytesValue == null) return default;
                var value = Serializable.DMP<TValue?>(bytesValue);
                return value;
            }
            else
            {
                var strValue = await GetAsync(key);
                if (strValue == null) return default;
                var value = Serializable.DMPB64U<TValue?>(strValue);
                return value;
            }
        }
        catch (Exception e)
        {
            Log.Error(nameof(ISecureStorage), e, "GetAsync Fail, key: {0}.", key);
            return default;
        }
    }

    /// <inheritdoc cref="SetAsync(string, string?)"/>
    Task SetAsync<TValue>(string key, TValue value)
    {
        if (IsNativeSupportedBytes)
        {
            var bytesValue = Serializable.SMP(value);
            return SetAsync(key, bytesValue);
        }
        else
        {
            var strValue = Serializable.SMPB64U(value);
            return SetAsync(key, strValue);
        }
    }

    /// <summary>
    /// 检查给定键是否存在
    /// </summary>
    Task<bool> ContainsKeyAsync(string key);

    /// <summary>
    /// 获取 <see cref="ISecureStorage"/> 的实例
    /// </summary>
    static ISecureStorage Instance => Ioc.Get<ISecureStorage>();

    #endregion

    /// <summary>
    /// 获取是否原生支持字节数组存储
    /// </summary>
    protected bool IsNativeSupportedBytes { get; }

    /// <inheritdoc cref="GetAsync(string)"/>
    protected async Task<byte[]?> GetBytesAsync(string key)
    {
        if (IsNativeSupportedBytes)
        {
            throw new NotImplementedException();
        }
        else
        {
            var strValue = await GetAsync(key);
            try
            {
                return strValue.Base64UrlDecodeToByteArray_Nullable();
            }
            catch
            {
                return null;
            }
        }
    }

    /// <inheritdoc cref="SetAsync(string, string?)"/>
    protected Task SetAsync(string key, byte[]? value)
    {
        if (IsNativeSupportedBytes)
        {
            throw new NotImplementedException();
        }
        else
        {
            var strValue = value.Base64UrlEncode_Nullable();
            return SetAsync(key, strValue);
        }
    }
}