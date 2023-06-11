namespace BD.Common.Repositories.SourceGenerator.Annotations;

/// <summary>
/// 用于后台管理添加或编辑的字段特性
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class BackManageFieldAttribute : Attribute
{
    /// <summary>
    /// 指示该字段需要在后台管理中添加，默认值为：<see langword="true"/>
    /// </summary>
    public bool Add { get; set; } = true;

    /// <summary>
    /// 指示该字段需要在后台管理中编辑，默认值为：<see langword="false"/>
    /// </summary>
    public bool Edit { get; set; }

    /// <summary>
    /// 指示该字段需要在后台管理中作为详情只读仅展示，默认值为：<see langword="false"/>
    /// </summary>
    public bool Detail { get; set; }

    /// <summary>
    /// 指示该字段需要在后台管理中表格作为【查询结果】，默认值为：<see langword="true"/>
    /// </summary>
    public bool Table { get; set; } = true;

    /// <summary>
    /// 指示该字段需要在后台管理中表格作为【查询条件】，默认值为：<see langword="false"/>
    /// </summary>
    public bool Query { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetValue(string key, object? value)
    {
        switch (key)
        {
            case nameof(Add):
                Add = Convert.ToBoolean(value);
                break;
            case nameof(Edit):
                Edit = Convert.ToBoolean(value);
                break;
            case nameof(Detail):
                Detail = Convert.ToBoolean(value);
                break;
            case nameof(Table):
                Table = Convert.ToBoolean(value);
                break;
            case nameof(Query):
                Query = Convert.ToBoolean(value);
                break;
        }
    }
}
