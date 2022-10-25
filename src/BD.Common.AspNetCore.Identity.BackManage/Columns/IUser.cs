namespace BD.Common.Columns;

#if !BLAZOR
public interface IOperatorUser
{
    /// <inheritdoc cref="IOperatorUserId.OperatorUserId"/>
    SysUser? OperatorUser { get; set; }
}
#endif

#if !BLAZOR
public interface ICreateUser
{
    /// <inheritdoc cref="ICreateUserId.CreateUserId"/>
    SysUser? CreateUser { get; set; }
}
#endif