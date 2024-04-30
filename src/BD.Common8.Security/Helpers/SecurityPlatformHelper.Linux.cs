#if LINUX
namespace BD.Common8.Security.Helpers;

static partial class SecurityPlatformHelper
{
    /// <summary>
    /// 获取 MachineId
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
#if !LINUX
    [SupportedOSPlatform("linux")]
#endif
    public static string GetEtcMachineId()
    {
        const string filePath = "/etc/machine-id";
        var value = File.ReadAllText(filePath);
        return value;
    }
}
#endif