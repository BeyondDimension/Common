namespace BD.Common.Repositories.SourceGenerator.Templates.Abstractions;

/// <summary>
/// 仓储层源码生成模板基类
/// </summary>
/// <typeparam name="TTemplate"></typeparam>
/// <typeparam name="TTemplateMetadata"></typeparam>
public abstract class RepositoryTemplateBase<TTemplate, TTemplateMetadata> : TemplateBase<TTemplate, TTemplateMetadata>
    where TTemplate : RepositoryTemplateBase<TTemplate, TTemplateMetadata>, new()
    where TTemplateMetadata : ITemplateMetadata
{
    /** Functions
     * QueryAsync 后台表格查询，或其他查询返回集合或数组的
     * InsertAsync 插入一行数据
     * UpdateAsync 更新一行数据
     * GetEditByIdAsync 根据主键获取编辑的 DTO
     * SetDisableByIdAsync 根据主键设置是否禁用
     * GetSelectAsync 获取用于选择框的 DTO 数据，类型使用 SelectItemDTO<T> 泛型
     */

    public RepositoryTemplateBase()
    {
        IsInterface = typeof(TTemplate) == typeof(RepositoryTemplate);
    }

    protected virtual bool IsInterface { get; }

    protected void WriteMethodQueryAsync(
        Stream stream,
        TTemplateMetadata metadata,
        ImmutableArray<PropertyMetadata> fields)
    {
        var format =
"""
    /// <summary>
    /// 分页查询{0}表格
    /// </summary>
    /// <param name="current">当前页码，页码从 1 开始，默认值：<see cref="IPagedModel.DefaultCurrent"/></param>
    /// <param name="pageSize">页大小，如果为 0 必定返回空集合，默认值：<see cref="IPagedModel.DefaultPageSize"/></param>
"""u8;
        stream.WriteFormat(format, metadata.Summary);

        fields = fields.
            Where(x => x.BackManageField != null &&
                x.BackManageField.Query &&
                x.FixedProperty != FixedProperty.SoftDeleted).
            ToImmutableArray();
        foreach (var field in fields)
        {
            field.WriteParam(stream);
        }
        format =
"""

    /// <returns>{0}</returns>
    {1}Task<PagedModel<Table{2}DTO>> QueryAsync(
        int current = IPagedModel.DefaultCurrent,
        int pageSize = IPagedModel.DefaultPageSize
"""u8;
        stream.WriteFormat(format,
            $"{metadata.Summary}分页表格查询结果数据",
            IsInterface ? null : "public async "u8.ToArray(),
            metadata.ClassName);
    }
}
