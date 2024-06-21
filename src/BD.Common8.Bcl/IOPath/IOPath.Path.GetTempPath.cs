namespace System;

public static partial class IOPath
{
    /// <inheritdoc cref="Path.GetTempPath"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetTempPath()
    {
        var result = Path.GetTempPath();
#if WINDOWS || NETFRAMEWORK
        //if (OperatingSystem.IsWindows())
        {
            var windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            if (result.StartsWith(windowsPath, StringComparison.OrdinalIgnoreCase))
            {
                // Win 上在某些情况下返回 C:\Windows\Temp，然后没有权限写入抛出异常
                // 疑似环境变量被修改
                var windowsPathRoot = Path.GetPathRoot(windowsPath);
                windowsPathRoot.ThrowIsNull();
                result = Path.Combine(windowsPathRoot,
                    "Users",
                    Environment.UserName,
                    "AppData",
                    "Local",
                    "Temp");
            }
        }
#endif
        return result;
    }
}