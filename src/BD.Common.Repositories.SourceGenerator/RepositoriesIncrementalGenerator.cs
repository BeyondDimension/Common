namespace BD.Common.Repositories.SourceGenerator;

/// <summary>
/// 配置式后台管理系统 CURD 增量源码生成器
/// </summary>
[Generator]
public sealed class RepositoriesIncrementalGenerator : IIncrementalGenerator
{
    const string namespaceSuffix = ".Entities.Design";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var attrs = context.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(GenerateRepositoriesAttribute).FullName,
            static (_, _) => true,
            static (content, _) => content);

        context.RegisterSourceOutput(attrs, (ctx, content) =>
        {
            if (content.TargetSymbol is not INamedTypeSymbol symbol)
                return;
            var fields = symbol.GetFields();
            var properties = PropertyMetadata.Parse(fields);

            var filePath = symbol.Locations.FirstOrDefault()?.SourceTree?.FilePath;
            var projPath = ProjPathHelper.GetProjPath(Path.GetDirectoryName(filePath));

            var className = symbol.Name;
            className = GeneratorConfig.Translate(className);
            var tableClassName = EntityTemplate.GetTableName(symbol);
            var @namespace = symbol.ContainingNamespace.ToDisplayString().TrimEnd(namespaceSuffix);

            EntityTemplate.Instance.AddSource(ctx, symbol,
                new(@namespace, symbol.Name, tableClassName, className),
                properties);
        });
    }
}
