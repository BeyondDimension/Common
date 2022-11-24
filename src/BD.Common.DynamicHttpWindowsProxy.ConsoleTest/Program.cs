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
            // https://ip.tool.lu
            var str = await client.GetStringAsync("https://pv.sohu.com/cityjson");
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