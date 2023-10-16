namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="CultureInfo"/> 类型的扩展函数
/// </summary>
public static partial class CultureInfoExtensions
{
    /// <summary>
    /// 判断 <see cref="CultureInfo"/> 是否指定的匹配区域字符串
    /// </summary>
    /// <param name="cultureInfo"></param>
    /// <param name="cultureName"></param>
    /// <returns></returns>
    public static bool IsMatch(this CultureInfo cultureInfo, string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureInfo.Name))
            return false;
        if (string.Equals(cultureInfo.Name, cultureName, StringComparison.OrdinalIgnoreCase))
            return true;
        else
            return cultureInfo.Parent.IsMatch(cultureName);
    }

    /// <summary>
    /// 根据 <see cref="CultureInfo"/> 获取 HTTP 头 Accept-Language 值
    /// <para>Accept-Language 请求头允许客户端声明它可以理解的自然语言，以及优先选择的区域方言。借助内容协商机制，服务器可以从诸多备选项中选择一项进行应用，并使用 Content-Language 应答头通知客户端它的选择。浏览器会基于其用户界面语言为这个请求头设置合适的值，即便是用户可以进行修改，但是这种情况极少发生（因为可增加指纹独特性，通常也不被鼓励）。</para>
    /// </summary>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static string GetAcceptLanguage(this CultureInfo culture)
    {
        if (culture.IsMatch("zh-Hans"))
            return "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7";
        else if (culture.IsMatch("zh-Hant"))
            return "zh-HK,zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7";
        else if (culture.IsMatch("ko"))
            return "ko;q=0.9,en-US;q=0.8,en;q=0.7";
        else if (culture.IsMatch("ja"))
            return "ja;q=0.9,en-US;q=0.8,en;q=0.7";
        else if (culture.IsMatch("ru"))
            return "ru;q=0.9,en-US;q=0.8,en;q=0.7";
        else
            return "en-US;q=0.9,en;q=0.8";
    }

    // https://docs.microsoft.com/zh-cn/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c
}