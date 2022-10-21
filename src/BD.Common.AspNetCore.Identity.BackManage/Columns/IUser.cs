namespace BD.Common.Columns;

#if !BLAZOR
public interface IOperatorUser<TBMUser> where TBMUser : BMUser
{
    /// <inheritdoc cref="IOperatorUserId.OperatorUserId"/>
    TBMUser? OperatorUser { get; set; }
}

public interface IOperatorUser : IOperatorUser<BMUser>
{

}
#endif

#if !BLAZOR
public interface ICreateUser<TBMUser> where TBMUser : BMUser
{
    /// <inheritdoc cref="ICreateUserId.CreateUserId"/>
    TBMUser? CreateUser { get; set; }
}
public interface ICreateUser : ICreateUser<BMUser>
{

}
#endif