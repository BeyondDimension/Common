namespace BD.Common8.SmsSender.Models.SmsSender.Abstractions;

/// <summary>
/// 提供帮助管理和判断短信渠道
/// </summary>
public abstract class ChannelSmsOptions : JsonModel, IDisable, IExplicitHasValue
{
    /// <summary>
    /// 获取或设置是否禁用短信渠道
    /// </summary>
    public bool Disable { get; set; }

    /// <summary>
    /// 验证短信渠道选项是否有效
    /// </summary>
    public virtual bool IsValid() => !Disable;

    /// <summary>
    /// 获取短信渠道选项是否具有值
    /// </summary>
    bool IExplicitHasValue.ExplicitHasValue() => IsValid();
}
