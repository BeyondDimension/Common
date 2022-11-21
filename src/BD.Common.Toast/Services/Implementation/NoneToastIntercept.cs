namespace BD.Common.Services.Implementation;

sealed class NoneToastIntercept : IToastIntercept
{
    bool IToastIntercept.OnShowExecuting(string text)
    {
        return false;
    }
}