namespace BD.Common.Models.Abstractions;

public interface IFilePickerFileType
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Images() => IInternalFilePickerPlatformService.Instance?.Images;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Png() => IInternalFilePickerPlatformService.Instance?.Png;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Jpeg() => IInternalFilePickerPlatformService.Instance?.Jpeg;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Videos() => IInternalFilePickerPlatformService.Instance?.Videos;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IFilePickerFileType? Pdf() => IInternalFilePickerPlatformService.Instance?.Pdf;

    IEnumerable<string>? GetPlatformFileType(Platform platform);
}