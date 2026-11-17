#if WINDOWS
using Microsoft.Win32.SafeHandles;

namespace System.Diagnostics;

public static partial class WindowsProcessCommandLine
{
    // Windows 8.1+ supports querying the command line without reading another process's PEB.
    const int ProcessCommandLineInformation = 60;
    const int StatusInfoLengthMismatch = unchecked((int)0xC0000004);
    const int StatusBufferTooSmall = unchecked((int)0xC0000023);

    public static unsafe string Read(int processId)
    {
        using var process = OpenProcess(0x1000 /* PROCESS_QUERY_LIMITED_INFORMATION */, 0, processId);
        if (process.IsInvalid)
            throw new Win32Exception(Marshal.GetLastPInvokeError());

        byte* buffer = stackalloc byte[1024];
        var status = NtQueryInformationProcess(process, ProcessCommandLineInformation, buffer, 1024, out var length);
        if (status != StatusInfoLengthMismatch && status != StatusBufferTooSmall)
        {
            ThrowIfFailed(status);
            return ReadString(buffer);
        }

        var allocation = NativeMemory.Alloc(length);
        try
        {
            status = NtQueryInformationProcess(process, ProcessCommandLineInformation, allocation, length, out _);
            ThrowIfFailed(status);
            return ReadString(allocation);
        }
        finally
        {
            NativeMemory.Free(allocation);
        }
    }

    static void ThrowIfFailed(int status)
    {
        if (status < 0)
            throw new Win32Exception((int)RtlNtStatusToDosError(status));
    }

    static unsafe string ReadString(void* buffer)
    {
        var value = (UnicodeString*)buffer;
        return value->Length == 0 ? string.Empty : new string(value->Buffer, 0, value->Length / sizeof(char));
    }

    [StructLayout(LayoutKind.Sequential)]
    struct UnicodeString
    {
        public ushort Length;
        public ushort MaximumLength;
        public unsafe char* Buffer;
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial SafeProcessHandle OpenProcess(uint access, int inheritHandle, int processId);

    [LibraryImport("ntdll.dll")]
    private static unsafe partial int NtQueryInformationProcess(SafeProcessHandle process, int informationClass,
        void* information, uint informationLength, out uint returnLength);

    [LibraryImport("ntdll.dll")]
    private static partial uint RtlNtStatusToDosError(int status);
}
#endif
