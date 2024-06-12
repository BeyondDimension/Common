namespace System.CodeDom.Compiler;

/// <summary>
/// 用于标注 BD.Common8.SourceGenerator.ResX 源生成器的 <see cref="Attribute"/>，替代 Resx 嵌入二进制资源实现零反射
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class BinaryResourceAttribute(string[] relativeFilePaths) : Attribute
{
    public string[] RelativeFilePaths { get; init; } = relativeFilePaths;
}
