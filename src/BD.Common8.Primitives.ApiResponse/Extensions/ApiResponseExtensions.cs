namespace BD.Common8.Extensions;

public static partial class ApiResponseExtensions
{
    /// <summary>
    /// 获取错误消息
    /// </summary>
    /// <param name="rsp"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetMessages(this IApiResponse rsp)
    {
        var messages = rsp.Messages;
        return messages.Length switch
        {
            0 => string.Empty,
            1 => messages[0],
            _ => string.Join(Environment.NewLine, messages),
        };
    }

    /// <summary>
    /// 设置错误消息
    /// </summary>
    /// <param name="rsp"></param>
    /// <param name="messages"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetMessages(this IApiResponse rsp, params string[] messages)
        => rsp.Messages = messages;
}