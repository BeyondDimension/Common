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
#if REF_SUB_MODULE

        // 阻止在开发过程中源生成器自动执行，
        // 仅在开发人员主动使用调试的方式运行此项目时才执行源生成。
        if (!Debugger.IsAttached)
            return;

#endif

        var attrs = context.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(GenerateRepositoriesAttribute).FullName!,
            static (_, _) => true,
            static (content, _) => content);

        context.RegisterSourceOutput(attrs, async (sourceProductionContext, it) =>
        {
            if (it.TargetSymbol is not INamedTypeSymbol symbol)
                return;

            try
            {
                var filePath = symbol.Locations.FirstOrDefault()?.SourceTree?.FilePath;
                var projPath = ProjPathHelper.GetProjPath(Path.GetDirectoryName(filePath));

                var fields = symbol.GetFields();
                var properties = PropertyMetadata.Parse(fields);

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
                            EntityTemplate.Instance.AddSource(sourceProductionContext, symbol,
                                new(@namespace, symbol.Name, tableClassName, className,
                                generateRepositories),
                                properties);
                        }));
                    }
                    if (generateRepositories.BackManageAddModel ||
                        generateRepositories.BackManageEditModel ||
                        generateRepositories.BackManageTableModel)
                    {
                        tasks.Add(InBackground(() =>
                        {
                            BackManageModelTemplate.Instance.AddSource(sourceProductionContext, symbol,
                                new(@namespace, symbol.Name, className,
                                GenerateRepositoriesAttribute: generateRepositories),
                                properties);
                        }));
                    }
                    if (generateRepositories.Repository)
                    {
                        tasks.Add(InBackground(() =>
                        {
                            RepositoryTemplate.Instance.AddSource(sourceProductionContext, symbol,
                                new(@namespace, symbol.Name, className,
                                GenerateRepositoriesAttribute: generateRepositories),
                                properties);
                        }));
                        tasks.Add(InBackground(() =>
                        {
                            RepositoryImplTemplate.Instance.AddSource(sourceProductionContext, symbol,
                                new(@namespace, symbol.Name, className,
                                GenerateRepositoriesAttribute: generateRepositories),
                                properties);
                        }));
                    }
                    if (generateRepositories.ApiController)
                    {
                        tasks.Add(InBackground(() =>
                        {
                            BackManageControllerTemplate.Instance.AddSource(sourceProductionContext, symbol,
                                new(@namespace, symbol.Name, className,
                                GenerateRepositoriesAttribute: generateRepositories),
                                properties);
                        }));
                    }
                    //if (generateRepositories.BackManageUIPage)
                    //{
                    //    tasks.Add(InBackground(() =>
                    //    {
                    //        BackManageUIPageTemplate.Instance.AddSource(sourceProductionContext, symbol,
                    //            new(@namespace, symbol.Name, className,
                    //            GenerateRepositoriesAttribute: generateRepositories),
                    //            properties);
                    //    }));
                    //}
                    Task.WaitAll(tasks.ToArray());
                    GeneratorConfig.Save();
                }
            }
            catch (Exception? ex)
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
                sourceProductionContext.AddSource(hintName, SourceText.From(errorString, Encoding.UTF8));
            }
        });
    }
}
