// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

global using Microsoft.Extensions.Logging;
global using BD.Common.Controllers.Abstractions;
global using BD.Common.Repositories.SourceGenerator.Annotations;

global using BD.Common.Repositories.SourceGenerator.ConsoleTest.Entities;
global using BD.Common.Repositories.SourceGenerator.ConsoleTest.Repositories;
global using BD.Common.Repositories.SourceGenerator.ConsoleTest.Repositories.Abstractions;
global using BD.Common.Repositories.SourceGenerator.ConsoleTest.Controllers;
global using BD.Common.Repositories.SourceGenerator.ConsoleTest.Controllers.Abstractions;
global using BD.Common.Repositories.SourceGenerator.ConsoleTest.Data;
global using BD.Common.Repositories.SourceGenerator.ConsoleTest.Data.Abstractions;

global using IIPAddress = BD.Common.Columns.IPAddress;