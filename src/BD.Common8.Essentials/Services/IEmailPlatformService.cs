namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented

public interface IEmailPlatformService
{
    static IEmailPlatformService? Instance => Ioc.Get_Nullable<IEmailPlatformService>();

    ValueTask PlatformComposeAsync(EmailMessage? message);
}