using BD.Common8.SourceGenerator.Bcl.Templates;

namespace BD.Common8.SourceGenerator.Bcl;

/// <summary>
/// BD.Common8.Bcl 增量源生成器
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
        InitializeByAttribute(ctx,
            SingletonPartitionTemplate.AttrName,
            SingletonPartitionTemplate.Execute);

        InitializeByAttribute(ctx,
            ViewModelWrapperTemplate.AttrName,
            ViewModelWrapperTemplate.Execute);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void InitializeByAttribute(IncrementalGeneratorInitializationContext ctx,
        string attributeName,
        Action<SourceProductionContext, GeneratorAttributeSyntaxContext> action)
    {
        var source = ctx.SyntaxProvider.ForAttributeWithMetadataName(
            attributeName,
            static (_, _) => true,
            static (content, _) => content);
        ctx.RegisterSourceOutput(source, action);
    }
}
