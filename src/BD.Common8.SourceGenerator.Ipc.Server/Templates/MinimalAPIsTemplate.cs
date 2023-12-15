namespace BD.Common8.SourceGenerator.Ipc.Templates;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 用于服务端的 Minimal APIs 源文件模板
/// </summary>
[Generator]
public sealed class MinimalAPIsTemplate :
    GeneratedAttributeTemplateBase<
        ServiceContractImplAttribute,
        MinimalAPIsTemplate.SourceModel>
{
    protected override string Id =>
        "ServiceContract";

    protected override string AttrName =>
        "BD.Common8.Ipc.Attributes.ServiceContractAttribute";

    protected override string FileId => "MinimalAPIs";

    protected override ServiceContractImplAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        return null!;
    }

    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    public readonly record struct SourceModel : ISourceModel
    {
        /// <inheritdoc cref="INamedTypeSymbol"/>
        public required INamedTypeSymbol NamedTypeSymbol { get; init; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public required string Namespace { get; init; }

        /// <summary>
        /// 类型名
        /// </summary>
        public required string TypeName { get; init; }

        /// <summary>
        /// 方法，函数
        /// </summary>
        public required ImmutableArray<IMethodSymbol> Methods { get; init; }

        /// <inheritdoc cref="ServiceContractImplAttribute"/>
        public required ServiceContractImplAttribute Attribute { get; init; }
    }

    protected override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        var methods = args.symbol.GetMembers().OfType<IMethodSymbol>().ToImmutableArray();

        SourceModel model = new()
        {
            NamedTypeSymbol = args.symbol,
            Namespace = args.@namespace,
            TypeName = args.typeName,
            Attribute = args.attr,
            Methods = methods,
        };
        return model;
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
        if (m.Methods.Length == 0) return;

        WriteFileHeader(stream);
        stream.WriteNewLine();
        WriteNamespace(stream, m.Namespace);
        stream.WriteNewLine();
        stream.WriteFormat(
"""
partial interface {0} : IEndpointRouteMapGroup
"""u8, m.TypeName);
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();
        stream.Write(
"""
    /// <inheritdoc cref="IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder endpoints)
"""u8);
        stream.WriteNewLine();
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();
        stream.WriteFormat(
"""
        var builder = endpoints.MapGroup("/{0}");
"""u8, m.TypeName);
        stream.WriteNewLine();

        foreach (var method in m.Methods)
        {
            string typeString;
            IParameterSymbol parameter;
            var category = method.GetMethodParametersCategory();
            switch (category)
            {
                case MethodParametersCategory.None:
                    stream.WriteFormat(
"""
        builder.MapPost("/{0}", ([FromServices] {1} s) => s.{0}());
"""u8, method.Name, m.TypeName);
                    stream.WriteNewLine();
                    break;
                case MethodParametersCategory.SimpleTypes:
                    stream.WriteFormat(
"""
        builder.MapPost("/{0}", ([FromServices] {1} s
"""u8, method.Name, m.TypeName);
                    for (int i = 0; i < method.Parameters.Length; i++)
                    {
                        parameter = method.Parameters[i];
                        typeString = category.GetParameterTypeString(parameter);
                        stream.WriteFormat(
"""
, [FromRoute] {0} {1}
"""u8, typeString, parameter.Name);
                    }
                    stream.WriteFormat(
"""
) => s.{0}(
"""u8, method.Name);
                    var isFirstParameter = true;
                    foreach (var parameters in method.Parameters)
                    {
                        if (isFirstParameter)
                        {
                            stream.WriteUtf16StrToUtf8OrCustom(parameters.Name);
                            isFirstParameter = false;
                        }
                        else
                        {
                            stream.WriteFormat(
"""
, {0}
"""u8, parameters.Name);
                        }
                    }
                    stream.Write(
"""
));
"""u8);
                    stream.WriteNewLine();
                    break;
                case MethodParametersCategory.FromBody:
                    parameter = method.Parameters[0];
                    typeString = category.GetParameterTypeString(parameter);
                    stream.WriteFormat(
"""
        builder.MapPost("/{0}", ([FromServices] {1} s, [FromBody] {2} {3}) => s.{0}({3}));
"""u8, method.Name, m.TypeName, typeString, parameter.Name);
                    stream.WriteNewLine();
                    break;
                default:
                    continue;
            }
        }

        stream.Write(
"""
    
"""u8);
        stream.WriteCurlyBracketRight();
        stream.WriteNewLine();
        stream.WriteCurlyBracketRight();
    }
}
