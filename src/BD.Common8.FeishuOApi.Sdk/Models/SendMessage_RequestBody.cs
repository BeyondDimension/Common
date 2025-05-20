#pragma warning disable IDE1006 // 命名样式
using System.Diagnostics;

namespace BD.Common8.FeishuOApi.Sdk.Models;

[DebuggerDisplay("{DebuggerDisplay(),nq}")]
sealed class SendMessage_RequestBody
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
    string DebuggerDisplay() => global::System.Text.Json.JsonSerializer.Serialize(this, FeishuApiClientJsonSerializerContext.Default.Options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    public string msg_type { get; set; } = "post";

    public required Content content { get; set; }

    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public sealed class Content
    {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        string DebuggerDisplay() => global::System.Text.Json.JsonSerializer.Serialize(this, FeishuApiClientJsonSerializerContext.Default.Options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

        public required Post post { get; set; }

        public sealed class Post
        {
            public required ZhCn zh_cn { get; set; }

            public sealed class ZhCn
            {
                public string? title { get; set; }

                public required Content2[][] content { get; set; }

                public sealed class Content2
                {
                    public string? tag { get; set; }

                    public string? text { get; set; }
                }
            }
        }
    }

    public static SendMessage_RequestBody CreateTextMessage(string? title, string? text)
    {
        var body = new SendMessage_RequestBody
        {
            content = new()
            {
                post = new()
                {
                    zh_cn = new()
                    {
                        title = title,
                        content =
                        [
                            [
                                    new()
                                    {
                                        tag = "text",
                                        text = text,
                                    },
                                ],
                            ],
                    },
                },
            },
        };
        return body;
    }
}