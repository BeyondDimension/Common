namespace BD.Common8.Security;

#pragma warning disable SA1600 // Elements should be documented

public static partial class MachineUniqueIdentifier
{
    static AESUtils.KeyIV GetMachineSecretKey(string? value)
    {
        value ??= string.Empty;
        var result = AESUtils.GetParameters(value);
        return result;
    }

    static Lazy<AESUtils.KeyIV> GetMachineSecretKey(Func<string?> action) => new(() =>
    {
        string? value = null;
        try
        {
            value = action();
        }
        catch (Exception e)
        {
            Log.Warn(nameof(MachineUniqueIdentifier), e, "GetMachineSecretKey fail.");
        }
        if (string.IsNullOrWhiteSpace(value))
        {
            value = Environment.MachineName;
        }
        return GetMachineSecretKey(value);
    });

    static AESUtils.KeyIV GetMachineSecretKeyBySecureStorage()
    {
        const string KEY_MACHINE_SECRET = "KEY_MACHINE_SECRET_2105";
        var guid = GetMachineSecretKeyGuid();
        static Guid GetMachineSecretKeyGuid()
        {
            var secureStorage = ISecureStorage.Instance;
            Func<Task<string?>> getAsync = () => secureStorage.GetAsync(KEY_MACHINE_SECRET);
            var guidStr = getAsync.RunSync();
            if (Guid.TryParse(guidStr, out var guid))
                return guid;
            guid = Guid.NewGuid();
            guidStr = guid.ToString();
            Func<Task> setAsync = () => secureStorage.SetAsync(KEY_MACHINE_SECRET, guidStr);
            setAsync.RunSync();
            return guid;
        }
        var r = AESUtils.GetParameters(guid.ToByteArray());
        return r;
    }

    static readonly Lazy<AESUtils.KeyIV> mMachineSecretKey;

    static MachineUniqueIdentifier()
    {
        if (OperatingSystem.IsWindows())
        {
            mMachineSecretKey = GetMachineSecretKey(GetMachineGuid);
        }
        else if (OperatingSystem.IsMacOS())
        {
            mMachineSecretKey = GetMachineSecretKey(GetIOPlatformSerialNumber);
        }
        else if (OperatingSystem.IsLinux())
        {
            mMachineSecretKey = GetMachineSecretKey(GetEtcMachineId);
        }
        else
        {
            mMachineSecretKey = new(GetMachineSecretKeyBySecureStorage);
        }
    }

    public static AESUtils.KeyIV Value => mMachineSecretKey.Value;
}
