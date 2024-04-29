namespace BD.Common8.AspNetCore.Columns;

/// <inheritdoc cref="IOperatorUserId.OperatorUserId"/>
public interface IOperatorUser
{
    /// <inheritdoc cref="IOperatorUserId.OperatorUserId"/>
    BMUser? OperatorUser { get; set; }
}

/// <inheritdoc cref="ICreateUserId.CreateUserId"/>
public interface ICreateUser
{
    /// <inheritdoc cref="ICreateUserId.CreateUserId"/>
    BMUser? CreateUser { get; set; }
}