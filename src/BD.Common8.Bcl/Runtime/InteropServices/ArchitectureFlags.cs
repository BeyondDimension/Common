namespace System.Runtime.InteropServices;

/// <summary>
/// CPU 架构(处理器体系结构)，例如 x86、Arm 等 <see cref="Architecture"/>
/// 例如不同的 Android 设备使用不同的 CPU，而不同的 CPU 支持不同的指令集。
/// <para>CPU 与指令集的每种组合都有专属的应用二进制接口 (ABI)。</para>
/// <para>https://developer.android.google.cn/ndk/guides/abis.html</para>
/// <para>在比较版本支持的值与设备支持的值中的交集中，值越大的，优先选取</para>
/// </summary>
[Flags]
public enum ArchitectureFlags
{
    /// <summary>
    /// 基于 Intel/AMD 的 32 位处理器体系结构
    /// https://developer.android.google.cn/ndk/guides/abis.html#x86
    /// </summary>
    X86 = 16,

    /// <summary>
    /// 32 位 ARM 处理器体系结构
    /// https://developer.android.google.cn/ndk/guides/abis.html#v7a
    /// </summary>
    Arm = 32,

    /// <summary>
    /// 基于 Intel/AMD 的 64 位处理器体系结构(AMD64)
    /// https://developer.android.google.cn/ndk/guides/abis.html#86-64
    /// </summary>
    X64 = 128,

    /// <summary>
    /// 64 位 ARM 处理器体系结构
    /// https://developer.android.google.cn/ndk/guides/abis.html#arm64-v8a
    /// </summary>
    Arm64 = 256,

    /// <summary>
    /// WebAssembly 平台
    /// </summary>
    Wasm = 512,

    /// <summary>
    /// S390x 平台体系结构
    /// </summary>
    S390x = 1024,

    /// <summary>
    /// LoongArch64 处理器体系结构
    /// </summary>
    LoongArch64 = 2048,

    /// <summary>
    /// 32 位 ARMv6 处理器体系结构
    /// </summary>
    Armv6 = 4096,

    /// <summary>
    /// RiscV 64 位处理器体系结构
    /// </summary>
    Ppc64le = 8192,

    //_ = 16384,
    //_ = 32768,
    //_ = 65536,
    //_ = 131072,
    //_ = 262144,
}

public static partial class ArchitectureEnumExtensions
{
#if !NET7_0_OR_GREATER || NETFRAMEWORK || NETSTANDARD
    /// <summary>
    /// S390x 平台体系结构
    /// </summary>
    public const Architecture S390x = (Architecture)5;

    /// <summary>
    /// LoongArch64 处理器体系结构
    /// </summary>
    public const Architecture LoongArch64 = (Architecture)6;

    /// <summary>
    /// 32 位 ARMv6 处理器体系结构
    /// </summary>
    public const Architecture Armv6 = (Architecture)7;

    /// <summary>
    /// PowerPC 64 位（little-endian）处理器体系结构
    /// </summary>
    public const Architecture Ppc64le = (Architecture)8;

    /// <summary>
    /// RiscV 64 位处理器体系结构
    /// </summary>
    public const Architecture RiscV64 = (Architecture)9;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetWindowsRID(this Architecture architecture, bool @throw = true)
    {
        var rid = architecture switch
        {
            Architecture.X86 => "win-x86",
            Architecture.X64 => "win-x64",
            Architecture.Arm64 => "win-arm64",
            _ => @throw ?
                throw ThrowHelper.GetArgumentOutOfRangeException(architecture) :
                $"win-{architecture.ToString().ToLowerInvariant()}",
        };
        return rid;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetLinuxRID(this Architecture architecture, bool @throw = true)
    {
        var rid = architecture switch
        {
            Architecture.X86 => "linux-x86",
            Architecture.X64 => "linux-x64",
            Architecture.Arm64 => "linux-arm64",
            Architecture.Arm => "linux-arm",
#if NET7_0_OR_GREATER
            Architecture.
#endif
            Armv6 => "linux-armv6",
#if NET7_0_OR_GREATER
            Architecture.
#endif
            S390x => "linux-s390x",
#if NET7_0_OR_GREATER
            Architecture.
#endif
            LoongArch64 => "linux-loongarch64",
#if NET7_0_OR_GREATER
            Architecture.
#endif
            RiscV64 => "linux-riscv64",
            _ => @throw ?
                throw ThrowHelper.GetArgumentOutOfRangeException(architecture) :
                $"linux-{architecture.ToString().ToLowerInvariant()}",
        };
        return rid;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetOSXRID(this Architecture architecture, bool @throw = true)
    {
        var rid = architecture switch
        {
            Architecture.X64 => "osx-x64",
            Architecture.Arm64 => "osx-arm64",
            _ => @throw ?
                throw ThrowHelper.GetArgumentOutOfRangeException(architecture) :
                $"osx-{architecture.ToString().ToLowerInvariant()}",
        };
        return rid;
    }

#if DEBUG
    /// <inheritdoc cref="TryConvert(Architecture, bool)"/>
    [Obsolete("use TryConvert", true)]
    public static ArchitectureFlags Convert(this Architecture architecture, bool hasFlags) => architecture.TryConvert(hasFlags) ?? throw new ArgumentOutOfRangeException(nameof(architecture), architecture, null);
#endif

    /// <summary>
    /// 将 <see cref="Architecture"/> (CPU 架构)转换为 <see cref="ArchitectureFlags"/> (CPU 架构多选)
    /// </summary>
    /// <param name="architecture"></param>
    /// <param name="hasFlags"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArchitectureFlags? TryConvert(this Architecture architecture, bool hasFlags) => architecture switch
    {
        Architecture.Arm => ArchitectureFlags.Arm,
        Architecture.X86 => ArchitectureFlags.X86,
        Architecture.Arm64 => // 注意：一些较新的 Android 设备仅支持 Arm64 不支持 Arm，客户端应检测是否支持 Arm 传入 hasFlags 或直接传递 ArchitectureFlags
        hasFlags ? (ArchitectureFlags.Arm64 | ArchitectureFlags.Arm) : ArchitectureFlags.Arm64,
        Architecture.X64 =>
        hasFlags ? (ArchitectureFlags.X64 | ArchitectureFlags.X86) : ArchitectureFlags.X64,
#if NETFRAMEWORK || NETSTANDARD
        (Architecture)4 => ArchitectureFlags.Wasm,
        (Architecture)5 => ArchitectureFlags.S390x,
#else
        Architecture.Wasm => ArchitectureFlags.Wasm,
        Architecture.S390x => ArchitectureFlags.S390x,
#endif
#if NET7_0_OR_GREATER
        Architecture.
#endif
        LoongArch64 => ArchitectureFlags.LoongArch64,
#if NET7_0_OR_GREATER
        Architecture.
#endif
        Armv6 => ArchitectureFlags.Armv6,
#if NET7_0_OR_GREATER
        Architecture.
#endif
        Ppc64le => ArchitectureFlags.Ppc64le,
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable CS8619
        _ => null,
#pragma warning restore CS8619
#pragma warning restore IDE0079 // 请删除不必要的忽略
    };
}
