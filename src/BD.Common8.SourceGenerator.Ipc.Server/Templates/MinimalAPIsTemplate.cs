namespace BD.Common8.SourceGenerator.Ipc.Server.Templates;

/// <summary>
/// 用于服务端的 Minimal APIs 源文件模板
/// </summary>
sealed class MinimalAPIsTemplate : TemplateBase
{
    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    internal readonly record struct SourceModel
    {
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
    }

    /// <summary>
    /// 写入源文件
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="m"></param>
    public static void WriteFile(Stream stream, SourceModel m)
    {
        try
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Invoke(IEndpointRouteBuilder endpoints)
"""u8);
            stream.WriteNewLine();
            stream.Write(
"""
    
"""u8);
            stream.WriteCurlyBracketLeft();
            stream.WriteNewLine();
            stream.WriteFormat(
"""
        var builder = endpoints.MapGroup("/{0}");
"""u8, m.TypeName);
            stream.WriteNewLine();

            foreach (var method in m.Methods)
            {
                if (method.Parameters.Length == 0) // 无参数函数
                {
                    stream.WriteFormat(
"""
        builder.MapPost("/{0}", ([FromServices] {1} s) => s.{0}());
"""u8, method.Name, m.TypeName);
                    stream.WriteNewLine();
                }
                else if (method.Parameters.All(x => x.Type.IsSimpleTypes())) // 参数全部为简单类型，使用路由
                {
                    stream.WriteFormat(
"""
        builder.MapPost("/{0}", ([FromServices] {1} s
"""u8, method.Name, m.TypeName);
                    foreach (var parameters in method.Parameters)
                    {
                        var typeString = parameters.Type.ToDisplayString();
                        if (typeString.Count(static x => x == '.') == 1)
                        {
                            typeString = typeString.TrimStart("System.");
                        }
                        stream.WriteFormat(
"""
, [FromRoute] {0} {1}
"""u8, typeString, parameters.Name);
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
                }
                else if (method.Parameters.Length == 1)
                {
                    var parameter = method.Parameters[0];
                    var typeString = parameter.Type.ToDisplayString();
                    stream.WriteFormat(
"""
        builder.MapPost("/{0}", ([FromServices] {1} s, [FromBody] {2} {3}) => s.{0}({3}));
"""u8, method.Name, m.TypeName, typeString, parameter.Name);
                    stream.WriteNewLine();
                }
            }

            stream.Write(
"""
    
"""u8);
            stream.WriteCurlyBracketRight();
            stream.WriteNewLine();
            stream.WriteCurlyBracketRight();
        }
        catch (OperationCanceledException)
        {
        }
    }
}
