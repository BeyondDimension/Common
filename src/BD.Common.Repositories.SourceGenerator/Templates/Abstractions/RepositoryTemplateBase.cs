namespace BD.Common.Repositories.SourceGenerator.Templates.Abstractions;

/// <summary>
/// 仓储层源码生成模板基类
/// </summary>
/// <typeparam name="TTemplate"></typeparam>
/// <typeparam name="TTemplateMetadata"></typeparam>
public abstract class RepositoryTemplateBase<TTemplate, TTemplateMetadata> : TemplateBase<TTemplate, TTemplateMetadata>
    where TTemplate : RepositoryTemplateBase<TTemplate, TTemplateMetadata>, new()
    where TTemplateMetadata : RepositoryTemplateBase<TTemplate, TTemplateMetadata>.IRepositoryTemplateMetadata
{
    public interface IRepositoryTemplateMetadata : ITemplateMetadata
    {
        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageAddModel"/>
        bool BackManageAddModel { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageAddMethodImplType"/>
        RepositoryMethodImplType BackManageAddMethodImplType { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditModel"/>
        bool BackManageEditModel { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditModelReadOnly"/>
        bool BackManageEditModelReadOnly { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditMethodImplType"/>
        RepositoryMethodImplType BackManageEditMethodImplType { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageTableModel"/>
        bool BackManageTableModel { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageTableMethodImplType"/>
        RepositoryMethodImplType BackManageTableMethodImplType { get; }
    }

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
        PropertyMetadata idField)
    {
        ReadOnlySpan<byte> utf8String;

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
    protected void WriteGetSelect(
        Stream stream,
        PropertyMetadata idField,
        string titlePropertyName = "Title")
    {
        ReadOnlySpan<byte> utf8String;

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

    {0}Task<SelectItemDTO<{1}>> GetSelectAsync(int takeCount = SelectItemDTO.Count)
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
        ImmutableArray<PropertyMetadata> fields,
        PropertyMetadata idField)
    {
        const ClassType classType = ClassType.BackManageEditModels;
        var repositoryMethodImplType = metadata.BackManageEditMethodImplType;
        ReadOnlySpan<byte> utf8String;

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

    {0}Task<Edit{1}DTO?> GetEditByIdAsync({2} id)
"""u8;
        var isCustomImpl = repositoryMethodImplType == RepositoryMethodImplType.Custom;
        stream.WriteFormat(utf8String,
            IsInterface ? null : (isCustomImpl ? "public partial "u8.ToArray() : "public async "u8.ToArray()),
            metadata.ClassName,
            idField.PropertyType);
        if (IsInterface || isCustomImpl)
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
        var r = await query.Where(x => x.Id == id)

"""u8);
            switch (repositoryMethodImplType)
            {
                case RepositoryMethodImplType.Expression:
                    stream.WriteFormat(
"""
            .Select(static x => new Edit{0}DTO
"""u8, metadata.ClassName);
                    stream.Write(
"""

            {

"""u8);
                    #region EditDTO Properties

                    fields = BackManageModelTemplate.Filter(fields, classType);
                    for (var i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];
                        // TODO：创建人，操作人等关系查询的需要特殊适配
                        stream.WriteFormat(
"""
                {0} = x.{0},

"""u8, field.Name);
                    }

                    #endregion

                    stream.Write(
"""
            })

"""u8);
                    break;
                case RepositoryMethodImplType.ProjectTo:
                    stream.WriteFormat(
"""
            .ProjectTo<Edit{0}DTO>(mapper.ConfigurationProvider)

"""u8, metadata.ClassName);
                    break;
            }
            stream.Write(
"""
            .FirstOrDefaultAsync(RequestAborted);
        return r;
    }


"""u8);
        }
    }

    protected void WriteInsert(
        Stream stream,
        TTemplateMetadata metadata,
        ImmutableArray<PropertyMetadata> fields,
        PropertyMetadata idField)
    {
        const ClassType classType = ClassType.BackManageAddModels;
        var repositoryMethodImplType = metadata.BackManageAddMethodImplType;
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 根据【添加模型】新增一条数据
    /// </summary>
    /// <param name="model">添加模型</param>
    /// <returns>受影响的行数</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    {0}Task<int> InsertAsync(Add{1}DTO model)
"""u8;
        var isCustomImpl = repositoryMethodImplType == RepositoryMethodImplType.Custom;
        stream.WriteFormat(utf8String,
            IsInterface ? null : (isCustomImpl ? "public partial "u8.ToArray() : "public async "u8.ToArray()),
            metadata.ClassName,
            idField.PropertyType);
        if (IsInterface || isCustomImpl)
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
        var r = await query.Where(x => x.Id == id)

"""u8);
            switch (repositoryMethodImplType)
            {
                case RepositoryMethodImplType.Expression:
                    stream.WriteFormat(
"""
            .Select(static x => new Edit{0}DTO
"""u8, metadata.ClassName);
                    stream.Write(
"""

            {

"""u8);
                    #region EditDTO Properties

                    fields = BackManageModelTemplate.Filter(fields, classType);
                    for (var i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];
                        // TODO：创建人，操作人等关系查询的需要特殊适配
                        stream.WriteFormat(
"""
                {0} = x.{0},

"""u8, field.Name);
                    }

                    #endregion

                    stream.Write(
"""
            })

"""u8);
                    break;
                case RepositoryMethodImplType.ProjectTo:
                    stream.WriteFormat(
"""
            .ProjectTo<Edit{0}DTO>(mapper.ConfigurationProvider)

"""u8, metadata.ClassName);
                    break;
            }
            stream.Write(
"""
            .FirstOrDefaultAsync(RequestAborted);
        return r;
    }


"""u8);
        }
    }

    protected void WriteUpdate(
        Stream stream,
        TTemplateMetadata metadata,
        ImmutableArray<PropertyMetadata> fields,
        PropertyMetadata idField)
    {
        const ClassType classType = ClassType.BackManageEditModels;
        var repositoryMethodImplType = metadata.BackManageAddMethodImplType;
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 根据【编辑模型】更新一条数据
    /// </summary>
    /// <param name="model">编辑模型</param>
    /// <returns>受影响的行数</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    {0}Task<int> UpdateAsync(Edit{1}DTO model)
"""u8;
        var isCustomImpl = repositoryMethodImplType == RepositoryMethodImplType.Custom;
        stream.WriteFormat(utf8String,
            IsInterface ? null : (isCustomImpl ? "public partial "u8.ToArray() : "public async "u8.ToArray()),
            metadata.ClassName,
            idField.PropertyType);
        if (IsInterface || isCustomImpl)
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
        var r = await query.Where(x => x.Id == id)

"""u8);
            switch (repositoryMethodImplType)
            {
                case RepositoryMethodImplType.Expression:
                    stream.WriteFormat(
"""
            .Select(static x => new Edit{0}DTO
"""u8, metadata.ClassName);
                    stream.Write(
"""

            {

"""u8);
                    #region EditDTO Properties

                    fields = BackManageModelTemplate.Filter(fields, classType, isOnlyEdit: true);
                    for (var i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];
                        // TODO：创建人，操作人等关系查询的需要特殊适配
                        stream.WriteFormat(
"""
                {0} = x.{0},

"""u8, field.Name);
                    }

                    #endregion

                    stream.Write(
"""
            })

"""u8);
                    break;
                case RepositoryMethodImplType.ProjectTo:
                    stream.WriteFormat(
"""
            .ProjectTo<Edit{0}DTO>(mapper.ConfigurationProvider)

"""u8, metadata.ClassName);
                    break;
            }
            stream.Write(
"""
            .FirstOrDefaultAsync(RequestAborted);
        return r;
    }


"""u8);
        }
    }

    /// <summary>
    /// 生成方法 Methods
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="metadata"></param>
    /// <param name="fields"></param>
    protected void WriteMethods(Stream stream, TTemplateMetadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        WriteMethodQuery(stream, metadata, fields);

        var idField = fields.FirstOrDefault(x => x.FixedProperty == FixedProperty.Id);
        foreach (var field in fields)
        {
            switch (field.FixedProperty)
            {
                case FixedProperty.Disable:
                    WriteSetDisable(stream, metadata);
                    WriteSetDisableById(stream, idField);
                    break;
                case FixedProperty.Title:
                    WriteGetSelect(stream, idField);
                    break;
            }
        }

        if (metadata.BackManageEditModel)
        {
            WriteGetEditById(stream, metadata, fields, idField);
            if (!metadata.BackManageEditModelReadOnly)
            {
                WriteUpdate(stream, metadata, fields, idField);
            }
        }
        if (metadata.BackManageAddModel)
        {
            WriteInsert(stream, metadata, fields, idField);
        }
    }
}
