namespace BD.Common8.FeishuOApi.Sdk.Models;

#pragma warning disable IDE1006 // 命名样式
sealed class SendMessage_RequestBody
{
    public string msg_type { get; set; } = "post";

    public required Content content { get; set; }

    public sealed class Content
    {
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