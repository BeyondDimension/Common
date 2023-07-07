namespace BD.Common.Services.Implementation;

sealed class NoneToastIntercept : IToastIntercept
{
    bool IToastIntercept.OnShowExecuting(ToastIcon icon, string text, int? duration)
    {
        return false;
    }
}