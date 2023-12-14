#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter

#if WINDOWS7_0_OR_GREATER
using CsWin32 = Windows.Win32;
#endif

namespace System.Net;

/// <summary>
/// 端口号助手类
/// </summary>
public static partial class PortHelper
{
    /// <summary>
    /// 获取一个随机的未使用的端口
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetRandomUnusedPort(IPAddress address)
    {
        using var listener = new TcpListener(address, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        return port;
    }

    /// <summary>
    /// 检查指定的端口是否被占用
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUsePort(IPAddress address, int port)
    {
        try
        {
            using var listener = new TcpListener(address, port);
            listener.Start();
            return false;
        }
        catch
        {
            return true;
        }
    }

    /// <inheritdoc cref="IsUsePort(IPAddress, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUsePort(int port)
    {
        try
        {
            return IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpListeners()
                .Any(x => x.Port == port);
        }
        catch
        {
            return IsUsePort(IPAddress.Loopback, port);
        }
    }

    /// <summary>
    /// 根据 TCP 端口号获取占用的进程
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    [SupportedOSPlatform("Windows7.0")]
    public static unsafe Process? GetProcessByTcpPort(ushort port)
    {
#if WINDOWS7_0_OR_GREATER
        uint bufferSize = 0;

        // Getting the size of TCP table, that is returned in 'bufferSize' variable.
        _ = CsWin32.PInvoke.GetExtendedTcpTable(default, ref bufferSize, true, (uint)AddressFamily.InterNetwork,
            CsWin32.NetworkManagement.IpHelper.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);

        // Allocating memory from the unmanaged memory of the process by using the
        // specified number of bytes in 'bufferSize' variable.
        IntPtr tcpTableRecordsPtr = Marshal.AllocHGlobal((int)bufferSize);

        try
        {
            // The size of the table returned in 'bufferSize' variable in previous
            // call must be used in this subsequent call to 'GetExtendedTcpTable'
            // function in order to successfully retrieve the table.
            var result = CsWin32.PInvoke.GetExtendedTcpTable((void*)tcpTableRecordsPtr, ref bufferSize, true,
               (uint)AddressFamily.InterNetwork, CsWin32.NetworkManagement.IpHelper.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);

            // Non-zero value represent the function 'GetExtendedTcpTable' failed,
            // hence empty list is returned to the caller function.
            if (result != 0)
                return null;

            // Marshals data from an unmanaged block of memory to a newly allocated
            // managed object 'tcpRecordsTable' of type 'MIB_TCPTABLE_OWNER_PID'
            // to get number of entries of the specified TCP table structure.
            MIB_TCPTABLE_OWNER_PID tcpRecordsTable =
                                    Marshal.PtrToStructure<MIB_TCPTABLE_OWNER_PID>(tcpTableRecordsPtr);
            IntPtr tableRowPtr = tcpTableRecordsPtr +
                                    Marshal.SizeOf(tcpRecordsTable.dwNumEntries);

            // Reading and parsing the TCP records one by one from the table and
            // storing them in a list of 'TcpProcessRecord' structure type objects.
            for (int row = 0; row < tcpRecordsTable.dwNumEntries; row++)
            {
                MIB_TCPROW_OWNER_PID tcpRow = Marshal.
                    PtrToStructure<MIB_TCPROW_OWNER_PID>(tableRowPtr);
                var localPort = BitConverter.ToUInt16(
                [
                    tcpRow.localPort[1],
                    tcpRow.localPort[0],
                ], 0);
                if (localPort == port)
                {
                    try
                    {
                        return Process.GetProcessById(tcpRow.owningPid);
                    }
                    catch
                    {
                        return null;
                    }
                }
                tableRowPtr += Marshal.SizeOf(tcpRow);
            }
        }
        catch
        {
        }
        finally
        {
            Marshal.FreeHGlobal(tcpTableRecordsPtr);
        }
        return null;
#else
        throw new PlatformNotSupportedException();
#endif
    }

#if WINDOWS7_0_OR_GREATER

    // https://github.com/microsoftarchive/msdn-code-gallery-microsoft/blob/master/OneCodeTeam/C%23%20Sample%20to%20list%20all%20the%20active%20TCP%20and%20UDP%20connections%20using%20Windows%20Form%20appl/%5BC%23%5D-C%23%20Sample%20to%20list%20all%20the%20active%20TCP%20and%20UDP%20connections%20using%20Windows%20Form%20appl/C%23/SocketConnection/SocketConnection.cs

    /// <summary>
    /// 此函数读取并解析可用的活动 TCP 套接字连接
    /// 并将它们存储在列表中
    /// </summary>
    /// <returns>
    /// 它返回当前活动的 TCP 套接字连接集
    /// </returns>
    /// <exception cref="OutOfMemoryException">
    /// 此异常可能由函数 Marshal 引发，AllocHGlobal 内存不足，无法满足请求
    /// </exception>
    [SupportedOSPlatform("Windows7.0")]
    public static unsafe List<TcpProcessRecord> GetAllTcpConnections()
    {
        uint bufferSize = 0;
        List<TcpProcessRecord> tcpTableRecords = [];

        // 正在获取 TCP 表的大小，该大小在“bufferSize”变量中返回
        _ = CsWin32.PInvoke.GetExtendedTcpTable(default, ref bufferSize, true, (uint)AddressFamily.InterNetwork,
            CsWin32.NetworkManagement.IpHelper.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);

        // Allocating memory from the unmanaged memory of the process by using the
        // specified number of bytes in 'bufferSize' variable.
        var tcpTableRecordsPtr = Marshal.AllocHGlobal((int)bufferSize);

        try
        {
            // The size of the table returned in 'bufferSize' variable in previous
            // call must be used in this subsequent call to 'GetExtendedTcpTable'
            // function in order to successfully retrieve the table.
            var result = CsWin32.PInvoke.GetExtendedTcpTable((void*)tcpTableRecordsPtr, ref bufferSize, true,
                (uint)AddressFamily.InterNetwork, CsWin32.NetworkManagement.IpHelper.TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);

            // Non-zero value represent the function 'GetExtendedTcpTable' failed,
            // hence empty list is returned to the caller function.
            if (result != 0)
                return tcpTableRecords;

            // Marshals data from an unmanaged block of memory to a newly allocated
            // managed object 'tcpRecordsTable' of type 'MIB_TCPTABLE_OWNER_PID'
            // to get number of entries of the specified TCP table structure.
            MIB_TCPTABLE_OWNER_PID tcpRecordsTable =
                                    Marshal.PtrToStructure<MIB_TCPTABLE_OWNER_PID>(tcpTableRecordsPtr);
            IntPtr tableRowPtr = tcpTableRecordsPtr +
                                    Marshal.SizeOf(tcpRecordsTable.dwNumEntries);

            // Reading and parsing the TCP records one by one from the table and
            // storing them in a list of 'TcpProcessRecord' structure type objects.
            for (int row = 0; row < tcpRecordsTable.dwNumEntries; row++)
            {
                MIB_TCPROW_OWNER_PID tcpRow = Marshal.
                    PtrToStructure<MIB_TCPROW_OWNER_PID>(tableRowPtr);
                tcpTableRecords.Add(new TcpProcessRecord(
                                      new IPAddress(tcpRow.localAddr),
                                      new IPAddress(tcpRow.remoteAddr),
                                      BitConverter.ToUInt16(
                                      [
                                          tcpRow.localPort[1],
                                          tcpRow.localPort[0],
                                      ], 0),
                                      BitConverter.ToUInt16(
                                      [
                                          tcpRow.remotePort[1],
                                          tcpRow.remotePort[0],
                                      ], 0),
                                      tcpRow.owningPid, tcpRow.state));
                tableRowPtr += Marshal.SizeOf(tcpRow);
            }
        }
        catch
        {
        }
        finally
        {
            Marshal.FreeHGlobal(tcpTableRecordsPtr);
        }
        return tcpTableRecords;
    }

    /// <summary>
    /// 该结构包含描述 IPv4 TCP 连接的信息
    /// IPv4地址、TCP连接使用的端口以及与连接关联的特定进程ID（PID）
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct MIB_TCPROW_OWNER_PID
    {
        public MibTcpState state;
        public uint localAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] localPort;
        public uint remoteAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] remotePort;
        public int owningPid;
    }

    /// <summary>
    /// 该结构包含进程 ID（PID）的表以及上下文绑定到这些 PID 的 IPv4 TCP 链路
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct MIB_TCPTABLE_OWNER_PID
    {
        public uint dwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public MIB_TCPROW_OWNER_PID[] table;
    }

    /// <summary>
    /// 此类提供访问 IPv4 TCP 连接地址和端口及其关联的进程 ID 和名称
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class TcpProcessRecord
    {
        /// <summary>
        /// 获取或设置本地地址
        /// </summary>
        [DisplayName("Local Address")]
        public IPAddress LocalAddress { get; set; }

        /// <summary>
        /// 获取或设置本地端口
        /// </summary>
        [DisplayName("Local Port")]
        public ushort LocalPort { get; set; }

        /// <summary>
        /// 获取或设置远程地址
        /// </summary>
        [DisplayName("Remote Address")]
        public IPAddress RemoteAddress { get; set; }

        /// <summary>
        /// 获取或设置远程端口
        /// </summary>
        [DisplayName("Remote Port")]
        public ushort RemotePort { get; set; }

        /// <summary>
        /// 获取或设置 TCP 连接状态
        /// </summary>
        [DisplayName("State")]
        public MibTcpState State { get; set; }

        int _ProcessId;

        /// <summary>
        /// 获取或设置进程 ID
        /// </summary>
        [DisplayName("Process ID")]
        public int ProcessId
        {
            get => _ProcessId;
            set
            {
                _ProcessId = value;
                _Process = new(() =>
                {
                    try
                    {
                        return Process.GetProcessById(value);
                    }
                    catch
                    {
                    }
                    return null;
                });
            }
        }

        Lazy<Process?>? _Process;

        /// <summary>
        /// 获取与当前进程 ID 关联的进程
        /// </summary>
        [DisplayName("Process")]
        public Process? Process => _Process?.Value;

        /// <summary>
        /// 获取进程的名称
        /// </summary>
        [DisplayName("Process Name")]
        public string? ProcessName => Process?.ProcessName;

        /// <summary>
        /// 初始化 TcpProcessRecord 类的新实例
        /// </summary>
        /// <param name="localIp">本地 IP 地址</param>
        /// <param name="remoteIp">远程 IP 地址</param>
        /// <param name="localPort">本地端口号</param>
        /// <param name="remotePort">远程端口号</param>
        /// <param name="pId">进程 ID</param>
        /// <param name="state">TCP 连接状态</param>
        public TcpProcessRecord(IPAddress localIp, IPAddress remoteIp, ushort localPort,
            ushort remotePort, int pId, MibTcpState state)
        {
            LocalAddress = localIp;
            RemoteAddress = remoteIp;
            LocalPort = localPort;
            RemotePort = remotePort;
            State = state;
            ProcessId = pId;
        }
    }

    /// <summary>
    /// TCP 连接的不同可能状态的枚举
    /// </summary>
    public enum MibTcpState
    {
        CLOSED = 1,
        LISTENING = 2,
        SYN_SENT = 3,
        SYN_RCVD = 4,
        ESTABLISHED = 5,
        FIN_WAIT1 = 6,
        FIN_WAIT2 = 7,
        CLOSE_WAIT = 8,
        CLOSING = 9,
        LAST_ACK = 10,
        TIME_WAIT = 11,
        DELETE_TCB = 12,
        NONE = 0,
    }

#endif
}
