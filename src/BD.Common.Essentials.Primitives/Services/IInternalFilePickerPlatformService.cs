namespace BD.Common.Services;

public interface IInternalFilePickerPlatformService
{
    static IInternalFilePickerPlatformService? Instance => Ioc.Get_Nullable<IInternalFilePickerPlatformService>();

    IFilePickerFileType Images { get; }

    IFilePickerFileType Png { get; }

    IFilePickerFileType Jpeg { get; }

    IFilePickerFileType Videos { get; }

    IFilePickerFileType Pdf { get; }
}
