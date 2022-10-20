// C# 10 定义全局 using

global using Microsoft.JSInterop;
global using AntDesign;
global using System.Net.Http.Headers;
global using Microsoft.AspNetCore.Components;
global using Blazored.LocalStorage;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
#if BLAZOR_WEBASSEMBLY
global using Microsoft.AspNetCore.Components.WebAssembly.Http;
global using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endif
global using BD.Common.Pages.Utils;
global using BD.Common.Pages.Abstractions;