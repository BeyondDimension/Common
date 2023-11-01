namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 电子邮件，参考 Essentials.Email。
/// <para>https://docs.microsoft.com/zh-cn/xamarin/essentials/email</para>
/// <para>https://github.com/xamarin/Essentials/blob/main/Xamarin.Essentials/Email/Email.shared.cs</para>
/// </summary>
public static class Email2
{
    /// <summary>
    /// 打开默认电子邮件客户端以允许用户发送邮件。
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask ComposeAsync()
        => ComposeAsync(null);

    /// <summary>
    /// 打开默认的电子邮件客户端，允许用户发送带有所提供主题、正文和收件人的邮件。
    /// </summary>
    /// <param name="subject">电子邮件主题。</param>
    /// <param name="body">电子邮件正文。</param>
    /// <param name="to">电子邮件收件人。</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask ComposeAsync(string subject, string body, params string[] to)
        => ComposeAsync(new EmailMessage(subject, body, to));

    /// <summary>
    /// 打开默认电子邮件客户端以允许用户发送邮件。
    /// </summary>
    /// <param name="message">电子邮件。</param>
    /// <returns></returns>
    public static async ValueTask ComposeAsync(EmailMessage? message)
    {
        var s = IEmailPlatformService.Instance;
        if (s != null)
        {
            try
            {
                await s.PlatformComposeAsync(message);
            }
            catch (Exception e)
            {
                HandlerException(e);
            }
        }
        await ComposeByProcessAsync(message);
    }

    /// <summary>
    /// 通过进程异步发送电子邮件
    /// </summary>
    static ValueTask ComposeByProcessAsync(EmailMessage? message)
    {
        try
        {
            var uri = GetMailToUri(message);
            Process.Start(new ProcessStartInfo
            {
                FileName = uri,
                UseShellExecute = true,
            });
        }
        catch (Exception e)
        {
            HandlerException(e);
        }
        return default;
    }

    /// <summary>
    /// 获取电子邮件地址的收件人 URI
    /// </summary>
    public static string GetMailToUri(EmailMessage? message)
    {
        if (message != null && message.BodyFormat != EmailBodyFormat.PlainText)
            throw new ApplicationException("Only EmailBodyFormat.PlainText is supported if no email account is set up.");

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(message?.Body))
            parts.Add("body=" + Uri.EscapeDataString(message.Body));
        if (!string.IsNullOrEmpty(message?.Subject))
            parts.Add("subject=" + Uri.EscapeDataString(message.Subject));
        if (message?.Cc?.Count > 0)
            parts.Add("cc=" + Uri.EscapeDataString(string.Join(",", message.Cc)));
        if (message?.Bcc?.Count > 0)
            parts.Add("bcc=" + Uri.EscapeDataString(string.Join(",", message.Bcc)));

        var uri = "mailto:";

        if (message?.To?.Count > 0)
            uri += Uri.EscapeDataString(string.Join(",", message.To));

        if (parts.Count > 0)
            uri += "?" + string.Join("&", parts);

        return uri;
    }

    /// <summary>
    /// 异常事件
    /// </summary>
    public static event Action<Exception>? OnError;

    /// <summary>
    /// 标签名称
    /// </summary>
    const string TAG = nameof(Email2);

    /// <summary>
    /// 异常处理方法
    /// </summary>
    static void HandlerException(Exception e)
    {
        if (OnError == null)
        {
            try
            {
                e.LogAndShow(TAG);
            }
            catch
            {
            }
        }
        else
        {
            OnError(e);
        }
    }
}