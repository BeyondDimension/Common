namespace BD.Common.Repositories.SourceGenerator.Templates;

/// <summary>
/// 仓储层实现类类型源码生成模板
/// </summary>
sealed class RepositoryImplTemplate : RepositoryTemplateBase<RepositoryImplTemplate, RepositoryImplTemplate.Metadata>
{
    public readonly record struct Metadata(
        string Namespace,
        string Summary,
        string ClassName,
        string? PrimaryKeyTypeName = null,
        GenerateRepositoriesAttribute GenerateRepositoriesAttribute = null!) : IRepositoryTemplateMetadata
    {
        public string[]? ConstructorArguments => GenerateRepositoriesAttribute.RepositoryConstructorArguments;

        public bool BackManageAddModel => GenerateRepositoriesAttribute.BackManageAddModel;

        public RepositoryMethodImplType BackManageAddMethodImplType => GenerateRepositoriesAttribute.BackManageAddMethodImplType;

        public bool BackManageEditModel => GenerateRepositoriesAttribute.BackManageEditModel;

        public bool BackManageEditModelReadOnly => GenerateRepositoriesAttribute.BackManageEditModelReadOnly;

        public RepositoryMethodImplType BackManageEditMethodImplType => GenerateRepositoriesAttribute.BackManageEditMethodImplType;

        public bool BackManageTableModel => GenerateRepositoriesAttribute.BackManageTableModel;

        public RepositoryMethodImplType BackManageTableMethodImplType => GenerateRepositoriesAttribute.BackManageTableMethodImplType;
    }

    void WriteConstructor(
        Stream stream,
        Metadata metadata)
    {
        int i;
        ReadOnlySpan<byte> utf8String;

        if (metadata.ConstructorArguments == null)
            return;

        Dictionary<string, string> arguments = new();
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
    public {0}Repository(
        TDbContext dbContext,
        IRequestAbortedProvider requestAbortedProvider
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
) : base(dbContext, requestAbortedProvider
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
        var format =
"""
namespace {0};

/// <summary>
/// {1} - 仓储层实现类
/// </summary>
public sealed partial class {2}Repository<TDbContext> : Repository<TDbContext, {2}, {3}>, I{2}Repository where TDbContext : DbContext
"""u8;
        // TODO 支持 TDbContext 自定义接口
        //, IAuthenticatorDbContext
        args[0] = string.Format(args[0]!.ToString(), "Repositories");
        args[3] = fields.Single(x => x.FixedProperty == FixedProperty.Id).PropertyType;
        stream.WriteFormat(format, args);

        stream.Write(
"""

{

"""u8);

        WriteConstructor(stream, metadata);
        WriteMethods(stream, metadata, fields);

        stream.Write(
"""
}

"""u8);
    }
}
