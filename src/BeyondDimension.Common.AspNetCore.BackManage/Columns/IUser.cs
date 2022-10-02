namespace BD.Common.Columns;

#if !BLAZOR
public interface IOperatorUser
{
    /// <inheritdoc cref="IOperatorUserId.OperatorUserId"/>
    BMUser? OperatorUser { get; set; }
}
#endif

#if !BLAZOR
public interface ICreateUser
{
    /// <inheritdoc cref="ICreateUserId.CreateUserId"/>
    BMUser? CreateUser { get; set; }
}
#endif