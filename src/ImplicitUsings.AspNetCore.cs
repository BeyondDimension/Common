// C# 10 定义全局 using

global using Microsoft.Net.Http.Headers;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Hosting;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Cryptography.KeyDerivation;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.HttpOverrides;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Server.Kestrel.Core;
global using Microsoft.AspNetCore.Mvc.ApplicationParts;
global using Microsoft.AspNetCore.Mvc.Controllers;
global using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
global using Microsoft.AspNetCore.Mvc.ViewComponents;
global using Microsoft.Extensions.FileProviders;
global using Microsoft.AspNetCore.Authorization.Policy;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Caching.Distributed;
global using IOFile = System.IO.File;

#if NET7_0_OR_GREATER && OPENAPI && DEBUG
global using Microsoft.OpenApi.Models;
#endif