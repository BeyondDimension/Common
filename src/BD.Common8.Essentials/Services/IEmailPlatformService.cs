namespace BD.Common8.Essentials.Services;

/// <summary>
/// 邮件平台服务接口
/// </summary>
public interface IEmailPlatformService
{
    /// <summary>
    /// 获取 <see cref="IEmailPlatformService"/> 实例
    /// </summary>
    static IEmailPlatformService? Instance => Ioc.Get_Nullable<IEmailPlatformService>();

    /// <summary>
    /// 用于向指定的收信人发送邮件
    /// </summary>
    ValueTask PlatformComposeAsync(EmailMessage? message);
}