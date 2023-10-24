namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented

public interface IPresetFilePickerPlatformService
{
    static IPresetFilePickerPlatformService? Instance => Ioc.Get_Nullable<IPresetFilePickerPlatformService>();

    IFilePickerFileType Images { get; }

    IFilePickerFileType Png { get; }

    IFilePickerFileType Jpeg { get; }

    IFilePickerFileType Videos { get; }

    IFilePickerFileType Pdf { get; }
}
