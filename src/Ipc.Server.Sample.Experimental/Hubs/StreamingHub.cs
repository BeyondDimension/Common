using Microsoft.AspNetCore.SignalR;

namespace Ipc.Server.Sample.Experimental.Hubs;

/// <summary>
/// 流式操作
/// </summary>
public class StreamingHub : Hub
{
    /// <summary>
    /// 测试 AsyncEnumerable
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<Pack> AsyncEnumerable(int count)
    {
        await Task.Yield();

        while (count > 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1));
            yield return new Pack();
            count--;
        }
    }

    /// <summary>
    /// 测试包对象
    /// </summary>
    public class Pack
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime DateTime { get; set; } = DateTime.Now;

        /// <summary>
        ///  id
        /// </summary>
        public string PackId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// 数据
        /// </summary>
        public object? Data { get; set; }
    }
}