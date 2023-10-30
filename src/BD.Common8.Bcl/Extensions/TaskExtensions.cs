// https://github.com/aspnet/AspNetIdentity/blob/master/src/Microsoft.AspNet.Identity.Core/AsyncHelper.cs
namespace System.Extensions;

public static partial class TaskExtensions
{
    static readonly TaskFactory _myTaskFactory = new(CancellationToken.None,
        TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    /// <summary>
    /// 执行带返回值的异步任务
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 执行不带返回值的异步任务
    /// </summary>
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

    /// <summary>
    /// 执行带返回值的 ValueTask 异步方法
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 执行不带返回值的 ValueTask 异步方法
    /// </summary>
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

    /// <summary>
    /// 将任务忽略，并在发生错误时记录异常信息
    /// </summary>
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

    /// <summary>
    /// 将任务忽略并进行资源释放，在发生错误时记录异常信息
    /// </summary>
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

    /// <summary>
    /// 等待所有任务完成
    /// </summary>
    public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

    /// <summary>
    /// 等待所有任务完成并返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tasks"></param>
    /// <returns></returns>
    public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks) => Task.WhenAll(tasks);

    /// <summary>
    /// 用于记录未处理异常的相关信息
    /// </summary>
    sealed class TaskLog(string callerMemberName, string callerFilePath, int callerLineNumber, Exception? exception)
    {
        /// <summary>
        /// 调用者成员名称
        /// </summary>
        public string CallerMemberName { get; } = callerMemberName;

        /// <summary>
        /// 调用者文件路径
        /// </summary>
        public string CallerFilePath { get; } = callerFilePath;

        /// <summary>
        /// 调用者代码行号
        /// </summary>
        public int CallerLineNumber { get; } = callerLineNumber;

        /// <summary>
        /// 异常实例
        /// </summary>
        public Exception? Exception { get; } = exception;

        /// <summary>
        /// 异常发生时的事件处理程序
        /// </summary>
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

        /// <summary>
        /// 触发未处理异常事件
        /// </summary>
        internal static void Raise(TaskLog log)
        {
            Occured?.Invoke(typeof(TaskLog), log);
        }
    }
}