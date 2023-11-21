namespace BD.Common8.SourceGenerator.Ipc;

/// <summary>
/// Ipc 客户端增量源生成器
/// </summary>
[Generator]
public sealed class IncrementalGenerator : IIncrementalGenerator
{
    /// <summary>
    /// 编译器调用该函数初始化生成器，并通过上下文回调注册生成步骤
    /// </summary>
    /// <param name="ctx"></param>
    public void Initialize(IncrementalGeneratorInitializationContext ctx)
    {
#if DEBUG
        Console.WriteLine("Initialize(IncrementalGeneratorInitializationContext..");
#endif
        InitializeByAttribute(ctx);
    }

    void InitializeByAttribute(IncrementalGeneratorInitializationContext ctx)
    {
        var source = ctx.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(ServiceContractAttribute).FullName!,
            static (_, _) => true,
            static (content, _) => content);
        ctx.RegisterSourceOutput(source, Execute);
    }

    static void Execute(SourceProductionContext spc, GeneratorAttributeSyntaxContext m)
    {
        if (m.TargetSymbol is not INamedTypeSymbol symbol)
            return;

        var @namespace = symbol.ContainingNamespace.ToDisplayString();
        var typeName = symbol.Name;

        var methods = symbol.GetMembers().OfType<IMethodSymbol>().ToImmutableArray();
        if (methods.Length == 0)
            return;

        WebApiClientTemplate.SourceModel model = new()
        {
            Namespace = @namespace,
            TypeName = typeName,
            Methods = methods,
        };
        Execute(spc, model);
    }

    static void Execute(SourceProductionContext spc, WebApiClientTemplate.SourceModel m)
    {
        SourceText sourceText;
        try
        {
            using var memoryStream = new MemoryStream();
            WebApiClientTemplate.WriteFile(memoryStream, m);
            sourceText = SourceText.From(memoryStream, canBeEmbedded: true);
#if DEBUG
            var sourceTextString = sourceText.ToString();
            Console.WriteLine();
            Console.WriteLine(sourceTextString);
#endif
        }
        catch (Exception ex)
        {
            StringBuilder builder = new();
            builder.Append("Namespace: ");
            builder.AppendLine(m.Namespace);
            builder.Append("TypeName: ");
            builder.AppendLine(m.TypeName);
            builder.AppendLine();
            builder.AppendLine(ex.ToString());
            sourceText = builder.ToSourceText();
        }
        spc.AddSource($"{m.Namespace}.{m.TypeName}.{WebApiClientTemplate.Id}.g.cs", sourceText);
    }
}
