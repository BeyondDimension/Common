namespace BD.Common.Services;

/// <summary>
/// Toast 属于一种轻量级的反馈，常常以小弹框的形式出现，一般出现 2 秒或 3.5 秒后会自动消失，可以出现在屏幕上中下任意位置，但同个产品会模块尽量使用同一位置，让用户产生统一认知
/// </summary>
public interface IToast
{
    /// <inheritdoc cref="Show(ToastIcon, string, ToastLength)"/>
    [Obsolete("use Show(ToastIcon, string, int?)")]
    void Show(string text, int? duration = null) => Show(ToastIcon.None, text, duration);

    /// <inheritdoc cref="Show(ToastIcon, string, ToastLength)"/>
    [Obsolete("use Show(ToastIcon, string, ToastLength)")]
    void Show(string text, ToastLength duration) => Show(ToastIcon.None, text, duration);

    /// <summary>
    /// 显示 Toast
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    void Show(ToastIcon icon, string text, int? duration = null);

    /// <inheritdoc cref="Show(ToastIcon, string, ToastLength)"/>
    void Show(ToastIcon icon, string text, ToastLength duration);
}
