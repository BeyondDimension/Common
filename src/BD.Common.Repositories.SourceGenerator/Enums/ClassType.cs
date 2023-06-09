namespace BD.Common.Repositories.SourceGenerator.Enums;

/// <summary>
/// 类类型
/// </summary>
public enum ClassType : byte
{
    /// <summary>
    /// 实体类型
    /// </summary>
    Entities,

    /// <summary>
    /// 后台管理页面添加模型
    /// </summary>
    BackManageAddModels,

    /// <summary>
    /// 后台管理页面编辑模型
    /// </summary>
    BackManageEditModels,

    /// <summary>
    /// 后台管理页面表格查询模型
    /// </summary>
    BackManageTableModels,
}
