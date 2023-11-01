namespace BD.Common8.SmsSender.Models.SmsSender.Abstractions;

/// <summary>
/// 定义了一个短信配置
/// </summary>
public interface ISmsSettings
{
    /// <summary>
    /// 是否使用不发送的短信渠道
    /// <para>默认值：<see langword="null"/></para>
    /// <para>当 <see cref="IHostingEnvironment.EnvironmentName"/> == <see cref="EnvironmentName.Development"/> 时并保持默认值，将使用 <see cref="DebugSmsSenderProvider"/></para>
    /// <para>当 <see cref="IHostingEnvironment.EnvironmentName"/> != <see cref="EnvironmentName.Development"/> 时并保持默认值，将使用配置的正式渠道</para>
    /// </summary>
    bool? UseDebugSmsSender { get; set; }

    /// <summary>
    /// 短信平台配置项
    /// </summary>
    SmsOptions? SmsOptions { get; }
}