### [DbContext 生存期、配置和初始化](https://learn.microsoft.com/zh-cn/ef/core/dbcontext-configuration)
当前项目中在 BD.WTTS.BackManage 中的 BackManageDbContext 作为主要 DbContext，微服务业务模块中的仅为部分表的 DbContext，迁移需使用 BackManageDbContext  
迁移源文件生成在 BD.WTTS.BackManage\Migrations

### [创建并配置模型](https://learn.microsoft.com/zh-cn/ef/core/modeling)
优先使用 [数据注释(Annotations)](https://learn.microsoft.com/zh-cn/ef/core/modeling/#use-data-annotations-to-configure-a-model) 而不是 Fluent API 配置列或表
例如表名重命名，通常实体类名+s复数作为表名称
```
[Table("Blogs")]
public class Blog ...
```
[索引](https://learn.microsoft.com/zh-cn/ef/core/modeling/indexes?tabs=data-annotations)  
```
[Index(nameof(Url))]
public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
}
```
[最大长度](https://learn.microsoft.com/zh-cn/ef/core/modeling/entity-properties?tabs=data-annotations%2Cwithout-nrt#maximum-length)
```
public class Blog
{
    [MaxLength(500)]
    public string Url { get; set; }
}
```
[精度和小数位数](https://learn.microsoft.com/zh-cn/ef/core/modeling/entity-properties?tabs=data-annotations%2Cwithout-nrt#maximum-length)  
```
public class Blog
{
    [Precision(14, 2)]
    public decimal Score { get; set; }
    [Precision(3)]
    public DateTime LastUpdated { get; set; }
}
```
[列注释](https://learn.microsoft.com/zh-cn/ef/core/modeling/entity-properties?tabs=data-annotations%2Cwithout-nrt#column-comments)
```
public class Blog
{
    [Comment("The URL of the blog")]
    public string Url { get; set; }
}
```
[序列](https://learn.microsoft.com/zh-cn/ef/core/modeling/sequences)  
[数据种子设定](https://learn.microsoft.com/zh-cn/ef/core/modeling/data-seeding)  


### [EF Core 迁移](https://docs.microsoft.com/zh-cn/ef/core/managing-schemas/migrations/?tabs=vs)
创建迁移
```
Add-Migration InitialCreate -Context BackManageDbContext
```

更新迁移
```
Update-Database -Context BackManageDbContext
```

生成 SQL 脚本
```
Script-Migration InitialCreate -Context BackManageDbContext
```

创建分布式 SQL Server 缓存
```
dotnet sql-cache create "Server=192.168.1.22;Database=Recharge; User=sa;Password=Aa123456;" dbo DistributedCache
```

### [ExecuteUpdate 和 ExecuteDelete (批量更新)](https://learn.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates)
```
await context.Blogs.ExecuteUpdateAsync(
    s => s.SetProperty(b => b.Name, b => b.Name + " *Featured!*"));
```
```
UPDATE [b]
    SET [b].[Name] = [b].[Name] + N' *Featured!*'
FROM [Blogs] AS [b]
```

### [关系](https://learn.microsoft.com/zh-cn/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key)
- [一对一](https://learn.microsoft.com/zh-cn/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#one-to-one)
- [多对多](https://learn.microsoft.com/zh-cn/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#many-to-many)

### [级联删除](https://learn.microsoft.com/zh-cn/ef/core/saving/cascade-delete)

### [使用事务](https://learn.microsoft.com/zh-cn/ef/core/saving/transactions)

### [性能简介](https://learn.microsoft.com/zh-cn/ef/core/performance/)

### 函数映射
- [Microsoft SQL Server](https://learn.microsoft.com/zh-cn/ef/core/providers/sql-server/functions)
- [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/Translations)
- [SQLite](https://learn.microsoft.com/zh-cn/ef/core/providers/sqlite/functions)
- [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.npgsql.org/efcore/mapping/translations.html)

### [多租户](https://learn.microsoft.com/zh-cn/ef/core/miscellaneous/multitenancy)
许多业务线应用程序旨在与多个客户合作。 保护数据非常重要，这样客户数据就不会被其他客户和潜在竞争对手“泄露”或看到。 这些应用程序被归类为“多租户”，因为每个客户都被视为具有其自己的数据集的应用程序租户。

### [Code First 的自包含类型配置](https://learn.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-2.0/#self-contained-type-configuration-for-code-first)
在 EF6 中可以通过从 EntityTypeConfiguration 派生来封装特定实体类型的 Code First 配置。 在 EF Core 2.0 中重新添加了此模式：
```
class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.AlternateKey);
        builder.Property(c => c.Name).HasMaxLength(200);
    }
}

...
// OnModelCreating
builder.ApplyConfiguration(new CustomerConfiguration());
```

### 表实体接口
- ```IEntity<TPrimaryKey>``` / ```Entity<TPrimaryKey>```
    - 实体基类接口与抽象类，定义主键类型
- ```CreationBaseEntityV2 : Entity```
    - 包含**创建时间**与**创建人**
    - 需要继承自嵌套类 ```EntityTypeConfiguration``` 配置数据库关系
- ```OperatorBaseEntityV2 : CreationBaseEntityV2``` 
    - 包含**修改时间**与**操作人**与**创建时间**与**创建人**
    - 需要继承自嵌套类 ```EntityTypeConfiguration``` 配置数据库关系
- ```TenantBaseEntityV2 : OperatorBaseEntityV2``` 
    - 包含**租户**与**软删除**与**修改时间**与**操作人**与**创建时间**与**创建人**
    - 需要继承自嵌套类 ```EntityTypeConfiguration``` 配置数据库关系

### 列接口，用于统一字段命名与接口注释以及根据类型附加默认值或设置索引等一些额外操作
- 头像 Avatar
    - Guid 关联到图片表，由 Get api/Image/Guid 获取图片
    - 为 null 或 默认值时将使用返回默认头像
    - 接口 IAvatar 需要实现 get/set
    - 接口 IReadOnlyAvatar 仅需实现 get
- 创建时间 CreationTime
    - 类型使用 DateTimeOffset 带时区
    - 使用 ToUnixTimeMilliseconds 可转换为 Unix 时间戳毫秒 或 ToUnixTimeSeconds 转换为秒单位
    - 在 [BuildEntities](https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common.EFCore/Extensions/ModelBuilderExtensions.cs#L90) 中实现插入数据库时自动赋值当前时间
    - 接口 ICreationTime 需要实现 get/set
- 更新时间 UpdateTime
    - 类型使用 DateTimeOffset 带时区
    - 在 [BuildEntities](https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common.EFCore/Extensions/ModelBuilderExtensions.cs#L102) 与 [DbContext.SetUpdateTime](https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common.EFCore/Data/Abstractions/IDbContext.cs#L7) 中实现更改该行数据时自动赋值当前时间
    - 接口 IUpdateTime 需要实现 get/set
- 是否禁用 Disable
    - 在 [BuildEntities](https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common.EFCore/Extensions/ModelBuilderExtensions.cs#L139) 中实现默认值为 false
    - 接口 IDisable 需要实现 get/set
- 禁用原因 DisableReason
    - 无字符串最大长度限制
    - 接口 IDisableReason 需要实现 get/set
- 性别 Gender
    - enum Gender 类型为 byte
        - 0 = 未知
        - 1 = 男
        - 2 = 女
    - ~~不考虑增加跨性别等美式政治正确性别类型~~
    - 接口 IGender 需要实现 get/set
    - 接口 IReadOnlyGender 仅需实现 get
- 昵称 NickName
    - 接口 INickName 需要实现 get/set
    - 接口 IReadOnlyNickName 仅需实现 get
- 排序 Order
    - long Order { get; set; }
    - 在 [BuildEntities](https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common.EFCore/Extensions/ModelBuilderExtensions.cs#L42) 中使用序列中取值作为默认值，并加入索引 Index
    - 序列将从小到大生成值，排序为正序或倒序取决于具体业务
- 排序(旧版本方案) OrderInt32
    - int Order { get; set; }
    - 已弃用，仅因兼容保留
- 是否置顶 IsTop
    - bool IsTop { get; set; }
- IP 地址 IPAddress
    - string IPAddress { get; set; }
    - 在 Mvc 中可通过 Request 调用 [UserHostAddress](https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common.AspNetCore/Extensions/SystemWebExtensions.cs#L140) 获取客户端 IP 地址，在 Docker 容器以及反向代理中需要一些额外的配置才能拿到正确的地址
- 密码 Password
    - 通常密码不应以明文存储，应使用像 ASP.NET Core Identity 中的 [PasswordHasher](https://github.com/dotnet/aspnetcore/blob/v7.0.3/src/Identity/Extensions.Core/src/PasswordHasher.cs) 存储哈希值
    - 存储哈希值后在服务端是看不到用户的明文密码的，如果需要强行将已忘记的密码重置，可以将值改为某个已知明文密码哈希后的值，来重置改密码为某个值
- 短信验证码值 SmsCode
    - 记录下发或提交的短信验证码值
    - 接口 ISmsCode 需要实现 get/set
    - 接口 IReadOnlySmsCode 仅需实现 get
- 备注 Remarks
    - string? Remarks { get; set; }
    - 由业务上定义最大字符串长度或无限长
- 软删除 SoftDeleted
    - bool SoftDeleted { get; set; }
    - 在 [BuildEntities](https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common.EFCore/Extensions/ModelBuilderExtensions.cs#L75) 使用 [HasQueryFilter](https://learn.microsoft.com/zh-cn/ef/core/querying/filters) 实现默认全局过滤被软删除的数据
    - 可通过 [IgnoreQueryFilters](https://learn.microsoft.com/zh-cn/ef/core/querying/filters#disabling-filters) 忽略该过滤筛选
- 租户 Tenant
    - Guid TenantId { get; set; }
    - 多租户系统需要对相关表加上租户 Id，根据当前租户进行过滤
- 标题 Title
    - string Title { get; set; }
    - 由业务上定义最大字符串长度或无限长
- 创建用户 CreateUserId/CreateUserIdNullable
    - Guid CreateUserId { get; set; }
    - Guid? CreateUserId { get; set; }
    - 记录当前行数据是由某个后台管理员用户或用户侧用户**创建**的数据，需要相关业务部分手动赋值
- 操作用户 OperatorUserId
    - Guid? OperatorUserId { get; set; }
    - 记录当前行数据是由某个后台管理员用户或用户侧用户**更新**的数据，需要相关业务部分手动赋值
- INEWSEQUENTIALID 用于主键类型为 Guid 的主键自增
    - SQL Server 中使用 [NEWSEQUENTIALID (Transact-SQL)](https://learn.microsoft.com/zh-cn/sql/t-sql/functions/newsequentialid-transact-sql?view=sql-server-ver16)
    - PostgreSQL 中使用 [gen_random_uuid()](https://www.postgresql.org/docs/15/functions-uuid.html)

### [常用最大长度常量](https://github.com/BeyondDimension/Common/blob/1.23.10304.11805/src/BD.Common.Primitives/Columns/MaxLengths.cs)
- ColorHex
    - MaxLengths.ColorHex = 9
    - 颜色16进制值，#AARRGGBB
- NickName
    - MaxLengths.NickName = 20
    - 昵称
- SMS_CAPTCHA
    - MaxLengths.SMS_CAPTCHA = 6
    - 短信验证码
- UserName
    - MaxLengths.UserName = 128
    - 用户名(用户名不是昵称，通常为唯一键，因可用用户名进行登录)
- Url
    - MaxLengths.Url = 2048
- Text
    - MaxLengths.Text = 1000
    - 一般文本字符串
- FileExtension
    - MaxLengths.FileExtension = 16
    - 文件扩展名，例如 .exe/.dll
- WeChatId
    - MaxLengths.WeChatId = 128
    - 微信 OpenId
- WeChatUnionId
    - MaxLengths.WeChatUnionId = 192
    - 微信 UnionId
- Email
    - MaxLengths.Email = 256
    - 邮箱地址
- DisableReason
    - MaxLengths.DisableReason = 1000
    - 禁用原因
- Remarks
    - MaxLengths.Remarks = 1000
    - 备注
- RealityAddress
    - MaxLengths.RealityAddress = 150
    - 现实地址/收货地址
- Title
    - MaxLengths.Title = 30
    - 标题
- LongTitle
    - MaxLengths.LongTitle = 200
    - 长标题
- Describe
    - MaxLengths.Describe = 500
    - 描述

### Repository 仓储层
- 增(Insert Funs) 立即执行并返回受影响的行数
    - ```InsertAsync``` 将实体插入到数据库中
    - ```InsertRangeAsync``` 将多个实体插入到数据库中
- 删(Delete Funs) 立即执行并返回受影响的行数
    - ```DeleteAsync``` 将实体从数据库中删除
    - ```DeleteRangeAsync``` 将多个实体从数据库中删除
- 改(Update Funs) 立即执行并返回受影响的行数
    - ```UpdateAsync``` 将实体更新到数据库中
    - ```UpdateRangeAsync``` 将多个实体更新到数据库中
- 增或改(InsertOrUpdate Funs) 立即执行并返回受影响的行数
    - ```InsertOrUpdateAsync``` 新增或更新实体
        - ```InsertOrUpdateAsync<TDTO>``` 版本支持直接使用 DTO 进行新增或更新
- 查(通用查询)
    - ```FirstOrDefaultAsync``` 返回序列中满足指定的条件或默认值，如果找到这样的元素的第一个元素
    - ```FindAsync``` 根据主键查询实体
    - ```ExistAsync``` 判断实体是否已经存在
- Expression 表达式
    - ```LambdaEqualId``` 返回主键匹配表达式
- 其他函数
    - ```IsDefault``` 判断主键是否为默认值
- ```IEFRepository``` EF Core 实现的额外内容
    - ```DbContext``` 获取当前数据库上下文
    - ```TableName``` 当前仓储层表名称，用于一些特殊场景下拼接 SQL 语句，应当尽可能避免拼接 SQL 语句执行
    - ```SaveChangesAsync``` 保存并更改已跟踪的实体
    - ```Entity``` 当前表的实体 ```DbSet<TEntity>```
    - ```EntityNoTracking``` 调用 ```AsNoTrackingWithIdentityResolution``` 的不跟踪的表实体 ```IQueryable<TEntity>```
    - ```RequestAborted``` 取消标记，实现通常为 ```HttpContext.RequestAborted```，传递此取消标记到数据库异步操作中可在中断 Http 连接时取消该请求的数据库操作，提高服务的吞吐量

### 扩展函数
- 删(Delete Funs) 立即执行并返回受影响的行数
    - 根据条件执行删除
        - ```GeneralDeleteAsync(IQueryable<TEntity> ...```
            - 根据条件常规删除（批量删除或软删除），根据实体是否继承了 ```ISoftDeleted``` 自动检测
        - ```SoftDeleteAsync(IQueryable<TEntity> ...```
            - 根据条件批量软删除
    - 根据主键进行删除/根据多个主键进行删除
        - ```GeneralDeleteByIdAsync(IQueryable<TEntity> ...```
            - 根据主键常规删除（批量删除或软删除），根据实体是否继承了 ```ISoftDeleted``` 自动检测
        - SoftDeleteByIdAsync
        - HardDeleteByIdAsync
            - 根据主键硬删除（从数据库中删除数据）
    - 根据实体进行删除/根据多个实体进行删除
        - GeneralDeleteAsync
        - SoftDeleteAsync
        - HardDeleteAsync