namespace BD.Common8.Http.ClientFactory.Services;

partial class WebApiClientBaseService
{
    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    protected virtual HttpContent? GetSJsonContent<T>(T inputValue, JsonTypeInfo<T> jsonTypeInfo, MediaTypeHeaderValue? mediaType = null)
    {
        if (inputValue == null)
            return null;
        try
        {
            JsonContent content;
            //            if (jsonTypeInfo == null)
            //            {
            //#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            //                content = JsonContent.Create(inputValue, mediaType);
            //#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            //            }
            //            else
            //            {
            content = JsonContent.Create(inputValue, jsonTypeInfo, mediaType);
            //}
            return content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error serializing request model class. (Parameter '{type}')",
                typeof(T));
            return default;
        }
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），推荐使用 Json 源生成，即传递 <see cref="JsonTypeInfo"/> 对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<T?> ReadFromSJsonAsync<T>(HttpContent content, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            T? jsonObj;
            //            if (jsonTypeInfo == null)
            //            {
            //#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            //                jsonObj = await content.ReadFromJsonAsync<T>(cancellationToken);
            //#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            //            }
            //            else
            //            {
            jsonObj = await content.ReadFromJsonAsync(jsonTypeInfo, cancellationToken);
            //}
            return jsonObj;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error reading and deserializing the response content into an instance. (Parameter '{type}')",
                typeof(T));
            return default;
        }
    }
}
