namespace BD.Common.Repositories.SourceGenerator.Templates.Abstractions;

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
#if REF_SUB_MODULE
        return """
            #nullable enable
            //------------------------------------------------------------------------------
            // <auto-generated>
            //     此代码由包 BD.Common.Repositories.SourceGenerator 源生成。
            //
            //     对此文件的更改可能会导致不正确的行为，并且如果
            //     重新生成代码，这些更改将会丢失。
            // </auto-generated>
            //------------------------------------------------------------------------------
            // ReSharper disable once CheckNamespace
            
            """u8.ToArray();
#endif

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
//     此代码由包 BD.Common.Repositories.SourceGenerator 源生成。
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
    /// <param name="symbol"></param>
    /// <param name="metadata"></param>
    /// <param name="fields"></param>
    public void AddSource(SourceProductionContext sourceProductionContext, ISymbol symbol, TTemplateMetadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        using var memoryStream = new MemoryStream();
        Write(memoryStream, metadata, fields);
        var partialFileName = (Name ?? typeof(TTemplate).Name).TrimEnd("Template");

#if DEBUG
        var sourceString = Encoding.UTF8.GetString(memoryStream.ToArray());
        Console.WriteLine(sourceString);

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
            case nameof(BackManageUIPageTemplate):
                break;
        }
#endif

        // 可选通过配置将源码生成到文件
        if (GeneratorConfig.Instance.SourcePath?.TryGetValue(partialFileName, out var value) ?? false)
        {
            string? sourceFileDirName = null;

            try
            {
                var sourceFilePath = symbol.Locations.FirstOrDefault()?.SourceTree?.FilePath;
                var sourceFilePathArray = sourceFilePath!.Split(Path.DirectorySeparatorChar);
                sourceFileDirName = sourceFilePathArray[^2];
            }
            catch
            {

            }

            //var hintName = symbol.GetSourceFileName(partialFileName);
            var hintName = $"{symbol.Name}.g.cs";
            var pathList = new List<string>() { ProjPathHelper.GetProjPath(null), };
            pathList.AddRange(value);
            if (!string.IsNullOrWhiteSpace(sourceFileDirName) &&
                !string.Equals("Entities.Design", sourceFileDirName, StringComparison.OrdinalIgnoreCase))
                pathList.Add(sourceFileDirName!);
            pathList.Add("Generated");
            pathList.Add(hintName);
            var filePath = Path.Combine(pathList.ToArray());
            var dirPath = Path.GetDirectoryName(filePath)!;
#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
#pragma warning restore RS1035 // 不要使用禁用于分析器的 API
            using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
            memoryStream.Position = 0;
            memoryStream.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.SetLength(fileStream.Position);
            return;
        }

        sourceProductionContext.AddSource(symbol, partialFileName, memoryStream);
    }
}
