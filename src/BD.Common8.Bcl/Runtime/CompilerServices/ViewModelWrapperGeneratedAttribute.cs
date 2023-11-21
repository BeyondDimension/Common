namespace System.Runtime.CompilerServices;

/// <summary>
/// 用于标注视图模型(ViewModel)包装模型(Model)的源生成
/// </summary>
/// <param name="modelType">模型类型</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ViewModelWrapperGeneratedAttribute(Type modelType) : Attribute
{
    /// <summary>
    /// 模型类型
    /// </summary>
    public Type ModelType { get; } = modelType;

    /// <summary>
    /// 是否生成构造函数，默认值为 <see langword="true"/>
    /// </summary>
    public bool Constructor { get; set; } = true;

    /// <summary>
    /// 是否生成隐式转换，默认值为 <see langword="true"/>
    /// </summary>
    public bool ImplicitOperator { get; set; } = true;

    /// <summary>
    /// 生成的隐式转换是否不考虑 <see langword="null"/>，默认值为 <see langword="true"/>。
    /// <para>生成示例：</para>
    /// <list type="bullet">
    /// <item>TViewModel(TModel m) => new(m);</item>
    /// <item>TViewModel?([NotNullIfNotNull(nameof(m))] TModel? m) => m is null ? null : new(m);</item>
    /// </list>
    /// </summary>
    public bool ImplicitOperatorNotNull { get; set; } = true;

    /// <summary>
    /// 是否为密封类，默认值为 <see langword="true"/>
    /// </summary>
    public bool IsSealed { get; set; } = true;

    /// <summary>
    /// 视图模型的基类
    /// </summary>
    public Type? ViewModelBaseType { get; set; }

    /// <summary>
    /// 标注需要生成属性通知的属性
    /// </summary>
    public string[]? Properties { get; set; }

    // attr 标记 partial 字段自动应用源生成属性通知与调用
    // https://github.com/dotnet/csharplang/issues/6420
    // https://github.com/dotnet/csharplang/discussions/3412
    // Properties 还需要标注 Type，根据 model 获取属性的类型自动补充？改成 string[] 仅标注属性名？
}
