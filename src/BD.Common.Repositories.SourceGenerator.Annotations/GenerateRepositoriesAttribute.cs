namespace BD.Common.Repositories.SourceGenerator.Annotations;

/// <summary>
/// 配置式后台管理系统 CURD 增量源码生成特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GenerateRepositoriesAttribute : Attribute
{
    /// <summary>
    /// 是否需要生成【表实体】模型，默认值为：<see langword="true"/>
    /// </summary>
    public bool Entity { get; set; } = true;

    /// <summary>
    /// 是否需要生成后台管理【添加】模型，默认值为：<see langword="true"/>
    /// </summary>
    public bool BackManageAddModel { get; set; } = true;

    /// <summary>
    /// 是否需要生成后台管理【添加】功能，默认值为：<see langword="true"/>
    /// </summary>
    public bool BackManageCanAdd { get; set; } = true;

    /// <summary>
    /// 指定生成后台管理【添加】仓储层函数实现种类，默认值为：<see cref="RepositoryMethodImplType.Expression"/>
    /// </summary>
    public RepositoryMethodImplType BackManageAddMethodImplType { get; set; } = RepositoryMethodImplType.Expression;

    /// <summary>
    /// 是否需要生成后台管理【编辑】模型，默认值为：<see langword="true"/>
    /// </summary>
    public bool BackManageEditModel { get; set; } = true;

    /// <summary>
    /// 是否需要生成后台管理【编辑】功能，默认值为：<see langword="true"/>
    /// </summary>
    public bool BackManageCanEdit { get; set; } = true;

    /// <summary>
    /// 后台管理【编辑】模型是否仅作为详情展示的只读用途，决定是否生成 Update 函数，默认值为：<see langword="false"/>
    /// </summary>
    public bool BackManageEditModelReadOnly { get; set; }

    /// <summary>
    /// 指定生成后台管理【编辑】仓储层函数实现种类，默认值为：<see cref="RepositoryMethodImplType.Expression"/>
    /// </summary>
    public RepositoryMethodImplType BackManageEditMethodImplType { get; set; } = RepositoryMethodImplType.Expression;

    /// <summary>
    /// 是否需要生成后台管理【表格查询】模型，默认值为：<see langword="true"/>
    /// </summary>
    public bool BackManageTableModel { get; set; } = true;

    /// <summary>
    /// 是否需要生成后台管理【表格查询】功能，默认值为：<see langword="true"/>
    /// </summary>
    public bool BackManageCanTable { get; set; } = true;

    /// <summary>
    /// 指定生成后台管理【表格查询】仓储层函数实现种类，默认值为：<see cref="RepositoryMethodImplType.Expression"/>
    /// </summary>
    public RepositoryMethodImplType BackManageTableMethodImplType { get; set; } = RepositoryMethodImplType.Expression;

    /// <summary>
    /// 指定查询的默认值排序行为，默认值为 ".OrderByDescending(x => x.CreationTime).ThenBy(x => x.Id)"
    /// </summary>
    public string? DefaultOrderBy { get; set; }

    /// <summary>
    ///  指定生成后台管理【表格查询】仓储层函数实现需要关联查询的表，例如 "Include(x => x.XYAB)"
    /// </summary>
    public string? BackManageTableQueryInclude { get; set; }

    /// <summary>
    /// 是否需要生成【仓储层】，默认值为：<see langword="true"/>
    /// </summary>
    public bool Repository { get; set; } = true;

    /// <summary>
    /// 是否需要生成【仓储层构造函数】，如果为 <see langword="null"/> 则不生成构造函数，否则将根据参数类型自动生成，默认值为：ArrayEmpty
    /// </summary>
    public string[]? RepositoryConstructorArguments { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 仓储层构造函数中是否需要生成 AutoMapper.IMapper
    /// </summary>
    public bool RepositoryConstructorArgumentMapper { get; set; } = true;

    /// <summary>
    /// 仓储层【数据库上下文】接口约束
    /// </summary>
    public string? DbContextBaseInterface { get; set; }

    /// <summary>
    /// 是否需要生成【控制器】，默认值为：<see langword="true"/>
    /// </summary>
    public bool ApiController { get; set; } = true;

    /// <summary>
    /// 是否需要生成【控制器构造函数】，如果为 <see langword="null"/> 则不生成构造函数，否则将根据参数类型自动生成，默认值为：ArrayEmpty
    /// </summary>
    public string[]? ApiControllerConstructorArguments { get; set; } = Array.Empty<string>();

    /// <summary>
    /// 控制器【路由前缀】，默认值为：<see langword="null"/>
    /// </summary>
    public string? ApiRoutePrefix { get; set; }

    /// <summary>
    /// 如果需要生成实体类型且主键为 <see cref="Guid"/> 时是否需要继承接口 INEWSEQUENTIALID，默认值为：<see langword="true"/>
    /// </summary>
    public bool NEWSEQUENTIALID { get; set; } = true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetValue(string key, object? value)
    {
        static string?[]? ToStringArray(object? value) =>
            value is string[] array ? array :
                (value is IEnumerable<string> enumerable ? enumerable.ToArray() :
                    (value is IEnumerable<object?> enumerable2 ? enumerable2.Select(x => x?.ToString()).ToArray() : null));
        switch (key)
        {
            case nameof(Entity):
                Entity = Convert.ToBoolean(value);
                break;
            case nameof(BackManageAddModel):
                BackManageAddModel = Convert.ToBoolean(value);
                break;
            case nameof(BackManageEditModel):
                BackManageEditModel = Convert.ToBoolean(value);
                break;
            case nameof(BackManageEditModelReadOnly):
                BackManageEditModelReadOnly = Convert.ToBoolean(value);
                break;
            case nameof(BackManageTableModel):
                BackManageTableModel = Convert.ToBoolean(value);
                break;
            case nameof(Repository):
                Repository = Convert.ToBoolean(value);
                break;
            case nameof(RepositoryConstructorArguments):
                RepositoryConstructorArguments = ToStringArray(value)!;
                break;
            case nameof(DbContextBaseInterface):
                DbContextBaseInterface = value?.ToString();
                break;
            case nameof(ApiController):
                ApiController = Convert.ToBoolean(value);
                break;
            case nameof(ApiControllerConstructorArguments):
                ApiControllerConstructorArguments = ToStringArray(value)!;
                break;
            case nameof(ApiRoutePrefix):
                ApiRoutePrefix = value?.ToString();
                break;
            case nameof(NEWSEQUENTIALID):
                NEWSEQUENTIALID = Convert.ToBoolean(value);
                break;
        }
    }
}
