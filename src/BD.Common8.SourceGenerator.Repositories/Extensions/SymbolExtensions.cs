namespace BD.Common8.SourceGenerator.Repositories.Extensions;

static class SymbolExtensions
{
    /// <summary>
    /// 从 <see cref="INamespaceOrTypeSymbol"/> 中获取所有字段
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ImmutableArray<IFieldSymbol> GetFields(this INamespaceOrTypeSymbol symbol)
    {
        var fields = symbol.GetMembers().
            OfType<IFieldSymbol>().
            Where(x => !x.Name.EndsWith("k__BackingField")).
            ToImmutableArray();
        return fields;
    }

    /// <summary>
    /// 获取源码文件的文件名
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="partialFileName"></param>
    /// <returns></returns>
    internal static string GetSourceFileName(this ISymbol symbol, string partialFileName)
    {
        using var hashAlgorithm = SHA384.Create();
        var sourceFileName = $"{symbol.Name}.{partialFileName}.{string.Join(null, hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(symbol.ToDisplayString())).Select(x => x.ToString("x2")).ToArray())}.g.cs";
        return sourceFileName;
    }

    /// <summary>
    /// 添加源生成的源码文件
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="className"></param>
    /// <param name="partialFileName"></param>
    /// <param name="stream"></param>
    public static void AddSource(this SourceProductionContext ctx,
        string className,
        string partialFileName,
        Stream stream)
    {
        ctx.AddSource($"{className}.{partialFileName}.g.cs", SourceText.From(stream, Encoding.UTF8, canBeEmbedded: true));
    }
}
