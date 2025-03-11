namespace BD.Common8.FeishuOApi.Sdk.Services;

/// <summary>
/// 飞书开放平台 WebApi 调用实现
/// </summary>
/// <param name="logger"></param>
/// <param name="httpClient"></param>
/// <param name="options"></param>
public sealed class FeishuApiClient(ILogger<FeishuApiClient> logger, HttpClient httpClient, IOptions<FeishuApiOptions> options) : IFeishuApiClient
{
    /// <inheritdoc cref="FeishuApiOptions.HookId"/>
    string HookId
    {
        get
        {
            var hookId = options.Value.HookId;
            if (string.IsNullOrWhiteSpace(hookId))
                throw GetInvalidKeyException(nameof(HookId));
            return hookId!;
        }
    }

    string? ServerTag => options.Value.ServerTag;

    static ApplicationException GetInvalidKeyException(string name) => throw new(
$"""
Enter (dotnet user-secrets set "FeishuApiOptions:{name}" "value") on the current csproj path to set the secret value see https://learn.microsoft.com/zh-cn/aspnet/core/security/app-secrets
""");

    /// <inheritdoc/>
    public async Task<ApiRspImpl<HttpStatusCode>> SendMessageAsync(
        string? title,
        string? text,
        CancellationToken cancellationToken = default)
    {
        // https://steamstar.feishu.cn/docx/O2qpdssrNokVKNxHG4XcLL59nJb
        // https://open.feishu.cn/document/client-docs/bot-v3/add-custom-bot
        HttpResponseMessage? rsp = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(ServerTag))
            {
                text = $"{ServerTag}: {text}";
            }

            var reqUrl = $"/open-apis/bot/v2/hook/{HookId}";
            var reqBody = SendMessage_RequestBody.CreateTextMessage(title, text);

            // 👇 IL2026/IL3050 已使用源生成器（FeishuApiClientJsonSerializerContext），但因使用的是 JsonSerializerOptions 而不是 JsonTypeInfo<T> 引发分析器警告，可忽略
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#if DEBUG
            //var reqJson = SystemTextJsonSerializer.Serialize(reqBody, FeishuApiClientJsonSerializerContext.Instance.Options);
#endif
            rsp = await httpClient.PostAsJsonAsync(reqUrl, reqBody, FeishuApiClientJsonSerializerContext.Instance.Options, cancellationToken);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.

            return rsp.StatusCode;
        }
        catch (Exception ex)
        {
            string? rspContent = null;
            try
            {
                if (rsp != null)
                {
                    rspContent = await rsp.Content.ReadAsStringAsync(cancellationToken);
                }
            }
            catch
            {
            }

            logger.LogError(ex, "SendMessageAsync fail, title: {title}, text:{text}, rspContent: {rspContent}", title, text, rspContent);
            return ex;
        }
        finally
        {
            rsp?.Dispose();
        }
    }
}