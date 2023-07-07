namespace BD.Common.Services;

/// <summary>
/// Toast 显示拦截
/// </summary>
public interface IToastIntercept
{
    /// <summary>
    /// 显示 Toast 消息前拦截
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    bool OnShowExecuting(ToastIcon icon, string text, int? duration = null);
}