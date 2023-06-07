// ReSharper disable once CheckNamespace
namespace Microsoft.CodeAnalysis;

public static class SymbolExtensions
{
    /// <summary>
    /// 从 <see cref="INamespaceOrTypeSymbol"/> 中获取所有字段
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string GetSourceFileName(this ISymbol symbol, string partialFileName)
    {
        var format = SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted);
        var sourceFileName = $"{symbol.ToDisplayString(format)}.{partialFileName}.g.cs";
        return sourceFileName;
    }

    /// <summary>
    /// 添加源生成的源码文件
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="symbol"></param>
    /// <param name="partialFileName"></param>
    /// <param name="stream"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddSource(this SourceProductionContext ctx,
        ISymbol symbol,
        string partialFileName,
        Stream stream)
    {
        var hintName = symbol.GetSourceFileName(partialFileName);
        ctx.AddSource(hintName, SourceText.From(stream, Encoding.UTF8, canBeEmbedded: true));
    }
}
