const string pipeName = "BD.Common8.Ipc.Server.Sample.Experimental";

var clientNamedPipe = IpcAppConnectionStringHelper.GetHttpClient(
    new IpcAppConnectionString
    {
        Type = IpcAppConnectionStringType.NamedPipe,
        StringValue = pipeName,
    });
var clientHttps = IpcAppConnectionStringHelper.GetHttpClient(
    new IpcAppConnectionString
    {
        Type = IpcAppConnectionStringType.Https,
        Int32Value = 5076,
    });

static Task<string> GetStringAsync(HttpClient client) => client.GetStringAsync("/todos");

while (true)
{
    try
    {
        var resultNamedPipe = await GetStringAsync(clientNamedPipe);
        Console.WriteLine("resultNamedPipe: ");
        Console.WriteLine(resultNamedPipe);

        var resultHttps = await GetStringAsync(clientHttps);
        Console.WriteLine("resultHttps: ");
        Console.WriteLine(resultHttps);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    var line = Console.ReadLine();
    if ("exit".Equals(line, StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
}