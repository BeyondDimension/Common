// ReSharper disable once CheckNamespace
namespace BD.Common.Services.Implementation.Essentials;

sealed class EmailPlatformServiceImpl : IEmailPlatformService
{
    Task IEmailPlatformService.PlatformComposeAsync(EmailMessage? message)
    {
        var message2 = message.Convert();
        return message2 == null ? Email.ComposeAsync() : Email.ComposeAsync(message2);
    }
}