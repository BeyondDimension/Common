using BaseInternalService = BD.Common.Services.IInternalFilePickerPlatformService;
using BaseService = BD.Common.Services.IFilePickerPlatformService;
using IOpenFileDialogService = BD.Common.Services.IFilePickerPlatformService.IOpenFileDialogService;
using ISaveFileDialogService = BD.Common.Services.IFilePickerPlatformService.ISaveFileDialogService;

namespace BD.Common.Services.Implementation.Essentials;

sealed class FilePickerPlatformServiceImpl : BaseService, IOpenFileDialogService
{
    IOpenFileDialogService BaseService.OpenFileDialogService => this;

    ISaveFileDialogService BaseService.SaveFileDialogService => throw new NotImplementedException();

    IFilePickerFileType BaseInternalService.Images => E_FilePickerFileType.Images.Convert();

    IFilePickerFileType BaseInternalService.Png => E_FilePickerFileType.Png.Convert();

    IFilePickerFileType BaseInternalService.Jpeg => E_FilePickerFileType.Jpeg.Convert();

    IFilePickerFileType BaseInternalService.Videos => E_FilePickerFileType.Videos.Convert();

    IFilePickerFileType BaseInternalService.Pdf => E_FilePickerFileType.Pdf.Convert();

    async Task<IEnumerable<IFileResult>> IOpenFileDialogService.PlatformPickAsync(PickOptions? options, bool allowMultiple)
    {
        if (allowMultiple)
            return (await FilePicker.PickMultipleAsync(options.Convert())).Convert();
        else
            return new[] { (await FilePicker.PickAsync(options.Convert())).Convert() };
    }
}
