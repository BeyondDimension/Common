# Common8
[次元超越 .NET 8+ 通用类库文档](https://beyonddimension.github.io/Common)

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