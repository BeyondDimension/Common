namespace BD.Common.Repositories.SourceGenerator.Templates.Abstractions;

/// <summary>
/// 生成源码模板
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

    /// <summary>
    /// 添加当前源码模板生成的文件
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="symbol"></param>
    /// <param name="metadata"></param>
    /// <param name="fields"></param>
    public void AddSource(SourceProductionContext ctx, ISymbol symbol, TTemplateMetadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        using var memoryStream = new MemoryStream();
        Write(memoryStream, metadata, fields);
        var partialFileName = Name ?? typeof(TTemplate).Name;

#if DEBUG
        var sourceString = Encoding.UTF8.GetString(memoryStream.ToArray());
        Console.WriteLine(sourceString);
#endif

        ctx.AddSource(symbol, partialFileName, memoryStream);
    }
}
