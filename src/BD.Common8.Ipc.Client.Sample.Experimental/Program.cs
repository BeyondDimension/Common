static string GetConnectionString(IpcAppConnectionStringType connectionStringType, string value)
{
    using var stream = new MemoryStream();
    stream.WriteByte((byte)connectionStringType);
    switch (connectionStringType)
    {
        case IpcAppConnectionStringType.Https:
            stream.Write("https://localhost:"u8);
            stream.Write(Encoding.UTF8.GetBytes(value));
            break;
        case IpcAppConnectionStringType.UnixSocket:
            stream.Write(Encoding.UTF8.GetBytes(value));
            break;
        case IpcAppConnectionStringType.NamedPipe:
            stream.Write(Encoding.UTF8.GetBytes(value));
            break;
        default:
            ThrowHelper.ThrowArgumentOutOfRangeException(connectionStringType);
            return default!;
    }
    return stream.ToArray().Base64UrlEncode();
}

const string pipeName = "BD.Common8.Ipc.Server.Sample.Experimental";

var clientNamedPipe = IpcAppConnectionStringHelper.GetHttpClient(GetConnectionString(IpcAppConnectionStringType.NamedPipe, pipeName));
var clientHttps = IpcAppConnectionStringHelper.GetHttpClient(GetConnectionString(IpcAppConnectionStringType.Https, "5076"));

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