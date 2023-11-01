namespace BD.Common8.Essentials.Models.Abstractions;

/// <summary>
/// 文件类型选择器
/// </summary>
public interface IFilePickerFileType
{
    /// <summary>
    /// 获取图片类型
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Images() => IPresetFilePickerPlatformService.Instance?.Images;

    /// <summary>
    /// 获取 PNG 类型
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Png() => IPresetFilePickerPlatformService.Instance?.Png;

    /// <summary>
    /// 获取 JPEG 类型
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Jpeg() => IPresetFilePickerPlatformService.Instance?.Jpeg;

    /// <summary>
    /// 获取视频类型
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Videos() => IPresetFilePickerPlatformService.Instance?.Videos;

    /// <summary>
    /// 获取 PDF 类型
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Pdf() => IPresetFilePickerPlatformService.Instance?.Pdf;

    /// <summary>
    /// 根据设备平台获取对应的文件类型
    /// </summary>
    IEnumerable<string>? GetPlatformFileType(DevicePlatform2 platform);
}