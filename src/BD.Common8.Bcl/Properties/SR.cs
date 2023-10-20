#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace BD.Common8.Resources;
#pragma warning restore IDE0079 // 请删除不必要的忽略

static partial class SR
{
    /// <summary>
    /// 用于 <see cref="ArgumentOutOfRangeException(string?, object?, string?)"/> 的错误消息
    /// <para>https://github.com/dotnet/runtime/blob/v8.0.0-rc.2.23479.6/src/libraries/System.Private.CoreLib/src/Resources/Strings.resx#L205</para>
    /// </summary>
    public const string Arg_ArgumentOutOfRangeException__ =
"""
Specified argument was out of the range of valid values. (Parameter '{0}')
Actual value was {1}.
""";

    /// <summary>
    /// 用于 <see cref="ArgumentOutOfRangeException(string?, object?, string?)"/> 的错误消息
    /// <para>https://github.com/dotnet/runtime/blob/v8.0.0-rc.2.23479.6/src/libraries/System.Private.CoreLib/src/Resources/Strings.resx#L205</para>
    /// </summary>
    public const string Arg_ArgumentOutOfRangeException___ =
"""
{2} (Parameter '{0}')
Actual value was {1}.
""";
}
