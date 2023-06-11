namespace BD.Common.Repositories.SourceGenerator.Annotations;

/// <summary>
/// 仓储层函数实现种类
/// </summary>
public enum RepositoryMethodImplType : byte
{
    /// <summary>
    /// 使用 <see cref="System.Linq.Expressions.Expression"/> 表达式生成实现
    /// </summary>
    Expression = 1,

    /// <summary>
    /// 使用 AutoMapper.QueryableExtensions 的 ProjectTo 生成实现
    /// </summary>
    ProjectTo = 2,

    /// <summary>
    /// 仅生成分部方法，包含函数注释与定义，由自定义分部类中实现
    /// </summary>
    Custom = byte.MaxValue,
}
