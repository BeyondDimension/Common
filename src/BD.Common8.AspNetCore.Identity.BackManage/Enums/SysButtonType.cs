namespace BD.Common8.AspNetCore.Enums;

/// <summary>
/// 操作按钮枚举
/// </summary>
public enum SysButtonType : byte
{
    /// <summary>
    /// 编辑
    /// </summary>
    Edit = 1,

    /// <summary>
    /// 删除
    /// </summary>
    Delete = 2,

    /// <summary>
    /// 查看详情
    /// </summary>
    Detail = 3,

    /// <summary>
    /// 新增
    /// </summary>
    Add = 4,

    /// <summary>
    /// 查询
    /// </summary>
    Query = 5,

    /// <summary>
    /// 其他自定义按钮
    /// </summary>
    Other = 10,
}
