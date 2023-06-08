// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.Text;

global using Humanizer;

global using BD.Common.Repositories.SourceGenerator.Annotations;

global using BD.Common.Repositories.SourceGenerator.Models;

global using BD.Common.Repositories.SourceGenerator.Templates;
global using BD.Common.Repositories.SourceGenerator.Templates.Abstractions;

global using BD.Common.Repositories.SourceGenerator.Handlers.Properties;
global using BD.Common.Repositories.SourceGenerator.Handlers.Properties.Abstractions;

global using BD.Common.Repositories.SourceGenerator.Enums;

global using BD.Common.Repositories.SourceGenerator.Helpers;

global using BD.Common.Repositories.SourceGenerator.Handlers.Attributes;
global using BD.Common.Repositories.SourceGenerator.Handlers.Attributes.Abstractions;

global using static BD.Common.Repositories.SourceGenerator.Constants;