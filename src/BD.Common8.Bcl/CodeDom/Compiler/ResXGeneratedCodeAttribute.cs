namespace System.CodeDom.Compiler;

/// <summary>
/// 用于标注 BD.Common8.SourceGenerator.ResX 源生成器的 <see cref="Attribute"/>
/// </summary>
/// <param name="relativeFilePath"></param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ResXGeneratedCodeAttribute(string relativeFilePath) : Attribute
{
    /// <summary>
    /// ResX 文件相对路径，路径分隔符使用 Windows 分隔符，即 '/'
    /// </summary>
    public string RelativeFilePath { get; init; } = relativeFilePath;
}
