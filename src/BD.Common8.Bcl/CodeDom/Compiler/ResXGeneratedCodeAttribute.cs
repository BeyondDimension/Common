namespace System.CodeDom.Compiler;

/// <summary>
/// 用于标注 BD.Common8.SourceGenerator.ResX 源生成器的 <see cref="Attribute"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class ResXGeneratedCodeAttribute(string relativeFilePath, byte version = 0) : Attribute
{
    /// <summary>
    /// ResX 文件相对路径，路径分隔符使用 Windows 分隔符，即 '/'
    /// </summary>
    public string RelativeFilePath { get; init; } = relativeFilePath;

    /// <summary>
    /// 生成的代码模板版本
    /// </summary>
    public byte Version { get; init; } = version;
}
