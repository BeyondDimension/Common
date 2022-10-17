using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static System.Text.Encoding2_ActiveCodePages;
using static System.Text.Encoding2_GB2312;

namespace System.Text;

public static partial class Encoding2
{
    /// <summary>
    /// 适用于 .NET Core / .NET 5+ 中的注册编码提供程序
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RegisterCodePagesProvider()
        => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    /// <summary>
    /// 获取 GB 2312 编码
    /// </summary>
    /// <returns></returns>
    public static Encoding GB2312 => GB2312_;

    /// <summary>
    /// 还原 .NET Framework 中 <see cref="Encoding.Default"/> 行为
    /// </summary>
    /// <returns></returns>
    public static Encoding Default
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                return ACP ?? Encoding.Default;
            }
            return Encoding.Default;
        }
    }
}

public static partial class EncodingCodePages
{
    public const int GB2312 = 936;
}

/// <summary>
/// Windows 代码页(活动代码页)
/// </summary>
[SupportedOSPlatform("Windows")]
static partial class Encoding2_ActiveCodePages
{
    internal static readonly Encoding? ACP = GetACP_();

    [DllImport("kernel32.dll")]
    [ResourceExposure(ResourceScope.None)]
    internal static extern int GetACP();

    internal static Encoding? GetACP_()
    {
        int codePage = GetACP();
        var encoding = CodePagesEncodingProvider.Instance.GetEncoding(codePage);
        return encoding;
    }
}

static partial class Encoding2_GB2312
{
    public static readonly Encoding GB2312_ = Encoding.GetEncoding(EncodingCodePages.GB2312);
}