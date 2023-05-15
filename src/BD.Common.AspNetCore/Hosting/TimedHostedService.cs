// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio#timed-background-tasks

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
/// 异步定时后台任务
/// <para>https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-7.0&tabs=visual-studio#asynchronous-timed-background-task</para>
/// </summary>
public abstract class TimedHostedService : BackgroundService
{
    /// <summary>
    /// 调用 <see cref="DoWork()"/> 之前延迟的时间量。指定 <see cref="TimeSpan.Zero"/> 可立即启动计时器。默认值为 <see cref="TimeSpan.Zero"/>。
    /// </summary>
    protected virtual TimeSpan TimerDueTime => TimeSpan.Zero;

    /// <summary>
    /// 调用 <see cref="DoWork()"/> 的时间间隔。 指定 <see cref="Timeout.InfiniteTimeSpan"/> 可以禁用定期终止。
    /// </summary>
    protected abstract TimeSpan TimerPeriod { get; }

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimerDueTime, stoppingToken);

        // When the timer should have no due-time, then do the work once now.
        DoWork();

        using PeriodicTimer timer = new(TimerPeriod);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            DoWork();
        }
    }

    // Could also be a async method, that can be awaited in ExecuteAsync above
    protected abstract void DoWork();
}

///// <summary>
///// 计时的后台任务
///// <para>https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-7.0&tabs=visual-studio#timed-background-tasks</para>
///// </summary>
//public abstract class TimedHostedService : IHostedService, IDisposable, IAsyncDisposable
//{
//    bool disposedValue;
//    Timer? timer = null;

//    /// <summary>
//    /// 调用 <see cref="DoWork()"/> 之前延迟的时间量。 指定 <see cref="Timeout.InfiniteTimeSpan"/> 可防止启动计时器。 指定 <see cref="TimeSpan.Zero"/> 可立即启动计时器。默认值为 <see cref="TimeSpan.Zero"/>。
//    /// </summary>
//    protected virtual TimeSpan TimerDueTime => TimeSpan.Zero;

//    /// <summary>
//    /// 调用 <see cref="DoWork()"/> 的时间间隔。 指定 <see cref="Timeout.InfiniteTimeSpan"/> 可以禁用定期终止。
//    /// </summary>
//    protected abstract TimeSpan TimerPeriod { get; }

//    public virtual Task StartAsync(CancellationToken stoppingToken)
//    {
//        timer = new Timer(DoWork, stoppingToken, TimerDueTime, TimerPeriod);
//        return Task.CompletedTask;
//    }

//    void DoWork(object? state)
//    {
//        if (state is CancellationToken stoppingToken)
//        {
//            if (stoppingToken.IsCancellationRequested)
//            {
//                Dispose();
//                return;
//            }
//        }
//        DoWork();
//    }

//    protected abstract void DoWork();

//    public virtual Task StopAsync(CancellationToken stoppingToken)
//    {
//        timer?.Change(Timeout.Infinite, 0);
//        return Task.CompletedTask;
//    }

//    public async ValueTask DisposeAsync()
//    {
//        await DisposeAsyncCore().ConfigureAwait(false);
//        GC.SuppressFinalize(this);
//    }

//    protected virtual async ValueTask DisposeAsyncCore()
//    {
//        if (!disposedValue)
//        {
//            if (timer != null)
//            {
//                await timer.DisposeAsync();
//                timer = null;
//            }
//            disposedValue = true;
//        }
//    }

//    protected virtual void Dispose(bool disposing)
//    {
//        if (!disposedValue)
//        {
//            if (disposing)
//            {
//                // 释放托管状态(托管对象)
//                timer?.Dispose();
//            }

//            // 释放未托管的资源(未托管的对象)并重写终结器
//            // 将大型字段设置为 null
//            timer = null;
//            disposedValue = true;
//        }
//    }

//    public void Dispose()
//    {
//        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
//        Dispose(disposing: true);
//        GC.SuppressFinalize(this);
//    }
//}