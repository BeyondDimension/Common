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

        context.RegisterSourceOutput(attrs, async (ctx, content) =>
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
                    List<Task> tasks = new(); // 不同类型的模板使用多线程并行化执行
                    if (generateRepositories.Entity)
                    {
                        tasks.Add(InBackground(() =>
                        {
                            EntityTemplate.Instance.AddSource(ctx, symbol,
                                new(@namespace, symbol.Name, tableClassName, className,
                                generateRepositories),
                                properties);
                        }));
                        if (generateRepositories.BackManageAddModel ||
                            generateRepositories.BackManageEditModel ||
                            generateRepositories.BackManageTableModel)
                        {
                            tasks.Add(InBackground(() =>
                            {
                                BackManageModelTemplate.Instance.AddSource(ctx, symbol,
                                    new(@namespace, symbol.Name, className,
                                    GenerateRepositoriesAttribute: generateRepositories),
                                    properties);
                            }));
                            if (generateRepositories.Repository)
                            {
                                tasks.Add(InBackground(() =>
                                {
                                    RepositoryTemplate.Instance.AddSource(ctx, symbol,
                                        new(@namespace, symbol.Name, className,
                                        GenerateRepositoriesAttribute: generateRepositories),
                                        properties);
                                }));
                                tasks.Add(InBackground(() =>
                                {
                                    RepositoryImplTemplate.Instance.AddSource(ctx, symbol,
                                        new(@namespace, symbol.Name, className,
                                        GenerateRepositoriesAttribute: generateRepositories),
                                        properties);
                                }));
                                if (generateRepositories.ApiController)
                                {
                                    tasks.Add(InBackground(() =>
                                    {
                                        BackManageControllerTemplate.Instance.AddSource(ctx, symbol,
                                            new(@namespace, symbol.Name, className,
                                            GenerateRepositoriesAttribute: generateRepositories),
                                            properties);
                                    }));
                                }
                            }
                        }
                    }
                    await Task.WhenAll(tasks);
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
                var errorString = builder.ToString();
                Debug.WriteLine(errorString);
                errorString = string.Join("\r\n", errorString.Split(new string[] { "\r\n" }, StringSplitOptions.None).Select(x => $"// {x}"));
                ctx.AddSource(hintName, SourceText.From(errorString, Encoding.UTF8));
            }
        });
    }
}
