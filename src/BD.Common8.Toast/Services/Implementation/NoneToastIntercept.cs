namespace BD.Common8.Toast.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

sealed class NoneToastIntercept : IToastIntercept
{
    bool IToastIntercept.OnShowExecuting(ToastIcon icon, string text, int? duration)
    {
        return false;
    }
}