namespace BD.Common8.UnitTest;

/// <summary>
/// <see cref="PortHelper"/> 测试
/// </summary>
public sealed class PortTest
{
    /// <summary>
    /// <see cref="PortHelper.GetRandomUnusedPort(IPAddress)"/>
    /// </summary>
    [Test]
    public void GetRandomUnusedPort()
    {
        var port = PortHelper.GetRandomUnusedPort(IPAddress.Loopback);
        TestContext.WriteLine(port);
    }

#if WINDOWS7_0_OR_GREATER
    /// <summary>
    /// <see cref="PortHelper.GetProcessByTcpPort(ushort)"/>
    /// </summary>
    [Test]
    public void GetProcessByTcpPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        TestContext.WriteLine(port);
        var process = PortHelper.GetProcessByTcpPort((ushort)port);
        TestContext.WriteLine(process?.ProcessName);
    }
#endif
}