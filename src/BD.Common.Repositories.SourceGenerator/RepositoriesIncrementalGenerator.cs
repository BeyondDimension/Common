namespace BD.Common.Repositories.SourceGenerator;

/// <summary>
/// 配置式后台管理系统 CURD 增量源码生成器
/// </summary>
[Generator]
public sealed class RepositoriesIncrementalGenerator : IIncrementalGenerator
{
    const string namespaceSuffix = "Entities.Design";

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

            try
            {
                var fields = symbol.GetFields();
                var properties = PropertyMetadata.Parse(fields);

                var filePath = symbol.Locations.FirstOrDefault()?.SourceTree?.FilePath;
                var projPath = ProjPathHelper.GetProjPath(Path.GetDirectoryName(filePath));

                var className = symbol.Name;
                className = GeneratorConfig.Translate(className);
                var tableClassName = EntityTemplate.GetTableName(symbol);
                var @namespace = symbol.ContainingNamespace.ToDisplayString().Replace(namespaceSuffix, "{0}");

                var attrs = symbol.GetAttributes();
                var generateRepositories = attrs.GetGenerateRepositoriesAttribute();
                if (generateRepositories != null)
                {
                    if (generateRepositories.Entity)
                    {
                        EntityTemplate.Instance.AddSource(ctx, symbol,
                            new(@namespace, symbol.Name, tableClassName, className),
                            properties);
                        if (generateRepositories.BackManageAddModel ||
                            generateRepositories.BackManageEditModel ||
                            generateRepositories.BackManageTableModel)
                        {
                            BackManageModelTemplate.Instance.AddSource(ctx, symbol,
                                new(@namespace, symbol.Name, className,
                                    generateRepositories.BackManageAddModel,
                                    generateRepositories.BackManageEditModel,
                                    generateRepositories.BackManageTableModel),
                                properties);
                            if (generateRepositories.Repository)
                            {
                                RepositoryTemplate.Instance.AddSource(ctx, symbol,
                                    new(@namespace, symbol.Name, className),
                                    properties);
                                RepositoryImplTemplate.Instance.AddSource(ctx, symbol,
                                    new(@namespace, symbol.Name, className),
                                    properties);
                                if (generateRepositories.ApiController)
                                {

                                }
                            }
                        }
                    }
                    GeneratorConfig.Save();
                }
            }
            catch (Exception ex)
            {
                var hintName = symbol.GetSourceFileName(nameof(Exception));
                byte counter = 0;
                StringBuilder builder = new();
                do
                {
                    if (counter++ == sbyte.MaxValue) break;
                    builder.AppendLine(ex.ToString());
                    builder.AppendLine();
                    ex = ex.InnerException;
                } while (ex != null);
                ctx.AddSource(hintName, SourceText.From(builder.ToString(), Encoding.UTF8));
            }
        });
    }
}
