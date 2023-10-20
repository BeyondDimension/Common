namespace BD.Common8.Security.Helpers;

/// <summary>
/// 机器识别码助手类
/// </summary>
public static partial class MachineUniqueIdentifier
{
    static AESUtils.KeyIV GetMachineSecretKey(string? value)
    {
        value ??= string.Empty;
        var result = AESUtils.GetParameters(value);
        return result;
    }

    static bool setMachineId;
    static bool isGetMachineSecretKeyLazy;
    static SecureString? machineId;
    static readonly Lazy<SecureString?> _MachineId;

    /// <summary>
    /// 获取机器识别码
    /// </summary>
    public static SecureString? MachineId => _MachineId.Value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    static Lazy<AESUtils.KeyIV> GetMachineSecretKeyLazy(Func<AESUtils.KeyIV> func)
    {
        isGetMachineSecretKeyLazy = true;
        return new(func);
    }

    static Lazy<AESUtils.KeyIV> GetMachineSecretKey(Func<string?> action) => GetMachineSecretKeyLazy(() =>
    {
        string? value = null;
        try
        {
            value = action();
            if (string.IsNullOrWhiteSpace(value))
            {
                value = Environment.MachineName;
            }
            else
            {
                machineId = new();
                for (int i = 0; i < value.Length; i++)
                {
                    machineId.AppendChar(value[i]);
                }
            }
            setMachineId = true;
        }
        catch (Exception e)
        {
            Log.Warn(nameof(MachineUniqueIdentifier), e, "GetMachineSecretKey fail.");
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
            mMachineSecretKey = GetMachineSecretKey(SecurityPlatformHelper.GetMachineGuid);
        }
        else if (OperatingSystem.IsMacOS())
        {
            mMachineSecretKey = GetMachineSecretKey(SecurityPlatformHelper.GetIOPlatformSerialNumber);
        }
        else if (OperatingSystem.IsLinux())
        {
            mMachineSecretKey = GetMachineSecretKey(SecurityPlatformHelper.GetEtcMachineId);
        }
        else
        {
            mMachineSecretKey = new(GetMachineSecretKeyBySecureStorage);
        }

        _MachineId = new(() =>
        {
            if (machineId == null)
            {
                if (!isGetMachineSecretKeyLazy)
                {
                    if (!setMachineId)
                    {
                        _ = mMachineSecretKey.Value;
                        return machineId;
                    }
                }
            }
            return machineId;
        });
    }

    /// <summary>
    /// 获取由机器识别码生成的本机加密 AES 密钥
    /// </summary>
    public static AESUtils.KeyIV MachineSecretKey => mMachineSecretKey.Value;
}
