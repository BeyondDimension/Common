namespace System.CodeDom.Compiler;

/// <summary>
/// 用于标注 BD.Common8.SourceGenerator.ResX 源生成器的 <see cref="Attribute"/>，替代 Resx 嵌入二进制资源实现零反射
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class BinaryResourceAttribute(string args, string? appendTemplate = null) : Attribute
{
    public string Arguments { get; init; } = args;

    /// <summary>
    /// 追加 C# 代码模板，并启用随机变量名
    /// </summary>
    public string? AppendTemplate { get; init; } = appendTemplate;
}
