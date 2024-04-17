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
        var caller = new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType;
        if (typeof(MachineUniqueIdentifier).Assembly != caller?.Assembly)
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