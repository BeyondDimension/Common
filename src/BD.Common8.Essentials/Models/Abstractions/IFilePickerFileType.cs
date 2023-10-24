namespace BD.Common8.Essentials.Models.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

public interface IFilePickerFileType
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Images() => IPresetFilePickerPlatformService.Instance?.Images;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Png() => IPresetFilePickerPlatformService.Instance?.Png;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Jpeg() => IPresetFilePickerPlatformService.Instance?.Jpeg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Videos() => IPresetFilePickerPlatformService.Instance?.Videos;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Pdf() => IPresetFilePickerPlatformService.Instance?.Pdf;

    IEnumerable<string>? GetPlatformFileType(DevicePlatform2 platform);
}