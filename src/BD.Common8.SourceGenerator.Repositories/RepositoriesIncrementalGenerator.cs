//using ReactUI = BD.Common8.SourceGenerator.Repositories.Templates.ReactUI;

namespace BD.Common8.SourceGenerator.Repositories;

/// <summary>
/// 配置式后台管理系统 CURD 增量源码生成器
/// </summary>
[Generator]
public sealed class RepositoriesIncrementalGenerator : IIncrementalGenerator
{
    static string? GetAssemblyName(SourceProductionContext spc)
    {
        try
        {
            {
                var fieldCompilation = typeof(SourceProductionContext).GetField("Compilation", BindingFlags.NonPublic | BindingFlags.Instance)
                     ?? typeof(SourceProductionContext).GetField("Compilation", BindingFlags.Public | BindingFlags.Instance);
                if (fieldCompilation != null && fieldCompilation.GetValue(spc) is Compilation compilation)
                {
                    return compilation.AssemblyName;
                }
            }
            {
                var propertyCompilation = typeof(SourceProductionContext).GetProperty("Compilation", BindingFlags.NonPublic | BindingFlags.Instance)
                     ?? typeof(SourceProductionContext).GetProperty("Compilation", BindingFlags.Public | BindingFlags.Instance);
                if (propertyCompilation != null && propertyCompilation.GetValue(spc) is Compilation compilation)
                {
                    return compilation.AssemblyName;
                }
            }
        }
        catch
        {
        }
        return null;
    }

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var jsonDatas = context.AdditionalTextsProvider
                                .Where(text => text.Path.EndsWith(".json"))
                                .Select((text, token) => text);

        context.RegisterSourceOutput(jsonDatas, (sourceProductionContext, additional) =>
        {
            try
            {
                var metadata = JsonConvert.DeserializeObject<EntityDesignMetadata>(additional.GetText()?.ToString()!);
                var projPath = ProjPathHelper.GetProjPath(Path.GetDirectoryName(additional.Path));
                var cfg = GeneratorConfig.Instance;
                if (metadata == null)
                    return;
                var properties = PropertyMetadata.Parse(metadata.Properties!);
                var className = metadata.Name!;
                className = GeneratorConfig.Translate(className);
                var summary = metadata.Summary;

                var @namespace = cfg.Namespace;

                var generateRepositories = metadata.Attribute;
                if (generateRepositories != null)
                {
                    //tasks.Add(InBackground(() =>
                    //{
                    //    JsonEntityDesignMetadataTemplate.Instance.AddSource(sourceProductionContext, symbol,
                    //        new(@namespace, symbol.Name, className,
                    //        GenerateRepositoriesAttribute: generateRepositories),
                    //        properties);
                    //}));

                    if (generateRepositories.Entity && IsGenerator("Entity"))
                    {
                        EntityTemplate.Instance.AddSource(sourceProductionContext, additional,
                               new(@namespace, summary, metadata.Name!, className,
                               generateRepositories),
                               properties);
                    }
                    if ((generateRepositories.BackManageAddModel ||
                        generateRepositories.BackManageEditModel ||
                        generateRepositories.BackManageTableModel) &&
                        IsGenerator("BackManageModel"))
                    {
                        BackManageModelTemplate.Instance.AddSource(sourceProductionContext, additional,
                            new(@namespace, metadata.Name!, className,
                            GenerateRepositoriesAttribute: generateRepositories),
                            properties);
                    }

                    if (generateRepositories.Repository && IsGenerator("Repository"))
                    {
                        RepositoryTemplate.Instance.AddSource(sourceProductionContext, additional,
                            new(@namespace, metadata.Name!, className,
                            GenerateRepositoriesAttribute: generateRepositories),
                            properties);

                        RepositoryImplTemplate.Instance.AddSource(sourceProductionContext, additional,
                            new(@namespace, metadata.Name!, className,
                            GenerateRepositoriesAttribute: generateRepositories),
                            properties);
                    }
                    if (generateRepositories.ApiController && IsGenerator("BackManageController"))
                    {
                        BackManageControllerTemplate.Instance.AddSource(sourceProductionContext, additional,
                            new(@namespace, metadata.Name!, className,
                            GenerateRepositoriesAttribute: generateRepositories),
                            properties);
                    }

                    if (generateRepositories.BackManageUIPage)
                    {
                        //ReactUI.BackManageUIPageTemplate.Instance.AddSource(sourceProductionContext, additional,
                        //    new(@namespace, summary, className,
                        //    GenerateRepositoriesAttribute: generateRepositories),
                        //    properties);

                        //ReactUI.BackManageUIPageApiTemplate.Instance.AddSource(sourceProductionContext, additional,
                        //    new(@namespace, summary, className,
                        //    GenerateRepositoriesAttribute: generateRepositories),
                        //    properties);

                        //ReactUI.BackManageUIPageIndexTemplate.Instance.AddSource(sourceProductionContext, additional,
                        //    new(@namespace, summary, className,
                        //    GenerateRepositoriesAttribute: generateRepositories),
                        //    properties);

                        //ReactUI.BackManageUIPageTypingsTemplate.Instance.AddSource(sourceProductionContext, additional,
                        //    new(@namespace, summary, className,
                        //    GenerateRepositoriesAttribute: generateRepositories),
                        //    properties);
                    }
                    GeneratorConfig.Save();

                    bool IsGenerator(string template)
                    {
                        var assemblyName = GetAssemblyName(sourceProductionContext);
                        return cfg.SourcePath.TryGetValue(template, out var value)
                                && value != null
                                && value.Length >= 2
                                && (assemblyName == value[1] || additional.Path.ToLowerInvariant().Contains(value[1].ToLowerInvariant()));
                    }
                }
            }
            catch (Exception? ex)
            {
                var hintName = $"{Path.GetFileName(additional.Path)}_GeneratorException";
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

public static class PackageReferenceTest
{
    public static void Assemblies()
    {
        Console.WriteLine(typeof(JsonSerializer));
        Console.WriteLine(typeof(EnglishArticle));
        Console.WriteLine("Sentence casing".Transform(To.LowerCase));
    }
}