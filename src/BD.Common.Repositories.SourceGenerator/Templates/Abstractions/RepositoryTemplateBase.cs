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

    /// <summary>
    /// 写入方法 - 后台表格查询
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="metadata"></param>
    /// <param name="fields"></param>
    protected void WriteMethodQuery(
        Stream stream,
        TTemplateMetadata metadata,
        ImmutableArray<PropertyMetadata> fields)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 分页查询{0}表格
    /// </summary>
    /// <param name="current">当前页码，页码从 1 开始，默认值：<see cref="IPagedModel.DefaultCurrent"/></param>
    /// <param name="pageSize">页大小，如果为 0 必定返回空集合，默认值：<see cref="IPagedModel.DefaultPageSize"/></param>
"""u8;
        stream.WriteFormat(utf8String, metadata.Summary);

        fields = fields.
            Where(x => x.BackManageField != null &&
                x.BackManageField.Query &&
                x.FixedProperty != FixedProperty.SoftDeleted).
            ToImmutableArray();
        foreach (var field in fields)
        {
            field.WriteParam(stream);
        }
        utf8String =
"""

    /// <returns>{0}</returns>
    {1}Task<PagedModel<Table{2}DTO>> QueryAsync(
        int current = IPagedModel.DefaultCurrent,
        int pageSize = IPagedModel.DefaultPageSize
"""u8;
        stream.WriteFormat(utf8String,
            $"{metadata.Summary}分页表格查询结果数据",
            IsInterface ? null : "public async "u8.ToArray(),
            metadata.ClassName);
    }

    /// <summary>
    /// 写入方法 - 根据查询条件设置是否禁用
    /// </summary>
    protected void WriteSetDisable(
        Stream stream,
        TTemplateMetadata metadata)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 根据【提供查询功能的数据源】设置【是否禁用】
    /// </summary>
    /// <param name="query">提供查询功能的数据源</param>
    /// <param name="disable">是否禁用，当值为 <see langword="true"/> 时禁用，为 <see langword="false"/> 时启用</param>
    /// <returns>受影响的行数</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    {0}Task<int> SetDisableAsync(IQueryable<{1}> query, bool disable)
"""u8;
        stream.WriteFormat(utf8String,
            IsInterface ? null : "public async "u8.ToArray(),
            metadata.ClassName);
        if (IsInterface)
        {
            stream.Write(
"""
;


"""u8);
        }
        else
        {
            stream.Write(
"""

    {
        var r = await query.ExecuteUpdateAsync(x => x.SetProperty(y => y.Disable, y => disable));
        return r;
    }


"""u8);
        }
    }

    /// <summary>
    /// 写入方法 - 根据主键设置设置是否禁用
    /// </summary>
    protected void WriteSetDisableById(
        Stream stream,
        TTemplateMetadata metadata,
        ImmutableArray<PropertyMetadata> fields)
    {
        ReadOnlySpan<byte> utf8String;

        var idField = fields.FirstOrDefault(x => x.FixedProperty == FixedProperty.Id);
        utf8String =
"""
    /// <summary>
    /// 根据【主键】设置设置【是否禁用】
    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="disable">是否禁用，当值为 <see langword="true"/> 时禁用，为 <see langword="false"/> 时启用</param>
    /// <returns>受影响的行数</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    {0}Task<int> SetDisableByIdAsync({1} id, bool disable)
"""u8;
        stream.WriteFormat(utf8String,
            IsInterface ? null : "public async "u8.ToArray(),
            idField.PropertyType);
        if (IsInterface)
        {
            stream.Write(
"""
;


"""u8);
        }
        else
        {
            stream.Write(
"""

    {
        var r = await Entity.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.Disable, y => disable));
        return r;
    }


"""u8);
        }
    }

    /// <summary>
    /// 写入方法 - 获取用于选择框/下拉列表(Select)的数据
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="metadata"></param>
    /// <param name="fields"></param>
    protected void WriteGetSelect(
        Stream stream,
        TTemplateMetadata metadata,
        ImmutableArray<PropertyMetadata> fields,
        string titlePropertyName = "Title")
    {
        ReadOnlySpan<byte> utf8String;

        var idField = fields.FirstOrDefault(x => x.FixedProperty == FixedProperty.Id);
        utf8String =
"""
    /// <summary>
    /// 获取用于【选择框/下拉列表(Select)】的数据
    /// </summary>
    /// <param name="takeCount">要获取的最大数量限制，默认值：<see cref="SelectItemDTO.Count"/></param>
    /// <returns>用于【选择框/下拉列表(Select)】的数据</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    {0}SelectItemDTO<{1}> GetSelectAsync(int takeCount = SelectItemDTO.Count)
"""u8;
        stream.WriteFormat(utf8String,
            IsInterface ? null : "public async "u8.ToArray(),
            idField.PropertyType);
        if (IsInterface)
        {
            stream.Write(
"""
;


"""u8);
        }
        else
        {
            stream.Write(
"""

    {
        var query = Entity.AsNoTrackingWithIdentityResolution();
        var r = await query
"""u8);
            stream.WriteFormat(
"""

            .Select(x => new SelectItemDTO<{0}>
"""u8, idField.PropertyType);
            stream.Write(
"""

                Id = x.Id,
                Title = x.
"""u8);
            stream.Write(Encoding.UTF8.GetBytes(titlePropertyName));
            stream.Write(
"""
,
            })

"""u8);
            // 添加可选的排序支持 .OrderBy?OrderByDescending
            stream.Write(
"""
            .Take(takeCount).ToArrayAsync();
        return r;
    }


"""u8);
        }
    }

    protected void WriteGetEditById(
        Stream stream,
        TTemplateMetadata metadata,
        ImmutableArray<PropertyMetadata> fields)
    {
        throw new NotImplementedException("TODO");

        ReadOnlySpan<byte> utf8String;

        var idField = fields.FirstOrDefault(x => x.FixedProperty == FixedProperty.Id);
        utf8String =
"""
    /// <summary>
    /// 根据主键获取【编辑模型】
    /// </summary>
    /// <param name="id">主键</param>
    /// <returns>编辑模型</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    {0}SelectItemDTO<{1}> GetSelectAsync(int takeCount = SelectItemDTO.Count)
"""u8;
        stream.WriteFormat(utf8String,
            IsInterface ? null : "public async "u8.ToArray(),
            idField.PropertyType);
        if (IsInterface)
        {
            stream.Write(
"""
;


"""u8);
        }
        else
        {
            stream.Write(
"""

    {
        var query = Entity.AsNoTrackingWithIdentityResolution();
        var r = await query
"""u8);
            stream.WriteFormat(
"""

            .Select(x => new SelectItemDTO<{0}>
"""u8, idField.PropertyType);
            stream.Write(
"""

                Id = x.Id,
                Title = x.
"""u8);
            //stream.Write(Encoding.UTF8.GetBytes(titlePropertyName));
            stream.Write(
"""
,
            })

"""u8);
            // 添加可选的排序支持 .OrderBy?OrderByDescending
            stream.Write(
"""
            .Take(takeCount).ToArrayAsync();
        return r;
    }


"""u8);
        }
    }

    protected void WriteByFields(Stream stream, TTemplateMetadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        foreach (var field in fields)
        {
            switch (field.FixedProperty)
            {
                case FixedProperty.Disable:
                    WriteSetDisable(stream, metadata);
                    WriteSetDisableById(stream, metadata, fields);
                    break;
                case FixedProperty.Title:
                    WriteGetSelect(stream, metadata, fields);
                    break;
            }
        }
    }
}
