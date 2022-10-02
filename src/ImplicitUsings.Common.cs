// C# 10 定义全局 using

global using BD.Common;
global using BD.Common.Columns;
#if !BLAZOR
global using BD.Common.Controllers;
global using BD.Common.Controllers.Abstractions;
global using BD.Common.Data;
global using BD.Common.Data.Abstractions;
#endif
global using BD.Common.Enums;
#if !BLAZOR
global using BD.Common.Identity;
global using BD.Common.Identity.Abstractions;
#else
global using BD.Common.Pages;
global using BD.Common.Pages.Utils;
global using BD.Common.Pages.Abstractions;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
#endif