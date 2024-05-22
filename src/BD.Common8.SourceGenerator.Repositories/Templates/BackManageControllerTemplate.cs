namespace BD.Common8.SourceGenerator.Repositories.Templates;

/// <summary>
/// 后台管理控制器源码生成模板
/// </summary>
sealed class BackManageControllerTemplate : TemplateBase<BackManageControllerTemplate, BackManageControllerTemplate.Metadata>
{
    public readonly record struct Metadata(
        string Namespace,
        string? Summary,
        string ClassName,
        string? PrimaryKeyTypeName = null,
        GenerateRepositoriesAttribute GenerateRepositoriesAttribute = null!) : ITemplateMetadata
    {
        /// <inheritdoc cref="GenerateRepositoriesAttribute.ApiControllerConstructorArguments"/>
        public string[]? ConstructorArguments => GenerateRepositoriesAttribute.ApiControllerConstructorArguments;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.ApiRoutePrefix"/>
        public string? ApiRoutePrefix => GenerateRepositoriesAttribute.ApiRoutePrefix;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.ApiRouteIgnoreRedundantEntityPrefix"/>
        public bool ApiRouteIgnoreRedundantEntityPrefix => GenerateRepositoriesAttribute.ApiRouteIgnoreRedundantEntityPrefix;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanAdd"/>
        public bool BackManageCanAdd => GenerateRepositoriesAttribute.BackManageCanAdd;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanDelete"/>
        public bool BackManageCanDelete => GenerateRepositoriesAttribute.BackManageCanDelete;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanEdit"/>
        public bool BackManageCanEdit => GenerateRepositoriesAttribute.BackManageCanEdit;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditModelReadOnly"/>
        public bool BackManageEditModelReadOnly => GenerateRepositoriesAttribute.BackManageEditModelReadOnly;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageCanTable"/>
        public bool BackManageCanTable => GenerateRepositoriesAttribute.BackManageCanTable;
    }

    void WriteConstructor(
        Stream stream,
        Metadata metadata,
        string classNamePluralize)
    {
        int i;
        ReadOnlySpan<byte> utf8String;

        if (metadata.ConstructorArguments == null)
            return;

        var repositoryInterfaceType = $"I{metadata.ClassName}Repository";
        Dictionary<string, string> arguments = new() // Dictionary<argName, typeName>
        {
            { GetArgumentName(repositoryInterfaceType), repositoryInterfaceType },
        };
        foreach (var constructorArgument in metadata.ConstructorArguments)
        {
            var constructorArgumentName = GetArgumentName(constructorArgument);
            if (!arguments.ContainsKey(constructorArgumentName))
            {
                arguments.Add(constructorArgumentName, constructorArgument);
            }
        }

        i = 0;
        foreach (var argument in arguments)
        {
            utf8String =
"""
    readonly {0} {1};
"""u8;
            stream.WriteFormat(utf8String, argument.Value, argument.Key);
            stream.WriteNewLine();
            if (i == arguments.Count - 1)
                stream.WriteNewLine();
            i++;
        }

        utf8String =
"""
    public {0}Controller(
        ILogger<{0}Controller> logger
"""u8;
        stream.WriteFormat(utf8String, metadata.ClassName);

        // args
        foreach (var argument in arguments)
        {
            utf8String =
"""
,
        {0} {1}
"""u8;
            stream.WriteFormat(utf8String, argument.Value, argument.Key);
        }

        utf8String =
"""
) : base(logger
"""u8;
        stream.Write(utf8String);

        // base args?

        utf8String =
"""
)
    {
"""u8;
        stream.Write(utf8String);

        // this.xxx = xxx;
        i = 0;
        foreach (var argument in arguments.Keys)
        {
            if (i != 0 && i != arguments.Count - 1)
                stream.WriteNewLine();
            utf8String =
"""

        this.{0} = {0};
"""u8;
            stream.WriteFormat(utf8String, argument);
            i++;
        }

        utf8String =
"""

    }


"""u8;
        stream.Write(utf8String);
    }

    protected override void WriteCore(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        WriteSourceHeader(stream);

        var routePrefix = metadata.ApiRoutePrefix;
        byte[] routePrefixU8;
        if (string.IsNullOrWhiteSpace(routePrefix))
        {
            routePrefixU8 = "bm/"u8.ToArray();
        }
        else
        {
            routePrefix = routePrefix!.TrimStart("/").ToLowerInvariant();
            if (!routePrefix.EndsWith("/"))
            {
                routePrefix = $"{routePrefix}/";
            }
            routePrefixU8 = Encoding.UTF8.GetBytes(routePrefix);
        }

        var format =
"""
namespace {0};

/// <summary>
/// {1} - 后台管理控制器
/// </summary>
[Route("{4}{3}")]
public sealed partial class {2}Controller : BaseAuthorizeController<{2}Controller>
"""u8;
        //, IAuthenticatorDbContext
        args[0] = $"{args[0]!.ToString()}.Controllers.{metadata.GenerateRepositoriesAttribute.ModuleName}";
        var classNamePluralize = metadata.ClassName.Pluralize();
        var routeNamePluralize = classNamePluralize.ToLowerInvariant();
        // 去除冗余的路由前缀
        var routeEnd = Encoding.UTF8.GetString(routePrefixU8).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
        if (metadata.ApiRouteIgnoreRedundantEntityPrefix &&
            !metadata.ClassName.Equals(routeEnd, StringComparison.OrdinalIgnoreCase) && // 实体名和路由结尾一样的话，不能忽略
            metadata.ClassName.Titleize().Split(' ').First().Equals(routeEnd, StringComparison.OrdinalIgnoreCase)) // 实体名第一个单词和路由结尾一致
        {
            routeNamePluralize = routeNamePluralize.TrimStart(routeEnd, StringComparison.OrdinalIgnoreCase);
        }
        var routeNamePluralizeLowerU8 = Encoding.UTF8.GetBytes(routeNamePluralize);
        stream.WriteFormat(format, new object?[]
        {
            args[0],
            args[1],
            metadata.ClassName,
            routeNamePluralizeLowerU8,
            routePrefixU8,
        });

        stream.Write(
"""

{

"""u8);
        stream.WriteFormat(
"""
    const string ControllerName = "{0}";

"""u8, new object?[]
{
    metadata.ClassName,
});

        WriteConstructor(stream, metadata, classNamePluralize);

        #region 生成方法 Methods

        var idField = fields.FirstOrDefault(x => x.FixedProperty == FixedProperty.Id);
        var repositoryInterfaceType = $"I{metadata.ClassName}Repository";
        var repositoryInterfaceTypeArgName = GetArgumentName(repositoryInterfaceType);
        var repositoryInterfaceTypeArgNameU8 = Encoding.UTF8.GetBytes(repositoryInterfaceTypeArgName);

        foreach (var field in fields)
        {
            switch (field.FixedProperty)
            {
                case FixedProperty.Disable:
                    WriteSetDisable(stream, routePrefixU8, routeNamePluralizeLowerU8, idField, repositoryInterfaceTypeArgNameU8);
                    break;
                case FixedProperty.Title:
                    WriteGetSelect(stream, routePrefixU8, routeNamePluralizeLowerU8, idField, repositoryInterfaceTypeArgNameU8);
                    break;
                case FixedProperty.SoftDeleted:
                    break;
            }
        }

        #endregion

        if (metadata.BackManageCanEdit)
        {
            WriteEditById(stream, metadata, routePrefixU8, routeNamePluralizeLowerU8, idField, repositoryInterfaceTypeArgNameU8);
            if (!metadata.BackManageEditModelReadOnly)
            {
                WriteUpdate(stream, metadata, idField, routePrefixU8, routeNamePluralizeLowerU8, repositoryInterfaceTypeArgNameU8);
            }
        }
        if (metadata.BackManageCanAdd)
        {
            WriteInsert(stream, metadata, routePrefixU8, routeNamePluralizeLowerU8, repositoryInterfaceTypeArgNameU8);
        }
        if (metadata.BackManageCanTable)
        {
            WriteQuery(stream, metadata, fields, routePrefixU8, routeNamePluralizeLowerU8, repositoryInterfaceTypeArgNameU8);
        }
        if (metadata.BackManageCanDelete)
        {
            WriteDelete(stream, routePrefixU8, routeNamePluralizeLowerU8, idField, repositoryInterfaceTypeArgNameU8);
        }
        stream.Write(
"""
}

"""u8);
    }

    static void WriteApiUrlSummary(
        Stream stream,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower,
        ReadOnlySpan<byte> route)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""

    /// <para>Local: 
"""u8;
        stream.Write(utf8String);
        stream.Write(GeneratorConfig.GetApiBaseUrlBackManageLocal());
        stream.Write("/"u8);
        stream.Write(routePrefixU8);
        stream.Write(routeNamePluralizeLower);
        stream.Write(route);
        utf8String =
"""
</para>
"""u8;
        stream.Write(utf8String);

        utf8String =
"""

    /// <para>Development: 
"""u8;
        stream.Write(utf8String);
        stream.Write(GeneratorConfig.GetApiBaseUrlBackManageDevelopment());
        stream.Write("/"u8);
        stream.Write(routePrefixU8);
        stream.Write(routeNamePluralizeLower);
        stream.Write(route);
        utf8String =
"""
</para>
"""u8;
        stream.Write(utf8String);
    }

    /// <summary>
    /// 写入方法 - 获取用于选择框/下拉列表(Select)的数据
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="routePrefixU8"></param>
    /// <param name="routeNamePluralizeLower"></param>
    /// <param name="idField"></param>
    /// <param name="repositoryInterfaceTypeArgName"></param>
    void WriteGetSelect(
        Stream stream,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower,
        PropertyMetadata idField,
        byte[] repositoryInterfaceTypeArgName)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 获取用于【选择框/下拉列表(Select)】的数据
"""u8;
        stream.Write(utf8String);

        WriteApiUrlSummary(stream, routePrefixU8, routeNamePluralizeLower, "/select"u8);

        utf8String =
"""

    /// </summary>
    /// <returns>用于【选择框/下拉列表(Select)】的数据</returns>
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    [HttpGet("select"), PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ApiResponse<SelectItemDTO<{0}>[]>> Select()
"""u8;
        stream.WriteFormat(utf8String,
            idField.PropertyType);

        stream.Write(
"""

    {
"""u8);
        stream.WriteFormat(
"""

        var r = await {0}.GetSelectAsync();
"""u8, repositoryInterfaceTypeArgName);
        stream.Write(
"""

        return r;
    }


"""u8);
    }

    /// <summary>
    /// 写入方法 - 根据查询条件设置是否禁用
    /// </summary>
    void WriteSetDisable(
        Stream stream,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower,
        PropertyMetadata idField,
        byte[] repositoryInterfaceTypeArgName)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 根据【主键】设置设置【是否禁用】
"""u8;
        stream.Write(utf8String);

        WriteApiUrlSummary(stream, routePrefixU8, routeNamePluralizeLower, "/disable/{id}/{disable}"u8);

        utf8String =
"""

    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="disable">是否禁用，当值为 <see langword="true"/> 时禁用，为 <see langword="false"/> 时启用</param>
    /// <returns>受影响的行数</returns>
    [HttpPut("disable/{id}/{disable}"), PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    public async Task<ApiResponse<int>> Disable([FromRoute] {0} id, [FromRoute] bool disable)
"""u8;
        stream.WriteFormat(utf8String,
            idField.PropertyType);

        stream.Write(
"""

    {
"""u8);
        stream.WriteFormat(
"""

        if (!TryGetUserId(out Guid userId))
            throw new ArgumentNullException(nameof(userId));
        var r = await {0}.SetDisableByIdAsync(userId, id, disable);
"""u8, repositoryInterfaceTypeArgName);
        stream.Write(
"""

        return new ApiResponse<int>()
        {
            IsSuccess = r > 0,
            Data = r,
        };
    }


"""u8);
    }

    /// <summary>
    /// 写入方法 - 根据主键删除/软删除
    /// </summary>
    void WriteDelete(
        Stream stream,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower,
        PropertyMetadata idField,
        byte[] repositoryInterfaceTypeArgName)
    {
        ReadOnlySpan<byte> utf8String;
        utf8String =
"""
    /// <summary>
    /// 根据【主键】删除
"""u8;
        stream.Write(utf8String);

        WriteApiUrlSummary(stream, routePrefixU8, routeNamePluralizeLower, "/{id}"u8);

        utf8String =
"""

    /// </summary>
    /// <param name="id">主键</param>
    /// <returns>受影响的行数</returns>
    [HttpDelete("{id}"), PermissionFilter(ControllerName + nameof(SysButtonType.Delete))]
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    public async Task<ApiResponse<int>> Delete([FromRoute] {0} id)
"""u8;
        stream.WriteFormat(utf8String,
            idField.PropertyType);

        stream.Write(
"""

    {
"""u8);
        stream.WriteFormat(
"""

        var r = await {0}.DeleteAsync(id);
"""u8, repositoryInterfaceTypeArgName);
        stream.Write(
"""

        return new ApiResponse<int>()
        {
            IsSuccess = r > 0,
            Data = r,
        };
    }


"""u8);
    }

    /// <summary>
    /// 写入方法 - 根据主键获取编辑模型
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="metadata"></param>
    /// <param name="routePrefixU8"></param>
    /// <param name="routeNamePluralizeLower"></param>
    /// <param name="idField"></param>
    /// <param name="repositoryInterfaceTypeArgName"></param>
    void WriteEditById(
        Stream stream,
        Metadata metadata,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower,
        PropertyMetadata idField,
        byte[] repositoryInterfaceTypeArgName)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 根据【主键】获取【编辑模型】
"""u8;
        stream.Write(utf8String);

        WriteApiUrlSummary(stream, routePrefixU8, routeNamePluralizeLower, "/{id}"u8);

        utf8String =
"""

    /// </summary>
    /// <param name="id">主键</param>
    /// <returns>编辑模型</returns>
    [HttpGet("{id}"), PermissionFilter(ControllerName + nameof(SysButtonType.Detail))]
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    public async Task<ApiResponse<Edit{1}Model?>> EditById([FromRoute] {0} id)
"""u8;
        stream.WriteFormat(utf8String,
            idField.PropertyType,
            metadata.ClassName);

        stream.Write(
"""

    {
"""u8);
        stream.WriteFormat(
"""

        var r = await {0}.GetEditByIdAsync(id);
"""u8, repositoryInterfaceTypeArgName);
        stream.Write(
"""

        return r;
    }


"""u8);
    }

    /// <summary>
    /// 写入方法 - 根据添加模型新增一条数据
    /// </summary>
    void WriteInsert(
        Stream stream,
        Metadata metadata,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower,
        byte[] repositoryInterfaceTypeArgName)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 根据【添加模型】新增一条数据
"""u8;
        stream.Write(utf8String);

        WriteApiUrlSummary(stream, routePrefixU8, routeNamePluralizeLower, ""u8);

        utf8String =
"""

    /// </summary>
    /// <param name="model">添加模型</param>
    /// <returns>受影响的行数</returns>
    [HttpPost, PermissionFilter(ControllerName + nameof(SysButtonType.Add))]
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    public async Task<ApiResponse<int>> Insert([FromBody] Add{0}Model model)
"""u8;
        stream.WriteFormat(utf8String,
            metadata.ClassName);

        stream.Write(
"""

    {
"""u8);
        stream.WriteFormat(
"""
        if (!TryGetUserId(out Guid userId))
            throw new ArgumentNullException(nameof(userId));
        var r = await {0}.InsertAsync(userId, model);
"""u8, repositoryInterfaceTypeArgName);
        stream.Write(
"""

        return r;
    }


"""u8);
    }

    /// <summary>
    /// 写入方法 - 根据编辑模型更新一条数据
    /// </summary>
    void WriteUpdate(
        Stream stream,
        Metadata metadata,
        PropertyMetadata idField,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower,
        byte[] repositoryInterfaceTypeArgName)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 根据【编辑模型】更新一条数据
"""u8;
        stream.Write(utf8String);

        WriteApiUrlSummary(stream, routePrefixU8, routeNamePluralizeLower, "/{id}"u8);

        utf8String =
"""

    /// </summary>
    /// <param name="id">主键</param>
    /// <param name="model">编辑模型</param>
    /// <returns>受影响的行数</returns>
    [HttpPut("{id}"), PermissionFilter(ControllerName + nameof(SysButtonType.Edit))]
"""u8;
        stream.Write(utf8String);
        utf8String =
"""

    public async Task<ApiResponse<int>> Update([FromRoute] {1} id, [FromBody] Edit{0}Model model)
"""u8;
        stream.WriteFormat(utf8String,
            metadata.ClassName,
            idField.PropertyType);

        stream.Write(
"""

    {
"""u8);
        stream.WriteFormat(
"""

        if (!TryGetUserId(out Guid userId))
            throw new ArgumentNullException(nameof(userId));
        var r = await {0}.UpdateAsync(userId, id, model);
"""u8, repositoryInterfaceTypeArgName);
        stream.Write(
"""

        return r;
    }


"""u8);
    }

    /// <summary>
    /// 写入方法 - 后台表格查询
    /// </summary>
    void WriteQuery(
        Stream stream,
        Metadata metadata,
        ImmutableArray<PropertyMetadata> fields,
        byte[] routePrefixU8,
        byte[] routeNamePluralizeLower,
        byte[] repositoryInterfaceTypeArgName)
    {
        ReadOnlySpan<byte> utf8String;

        utf8String =
"""
    /// <summary>
    /// 分页查询{0}表格
"""u8;
        stream.WriteFormat(utf8String, metadata.Summary);
        WriteApiUrlSummary(stream, routePrefixU8, routeNamePluralizeLower, ""u8);
        stream.Write(
"""

    /// </summary>
"""u8);

        fields = RepositoryTemplate.GetTableQueryFields(fields);
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
    [HttpGet, PermissionFilter(ControllerName + nameof(SysButtonType.Query))]
    public async Task<ApiResponse<PagedModel<Table{1}Model>>> QueryAsync(

"""u8;
        stream.WriteFormat(utf8String,
            $"{metadata.Summary}分页表格查询结果数据",
            metadata.ClassName);
        List<byte[]> camelizeNames = new();
        foreach (var field in fields)
        {
            if (field.BackManageField == null || !field.BackManageField.Query)
                continue;

            utf8String =
"""
        [FromQuery] {0} {1} = null,

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
            var camelizeNameU8 = Encoding.UTF8.GetBytes(camelizeName);
            stream.WriteFormat(utf8String,
                propertyType,
                camelizeNameU8);
            camelizeNames.Add(camelizeNameU8);
        }
        stream.Write(
"""
        [FromQuery] int current = IPagedModel.DefaultCurrent,
        [FromQuery] int pageSize = IPagedModel.DefaultPageSize)
"""u8);
        stream.Write(
"""

    {

"""u8);
        stream.WriteFormat(
"""
         var r = await {0}.QueryAsync(
"""u8, repositoryInterfaceTypeArgName);
        foreach (var camelizeName in camelizeNames)
        {
            stream.Write(camelizeName);
            stream.Write(", "u8);
        }
        stream.WriteFormat(
"""
current, pageSize);
         return r;
"""u8);
        stream.Write(
"""

    }


"""u8);
    }
}
