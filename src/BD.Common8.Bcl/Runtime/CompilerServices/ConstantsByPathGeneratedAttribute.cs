namespace System.Runtime.CompilerServices;

/// <summary>
/// 用于标注需要根据路径生成常量的源生成
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ConstantsByPathGeneratedAttribute(string relativePath, string? valuePrefix = null, string? namePrefix = null) : Attribute
{
    /// <summary>
    /// 相对路径，路径分隔符使用 Windows 分隔符，即 '/'
    /// </summary>
    public string RelativePath { get; } = relativePath;

    /// <summary>
    /// 常量值前缀
    /// </summary>
    public string? ValuePrefix { get; } = valuePrefix;

    /// <summary>
    /// 键名前缀
    /// </summary>
    public string? NamePrefix { get; } = namePrefix;
}
