namespace BD.Common8.Settings5.Infrastructure;

/// <summary>
/// 用于标注设置静态类的源生成
/// </summary>
/// <param name="modelType">模型类型</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SettingsPropertyGeneratedAttribute(Type modelType) : Attribute
{
    /// <summary>
    /// 模型类型
    /// </summary>
    public Type ModelType { get; } = modelType;
}
