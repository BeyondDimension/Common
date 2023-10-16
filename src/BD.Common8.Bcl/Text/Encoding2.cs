namespace System.Text;

/// <inheritdoc cref="Encoding"/>
public static partial class Encoding2
{
    /// <summary>
    /// GB2312 CodePage
    /// </summary>
    public const int CodePage_GB2312 = 936;

#if NETCOREAPP
    /// <summary>
    /// 适用于 .NET Core / .NET 5+ 中的注册编码提供程序
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RegisterCodePagesProvider()
        => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

    /// <summary>
    /// 获取 GB2312 编码
    /// </summary>
    /// <returns></returns>
    public static Encoding GB2312 => Encoding2_GB2312.GB2312_;

    /// <summary>
    /// 还原 .NET Framework 中 <see cref="Encoding.Default"/> 行为
    /// </summary>
    /// <returns></returns>
    public static Encoding Default
    {
        get
        {
#if WINDOWS7_0_OR_GREATER
            return Encoding2_ActiveCodePages.ACP ?? Encoding.Default;
#else
            return Encoding.Default;
#endif
        }
    }

    static partial class Encoding2_GB2312
    {
        public static readonly Encoding GB2312_ = Encoding.GetEncoding(CodePage_GB2312);
    }

#if WINDOWS7_0_OR_GREATER
    /// <summary>
    /// Windows 代码页(活动代码页)
    /// </summary>
    [SupportedOSPlatform("Windows")]
    static partial class Encoding2_ActiveCodePages
    {
        internal static readonly Encoding? ACP = GetACP_();

        [LibraryImport("kernel32.dll")]
        [ResourceExposure(ResourceScope.None)]
        private static partial int GetACP();

        static Encoding? GetACP_()
        {
            int codePage = GetACP();
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(codePage);
            return encoding;
        }
    }
#endif

    /// <summary>
    /// https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/EncodingCache.cs
    /// </summary>
    public static readonly Encoding UTF8NoBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
}

