public static class ApiResponseExtensions
{
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetMessages(this IApiResponse rsp, params string[] messages) => rsp.Messages = messages;
}