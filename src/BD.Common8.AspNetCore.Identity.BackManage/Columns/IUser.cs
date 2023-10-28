namespace BD.Common8.AspNetCore.Columns;

/// <inheritdoc cref="IOperatorUserId.OperatorUserId"/>
public interface IOperatorUser
{
    /// <inheritdoc cref="IOperatorUserId.OperatorUserId"/>
    SysUser? OperatorUser { get; set; }
}

/// <inheritdoc cref="ICreateUserId.CreateUserId"/>
public interface ICreateUser
{
    /// <inheritdoc cref="ICreateUserId.CreateUserId"/>
    SysUser? CreateUser { get; set; }
}