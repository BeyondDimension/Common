namespace BD.Common8.Security.Helpers;

static partial class SecurityPlatformHelper
{
    /// <summary>
    /// 获取 MachineId
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetEtcMachineId()
    {
        // 禁止反射调用此函数
        var caller = new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType;
        if (typeof(MachineUniqueIdentifier).Assembly != caller?.Assembly)
            throw new InvalidOperationException("Direct invocation of this method is not allowed.");

        const string filePath = "/etc/machine-id";
        var value = File.ReadAllText(filePath);
        return value;
    }
}