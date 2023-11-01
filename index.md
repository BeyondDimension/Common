### Code Style 代码风格 与 编码规范
普通的工程师堆砌代码，优秀的工程师优雅代码，卓越的工程师简化代码。如何写出优雅整洁易懂的代码是一门学问，也是软件工程实践里重要的一环  
在我们所有的项目源码中 src 目录下有一个 .editorconfig 文件， 也在 VS 解决方案中 顶部的 Root 文件夹下  
其中配置了一些代码样式规则， 相对于其他项目， 一些样式不规范也将被视为错误 阻止编译的执行  
还使用了 StyleCop.Analyzers 加入了一些样式规则，关于 StyleCop.Analyzers 以下有一些博客园的文章介绍  
https://www.cnblogs.com/mondol/p/6475957.html  
https://www.cnblogs.com/liangqihui/p/9241603.html  
好的代码质量有助于提高项目的维护性，降低其他人接手的成本。

- [C# Coding Style](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md)

- [C# at Google Style Guide](https://google.github.io/styleguide/csharp-style.html)

- [SA1413](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/1.2.0-beta.435/documentation/SA1413.md)  
多行 C# 初始值设定项或列表中的最后一条语句缺少尾随逗号。  
在 JSON 这种格式中， 最后一条末尾是不能有逗号的，但在 C# 中， 如果之后需要新加一行写新的字段，那么已有逗号能够方便后续维护的人添加。  
- **if + return** 不推荐在一行上简化  
if 简化大括号并没有问题，但是 return 最好换行  
在一行中看上去并不直观，推荐使用新行 return
- **注释**  
    - **单行**注释  
    双斜杠+空格+说明  
    例如 // TODO  
    双斜杠后面不要直接跟说明文字  
    - **类型或属性**注释
    注释可使用 /// <inheritdoc cref="指向某个类型或方法或属性等其他能写注释的地方"/> 引用其他地方的注释，比如较多相同的地方需要写上注释的时候，例如  
        ```
        /// <inheritdoc cref="IAuthMessageRecord"/>
        public abstract class AuthMessageRecordBase
        ```
    - 注释中不需要写多余的内容，例如类型已在属性(Properties) 中声明，无需再注释中标明  
- 文件范围的命名空间声明
    - 当一个源文件中仅有一个命名空间时应优先使用文件范围的命名空间声明
    - https://zhuanlan.zhihu.com/p/439464101
    - https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/namespace
    - 在命名空间与类型中间需要加入一个空格
- [IDE0005](https://learn.microsoft.com/zh-cn/dotnet/fundamentals/code-analysis/style-rules/ide0005)  
删除不必要的 using 指令  
- [IDE0042](https://learn.microsoft.com/zh-cn/dotnet/fundamentals/code-analysis/style-rules/ide0042)  
析构变量声明  
- [CA2254](https://learn.microsoft.com/zh-cn/dotnet/fundamentals/code-analysis/quality-rules/ca2254)  
模板应为静态表达式  

#### RESTful API
- [RESTful API 一种流行的 API 设计风格](https://restfulapi.cn/)
- [RESTful API 设计指南 - 阮一峰的网络日志](https://ruanyifeng.com/blog/2014/05/restful_api.html)
- [RESTful API 最佳实践 - 阮一峰的网络日志](https://www.ruanyifeng.com/blog/2018/10/restful-api-best-practices.html)

#### 文件夹与命名空间
通常业务文件夹中的命名空间保持和上一级文件夹一致  
在命名空间上方插入一行注释  
```// ReSharper disable once CheckNamespace```  
避免命名空间与文件夹不一致的警告  
例如在 Entities 中有多个业务文件夹，例如 Entities\微服务业务名 等  

#### Helpers 文件夹
存放助手类， 通常类型名称使用 Helper 结尾，并且命名空间保持上一级文件夹一致，避免使用 Util/Utility 的命名，除非是从别的源码中 Copy，例如  
https://github.com/microsoft/nodejstools/blob/main/Nodejs/Product/Nodejs/SharedProject/SystemUtilities.cs  

#### Abstractions 文件夹
存放接口，抽象类等抽象内容  
例如 Repositories\Abstractions, Entities\Abstractions, Models\Abstractions  

#### Services 文件夹
存放服务的接口，例如 IPlatformService 平台服务  
Services.Implementation 存放接口的实现类  

#### 资源项目 Resources 
Strings.resx 可由社区 PR 贡献多语言翻译，也可使用翻译工具自动机翻  
资源键中，值有 {0} 这类需要 String.Format 的内容，键末尾加上下划线_, 有几个参数就加几个下划线  
例如值为 【昵称最大长度不能超过{0}个字】，键为【昵称最大长度不能超过_】  
使用时，应当用扩展函数的 Format 进行拼装字符串，具体实现在下方的链接，因为不确定机翻后可能将此占位符修改成其他内容， String.Format 在没有正确的占位符下调用会抛出异常  
https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common/Extensions/StringExtensions.Format.cs#L6-L16  

#### 基本层规范 Primitives
此命名借鉴 Microsoft.Extensions.Primitives  
存放 Enums 所有相关的枚举， Constants 常量值， Columns 列的接口等一些基本共享内容  
Columns 中的接口， 根据 .NET 规范 接口使用 I 开头， 对于列仅需要 get 使用 IReadOnly列名 作为接口类型名称  
不带 ReadOnly 的需要实现 set  
Enums 中枚举名称不用 E 开头， 某些第三方库会像接口用 I 开头一样使用 E 开头，或者末尾加上 Enum， 不推荐  
枚举使用值枚举+扩展函数，不推荐使用自定义类作为枚举，值枚举性能优先  
枚举类型不是很多的情况下，指定继承 byte 可节约一些内存， 抠一点性能优化 例如 public enum X : byte  

#### 模型层/视图模型层规范 Primitives.Models/Primitives.ViewModels 
XXX.Primitives.Models 存放模型类， XXX.Primitives.ViewModels 通常仅 Link 模型类，通过 DefineConstants 定义符号 MVVM_VM 配合 #if 标注一些在客户端 MVVM 上使用的内容  
通常表实体映射成 DTO，对应的类名在末尾加上 DTO，例如 User 表实体，与 UserDTO  
就 DTO 字段不用写注释，类型名上自己查表类型看字段的注释了，遵循这个约定就好了  
用于 API 的 Request Body 模型在名称末尾加上 Request，例如 LoginOrRegisterRequest  
Response 也一样  
DTO 推荐使用 AutoMapper 映射  

#### 仓储层规范
**仓储层持有 DbContext 对象， 所有对数据库的操作应在仓储层中执行， 返回的对象必须都为 DTO，不能将表实体类型返回出去**
**因微服务业务拆分多个 DbContext， 对于父类接口等抽象比较深，在仓储层使用的是 DbContext 接口， 如果没有对应的表实体类型，要么是业务拆分有问题，引用了其他微服务的表，要强行使用只能通过 DbSet 指定泛型获取，或将接口转换为其他微服务的 DbContext 接口，但通常不一定会有该实体**  
Data 存放 DbContext， EF 的数据库上下文
Entities 存放表实体，实体类型通常使用 [数据注释(Annotations)](https://learn.microsoft.com/zh-cn/ef/core/modeling/#use-data-annotations-to-configure-a-model)  
更改表名称， 在数据库中的表名称默认为复数单词，通常在末尾加 s 即可，但也要注意一些单词的复数并不是加s，例如 Repository 的复数为 Repositories, Property 的复数为 Properties  
例如 ```[Table(nameof(AppVer) + "s")]```  
仓储层的接口与实现类型，需要在依赖注入中手动配置，实现类型中 DbContext 使用的泛型，因 WTTS 项目中业务使用不同的精简 DbContext， 之前有方案利用反射自动搜索类型添加进依赖注入配置，但考虑性能因素以及未来的裁剪与 AOT，还是手动写一下比较合适，依赖注入配置位于相关的启动项目 WebAPI 的 ProgramConstants 类的 AddRepositories 中的 AddXXXRepositories XXX为该微服务的业务名称，例如大数据分析业务微服务的名称为 AddBigDataAnalysisRepositories
通用函数命名规范  
- QueryAsync 后台表格查询，或其他查询返回集合或数组的  
- InsertOrUpdateAsync 插入或更新一行数据  
- GetEditByIdAsync 根据主键获取编辑的 DTO  
- SetDisableByIdAsync 根据主键设置是否禁用  
- GetSelectAsync 获取用于选择框的 DTO 数据，类型使用 ```SelectItemDTO<T>``` 泛型  

#### ApiResponse 类型
这些类型都定义了 implicit operator 实现了 Code/Message/T 类型的转换。  
例如在返回错误语的时候，直接 return string; 可直接隐式转换， 前提是 T 类型不能为 string，不然有二义性错误。  
后台管理系统中使用的是 BD.Common.Models.ApiResponse  
通过 bool IsSuccess 来判断是否成功  
错误消息为 string[] Messages， 前端使用换行拼接或者多行显示  
可选的响应类型为泛型 T? Data  
微服务模块中使用的类型(仅 WTTS 项目)为 BD.WTTS.Models.ApiRsp  
其中有自定义 Code 枚举 BD.WTTS.Enums.ApiRspCode Code  
但也是通过 bool IsSuccess 来判断是否成功， 其中的实现为 code >= 200 && code <= 299  
错误消息为 string Message， 与后台不同的是，此处不为数组  

#### WebAPI 控制器规范
**控制器不应持有 DbContext， 通过构造函数注入 仓储层执行操作**  
需要注意一点的是，用户侧的微服务与后台管理，用的控制器基类是不同的  
基类默认路由都是 ```api/[controller]```  
注意业务控制器构造函数中的 ```ILogger<T>``` 与基类如果需要泛型， 这个泛型都是当前控制器类型，复制粘贴的时候不要漏了改  
其中 AllowAnonymous 是允许匿名访问的控制器基类， 在这个基类中取 UserId 可能为 null  
BasicAuthorize 是需要登录才能访问的， 在这个基类中取 UserId 必定有值  
在用户侧微服务上，控制器基类是 ```ApiController.AllowAnonymous``` 与 ```ApiController.BasicAuthorize```  
后台管理中， 基类为 ```AllowAnonymousApiController<T>``` 与 ```BaseAuthorizeController<T>```， 没有 MVC 的版本  
MVC 版本的控制器是将 ```ApiController``` 部分替换成 ```ViewController``` 相比 API 的版本有页面相关的函数，例如 ```View()```  

后台中有一个 ```InfoController``` 在 Get ```api/info``` 接口上有一个调试用的输出，打印一些显示数据，例如当前客户端 IP，当前时间等  
Post 此接口可进行后台初始化，包括 添加管理员用户与预设角色 添加租户 添加预设菜单 添加预设按钮 添加预设菜单按钮关系 等等，在此 Post 方法中可以看到一个简单的密码验证，用于校验是否有权限调用此接口  

Url 相关的全部使用小写字母， 参数名除外，例如 ```[HttpGet("info/{userId}")]```  
根据 RESTful API 控制器名如果是操作表的增删改查，通常为复数单词，除非像 UserController 这个控制器，仅管理后台当前登录的用户个人资料才不使用复数  
API 响应模型统一使用 ```ApiResponse```， 方法返回类型为 ```Task<ApiResponse>```  

控制器方法命名  
对于路由中不包含 ```[action]``` 的，通常是使用什么 Http 方法，就叫什么名字，例如使用的是 ```[HttpGet]``` 就叫 Get 函数名  
**分页查询表格**  
对于分页查询的表格使用 ```PagedModel```，例如 ```Task<ApiResponse<PagedModel<DTO类型>>>>```  
分页查询使用扩展函数 ```PagingAsync``` 在 Common 库上实现  
分页查询之前必须排序！  
分页查询前两个参数统一为 当前页码 与 分页大小  
```
[FromQuery] int current = IPagedModel.DefaultCurrent,
[FromQuery] int pageSize = IPagedModel.DefaultPageSize,
[HttpGet, PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
public Task<ApiResponse<PagedModel<DTO类型>>> Get(
        [FromQuery] int current = IPagedModel.DefaultCurrent,
        [FromQuery] int pageSize = IPagedModel.DefaultPageSize,
```
对应仓储层中的函数名应为 ```QueryAsync```
用于下拉框的动态数据， 统一使用 ```SelectItemDTO``` 类型
```
/// <summary>
/// 用于下拉框的查询业务表类型
/// </summary>
/// <returns></returns>
[HttpGet("select")]
public async Task<ApiResponse<SelectItemDTO<表的主键>[]>> Get()
```
**新增**
```
/// <summary>
/// 创建业务表类型
/// </summary>
/// <param name="model"></param>
/// <returns></returns>
[HttpPost, PermissionFilter(ControllerName + nameof(SysButtonType.Add))]
public async Task<ApiResponse> Post([FromBody] AddDTO类型 model)
```
**修改**
```
/// <summary>
/// 修改业务表类型
/// </summary>
/// <param name="id"></param>
/// <param name="model"></param>
/// <returns></returns>
[ProducesResponseType(StatusCodes.Status404NotFound)]
[HttpPut("{id}"), PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
public async Task<ApiResponse> Put([FromRoute] int id, [FromBody] EditDTO类型 model)
```
EditDTO 类型  与 AddDTO 类型 可使用同一个类型，或者根据业务需要，使用两个不同的类型，如果使用同一个类型，类型名称前缀最好使用 AddOrEditXXXDTO
**删除/软删除/启用或禁用**
取决于业务需求， 启用或禁用权限为 ```SysButtonType.Edit```，删除则应改为 ```SysButtonType.Delete```
参数名同样需改为 isDelete，而不是 disable
如果同时有删除以及启用或禁用，那么启用或禁用考虑改为 HttpPut 而不是 HttpDelete
```
/// <summary>
/// 启用或禁用产品密钥类型
/// </summary>
/// <param name="id"></param>
/// <param name="disable"></param>
/// <returns></returns>
[HttpDelete("{id}/{disable}"), PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
public async Task<ApiResponse> Delete([FromRoute] int id, [FromRoute] bool disable)
```