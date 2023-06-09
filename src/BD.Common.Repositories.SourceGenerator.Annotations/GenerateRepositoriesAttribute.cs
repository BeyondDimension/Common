namespace BD.Common.Repositories.SourceGenerator.Annotations;

/// <summary>
/// 配置式后台管理系统 CURD 增量源码生成特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GenerateRepositoriesAttribute : Attribute
{
    /// <summary>
    /// 是否需要生成表实体模型，默认值为：<see langword="true"/>
    /// </summary>
    public bool Entity { get; set; } = true;

    /// <summary>
    /// 是否需要生成后台管理添加模型，默认值为：<see langword="true"/>
    /// </summary>
    public bool BackManageAddModel { get; set; } = true;

    /// <summary>
    /// 是否需要生成后台管理编辑模型，默认值为：<see langword="true"/>
    /// </summary>
    public bool BackManageEditModel { get; set; } = true;

    /// <summary>
    /// 是否需要生成后台管理表格查询模型，默认值为：<see langword="true"/>
    /// </summary>
    public bool BackManageTableModel { get; set; } = true;

    /// <summary>
    /// 是否需要生成仓储层，默认值为：<see langword="true"/>
    /// </summary>
    public bool Repository { get; set; } = true;

    /// <summary>
    /// 是否需要生成控制器，默认值为：<see langword="true"/>
    /// </summary>
    public bool ApiController { get; set; } = true;

    /// <summary>
    /// 如果需要生成实体类型且主键为 <see cref="Guid"/> 时是否需要继承接口 INEWSEQUENTIALID，默认值为：<see langword="true"/>
    /// </summary>
    public bool NEWSEQUENTIALID { get; set; } = true;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetValue(string key, object? value)
    {
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
            case nameof(BackManageTableModel):
                BackManageTableModel = Convert.ToBoolean(value);
                break;
            case nameof(Repository):
                Repository = Convert.ToBoolean(value);
                break;
            case nameof(ApiController):
                ApiController = Convert.ToBoolean(value);
                break;
            case nameof(NEWSEQUENTIALID):
                NEWSEQUENTIALID = Convert.ToBoolean(value);
                break;
        }
    }
}
