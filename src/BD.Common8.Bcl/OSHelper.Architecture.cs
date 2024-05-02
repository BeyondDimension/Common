namespace System;

partial class OSHelper
{
    /// <summary>
    /// 获取当前系统真实的平台体系结构
    /// </summary>
    public static Architecture Architecture =>
#if WINDOWS || NETFRAMEWORK
        _OSArchitecture.Value;

    const Architecture X64 = (Architecture)1;
    const Architecture Arm = (Architecture)2;
    const Architecture Arm64 = (Architecture)3;

    static Architecture GetOSArchitecture()
    {
        ushort imageFileMachine = 0;
        try
        {
            // 通过读取系统文件 PE 头来判断 Architecture，避免在兼容模式下获取到错误的值
            var filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "regedit.exe");
            using var stream = new FileStream(filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);
            imageFileMachine = PEReader.GetImageFileMachine(stream);
        }
        catch
        {
        }
        return imageFileMachine switch
        {
            PEReader.IMAGE_FILE_MACHINE_I386 => default,
            PEReader.IMAGE_FILE_MACHINE_IA64 or
            PEReader.IMAGE_FILE_MACHINE_AMD64 => X64,
            PEReader.IMAGE_FILE_MACHINE_ARM or
            PEReader.IMAGE_FILE_MACHINE_ARMNT => Arm,
            PEReader.IMAGE_FILE_MACHINE_ARM64 => Arm64,
            _ => RuntimeInformation.OSArchitecture,
        };
    }

    static readonly Lazy<Architecture> _OSArchitecture = new(GetOSArchitecture, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// https://github.com/dotnet/corefx-tools/blob/072ec7c28ef6850da05d50faa0aee3a523a198b7/src/StackParser/PEReader.cs
    /// </summary>
    static class PEReader
    {
        static int ReadAtOffset(Stream stream, byte[] fileBytes, int offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            int bytesReadTotal = 0, bytesRead;
            do
            {
                bytesRead = stream.Read(fileBytes, bytesReadTotal, fileBytes.Length - bytesReadTotal);
                bytesReadTotal += bytesRead;
            } while (bytesReadTotal != fileBytes.Length && bytesRead != 0);
            return bytesReadTotal;
        }

        static void ReadBytesAtFileOffset(Stream stream, byte[] bytes, int fileOffset)
        {
            if (bytes.Length != ReadAtOffset(stream, bytes, fileOffset))
            {
                throw new IOException("Unable to read at 0x" + fileOffset.ToString("x") + ", 0x" + bytes.Length.ToString("x") + " bytes");
            }
        }

        static int ReadDwordAtFileOffset(Stream stream, int fileOffset)
        {
            byte[] dword = new byte[4];
            ReadBytesAtFileOffset(stream, dword, fileOffset);
            return BitConverter.ToInt32(dword, 0);
        }

        /// <summary>
        /// 从流中读取 ImageFileMachine
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ushort GetImageFileMachine(Stream stream)
        {
            var peHeaderOffset = ReadDwordAtFileOffset(stream, 0x3c);
            byte[] bytes = new byte[2];
            ReadBytesAtFileOffset(stream, bytes, peHeaderOffset + 4);
            var value = BitConverter.ToUInt16(bytes, 0);
            return value;
        }

        #region ImageFileMachine

        // https://github.com/MicrosoftDocs/win32/blob/docs/desktop-src/SysInfo/image-file-machine-constants.md

        /// <summary>
        /// Intel 386
        /// </summary>
        public const ushort IMAGE_FILE_MACHINE_I386 = 0x014c;

        /// <summary>
        /// Intel 64
        /// </summary>
        public const ushort IMAGE_FILE_MACHINE_IA64 = 0x0200;

        /// <summary>
        /// AMD64 (K8)
        /// </summary>
        public const ushort IMAGE_FILE_MACHINE_AMD64 = 0x8664;

        /// <summary>
        /// ARM Little-Endian
        /// </summary>
        public const ushort IMAGE_FILE_MACHINE_ARM = 0x01c0;

        /// <summary>
        /// ARM Thumb-2 Little-Endian
        /// </summary>
        public const ushort IMAGE_FILE_MACHINE_ARMNT = 0x01c4;

        /// <summary>
        /// ARM64 Little-Endian
        /// </summary>
        public const ushort IMAGE_FILE_MACHINE_ARM64 = 0xAA64;

        #endregion
    }
#else
        RuntimeInformation.OSArchitecture;
#endif
}
