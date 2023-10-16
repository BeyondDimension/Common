// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

// 为现代 HTTP 应用程序提供一个编程接口。
// https://learn.microsoft.com/zh-cn/dotnet/api/system.net.http
global using System.Net.Http;
global using System.Net.Http.Headers;