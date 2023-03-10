### 🏗️ 项目结构
- Client Lib 客户端类库
	- BD.Common.Mvvm 客户端 MVVM 类库
	- BD.Common.Mvvm.ReactiveUI 客户端 MVVM ReactiveUI 类库
	- BD.Common.Security 安全相关类库
	- BD.Common.Toast Toast UI 提示抽象层类库
- Essentials
	- Implementation Essentials 的实现库
		- Preferences
			- BD.Common.Essentials.Preferences.SQLite SQLite 实现库
			- BD.Common.Essentials.Preferences.DBreeze DBreeze 实现库
		- BD.Common.Essentials.Maui 使用 Maui.Essentials 的实现库
		- BD.Common.Essentials.Xamarin 使用 Xamarin.Essentials 的实现库
	- BD.Common.Essentials Essentials 抽象层类库
	- BD.Common.Essentials.Primitives 基本模型类库
	- BD.Common.Essentials.Utils 静态工具类库
- Pinyin 汉语拼音库
	- BD.Common.Pinyin 汉语拼音类库
	- BD.Common.Pinyin.CFStringTransform 使用 CFStringTransform 实现的汉语拼音类库
	- BD.Common.Pinyin.ChnCharInfo 使用 ChnCharInfo 实现的汉语拼音类库
	- BD.Common.Pinyin.TinyPinyin 使用 TinyPinyin 实现的汉语拼音类库
- Repositories 仓储层
	- BD.Common.Repositories 通用仓储层基类库
	- BD.Common.Repositories.EFCore 使用 EF Core 实现的仓储层类库
	- BD.Common.Repositories.SQLitePCL 使用 SQLitePCL 实现的仓储层类库
- Server Lib 仅用于服务端的通用类库
	- BD.Common.SmsSender 短信服务库，支持阿里云，网易云信，世纪互联蓝云
- Settings
	- BD.Common.Settings 文件键值对设置存储类库
	- BD.Common.Settings.V3 文件键值对设置存储 DBreeze 实现类库
- Test
	- BD.Common.UnitTest 单元测试
- Web API 仅用于服务端的 Web API 通用类库
	- BD.Common.AspNetCore 用于 ASP.NET Core 的通用类库
	- BD.Common.AspNetCore.Identity 用于 ASP.NET Core 的身份通用类库
	- BD.Common.AspNetCore.Identity.BackManage 用于 ASP.NET Core 后台管理系统的身份通用类库
- Web UI
	- BD.Common.AspNetCore.Blazor.BackManage 用于 ASP.NET Core 后台管理系统 Blazor UI 的通用类库
- BD.Common 对 .NET BCL 的扩展或增强通用类库
- BD.Common.Area 地区区域省市区数据类库
- BD.Common.BirthDate 出生日期生日类库
- BD.Common.EFCore 对 EF Core 的扩展或增强通用类库
- BD.Common.ModelValidator 模型验证类库
- BD.Common.PhoneNumber 用于中国大陆的手机号相关的通用类库
- BD.Common.Primitives 通用模型，枚举，列接口的通用类库
- BD.Common.Primitives.ApiResponse 通用 API 响应模型类库

### ⚠ 注意事项
1. ```ServiceCollectionExtensions.*.cs``` **DI 注册服务扩展类，命名空间统一使用**  
<pre>
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
</pre>

### Resx 资源文件
- src\BD.Common\Resources\Strings.resx 通用类库字符串资源
- src\BD.Common.BirthDate\Strings.resx 生日类库字符串资源
- src\BD.Common.ModelValidator\Resources\Strings.resx 模型验证类库字符串资源
- src\BD.Common.PhoneNumber\Resources\Strings.resx 手机号类库字符串资源
- src\BD.Common.Primitives\Resources\Strings.resx 模型类库字符串资源