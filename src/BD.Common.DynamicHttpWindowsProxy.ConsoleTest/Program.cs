#if WINDOWS7_0_OR_GREATER
if (OperatingSystem.IsWindows())
{
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    HttpClient.DefaultProxy = DynamicHttpWindowsProxy.Instance;
    var client = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10),
    };
    while (true)
    {
        try
        {
            const string api_tool = "https://ip.tool.lu";
            //const string api_sohu = "https://pv.sohu.com/cityjson";
            var str = await client.GetStringAsync(api_tool);
            Console.WriteLine(str.TrimEnd());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        await Task.Delay(1000);
        Console.WriteLine();
    }
}
#endif