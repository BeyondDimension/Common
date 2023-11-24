#if IOS || MACCATALYST
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace BD.Common8.Essentials;

/// <summary>
/// 提供 UIKit 扩展方法的静态类
/// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Graphics/src/Graphics/Platforms/iOS/UIKitExtensions.cs
/// </summary>
[SupportedOSPlatform("maccatalyst")]
[SupportedOSPlatform("ios")]
public static partial class UIKitExtensions
{
    /// <summary>
    /// 将 <see cref="SDColor"/> 转换为 <see cref="UIColor"/>
    /// </summary>
    /// <param name="color">要转换的 <see cref="SDColor"/> 对象</param>
    /// <returns>转换后的 <see cref="UIColor"/> 对象</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UIColor AsUIColor(this SDColor color)
        => UIColor.FromRGBA(color.R, color.G, color.B, color.A);

    /// <summary>
    /// 将可空的 <see cref="SDColor"/> 转换为 <see cref="UIColor"/>
    /// </summary>
    /// <param name="color">要转换的可空 SDColor 对象</param>
    /// <returns>转换后的 <see cref="UIColor"/> 对象</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UIColor AsUIColor(this SDColor? color)
        => color.HasValue ?
        color.Value.AsUIColor() :
        UIColor.White;
}
#endif