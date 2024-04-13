namespace BD.Common8.SourceGenerator.Repositories.Enums;

/// <summary>
/// <see cref="string"/> 类型用于 <see cref="Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
/// </summary>
public enum StringWhereType : byte
{
    /// <summary>
    /// 使用 <see cref="string.Contains(string)"/>
    /// </summary>
    Contains = 0,

    /// <summary>
    /// 使用 <see cref="string.Equals(string)"/>
    /// </summary>
    Equals,
}