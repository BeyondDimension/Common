// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

global using Avalonia.Metadata;

#if BASE_CLASS_LIB_EX
[assembly: XmlnsDefinition("https://steampp.net/common", "System")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Collections.Generic")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Drawing")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.IO")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.IO.FileFormats")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Net")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Net.Http")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Net.Http.Client")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Net.Http.Headers")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Net.Http.Sockets")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Runtime")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Runtime.Serialization.Formatters")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Runtime.InteropServices")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Runtime.Devices")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Security.Cryptography")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Text")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Threading")]
[assembly: XmlnsDefinition("https://steampp.net/common", "System.Threading.Tasks")]
#elif COMMON_LIB_AREA
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Enums")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Models")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Models.Abstractions")]
#elif COMMON_LIB_BIRTHDATE
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Columns")]
#elif COMMON_LIB_MODELVALIDATOR
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Services")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Services.Implementation")]
#elif COMMON_LIB_PHONENUMBER
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Columns")]
#elif COMMON_LIB_PRIMITIVES
[assembly: XmlnsDefinition("https://steampp.net/common", "System")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Columns")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Enums")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Models")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Models.Abstractions")]
#elif COMMON_LIB_PRIMITIVES_API_RSP
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Models")]
#elif COMMON_LIB_MVVM
[assembly: XmlnsDefinition("https://steampp.net/common", "System")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Converters.Abstractions")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Services.Implementation")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.UI.Helpers")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.UI.ViewModels")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.UI.ViewModels.Abstractions")]
#elif COMMON_LIB_MVVM_RXUI
[assembly: XmlnsDefinition("https://steampp.net/common", "ReactiveUI")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.UI.Adapters")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.UI.ViewModels.Abstractions")]
#elif COMMON_LIB_NAV
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Enums")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Services")]
#elif COMMON_LIB_SECURITY
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Enums")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Services")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Services.Implementation")]
#elif COMMON_LIB_TOAST
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Enums")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Services")]
[assembly: XmlnsDefinition("https://steampp.net/common", "BD.Common.Services.Implementation")]
#endif