namespace BD.Common8.SourceGenerator.Repositories.Templates.Abstractions;

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
        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanAdd"/>
        bool BackManageCanAdd { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageAddMethodImplType"/>
        RepositoryMethodImplType BackManageAddMethodImplType { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanEdit"/>
        bool BackManageCanEdit { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditModelReadOnly"/>
        bool BackManageEditModelReadOnly { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditMethodImplType"/>
        RepositoryMethodImplType BackManageEditMethodImplType { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanTable"/>
        bool BackManageCanTable { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageTableMethodImplType"/>
        RepositoryMethodImplType BackManageTableMethodImplType { get; }

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanDelete"/>
        bool BackManageCanDelete { get; }
    }

    // Functions
    // QueryAsync 后台表格查询，或其他查询返回集合或数组的
    // InsertAsync 插入一行数据
    // UpdateAsync 更新一行数据
    // GetEditByIdAsync 根据主键获取编辑的 DTO
    // SetDisableByIdAsync 根据主键设置是否禁用
    // GetSelectAsync 获取用于选择框的 DTO 数据，类型使用 SelectItemDTO<T> 泛型

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryTemplateBase{TTemplate, TTemplateMetadata}"/> class.
    /// </summary>
    public RepositoryTemplateBase()
    {
        IsInterface = typeof(TTemplate) == typeof(RepositoryTemplate);
    }

    protected virtual bool IsInterface { get; }

    internal static ImmutableArray<PropertyMetadata> GetTableQueryFields(ImmutableArray<PropertyMetadata> fields)
    {
        fields = fields.
            Where(x => (x.FixedProperty == FixedProperty.Id) || (x.BackManageField != null &&
                (x.BackManageField.Query || x.BackManageField.Detail) &&
                (x.FixedProperty != FixedProperty.SoftDeleted))).
            ToImmutableArray();
        return fields;
    }

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
        var repositoryMethodImplType = metadata.BackManageTableMethodImplType;
        var isCustomImpl = repositoryMethodImplType == RepositoryMethodImplType.Custom;
        const ClassType classType = ClassType.BackManageTableModels;
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 分页查询{0}表格
    /// </summary>
"""u8;
        stream.WriteFormat(utf8String, metadata.Summary);

        fields = GetTableQueryFields(fields);
        foreach (var field in fields)
        {
            if (field.BackManageField == null || !field.BackManageField.Query)
                continue;

            var camelizeName = field.CamelizeName;
            switch (camelizeName)
            {
                case "createUserId":
                    camelizeName = "createUser";
                    break;
                case "operatorUserId":
                    camelizeName = "operatorUser";
                    break;
            }
            field.WriteParam(stream, camelizeName, field.HumanizeName);
        }
        stream.Write(
"""

    /// <param name="current">当前页码，页码从 1 开始，默认值：<see cref="IPagedModel.DefaultCurrent"/></param>
    /// <param name="pageSize">页大小，如果为 0 必定返回空集合，默认值：<see cref="IPagedModel.DefaultPageSize"/></param>
"""u8);
        utf8String =
"""

    /// <returns>{0}</returns>
    {1}Task<PagedModel<Table{2}Model>> QueryAsync(

"""u8;
        stream.WriteFormat(utf8String,
            $"{metadata.Summary}分页表格查询结果数据",
            IsInterface ? null : (isCustomImpl ? "public partial "u8.ToArray() : "public async "u8.ToArray()),
            metadata.ClassName);
        foreach (var field in fields)
        {
            if (field.BackManageField == null || !field.BackManageField.Query)
                continue;

            utf8String =
"""
        {0} {1},

"""u8;
            string propertyType = null!, camelizeName = field.CamelizeName;
            switch (camelizeName)
            {
                case "createUserId":
                    propertyType = "string?";
                    camelizeName = "createUser";
                    break;
                case "operatorUserId":
                    propertyType = "string?";
                    camelizeName = "operatorUser";
                    break;
                case "creationTime":
                case "updateTime":
                    propertyType = "DateTimeOffset[]?";
                    break;
                default:
                    break;
            }
            propertyType ??= field.PropertyType.EndsWith("?") ?
                    field.PropertyType :
                    $"{field.PropertyType}?";
            stream.WriteFormat(utf8String,
                propertyType,
                camelizeName);
        }
        stream.Write(
"""
        int current = IPagedModel.DefaultCurrent,
        int pageSize = IPagedModel.DefaultPageSize)
"""u8);
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
        var query = Entity.AsNoTrackingWithIdentityResolution()
"""u8);

            #region Include

            var include = metadata.GenerateRepositoriesAttribute.BackManageTableQueryInclude?.TrimStart(".");
            if (!string.IsNullOrWhiteSpace(include))
            {
                stream.WriteFormat(
    """

            .{0}
"""u8, include);
            }

            #endregion

            #region OrderBy

            WriteOrderBy(metadata.GenerateRepositoriesAttribute.DefaultOrderBy, stream, fields);

            #endregion

            stream.Write(
"""

            .AsQueryable();
"""u8);

            #region where if else

            foreach (var field in fields)
            {
                if (field.BackManageField == null || !field.BackManageField.Query)
                    continue;
                string camelizeName = field.CamelizeName;
                switch (camelizeName)
                {
                    case "createUserId":
                        stream.WriteFormat(
"""

        if (!string.IsNullOrEmpty(createUser))
            if (ShortGuid.TryParse(createUser, out Guid createUserId))
                query = query.Where(x => x.CreateUser!.Id == createUserId);
            else
                query = query.Where(x => x.CreateUser!.NickName!.Contains(createUser));
"""u8);
                        continue;
                    case "operatorUserId":
                        stream.WriteFormat(
"""

        if (!string.IsNullOrEmpty(operatorUser))
            if (ShortGuid.TryParse(operatorUser, out Guid operatorUserId))
                query = query.Where(x => x.OperatorUser!.Id == operatorUserId);
            else
                query = query.Where(x => x.OperatorUser!.NickName!.Contains(operatorUser));
"""u8, field.CamelizeName);
                        continue;
                    case "creationTime":
                    case "updateTime":
                        stream.WriteFormat(
"""

        if ({0} != null)
            query = {0}.Length switch

"""u8, field.CamelizeName);
                        stream.Write(
"""
            {

"""u8
                            );
                        stream.WriteFormat(
"""
                1 => query.Where(x => x.{1} >= {0}[0]),
                2 => query.Where(x => x.{1} >= {0}[0] && x.{1} <= {0}[1]),
                _ => query,
"""u8, field.CamelizeName, field.TranslateName);
                        stream.Write(
"""

            };
"""u8
                            );
                        continue;
                }

                switch (field.PropertyType)
                {
                    case "string":
                    case "string?":
                        switch (field.BackManageField?.QueryStringWhereType ?? StringWhereType.Contains)
                        {
                            case StringWhereType.Contains:
                                stream.WriteFormat(
"""

        if (!string.IsNullOrEmpty({0}))
            query = query.Where(x => x.{1}.Contains({0}));
"""u8, field.CamelizeName, field.TranslateName);
                                break;
                            case StringWhereType.Equals:
                                stream.WriteFormat(
"""

        if (!string.IsNullOrEmpty({0}))
            query = query.Where(x => x.{1} == {0});
"""u8, field.CamelizeName, field.TranslateName);
                                break;
                        }
                        break;
                    default:
                        if (!new string[] { "string", "object", "[]" }.Any(x => field.Field.Name!.Contains(x)))
                        {
                            stream.WriteFormat(
"""

        if ({0}.HasValue)
            query = query.Where(x => x.{1} == {0}.Value);
"""u8, field.CamelizeName, field.TranslateName);
                        }
                        else
                        {
                            stream.WriteFormat(
"""

        if ({0} != null)
            query = query.Where(x => x.{1} == {0});
"""u8, field.CamelizeName, field.TranslateName);
                        }
                        break;
                }
            }

            #endregion

            switch (repositoryMethodImplType)
            {
                case RepositoryMethodImplType.Expression:
                    stream.WriteFormat(
"""

        var r = await query
            .Select(x => new Table{0}Model()
"""u8, metadata.ClassName);
                    stream.Write(
"""

            {

"""u8);
                    #region Properties

                    fields = BackManageModelTemplate.Filter(fields, classType);
                    for (var i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];
                        WriteSetPropertyValue(stream, field);
                    }

                    #endregion

                    stream.Write(
"""
            })

"""u8);
                    stream.Write(
"""
            .PagingAsync(current, pageSize, RequestAborted);
        return r;
"""u8);
                    break;
                case RepositoryMethodImplType.ProjectTo:
                    stream.WriteFormat(
"""

        var r = await query
            .ProjectTo<Table{0}Model>(mapper.ConfigurationProvider)
            .PagingAsync(current, pageSize, RequestAborted);
        return r;
"""u8, metadata.ClassName);
                    break;
            }

            stream.Write(
"""

    }


"""u8);
        }
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
    /// <param name="operatorUserId">最后一次操作的人（记录后台管理员禁用或启用或编辑该条的操作）</param>
    /// <param name="id">主键</param>
    /// <param name="disable">是否禁用，当值为 <see langword="true"/> 时禁用，为 <see langword="false"/> 时启用</param>
    /// <returns>受影响的行数</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    {0}Task<int> SetDisableByIdAsync(Guid? operatorUserId, {1} id, bool disable)
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
            .ExecuteUpdateAsync(x => x
                .SetProperty(y => y.Disable, y => disable)
                .SetProperty(y => y.UpdateTime, y => DateTimeOffset.Now)
                .SetProperty(y => y.OperatorUserId, y => operatorUserId)
            );
        return r;
    }


"""u8);
        }
    }

    void WriteOrderBy(
        string? orderBy,
        Stream stream,
        ImmutableArray<PropertyMetadata> fields)
    {
        orderBy = orderBy?.TrimStart(".");
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            stream.WriteFormat(
"""

            .{0}
"""u8, orderBy);
        }
        else if (fields.Any(x => x.FixedProperty == FixedProperty.CreationTime))
        {
            stream.Write(
"""

            .OrderByDescending(x => x.CreationTime).ThenBy(x => x.Id)
"""u8);
        }
        else
        {
            stream.Write(
"""

            .OrderBy(x => x.Id)
"""u8);
        }
    }

    /// <summary>
    /// 写入方法 - 获取用于选择框/下拉列表(Select)的数据
    /// </summary>
    protected void WriteGetSelect(
        Stream stream,
        TTemplateMetadata metadata,
        ImmutableArray<PropertyMetadata> fields,
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

    {0}Task<SelectItemDTO<{1}>[]> GetSelectAsync(int takeCount = SelectItemDTO.Count)
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
            WriteOrderBy(metadata.GenerateRepositoriesAttribute.DefaultOrderBy, stream, fields);

            stream.WriteFormat(
"""

            .Select(x => new SelectItemDTO<{0}>
"""u8, idField.PropertyType);
            stream.Write(
"""

            {
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

    void WriteSetPropertyValue(
        Stream stream,
        PropertyMetadata field)
    {
        switch (field.FixedProperty)
        {
            case FixedProperty.CreateUserId:
                stream.WriteFormat(
"""
                CreateUserId = x.CreateUser!.Id,
                CreateUser = x.CreateUser!.NickName,

"""u8);
                break;
            case FixedProperty.OperatorUserId:
                stream.WriteFormat(
"""
                OperatorUserId = x.OperatorUser!.Id,
                OperatorUser = x.OperatorUser!.NickName,

"""u8);
                break;
            default:
                stream.WriteFormat(
"""
                {0} = x.{0},

"""u8, field.TranslateName);
                break;
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

    {0}Task<Edit{1}Model?> GetEditByIdAsync({2} id)
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
            .Select(static x => new Edit{0}Model
"""u8, metadata.ClassName);
                    stream.Write(
"""

            {

"""u8);
                    #region Properties

                    fields = BackManageModelTemplate.Filter(fields, classType);
                    for (var i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];
                        WriteSetPropertyValue(stream, field);
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
            .ProjectTo<Edit{0}Model>(mapper.ConfigurationProvider)

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
    /// <param name="createUserId">创建人</param>
    /// <param name="model">添加模型</param>
    /// <returns>受影响的行数</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    {0}Task<int> InsertAsync(Guid? createUserId, Add{1}Model model)
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

"""u8);
            switch (repositoryMethodImplType)
            {
                case RepositoryMethodImplType.Expression:
                    stream.WriteFormat(
        """
        {0} entity = new()
"""u8, metadata.ClassName);
                    stream.Write(
"""

        {

"""u8);

                    #region Properties

                    fields = BackManageModelTemplate.Filter(fields, classType);
                    for (var i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];
                        switch (field.FixedProperty)
                        {
                            case FixedProperty.Id:
                            case FixedProperty.CreationTime:
                            case FixedProperty.UpdateTime:
                            case FixedProperty.CreateUserId:
                            case FixedProperty.OperatorUserId:
                                continue; // 一些字段不可添加
                        }
                        stream.WriteFormat(
"""
            {0} = model.{0},

"""u8, field.TranslateName);
                    }
                    stream.Write(
"""
            CreateUserId = createUserId,

"""u8);
                    #endregion

                    stream.Write(
"""
        };
"""u8);

                    stream.Write(
        """

        await Entity.AddAsync(entity);
        var r = await db.SaveChangesAsync();
        return r;
"""u8);
                    break;
            }
            stream.Write(
"""

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
    /// <param name="operatorUserId">最后一次操作的人（记录后台管理员禁用或启用或编辑该条的操作）</param>
    /// <param name="id">主键</param>
    /// <param name="model">编辑模型</param>
    /// <returns>受影响的行数</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    {0}Task<int> UpdateAsync(Guid? operatorUserId, {2} id, Edit{1}Model model)
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
            switch (repositoryMethodImplType)
            {
                case RepositoryMethodImplType.Expression:

                    stream.Write(
"""

    {
        var query = Entity.AsNoTrackingWithIdentityResolution();
        var r = await query.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x

"""u8);
                    #region Properties

                    for (int i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];
                        switch (field.FixedProperty)
                        {
                            case FixedProperty.UpdateTime:
                                stream.Write(
"""
                .SetProperty(y => y.UpdateTime, y => DateTimeOffset.Now)
                .SetProperty(y => y.OperatorUserId, y => operatorUserId)

"""u8);
                                break;
                            case FixedProperty.OperatorUserId:
                                break;
                        }
                    }
                    fields = BackManageModelTemplate.Filter(fields, classType, isOnlyEdit: true);
                    for (var i = 0; i < fields.Length; i++)
                    {
                        var field = fields[i];
                        switch (field.FixedProperty)
                        {
                            case FixedProperty.Id:
                            case FixedProperty.CreationTime:
                            case FixedProperty.CreateUserId:
                            case FixedProperty.UpdateTime:
                            case FixedProperty.OperatorUserId:
                                continue; // 一些字段不可编辑
                        }
                        stream.WriteFormat(
"""
                .SetProperty(y => y.{0}, y => model.{0})

"""u8, field.TranslateName);
                    }

                    #endregion

                    stream.Write(
"""
            );

"""u8);

                    break;
            }
            stream.Write(
"""
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
        if (metadata.BackManageCanTable)
        {
            WriteMethodQuery(stream, metadata, fields);
        }
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
                    WriteGetSelect(stream, metadata, fields, idField);
                    break;
            }
        }

        if (metadata.BackManageCanEdit)
        {
            WriteGetEditById(stream, metadata, fields, idField);
            if (!metadata.BackManageEditModelReadOnly)
            {
                WriteUpdate(stream, metadata, fields, idField);
            }
        }
        if (metadata.BackManageCanAdd)
        {
            WriteInsert(stream, metadata, fields, idField);
        }
    }
}
