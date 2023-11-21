namespace BD.Common8.SourceGenerator.Helpers;

/// <summary>
/// 提供 <see cref="SourceText"/> 的助手类
/// </summary>
public static partial class SourceTextHelper
{
    /// <summary>
    /// 将 <see cref="StringBuilder"/> 转换为 <see cref="SourceText"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SourceText ToSourceText(this StringBuilder builder)
    {
        var str = builder.ToString();
        var result = SourceText.From(str, Encoding.UTF8);
        return result;
    }
}
