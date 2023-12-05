//using Microsoft.AspNetCore.SignalR;

//namespace Ipc.Server.Sample.Experimental.Hubs;

///// <summary>
///// 测试 Hub
///// </summary>
//public class TestHub : Hub
//{
//    private readonly ILogger<TestHub> logger;

//    /// <summary>
//    /// 构造
//    /// </summary>
//    /// <param name="logger"></param>
//    public TestHub(ILogger<TestHub> logger)
//    {
//        this.logger = logger;
//    }

//    /// <summary>
//    /// 连接时方法
//    /// </summary>
//    /// <returns></returns>
//    public override Task OnConnectedAsync()
//    {
//        logger.LogInformation("Id:{id},已连接:{date}", Context.ConnectionId, DateTime.Now);
//        return base.OnConnectedAsync();
//    }

//    /// <summary>
//    /// 断开连接时方法
//    /// </summary>
//    /// <param name="exception"></param>
//    /// <returns></returns>
//    public override Task OnDisconnectedAsync(Exception? exception)
//    {
//        logger.LogInformation("Id:{id},已断开连接:{date}", Context.ConnectionId, DateTime.Now);
//        return base.OnDisconnectedAsync(exception);
//    }

//    /// <summary>
//    /// 调用客户端回调方法 ServerReceivedMsg
//    /// </summary>
//    /// <param name="msg"></param>
//    /// <returns></returns>
//    public async Task CallClientCallback(string msg)
//    {
//        await Clients.Caller.SendAsync("ServerReceivedMsg", $"服务端收到了消息:{msg}");
//    }

//    /// <summary>
//    /// 直接返回客户端结果
//    /// </summary>
//    /// <param name="msg"></param>
//    /// <returns></returns>
//    public async Task<string> ReturnClientResult(string msg)
//    {
//        var req = Context.Features;
//        return $"服务端收到了消息:{msg}";
//    }
//}