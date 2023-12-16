namespace BD.Common8.SourceGenerator.Ipc.Templates;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 用于 Ipc 服务端调用的源文件模板
/// </summary>
[Generator]
public sealed class IpcServerTemplate : IpcTemplateBase
{
    protected override string FileId => "IpcServer";

    /// <inheritdoc/>
    protected override ServiceContractImplAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attr = base.GetAttribute(attributes);
        switch (attr.GeneratorType)
        {
            case IpcGeneratorType.Server:
                break;
            default:
                IgnoreExecute = true; // 非服务端生成类型直接跳过
                return null!;
        }
        return attr;
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
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
