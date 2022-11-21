// GenerateAssemblyInfo 设为 false
// 还原 .Net Framework 项目行为，使用此类定义程序集 Attribute
using System.Resources;
using _ThisAssembly_ = BD.Common.ThisAssembly;

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable SYSLIB0025 // 类型或成员已过时
[assembly: SuppressIldasm]
#pragma warning restore SYSLIB0025 // 类型或成员已过时
#pragma warning restore IDE0079 // 请删除不必要的忽略
[assembly: AssemblyTitle(_ThisAssembly_.AssemblyTitle)]
[assembly: AssemblyCopyright(_ThisAssembly_.AssemblyCopyright)]
[assembly: AssemblyCompany(_ThisAssembly_.AssemblyCompany)]
[assembly: AssemblyFileVersion(_ThisAssembly_.AssemblyVersion)]
[assembly: AssemblyVersion(_ThisAssembly_.AssemblyVersion)]
[assembly: NeutralResourcesLanguage("zh-Hans")]