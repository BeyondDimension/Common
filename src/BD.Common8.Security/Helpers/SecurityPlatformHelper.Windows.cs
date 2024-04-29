#if WINDOWS
namespace BD.Common8.Security.Helpers;

static partial class SecurityPlatformHelper
{
    /// <summary>
    /// 获取 MachineGuid
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [SupportedOSPlatform("windows")]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetMachineGuid()
    {
        // 禁止反射调用此函数
        Assembly? assembly = null;
        bool b = false;
        try
        {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var caller = new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType;
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            assembly = caller?.Assembly;
            b = true;
        }
        catch
        {
        }
        if (b && typeof(MachineUniqueIdentifier).Assembly != assembly)
            throw new InvalidOperationException("Direct invocation of this method is not allowed.");

        var rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
        if (rk != null)
        {
            var value = rk.GetValue("MachineGuid")?.ToString();
            rk.Close();
            return value ?? string.Empty;
        }
        return string.Empty;
    }
}
#endif