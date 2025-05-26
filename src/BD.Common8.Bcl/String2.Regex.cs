using System.Text.RegularExpressions;

namespace System;

static partial class String2
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Steam 会从用户名和密码中删除所有非 ASCII 字符
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex("[^\\u0000-\\u007F]")]
    public static partial Regex SteamUNPWDRegex();
#endif
}
