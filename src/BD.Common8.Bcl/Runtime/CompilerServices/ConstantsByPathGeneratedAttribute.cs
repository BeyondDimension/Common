namespace System.Runtime.CompilerServices;

/// <summary>
/// 用于标注需要根据路径生成常量的源生成
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ConstantsByPathGeneratedAttribute(string relativePath, string? prefix) : Attribute
{
    /// <summary>
    /// 相对路径
    /// </summary>
    public string RelativePath { get; } = relativePath;

    /// <summary>
    /// 常量值前缀
    /// </summary>
    public string? Prefix { get; } = prefix;
}
