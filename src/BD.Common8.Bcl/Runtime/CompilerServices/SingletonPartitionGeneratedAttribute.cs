namespace System.Runtime.CompilerServices;

/// <summary>
/// 用于标注需要使用单例(Singleton)模式的服务类源生成
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SingletonPartitionGeneratedAttribute : Attribute
{
    /// <summary>
    /// 是否生成构造函数，默认值为 <see langword="true"/>
    /// </summary>
    public bool Constructor { get; set; } = true;

    /// <summary>
    /// TSingleton.Current 的注释
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 生成的 TSingleton.Current 属性名，默认为 Current
    /// </summary>
    public string? PropertyName { get; set; }

    /// <summary>
    /// 是否为密封类，默认值为 <see langword="true"/>
    /// </summary>
    public bool IsSealed { get; set; } = true;
}
