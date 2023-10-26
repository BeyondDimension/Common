namespace BD.Common8.AspNetCore.Columns;

public interface IOperatorUser
{
    /// <inheritdoc cref="IOperatorUserId.OperatorUserId"/>
    SysUser? OperatorUser { get; set; }
}

public interface ICreateUser
{
    /// <inheritdoc cref="ICreateUserId.CreateUserId"/>
    SysUser? CreateUser { get; set; }
}