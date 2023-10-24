#if IOS || MACCATALYST
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace BD.Common8.Essentials;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Graphics/src/Graphics/Platforms/iOS/UIKitExtensions.cs
/// </summary>
[SupportedOSPlatform("maccatalyst")]
[SupportedOSPlatform("ios")]
public static partial class UIKitExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UIColor AsUIColor(this SDColor color)
        => UIColor.FromRGBA(color.R, color.G, color.B, color.A);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UIColor AsUIColor(this SDColor? color)
        => color.HasValue ?
        color.Value.AsUIColor() :
        UIColor.White;
}
#endif