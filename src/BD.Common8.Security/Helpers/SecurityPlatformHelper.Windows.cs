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