# Common8
次元超越 .NET 8+ 通用类库

## [🏗️ 项目结构](./articles/Project-Structure.md)

## Sdk 参数
- IsPackable 是否为 ```NuGet``` 包项目
- IsTestProject 是否为单元测试项目
- IsSourceGeneratorProject 是否为源生成器项目
- IsGlobalUsingsCommon8Bcl 是否全局引用命名空间 ```BD.Common8.Bcl```
- IsGlobalUsingsMSBuildProjectName 是否全局引用当前项目的命名空间，位于 ```src\Shared\GlobalUsings.$(MSBuildProjectName).cs```
- IsCommon8Project 是否为 ```Common8``` 仓库内的项目，将决定项目引用还是包引用
- UseCommon8Bcl 是否引用 ```BD.Common8.Bcl``` 类库
- UseAspNetCore 是否引用 ```ASP.NET Core``` 框架
- UseEFCore 是否引用 ```EF Core``` 框架
- UseProjectUtils 是否引用 ```src\Shared\ProjectUtils.cs```
- UseSourceGeneratorResx 是否引用 ```Resx``` 的源生成器
- UseMicrosoftIdentityModelTokens 是否引用包 ```Microsoft.IdentityModel.Tokens```
- UseAvalonia 是否引用包 ```Avalonia```
- LinkResXGeneratedCodeAttribute 是否引用源码 ```src\BD.Common8.Bcl\CodeDom\Compiler\ResXGeneratedCodeAttribute.cs```
- UseRedis 是否引用包 ```Redis```
- UseNLog 是否引用包 ```NLog```
- UseCommon8Essentials 是否引用类库 ```BD.Common8.Essentials```
- UseCommon8Repositories 是否引用类库 ```BD.Common8.Repositories```
- UseCommon8OrmEFCore 是否引用类库 ```BD.Common8.Orm.EFCore```
- UseCommon8RepositoriesEFCore 是否引用类库 ```BD.Common8.Repositories.EFCore```
- UseCommon8Primitives 是否引用类库 ```BD.Common8.Primitives```
- UseCommon8AspNetCore 是否引用类库 ```BD.Common8.AspNetCore``` 
- UseCommon8AspNetCoreIdentity 是否引用类库 ```BD.Common8.AspNetCore.Identity```
- UseCommon8AspNetCoreIdentityBackManage 是否引用类库 ```BD.Common8.AspNetCore.Identity.BackManage```
- UseCommon8PrimitivesApiResponse 是否引用类库 ```BD.Common8.Primitives.ApiResponse```
- UseCommon8PrimitivesPersonalDataPhoneNumber 是否引用类库 ```BD.Common8.Primitives.PersonalData.PhoneNumber```
- UseCommon8PrimitivesPersonalDataBirthDate 是否引用类库 ```BD.Common8.Primitives.PersonalData.BirthDate```
- UseCommon8PrimitivesDistrict 是否引用类库 ```BD.Common8.Primitives.District```
- UseCommon8PrimitivesApiRsp 是否引用类库 ```BD.Common8.Primitives.ApiRsp```
- UseSQLitePCL 是否引用包 ```sqlite-net-pcl``` ```Polly``` ```SQLitePCLRaw.bundle_green```
- UseCommon8Security 是否引用类库 ```BD.Common8.Security```
- UseSystemCommandLine 是否引用包 ```System.CommandLine```
- UseCommon8HttpClientFactory 是否引用类库 ```BD.Common8.Http.ClientFactory```
- UseCommon8Crawler 是否引用类库 ```BD.Common8.Crawler```
- UseReactiveUI 是否引用包 ```ReactiveUI``` 框架
- UseReactiveUIFody 是否引用包 ```ReactiveUI.Fody```
- UseFluentAvalonia 是否引用包 ```FluentAvaloniaUI```
- UseSourceGeneratorBcl 是否引用类库 ```BD.Common8.SourceGenerator.Bcl```
- UseMicrosoftExtensionsOptions 是否引用包 ```Microsoft.Extensions.Options```
- UseMicrosoftExtensionsFileProviders 是否引用源码 ```Shared\GlobalUsings.Microsoft.Extensions.FileProviders.cs```
- UseMicrosoftExtensionsPrimitives 是否引用源码 ```Shared\GlobalUsings.Microsoft.Extensions.Primitives.cs```
- UseAvaloniaReactiveUI 是否引用包 ```Avalonia.ReactiveUI```
- UseAvaloniaXamlInteractivity 是否引用包 ```Avalonia.Xaml.Behaviors```
- UseCommon8Toast 是否引用类库 ```BD.Common8.Toast```
- UseCommon8RepositoriesSQLitePCL 是否引用类库 ```BD.Common8.Repositories.SQLitePCL```
- UseAndroidXBrowser 是否引用包 ```Xamarin.AndroidX.Browser``` 目标框架为 android 时
- UseCommon8EssentialsImplementation 是否引用类库 ```BD.Common8.Essentials.Implementation```
- UseFusillade 是否引用包 ```fusillade```
- UseSystemNetHttpJson 是否引用包 ```System.Net.Http.Json```
- UseCommon8Ipc 是否引用类库 ```BD.Common8.Ipc```
- UseCommon8Pinyin 是否引用类库 ```BD.Common8.Pinyin```
- UseAngleSharp 是否引用包 ```AngleSharp```
- UseCommon8Settings5 是否引用类库 ```BD.Common8.Settings5```
- UseCommon8Settings5Frontend 是否引用类库 ```BD.Common8.Settings5.Frontend```
- UseCommon8Settings5Backend 是否引用类库 ```BD.Common8.Settings5.Backend```
- UseMicrosoftExtensionsConfiguration 是否引用包 ```Microsoft.Extensions.Configuration```
- UseMicrosoftExtensionsConfigurationJson 是否引用包 ```Microsoft.Extensions.Configuration.Json```
- UseMicrosoftExtensionsOptionsConfigurationExtensions 是否引用包 ```Microsoft.Extensions.Options.ConfigurationExtensions```

## SharedLibrary 共享库

### BD.Common8.Bcl
提供对基类库的扩展

### BD.Common8.Bcl.Compat
提供旧版 Runtime 上缺少的内容以兼容新版 C# 语法

## BD.Common8.Essentials
Essentials 提供单个跨平台 API，适用于任何 .NET 应用程序 (Win32、WinRT、Android、iOS、macOS、MacCatalyst)

## BD.Common8.Essentials.Implementation
具体平台的 Essentials 实现服务库

## BD.Common8.Essentials.Implementation.Avalonia
Avalonia UI 相关的 Essentials 实现服务库

### BD.Common8.Http.ClientFactory
适用于客户端的 HttpClient 工厂实现库 (使用 Fusillade 实现)

### BD.Common8.Http.ClientFactory.Server
适用于客户端的 HttpClient 工厂的服务端兼容实现库 (与 Microsoft.Extensions.Http 兼容)

### BD.Common8.Ipc
进程间通信 (IPC) 库

### BD.Common8.Ipc.Client
进程间通信 (IPC) 客户端库

### BD.Common8.Ipc.Server
进程间通信 (IPC) 服务端库

### BD.Common8.Orm.EFCore
EFCore 相关的封装库

### BD.Common8.Pinyin
汉语拼音封装库

### BD.Common8.Pinyin.ChnCharInfo
使用 ChnCharInfo 实现的汉语拼音库

### BD.Common8.Pinyin.CoreFoundation
使用 CoreFoundation 实现的汉语拼音库

### BD.Common8.Primitives.ApiResponse
提供 Api 响应 (BackManage 后台管理) 类型的封装库

### BD.Common8.Primitives.ApiRsp
提供 Api 响应类型的封装库

### BD.Common8.Primitives.PersonalData.BirthDate
提供个人资料（出生日期）格式

### BD.Common8.Primitives.PersonalData.PhoneNumber
提供个人资料（手机号码）格式

### BD.Common8.Primitives
基本模型，枚举类型库

### BD.Common8.Primitives.District
提供行政区域数据封装库

### BD.Common8.AspNetCore
ASP.NET Core 的通用封装库

### BD.Common8.AspNetCore.Identity
ASP.NET Core 的管理用户、密码、配置文件数据、角色、声明、令牌、电子邮件确认等封装库

### BD.Common8.AspNetCore.Identity.BackManage
ASP.NET Core 实现的多租户后台管理系统通用封装库

## SourceGenerator 源生成器

### BD.Common8.SourceGenerator.ResX
xyz.Designer.cs 源生成器

### BD.Common8.SourceGenerator.Ipc.Client
进程间通信 (IPC) 客户端源生成器

### BD.Common8.SourceGenerator.Ipc.Server
进程间通信 (IPC) 服务端源生成器

## Tools 工具

### BD.Common8.Tools.DocFX
启动 DocFX

### BD.Common8.Tools.Sort.PackageVersion
NuGet 包清单排序

## UnitTest 单元测试

### BD.Common8.UnitTest
当前仓库的单元测试项目