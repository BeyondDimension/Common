namespace BD.Common.Services;

public interface IEmailPlatformService
{
    static IEmailPlatformService? Instance => Ioc.Get_Nullable<IEmailPlatformService>();

    Task PlatformComposeAsync(EmailMessage? message);
}