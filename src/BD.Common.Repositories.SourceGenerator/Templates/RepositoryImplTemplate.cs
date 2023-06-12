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
        if (metadata.GenerateRepositoriesAttribute.RepositoryConstructorArgumentMapper)
        {
            arguments.Add("mapper", "IMapper");
        }
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
        foreach (var argument in arguments.Keys)
        {
            utf8String =
"""

        this.{0} = {0};
"""u8;
            stream.WriteFormat(utf8String, argument);
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
        args[0] = string.Format(args[0]!.ToString(), "Repositories");
        args[3] = fields.Single(x => x.FixedProperty == FixedProperty.Id).PropertyType;
        stream.WriteFormat(format, args);
        if (!string.IsNullOrWhiteSpace(metadata.GenerateRepositoriesAttribute.DbContextBaseInterface))
        {
            // 支持 TDbContext 自定义接口，例如, IAuthenticatorDbContext
            stream.Write(", "u8);
            stream.Write(metadata.GenerateRepositoriesAttribute.DbContextBaseInterface!);
        }

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
