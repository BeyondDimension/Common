namespace BD.Common8.SmsSender.Models.SmsSender;

/// <summary>
/// 定义泛型短信模板选项
/// </summary>
/// <typeparam name="T"></typeparam>
public class SmsOptionsTemplateId<T>
{
    /// <summary>
    /// 开发者平台分配的模板标志
    /// </summary>
    public T? Template { get; set; }

    /// <summary>
    /// 用于发送验证码的用途
    /// </summary>
    public int Type { get; set; }
}