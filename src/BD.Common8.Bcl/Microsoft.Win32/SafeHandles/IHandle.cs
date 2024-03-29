// https://github.com/dahall/Vanara/blob/v3.4.17/PInvoke/Shared/Handles/IHandle.cs

#if WINDOWS7_0_OR_GREATER
namespace Microsoft.Win32.SafeHandles
{
    ///// <summary>Signals that a structure or class holds a handle to a graphics object.</summary>
    //public interface IGraphicsObjectHandle : IUserHandle { }

    /// <summary>Signals that a structure or class holds a HANDLE.</summary>
    public interface IDangerousGetHandle
    {
        /// <summary>Returns the value of the handle field.</summary>
        /// <returns>An IntPtr representing the value of the handle field.</returns>
        IntPtr DangerousGetHandle();
    }

    ///// <summary>Signals that a structure or class holds a handle to a kernel object.</summary>
    //public interface IKernelHandle : IHandle { }

    ///// <summary>Signals that a structure or class holds a pointer to a security object.</summary>
    //public interface ISecurityObject : IHandle { }

    ///// <summary>Signals that a structure or class holds a handle to a shell object.</summary>
    //public interface IShellHandle : IHandle { }

    ///// <summary>Signals that a structure or class holds a handle to a synchronization object.</summary>
    //public interface ISyncHandle : IKernelHandle { }

    ///// <summary>Signals that a structure or class holds a handle to a user object.</summary>
    //public interface IUserHandle : IHandle { }
}

namespace Windows.Win32.Networking.WinHttp
{
    /// <summary>
    /// Provides a <see cref="SafeHandle"/> for <see cref="nint"/> that is disposed using <see cref="PInvoke.WinHttpCloseHandle(void*)"/>.
    /// <para>https://github.com/dahall/Vanara/blob/v3.4.17/PInvoke/WinHTTP/WinHTTP.Structs.cs#L1527-L1547</para>
    /// </summary>
    /// <remarks>Initializes a new instance of the <see cref="SafeHINTERNET"/> class and assigns an existing handle.</remarks>
    /// <param name="preexistingHandle">An <see cref="IntPtr"/> object that represents the pre-existing handle to use.</param>
    /// <param name="ownsHandle">
    /// <see langword="true"/> to reliably release the handle during the finalization phase; otherwise, <see langword="false"/> (not recommended).
    /// </param>
    public partial class SafeHINTERNET(IntPtr preexistingHandle, bool ownsHandle = true) : SafeHANDLE(preexistingHandle, ownsHandle)
    {
        /// <summary>Performs an implicit conversion from <see cref="SafeHINTERNET"/> to <see cref="nint"/>.</summary>
        /// <param name="h">The safe handle instance.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator nint(SafeHINTERNET h) => h.handle;

        public static unsafe implicit operator SafeHINTERNET(void* h) => new((nint)h);

        public static unsafe implicit operator void*(SafeHINTERNET h) => (void*)h.handle;

        /// <inheritdoc/>
        protected unsafe override bool InternalReleaseHandle()
            => PInvoke.WinHttpCloseHandle(this);
    }
}
#endif