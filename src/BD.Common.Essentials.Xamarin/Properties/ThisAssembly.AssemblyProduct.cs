#if MAUI
global using Microsoft.Maui.ApplicationModel;
global using Microsoft.Maui.ApplicationModel.DataTransfer;
global using Microsoft.Maui.ApplicationModel.Communication;
global using Microsoft.Maui.Devices;
global using Microsoft.Maui.Storage;
global using Microsoft.Maui.Networking;
global using E_FilePickerFileType = Microsoft.Maui.Storage.FilePickerFileType;
global using XEBrowserLaunchFlags = Microsoft.Maui.ApplicationModel.BrowserLaunchFlags;
global using XEBrowserLaunchMode = Microsoft.Maui.ApplicationModel.BrowserLaunchMode;
global using XEBrowserLaunchOptions = Microsoft.Maui.ApplicationModel.BrowserLaunchOptions;
global using XEBrowserTitleMode = Microsoft.Maui.ApplicationModel.BrowserTitleMode;
global using XEDeviceIdiom = Microsoft.Maui.Devices.DeviceIdiom;
global using XEDeviceType = Microsoft.Maui.Devices.DeviceType;
global using XEEmailAttachment = Microsoft.Maui.ApplicationModel.Communication.EmailAttachment;
global using XEEmailBodyFormat = Microsoft.Maui.ApplicationModel.Communication.EmailBodyFormat;
global using XEEmailMessage = Microsoft.Maui.ApplicationModel.Communication.EmailMessage;
global using XEFileBase = Microsoft.Maui.Storage.FileBase;
global using XEFilePickerFileType = Microsoft.Maui.Storage.FilePickerFileType;
global using XEFileResult = Microsoft.Maui.Storage.FileResult;
global using XENetworkAccess = Microsoft.Maui.Networking.NetworkAccess;
global using XEPermissionStatus = Microsoft.Maui.ApplicationModel.PermissionStatus;
global using XEPickOptions = Microsoft.Maui.Storage.PickOptions;
#else
global using E_FilePickerFileType = Xamarin.Essentials.FilePickerFileType;
global using XEBrowserLaunchFlags = Xamarin.Essentials.BrowserLaunchFlags;
global using XEBrowserLaunchMode = Xamarin.Essentials.BrowserLaunchMode;
global using XEBrowserLaunchOptions = Xamarin.Essentials.BrowserLaunchOptions;
global using XEBrowserTitleMode = Xamarin.Essentials.BrowserTitleMode;
global using XEDeviceIdiom = Xamarin.Essentials.DeviceIdiom;
global using XEDeviceType = Xamarin.Essentials.DeviceType;
global using XEEmailAttachment = Xamarin.Essentials.EmailAttachment;
global using XEEmailBodyFormat = Xamarin.Essentials.EmailBodyFormat;
global using XEEmailMessage = Xamarin.Essentials.EmailMessage;
global using XEFileBase = Xamarin.Essentials.FileBase;
global using XEFilePickerFileType = Xamarin.Essentials.FilePickerFileType;
global using XEFileResult = Xamarin.Essentials.FileResult;
global using XENetworkAccess = Xamarin.Essentials.NetworkAccess;
global using XEPermissionStatus = Xamarin.Essentials.PermissionStatus;
global using XEPickOptions = Xamarin.Essentials.PickOptions;
#endif
global using BrowserLaunchFlags = BD.Common.Enums.BrowserLaunchFlags;
global using BrowserLaunchMode = BD.Common.Enums.BrowserLaunchMode;
global using BrowserLaunchOptions = BD.Common.Models.BrowserLaunchOptions;
global using BrowserTitleMode = BD.Common.Enums.BrowserTitleMode;
global using DeviceIdiom = System.Runtime.Devices.DeviceIdiom;
global using DeviceType = System.Runtime.Devices.DeviceType;
global using EmailBodyFormat = BD.Common.Enums.EmailBodyFormat;
global using EmailMessage = BD.Common.Models.EmailMessage;
global using ISecureStorage = System.Security.ISecureStorage;
global using NetworkAccess = BD.Common.Enums.NetworkAccess;
global using PermissionStatus = BD.Common.Enums.PermissionStatus;
global using PickOptions = BD.Common.Models.PickOptions;
global using Platform = System.Runtime.Devices.Platform;

#if MAUI
using _ThisAssembly_ = BD.Common.Essentials_.Maui.ThisAssembly;
#else
using _ThisAssembly_ = BD.Common.Essentials_.Xamarin.ThisAssembly;
#endif

[assembly: AssemblyProduct(_ThisAssembly_.AssemblyProduct)]

// ReSharper disable once CheckNamespace
#if MAUI
namespace BD.Common.Essentials_.Maui;
#else
namespace BD.Common.Essentials_.Xamarin;
#endif

static partial class ThisAssembly
{
    /// <summary>
    /// ????????????????????????????????????????????????
    /// </summary>
    public const string AssemblyProduct = "???????????? Essentials ?????????";
}