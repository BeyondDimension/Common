namespace System.Runtime.CompilerServices;

/// <summary>
/// 生成将标注的类型中的属性复制到指定类型的方法
/// </summary>
/// <param name="destType"></param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class CopyPropertiesGeneratedAttribute(Type? destType = null) : Attribute
{
    /// <summary>
    /// 要复制到的目标类型，如果为 <see langword="null"/>，则默认为当前类型
    /// </summary>
    public Type? DestType { get; set; } = destType;

    /// <summary>
    /// 黑名单指定不生成的属性名数组
    /// </summary>
    public string[]? IgnoreProperties { get; set; }

    /// <summary>
    /// 白名单指定仅生成的属性名
    /// </summary>
    public string[]? OnlyProperties { get; set; }

    /// <summary>
    /// 指定原属性名到目标属性名不同时的规则，格式为 Dictionary json
    /// </summary>
    public string? MapProperties { get; set; }

    /// <summary>
    /// 自定义方法名称
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// 是否以表达树形式生成，默认生成扩展函数
    /// </summary>
    public bool IsExpression { get; set; }

    /// <summary>
    /// 指定原属性特定赋值，格式为 Dictionary json
    /// </summary>
    public string? AppointProperties { get; set; }
}
