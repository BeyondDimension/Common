# Common
次元超越通用类库

## 🏗️ 项目结构
- Server Lib 仅用于服务端的通用类库
	- BD.Common.SmsSender 短信服务库，支持阿里云，网易云信，世纪互联蓝云
- Test
	- BD.Common.UnitTest 单元测试
- Web API 仅用于服务端的 Web API 通用类库
	- BD.Common.AspNetCore 用于 ASP.NET Core 的通用类库
	- BD.Common.AspNetCore.Identity 用于 ASP.NET Core 的身份通用类库
	- BD.Common.AspNetCore.Identity.BackManage 用于 ASP.NET Core 后台管理系统的身份通用类库
- Web UI
	- BD.Common.AspNetCore.Blazor.BackManage 用于 ASP.NET Core 后台管理系统 Blazor UI 的通用类库
- BD.Common 对 .NET BCL 的扩展或增强通用类库
- BD.Common.EFCore 对 EF Core 的扩展或增强通用类库
- BD.Common.PhoneNumber 用于中国大陆的手机号相关的通用类库
- BD.Common.Primitives 通用模型，枚举，列接口的通用类库
- BD.Common.Repositories 通用仓储层基类库
- BD.Common.Repositories.EFCore 使用 EF Core 实现的仓储层类库
- BD.Common.Repositories.SQLitePCL 使用 SQLitePCL 实现的仓储层类库