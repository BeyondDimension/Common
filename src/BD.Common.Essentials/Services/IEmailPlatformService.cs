namespace BD.Common.Services;

interface IEmailPlatformService
{
    static IEmailPlatformService? Instance => Ioc.Get_Nullable<IEmailPlatformService>();

    Task PlatformComposeAsync(EmailMessage? message);
}