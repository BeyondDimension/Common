// https://github.com/aspnet/AspNetIdentity/blob/master/src/Microsoft.AspNet.Identity.Core/AsyncHelper.cs

#pragma warning disable SA1600 // Elements should be documented

namespace System.Extensions;

public static partial class TaskExtensions
{
    static readonly TaskFactory _myTaskFactory = new(CancellationToken.None,
        TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    public static TResult RunSync<TResult>(this Func<Task<TResult>> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        return _myTaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }

    public static void RunSync(this Func<Task> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        _myTaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func();
        }).Unwrap().GetAwaiter().GetResult();
    }

    public static TResult RunSync<TResult>(this Func<ValueTask<TResult>> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        return _myTaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func().AsTask();
        }).Unwrap().GetAwaiter().GetResult();
    }

    public static void RunSync(this Func<ValueTask> func)
    {
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        _myTaskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = cultureUi;
            return func().AsTask();
        }).Unwrap().GetAwaiter().GetResult();
    }

    public static Task Forget(
       this Task task,
       [CallerMemberName] string callerMemberName = "",
       [CallerFilePath] string callerFilePath = "",
       [CallerLineNumber] int callerLineNumber = 0)
    {
        task.ContinueWith(
               x => TaskLog.Raise(new TaskLog(callerMemberName, callerFilePath, callerLineNumber, x.Exception)),
               TaskContinuationOptions.OnlyOnFaulted).ConfigureAwait(false);
        return task;
    }

    public static void ForgetAndDispose(
        this Task task,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        task.ContinueWith(
            x => TaskLog.Raise(new TaskLog(callerMemberName, callerFilePath, callerLineNumber, x.Exception)),
            TaskContinuationOptions.OnlyOnFaulted).ContinueWith(s => s.Dispose()).ConfigureAwait(false);
    }

    public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

    public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks) => Task.WhenAll(tasks);

    sealed class TaskLog(string callerMemberName, string callerFilePath, int callerLineNumber, Exception? exception)
    {
        public string CallerMemberName { get; } = callerMemberName;

        public string CallerFilePath { get; } = callerFilePath;

        public int CallerLineNumber { get; } = callerLineNumber;

        public Exception? Exception { get; } = exception;

        public static readonly EventHandler<TaskLog> Occured = (sender, e) =>
        {
            const string format = @"Unhandled Exception occured from Task.Forget()
-----------
Caller file  : {1}
             : line {2}
Caller member: {0}
Exception: {3}

";
            Debug.WriteLine(format, e.CallerMemberName, e.CallerFilePath, e.CallerLineNumber, e.Exception);
        };

        internal static void Raise(TaskLog log)
        {
            Occured?.Invoke(typeof(TaskLog), log);
        }
    }
}