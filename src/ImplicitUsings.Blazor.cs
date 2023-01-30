// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

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