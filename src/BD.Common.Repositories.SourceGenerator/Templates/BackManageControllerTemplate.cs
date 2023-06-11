namespace BD.Common.Repositories.SourceGenerator.Templates;

/// <summary>
/// 后台管理控制器源码生成模板
/// </summary>
sealed class BackManageControllerTemplate : TemplateBase<BackManageControllerTemplate, BackManageControllerTemplate.Metadata>
{
    public readonly record struct Metadata(
        string Namespace,
        string Summary,
        string ClassName,
        string? PrimaryKeyTypeName = null,
        string[]? ConstructorArguments = null,
        string? ApiRoutePrefix = null) : ITemplateMetadata
    {

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
        stream.WriteFormat(utf8String, classNamePluralize);

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
            routePrefix = routePrefix!.TrimStart("/");
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
        args[0] = string.Format(args[0]!.ToString(), "Controllers");
        var classNamePluralize = metadata.ClassName.Pluralize();
        var classNamePluralizeLower = classNamePluralize.ToLowerInvariant();
        var classNamePluralizeLowerU8 = Encoding.UTF8.GetBytes(classNamePluralizeLower);
        stream.WriteFormat(format, new object?[]
        {
            args[0],
            args[1],
            classNamePluralize,
            classNamePluralizeLowerU8,
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
    classNamePluralize,
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
                    WriteSetDisable(stream, routePrefixU8, classNamePluralizeLowerU8, idField, repositoryInterfaceTypeArgNameU8);
                    break;
                case FixedProperty.Title:
                    WriteGetSelect(stream, routePrefixU8, classNamePluralizeLowerU8, idField, repositoryInterfaceTypeArgNameU8);
                    break;
            }
        }

        #endregion

        stream.Write(
"""
}

"""u8);
    }

    static void WriteApiUrlSummary(
        Stream stream,
        byte[] routePrefixU8,
        byte[] classNamePluralizeLower,
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
        stream.Write(classNamePluralizeLower);
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
        stream.Write(classNamePluralizeLower);
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
    /// <param name="metadata"></param>
    /// <param name="fields"></param>
    void WriteGetSelect(
        Stream stream,
        byte[] routePrefixU8,
        byte[] classNamePluralizeLower,
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

        WriteApiUrlSummary(stream, routePrefixU8, classNamePluralizeLower, "/select"u8);

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
        byte[] classNamePluralizeLower,
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

        WriteApiUrlSummary(stream, routePrefixU8, classNamePluralizeLower, "/disable/{id}/{disable}"u8);

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

        var r = await {0}.SetDisableAsync(id, disable);
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
}
