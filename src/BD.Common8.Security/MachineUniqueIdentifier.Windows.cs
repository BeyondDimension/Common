namespace BD.Common8.Security;

partial class MachineUniqueIdentifier
{
    [SupportedOSPlatform("windows")]
    [MethodImpl(MethodImplOptions.NoInlining)]
    static string GetMachineGuid()
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