namespace BD.Common8.AspNetCore.Middleware;

#pragma warning disable SA1600 // Elements should be documented

// https://github.com/dotnet/efcore/issues/19526#issuecomment-989036617
/* 2022-07-25 13:09:15.3924||ERROR|Microsoft.EntityFrameworkCore.Query|An exception occurred while iterating over the results of a query for context type 'System.Application.Data.ApplicationDbContext'.
System.Threading.Tasks.TaskCanceledException: A task was canceled.
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenAsync(CancellationToken cancellationToken, Boolean errorsExpected)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync() System.Threading.Tasks.TaskCanceledException: A task was canceled.
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenInternalAsync(Boolean errorsExpected, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Storage.RelationalConnection.OpenAsync(CancellationToken cancellationToken, Boolean errorsExpected)
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.InitializeReaderAsync(AsyncEnumerator enumerator, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.SingleQueryingEnumerable`1.AsyncEnumerator.MoveNextAsync()|url: https://api.steampp.net/ExternalLoginCallback|action: ExternalLoginCallback
 */

/// <summary>
/// 标准化异常中间件，过滤删除掉像 <see cref="TaskCanceledException"/>, <see cref="OperationCanceledException"/>, <see cref="ConnectionResetException"/> 等可忽略的异常信息
/// </summary>
/// <param name="next"></param>
public sealed partial class StandardizedExceptionMiddleware(RequestDelegate next)
{
    readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (IsCancellationException(ex))
            {
                if (context.RequestAborted.IsCancellationRequested)
                    // Try to ensure cancelled requests don't get reported as 5xx errors
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            // This exception wasn't handled so let it bubble up
            throw;
        }
    }

    static bool IsCancellationException(Exception ex)
    {
        if (ex == null) return false;
        if (ex is OperationCanceledException)
            return true;
        if (ex is IOException && ex.Message == "The client reset the request stream.")
            return true;
        if (ex is ConnectionResetException)
            return true;
        // .NET SQL has a number of different exceptions thrown when operations are cancelled
        if (ex.Source == "Microsoft.Data.SqlClient" && ex.Message == "Operation cancelled by user.")
            return true;
        var typeName = ex.GetType().FullName;
        switch (typeName)
        {
            case "Microsoft.Data.SqlClient.SqlException":
            case "System.Data.SqlClient.SqlException":
                if (ex.Message.Contains("Operation cancelled by user"))
                    return true;
                break;
        }
        if (ex.InnerException is not null)
            return IsCancellationException(ex.InnerException);
        return false;
    }
}
