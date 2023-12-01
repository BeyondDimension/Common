namespace BD.Common8.Columns;

/// <inheritdoc cref="OperatorUserId"/>
public interface IOperatorUserId
{
    /// <summary>
    /// 最后一次操作的人(纪录后台管理员禁用或启用或编辑该条的操作)
    /// </summary>
    Guid? OperatorUserId { get; set; }
}

/// <inheritdoc cref="CreateUserId"/>
public interface ICreateUserId
{
    /// <summary>
    /// 创建人(创建此条目的后台管理员)
    /// </summary>
    Guid CreateUserId { get; set; }
}

/// <inheritdoc cref="ICreateUserId.CreateUserId"/>
public interface ICreateUserIdNullable
{
    /// <inheritdoc cref="ICreateUserId.CreateUserId"/>
    Guid? CreateUserId { get; set; }
}