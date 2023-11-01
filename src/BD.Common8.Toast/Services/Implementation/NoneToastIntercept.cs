namespace BD.Common8.Toast.Services.Implementation;

/// <summary>
/// 不拦截 Toast 的实现类
/// </summary>
sealed class NoneToastIntercept : IToastIntercept
{
    /// <summary>
    /// 在显示 Toast 之前执行操作，以确定是否拦截该 Toast
    /// </summary>
    bool IToastIntercept.OnShowExecuting(ToastIcon icon, string text, int? duration)
    {
        return false;
    }
}