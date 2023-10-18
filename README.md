# Common8
次元超越 .NET 8+ 通用类库

## Sdk 参数
- IsPackable 是否为 ```NuGet``` 包项目
- IsTestProject 是否为单元测试项目
- IsSourceGeneratorProject 是否为源生成器项目
- IsGlobalUsingsCommon8Bcl 是否全局引用命名空间 ```BD.Common8.Bcl```
- IsGlobalUsingsMSBuildProjectName 是否全局引用当前项目的命名空间，位于 ```src\Shared\GlobalUsings.$(MSBuildProjectName).cs```
- IsCommon8Project 是否为 ```Common8``` 仓库内的项目，将决定项目引用还是包引用
- UseCommon8Bcl 是否引用 ```BD.Common8.Bcl``` 类库
- UseAspNetCore 是否引用 ```ASP.NET Core``` 框架
- UseProjectUtils 是否引用 ```src\Shared\ProjectUtils.cs```
- UseSourceGeneratorResx 是否引用 ```Resx``` 的源生成器
- UseMicrosoftIdentityModelTokens 是否引用包 ```Microsoft.IdentityModel.Tokens```

## SharedLibrary 共享库

### BD.Common8.Bcl
提供对基类库的扩展

### BD.Common8.Bcl.Compat
提供旧版 Runtime 上缺少的内容以兼容新版 C# 语法

### BD.Common8.Ipc
进程间通信 (IPC) 库

### BD.Common8.Ipc.Client
进程间通信 (IPC) 客户端库

### BD.Common8.Ipc.Server
进程间通信 (IPC) 服务端库

### BD.Common8.PersonalData.BirthDate
提供个人资料（出生日期）格式

### BD.Common8.PersonalData.PhoneNumber
提供个人资料（手机号码）格式

### BD.Common8.Primitives.District
提供行政区域数据封装库

### BD.Common8.Primitives.ApiResponse
提供 Api 响应 (BackManage 后台管理) 类型的封装库

### BD.Common8.Primitives.ApiRsp
提供 Api 响应类型的封装库

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

<!-- TODO

BD.Common8.ORM.EFCore
提供对对象关系映射(EFCore)的扩展

BD.Common8.UserInput.ModelValidator
提供用户输入的模型验证

BD.Common8.Primitives
提供常见枚举

BD.Common8.WebApi.Primitives.ApiResponse
提供 WebApi 响应模型类型

-->