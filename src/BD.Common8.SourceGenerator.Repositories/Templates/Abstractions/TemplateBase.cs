namespace BD.Common8.SourceGenerator.Repositories.Templates.Abstractions;

/// <summary>
/// 生成源码模板基类
/// </summary>
/// <typeparam name="TTemplate"></typeparam>
/// <typeparam name="TTemplateMetadata"></typeparam>
public abstract class TemplateBase<TTemplate, TTemplateMetadata>
    where TTemplate : TemplateBase<TTemplate, TTemplateMetadata>, new()
    where TTemplateMetadata : ITemplateMetadata
{
    /// <summary>
    /// 获取当前生成源码模板实例
    /// </summary>
    public static TTemplate Instance { get; } = new TTemplate();

    /// <summary>
    /// 当前模板的名称
    /// </summary>
    public virtual string? Name => null;

    /// <summary>
    /// 使用当前源码模板生成源码写入流
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="metadata"></param>
    /// <param name="fields"></param>
    public void Write(Stream stream, TTemplateMetadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        var args = metadata.GetArgs();
        WriteCore(stream, args, metadata, fields);
    }

    /// <inheritdoc cref="Write(Stream, TTemplateMetadata, ImmutableArray{PropertyMetadata})"/>
    protected abstract void WriteCore(Stream stream, object?[] args, TTemplateMetadata metadata, ImmutableArray<PropertyMetadata> fields);

    static readonly Lazy<byte[]> sourceHeader = new(() =>
    {
        string runtimeVersion, roslynVersion, thisVersion;
        static string GetVersion(Assembly assembly)
        {
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            var array = version.Split(new[] { '+', }, StringSplitOptions.RemoveEmptyEntries);
            return array.Length switch
            {
                1 => version,
                _ => $"{array[0]} ({(array[1].Length > 8 ? array[1][..8] : array[1])})",
            };
        }
        try
        {
            runtimeVersion = GetVersion(typeof(object).Assembly);
        }
        catch
        {
#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
            runtimeVersion = Environment.Version.ToString();
#pragma warning restore RS1035 // 不要使用禁用于分析器的 API
        }
        try
        {
            roslynVersion = GetVersion(typeof(Microsoft.CodeAnalysis.CSharp.LanguageVersion).Assembly);
        }
        catch
        {
            roslynVersion = string.Empty;
        }
        try
        {
            thisVersion = GetVersion(typeof(GeneratorConfig).Assembly);
        }
        catch
        {
            thisVersion = string.Empty;
        }

        const string sourceHeaderFormat =
"""
#nullable enable
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由包 BD.Common8.SourceGenerator.Repositories 源生成。
//     运行时版本：{0}
//     编译器版本：{1}
//     生成器版本：{2}
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
// ReSharper disable once CheckNamespace

""";
        return Encoding.UTF8.GetBytes(string.Format(sourceHeaderFormat, runtimeVersion, roslynVersion, thisVersion));
    });

    /// <summary>
    /// 写入源码头
    /// </summary>
    /// <param name="stream"></param>
    protected virtual void WriteSourceHeader(Stream stream) => stream.Write(sourceHeader.Value);

    /// <summary>
    /// 添加当前源码模板生成的文件
    /// </summary>
    /// <param name="sourceProductionContext"></param>
    /// <param name="additional"></param>
    /// <param name="metadata"></param>
    /// <param name="fields"></param>
    public void AddSource(SourceProductionContext sourceProductionContext, AdditionalText additional, TTemplateMetadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        using var memoryStream = new MemoryStream();
        Write(memoryStream, metadata, fields);
        var partialFileName = (Name ?? typeof(TTemplate).Name).TrimEnd("Template");

#if DEBUG
#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
        try
        {
            var sourceString = Encoding.UTF8.GetString(memoryStream.ToArray());
            Console.WriteLine(sourceString);
        }
        catch
        {
        }
#pragma warning restore RS1035 // 不要使用禁用于分析器的 API

        switch (typeof(TTemplate).Name)
        {
            case nameof(EntityTemplate):
                break;
            case nameof(BackManageModelTemplate):
                break;
            case nameof(RepositoryTemplate):
                break;
            case nameof(RepositoryImplTemplate):
                break;
            case nameof(BackManageControllerTemplate):
                break;
            case nameof(ReactUI.BackManageUIPageApiTemplate):
                break;
            case nameof(ReactUI.BackManageUIPageIndexTemplate):
                break;
            case nameof(ReactUI.BackManageUIPageTemplate):
                break;
            case nameof(ReactUI.BackManageUIPageTypingsTemplate):
                break;
        }
#endif
        var hintName = GetFilePath(partialFileName, metadata.GenerateRepositoriesAttribute.ModuleName, metadata.ClassName);
        if (hintName.EndsWith(".cs"))
        {
            sourceProductionContext.AddSource(hintName, SourceText.From(memoryStream, Encoding.UTF8, canBeEmbedded: true));
        }
        else
        {
            // 可选通过配置将源码生成到文件
            if (GeneratorConfig.Instance.SourcePath?.TryGetValue(partialFileName, out var templatePath) ?? false)
            {
                var rootPath = ProjPathHelper.GetProjPath(null);
                var pathList = new List<string>(templatePath);

                if (GeneratorConfig.Instance.GetModuleLevelConfig(additional)
                    ?.RelativeSourcePath?.TryGetValue(partialFileName, out var specifiedFolder) ?? false)
                {
                    pathList.Add(specifiedFolder);
                }
                pathList.Add(hintName);

#pragma warning disable RS1035 // 不要使用禁用于分析器的 API

                var filePath = Path.Combine(rootPath, ParseAgilePath(Path.Combine(pathList.ToArray())));

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                // 移除文件只读属性
                if (File.Exists(filePath))
                {
                    var attr = File.GetAttributes(filePath);
                    if (attr.HasFlag(FileAttributes.ReadOnly))
                        File.SetAttributes(filePath, attr ^ FileAttributes.ReadOnly);
                }

                // 生成的内容写入文件
                using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
                memoryStream.Position = 0;
                memoryStream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.SetLength(fileStream.Position);

                // 设置文件只读属性
                if (filePath.EndsWith(".g.cs"))
                {
                    var attr = File.GetAttributes(filePath);
                    File.SetAttributes(filePath, attr | FileAttributes.ReadOnly);
                }

#pragma warning restore RS1035 // 不要使用禁用于分析器的 API

                return;
            }
        }
    }
}
