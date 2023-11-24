namespace System.Diagnostics;

/// <summary>
/// 在调试器变量窗口中的显示方式
/// <para>https://docs.microsoft.com/zh-cn/dotnet/api/system.diagnostics.debuggerdisplayattribute</para>
/// </summary>
public interface IDebuggerDisplay
{
    /// <summary>
    /// 获取调试器显示值
    /// </summary>
    string? GetDebuggerDisplayValue(object obj);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER

    /// <summary>
    /// 根据 <paramref name="obj"/> 获取相应的调试显示值，如果无法获取调试显示值，则返回字符串表示形式
    /// </summary>
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