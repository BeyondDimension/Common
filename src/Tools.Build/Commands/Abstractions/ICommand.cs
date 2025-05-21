using System.CommandLine;
using System.Diagnostics;

namespace Tools.Build.Commands.Abstractions;

/// <summary>
/// 命令行业务接口
/// </summary>
public interface ICommand
{
    /// <summary>
    /// 获取当前业务的命令行实例
    /// </summary>
    /// <returns></returns>
    internal static abstract Command GetCommand();

    /// <summary>
    /// 添加当前业务命令行到 <see cref="RootCommand"/>
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="rootCommand"></param>
    static void AddCommand<TCommand>(RootCommand rootCommand) where TCommand : ICommand
    {
        var command = TCommand.GetCommand();
        rootCommand.AddCommand(command);
    }

    protected sealed class CommandStopwatch(string commandName) : IDisposable
    {
        readonly Stopwatch sw = Stopwatch.StartNew();
        bool disposedValue;

        public bool IsSuccess { get; set; }

        public Exception? Exception { get; set; }

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    sw.Stop();
                    var timeSpan = sw.Elapsed;
                    Console.WriteLine($"执行 {commandName} 命令完成，耗时：{Math.Floor(timeSpan.TotalHours):00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
                }

                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

sealed class ExitApplicationException : Exception
{
    public ExitApplicationException(int exitCode)
    {
        ExitCode = exitCode;
    }

    public ExitApplicationException(int exitCode, string message) : base(message)
    {
        ExitCode = exitCode;
    }

    public int ExitCode { get; }
}