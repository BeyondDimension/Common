// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public interface IRandomGetUserAgentService
{
    string GetUserAgent();

    const string Steam = "Mozilla/5.0 (Windows; U; Windows NT 10.0; en-US; Valve Steam GameOverlay/1669935987; ) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36";
    const string Win10EdgeLatest = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.69";
    const string Win10ChromeLatest = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";

    static string GetRandomUserAgent(string[] userAgents)
        => userAgents[Random2.Next(userAgents.Length)];
}
