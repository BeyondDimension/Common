#pragma warning disable SA1600 // Elements should be documented

namespace System.Diagnostics;

/// <summary>
/// 在调试器变量窗口中的显示方式
/// <para>https://docs.microsoft.com/zh-cn/dotnet/api/system.diagnostics.debuggerdisplayattribute</para>
/// </summary>
public interface IDebuggerDisplay
{
    string? GetDebuggerDisplayValue(object obj);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    public static string? GetValue(object obj)
    {
        try
        {
            var debuggerDisplay = Ioc.Get<IDebuggerDisplay>();
            return debuggerDisplay.GetDebuggerDisplayValue(obj);
        }
        catch
        {
            return obj.ToString();
        }
    }
#endif
}