namespace System;

/// <summary>
/// .NET 版本信息
/// </summary>
public static partial class DotNetVersionInfo
{
    /// <summary>
    /// .NET Framework 版本 Release 的值
    /// <para>https://learn.microsoft.com/zh-cn/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed#net-framework-45-and-later-versions</para>
    /// </summary>
    public enum DotNetFrameworkVersionReleaseValue
    {
        /// <summary>
        /// .NET Framework 4.5
        /// <para>所有 Windows 操作系统：378389</para>
        /// </summary>
        Net45 = 378389,

        /// <summary>
        /// .NET Framework 4.5.1
        /// <para>在 Windows 8.1 和 Windows Server 2012 R2 上：378675</para>
        /// </summary>
        Net451 = 378675,

        /// <summary>
        /// .NET Framework 4.5.1
        /// <para>在所有其他 Windows 操作系统上：378758</para>
        /// </summary>
        Net451_All = 378758,

        /// <summary>
        /// .NET Framework 4.5.2
        /// <para>所有 Windows 操作系统：379893</para>
        /// </summary>
        Net452 = 379893,

        /// <summary>
        /// .NET Framework 4.6
        /// <para>在 Windows 10 上：393295</para>
        /// </summary>
        Net46 = 393295,

        /// <summary>
        /// .NET Framework 4.6
        /// <para>在所有其他 Windows 操作系统上：393297</para>
        /// </summary>
        Net46_All = 393297,

        /// <summary>
        /// .NET Framework 4.6.1
        /// <para>在 Windows 10 11 月更新系统上：394254</para>
        /// </summary>
        Net461 = 394254,

        /// <summary>
        /// .NET Framework 4.6.1
        /// <para>在所有其他 Windows 操作系统（包括 Windows 10）上：394271</para>
        /// </summary>
        Net461_All = 394271,

        /// <summary>
        /// .NET Framework 4.6.2
        /// <para>在 Windows 10 周年更新和 Windows Server 2016 上：394802</para>
        /// </summary>
        Net462 = 394802,

        /// <summary>
        /// .NET Framework 4.6.2
        /// <para>在所有其他 Windows 操作系统（包括其他 Windows 10 操作系统）上：394806</para>
        /// </summary>
        Net462_All = 394806,

        /// <summary>
        /// .NET Framework 4.7
        /// <para>在 Windows 10 创意者更新上：460798</para>
        /// </summary>
        Net47 = 460798,

        /// <summary>
        /// .NET Framework 4.7
        /// <para>在所有其他 Windows 操作系统（包括其他 Windows 10 操作系统）上：460805</para>
        /// </summary>
        Net47_All = 460805,

        /// <summary>
        /// .NET Framework 4.7.1
        /// <para>在 Windows 10 Fall Creators Update 和 Windows Server 版本 1709 上：461308</para>
        /// </summary>
        Net471 = 461308,

        /// <summary>
        /// .NET Framework 4.7.1
        /// <para>在所有其他 Windows 操作系统（包括其他 Windows 10 操作系统）上：461310</para>
        /// </summary>
        Net471_All = 461310,

        /// <summary>
        /// .NET Framework 4.7.2
        /// <para>在 Windows 10 2018 年 4 月更新和 Windows Server 版本 1803 上：461808</para>
        /// </summary>
        Net472 = 461808,

        /// <summary>
        /// .NET Framework 4.7.2
        /// <para>在除 Windows 10 2018 年 4 月更新和 Windows Server 版本 1803 之外的所有 Windows 操作系统上：461814</para>
        /// </summary>
        Net472_All = 461814,

        /// <summary>
        /// .NET Framework 4.8
        /// <para>在 Windows 10 的 2019 年 5 月更新和 Windows 10 的 2019 年 11 月更新上：528040</para>
        /// </summary>
        Net48 = 528040,

        /// <summary>
        /// .NET Framework 4.8
        /// <para>在 Windows 10 的 2020 年 5 月更新、2020 年 10 月更新、2021 年 5 月更新、2021 年 11 月更新和 2022 年更新上：528372</para>
        /// </summary>
        Net48_Win10_2005 = 528372,

        /// <summary>
        /// .NET Framework 4.8
        /// <para>在 Windows 11 和 Windows Server 2022 上：528449</para>
        /// </summary>
        Net48_Win11_WinServer2022 = 528449,

        /// <summary>
        /// .NET Framework 4.8
        /// <para>在所有其他 Windows 操作系统（包括其他 Windows 10 操作系统）上：528049</para>
        /// </summary>
        Net48_All = 528049,

        /// <summary>
        /// .NET Framework 4.8.1
        /// <para>在 Windows 11 2022(22621) 更新和 Windows 11 2023(22631) 更新上：533320</para>
        /// </summary>
        Net481 = 533320,

        /// <summary>
        /// .NET Framework 4.8.1
        /// <para>所有其他 Windows 操作系统：533325</para>
        /// </summary>
        Net481_All = 533325,
    }

    /// <summary>
    /// 获取当前系统的 .NET Framework 版本
    /// </summary>
    /// <returns></returns>
    public static Version? GetOSFrameworkVersion()
    {
#if WINDOWS || NETFRAMEWORK
        try
        {
            var release = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full")
                ?.GetValue("Release");
            if (release is int release_int)
            {
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net481)
                    return new(4, 8, 1);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net48)
                    return new(4, 8);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net472)
                    return new(4, 7, 2);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net471)
                    return new(4, 7, 1);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net47)
                    return new(4, 7);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net462)
                    return new(4, 6, 2);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net461)
                    return new(4, 6, 1);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net46)
                    return new(4, 6);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net452)
                    return new(4, 5, 2);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net451)
                    return new(4, 5, 1);
                if (release_int >= (int)DotNetFrameworkVersionReleaseValue.Net45)
                    return new(4, 5);
            }
        }
        catch
        {
        }
#endif
        return null;
    }
}
