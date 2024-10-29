#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1604 // Element documentation should have summary
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable CS8603 // 可能返回 null 引用。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
#pragma warning disable CS8600 // 将 null 文本或可能的 null 值转换为不可为 null 类型。

namespace MS.Win32;

using System.IO;
using System.Runtime.InteropServices.ComTypes;

/// <summary>
/// ShellItem enum.  SIGDN_*.
/// </summary>
internal enum SIGDN : uint
{ // lower word (& with 0xFFFF)
    NORMALDISPLAY = 0x00000000, // SHGDN_NORMAL
    PARENTRELATIVEPARSING = 0x80018001, // SHGDN_INFOLDER | SHGDN_FORPARSING
    DESKTOPABSOLUTEPARSING = 0x80028000, // SHGDN_FORPARSING
    PARENTRELATIVEEDITING = 0x80031001, // SHGDN_INFOLDER | SHGDN_FOREDITING
    DESKTOPABSOLUTEEDITING = 0x8004c000, // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
    FILESYSPATH = 0x80058000, // SHGDN_FORPARSING
    URL = 0x80068000, // SHGDN_FORPARSING
    PARENTRELATIVEFORADDRESSBAR = 0x8007c001, // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
    PARENTRELATIVE = 0x80080001, // SHGDN_INFOLDER
}

// IShellFolder::GetAttributesOf flags
[Flags]
internal enum SFGAO : uint
{
    /// <summary>Objects can be copied</summary>
    /// <remarks>DROPEFFECT_COPY</remarks>
    CANCOPY = 0x1,

    /// <summary>Objects can be moved</summary>
    /// <remarks>DROPEFFECT_MOVE</remarks>
    CANMOVE = 0x2,

    /// <summary>Objects can be linked</summary>
    /// <remarks>
    /// DROPEFFECT_LINK.
    ///
    /// If this bit is set on an item in the shell folder, a
    /// 'Create Shortcut' menu item will be added to the File
    /// menu and context menus for the item.  If the user selects
    /// that command, your IContextMenu::InvokeCommand() will be called
    /// with 'link'.
    /// That flag will also be used to determine if 'Create Shortcut'
    /// should be added when the item in your folder is dragged to another
    /// folder.
    /// </remarks>
    CANLINK = 0x4,

    /// <summary>supports BindToObject(IID_IStorage)</summary>
    STORAGE = 0x00000008,

    /// <summary>Objects can be renamed</summary>
    CANRENAME = 0x00000010,

    /// <summary>Objects can be deleted</summary>
    CANDELETE = 0x00000020,

    /// <summary>Objects have property sheets</summary>
    HASPROPSHEET = 0x00000040,

    // unused = 0x00000080,

    /// <summary>Objects are drop target</summary>
    DROPTARGET = 0x00000100,

    CAPABILITYMASK = 0x00000177,

    // unused = 0x00000200,
    // unused = 0x00000400,
    // unused = 0x00000800,
    // unused = 0x00001000,

    /// <summary>Object is encrypted (use alt color)</summary>
    ENCRYPTED = 0x00002000,

    /// <summary>'Slow' object</summary>
    ISSLOW = 0x00004000,

    /// <summary>Ghosted icon</summary>
    GHOSTED = 0x00008000,

    /// <summary>Shortcut (link)</summary>
    LINK = 0x00010000,

    /// <summary>Shared</summary>
    SHARE = 0x00020000,

    /// <summary>Read-only</summary>
    READONLY = 0x00040000,

    /// <summary> Hidden object</summary>
    HIDDEN = 0x00080000,

    DISPLAYATTRMASK = 0x000FC000,

    /// <summary> May contain children with SFGAO_FILESYSTEM</summary>
    FILESYSANCESTOR = 0x10000000,

    /// <summary>Support BindToObject(IID_IShellFolder)</summary>
    FOLDER = 0x20000000,

    /// <summary>Is a win32 file system object (file/folder/root)</summary>
    FILESYSTEM = 0x40000000,

    /// <summary>May contain children with SFGAO_FOLDER (may be slow)</summary>
    HASSUBFOLDER = 0x80000000,

    CONTENTSMASK = 0x80000000,

    /// <summary>Invalidate cached information (may be slow)</summary>
    VALIDATE = 0x01000000,

    /// <summary>Is this removeable media?</summary>
    REMOVABLE = 0x02000000,

    /// <summary> Object is compressed (use alt color)</summary>
    COMPRESSED = 0x04000000,

    /// <summary>Supports IShellFolder, but only implements CreateViewObject() (non-folder view)</summary>
    BROWSABLE = 0x08000000,

    /// <summary>Is a non-enumerated object (should be hidden)</summary>
    NONENUMERATED = 0x00100000,

    /// <summary>Should show bold in explorer tree</summary>
    NEWCONTENT = 0x00200000,

    /// <summary>Obsolete</summary>
    CANMONIKER = 0x00400000,

    /// <summary>Obsolete</summary>
    HASSTORAGE = 0x00400000,

    /// <summary>Supports BindToObject(IID_IStream)</summary>
    STREAM = 0x00400000,

    /// <summary>May contain children with SFGAO_STORAGE or SFGAO_STREAM</summary>
    STORAGEANCESTOR = 0x00800000,

    /// <summary>For determining storage capabilities, ie for open/save semantics</summary>
    STORAGECAPMASK = 0x70C50008,

    /// <summary>
    /// Attributes that are masked out for PKEY_SFGAOFlags because they are considered
    /// to cause slow calculations or lack context
    /// (SFGAO_VALIDATE | SFGAO_ISSLOW | SFGAO_HASSUBFOLDER and others)
    /// </summary>
    PKEYSFGAOMASK = 0x81044000,
}

/// <summary>
/// SHELLITEMCOMPAREHINTF.  SICHINT_*.
/// </summary>
internal enum SICHINT : uint
{
    /// <summary>iOrder based on display in a folder view</summary>
    DISPLAY = 0x00000000,

    /// <summary>exact instance compare</summary>
    ALLFIELDS = 0x80000000,

    /// <summary>iOrder based on canonical name (better performance)</summary>
    CANONICAL = 0x10000000,

    TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
}

[
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid(IID.ShellItem),
]
internal interface IShellItem
{
    [return: MarshalAs(UnmanagedType.Interface)]
    object BindToHandler(IBindCtx pbc, [In] ref Guid bhid, [In] ref Guid riid);

    IShellItem GetParent();

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetDisplayName(SIGDN sigdnName);

    uint GetAttributes(SFGAO sfgaoMask);

    int Compare(IShellItem psi, SICHINT hint);
}

[
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid(IID.FileDialogEvents),
]
internal interface IFileDialogEvents
{
    [PreserveSig]
    HRESULT OnFileOk(IFileDialog pfd);

    [PreserveSig]
    HRESULT OnFolderChanging(IFileDialog pfd, IShellItem psiFolder);

    [PreserveSig]
    HRESULT OnFolderChange(IFileDialog pfd);

    [PreserveSig]
    HRESULT OnSelectionChange(IFileDialog pfd);

    [PreserveSig]
    HRESULT OnShareViolation(IFileDialog pfd, IShellItem psi, out FDESVR pResponse);

    [PreserveSig]
    HRESULT OnTypeChange(IFileDialog pfd);

    [PreserveSig]
    HRESULT OnOverwrite(IFileDialog pfd, IShellItem psi, out FDEOR pResponse);
}

[
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid(IID.ModalWindow),
]
internal interface IModalWindow
{
    [PreserveSig]
    HRESULT Show(IntPtr parent);
}

[
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid(IID.FileDialog),
]
internal interface IFileDialog : IModalWindow
{
    #region IModalWindow redeclarations

    [PreserveSig]
    new HRESULT Show(IntPtr parent);

    #endregion IModalWindow redeclarations

    void SetFileTypes(uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] COMDLG_FILTERSPEC[] rgFilterSpec);

    void SetFileTypeIndex(uint iFileType);

    uint GetFileTypeIndex();

    uint Advise(IFileDialogEvents pfde);

    void Unadvise(uint dwCookie);

    void SetOptions(FOS fos);

    FOS GetOptions();

    void SetDefaultFolder(IShellItem psi);

    void SetFolder(IShellItem psi);

    IShellItem GetFolder();

    IShellItem GetCurrentSelection();

    void SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetFileName();

    void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

    void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);

    void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

    IShellItem GetResult();

    void AddPlace(IShellItem psi, FDAP alignment);

    void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

    void Close([MarshalAs(UnmanagedType.Error)] int hr);

    void SetClientGuid([In] ref Guid guid);

    void ClearClientData();

    void SetFilter([MarshalAs(UnmanagedType.Interface)] object pFilter);
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct COMDLG_FILTERSPEC
{
    [MarshalAs(UnmanagedType.LPWStr)]
    public string pszName;

    [MarshalAs(UnmanagedType.LPWStr)]
    public string pszSpec;
}

/// <remarks>
/// Methods in this class will only work on Vista and above.
/// </remarks>
internal static class ShellUtil
{
    public static string GetPathFromShellItem(IShellItem item)
    {
        return item.GetDisplayName(SIGDN.DESKTOPABSOLUTEPARSING);
    }

    public static string GetPathForKnownFolder(Guid knownFolder)
    {
        if (knownFolder == default(Guid))
        {
            return null;
        }

        var pathBuilder = new StringBuilder(260); // NativeMethods.MAX_PATH
        HRESULT hr = NativeMethods2.SHGetFolderPathEx(ref knownFolder, 0, IntPtr.Zero, pathBuilder, (uint)pathBuilder.Capacity);
        // If we failed to find a path for the known folder then just ignore it.
        return hr.Succeeded
            ? pathBuilder.ToString()
            : null;
    }

    public static IShellItem2 GetShellItemForPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            // Internal function.  Should have verified this before calling if we cared.
            return null;
        }

        Guid iidShellItem2 = new Guid(IID.ShellItem2);
        object unk;
        HRESULT hr = NativeMethods2.SHCreateItemFromParsingName(path, null, ref iidShellItem2, out unk);

        // Silently absorb errors such as ERROR_FILE_NOT_FOUND, ERROR_PATH_NOT_FOUND.
        // Let others pass through
        if (hr == (HRESULT)Win32Error.ERROR_FILE_NOT_FOUND || hr == (HRESULT)Win32Error.ERROR_PATH_NOT_FOUND)
        {
            hr = HRESULT.S_OK;
            unk = null;
        }

        hr.ThrowIfFailed();

        return (IShellItem2)unk;
    }
}

/// <summary>
/// Shell Namespace helper 2
/// </summary>
[
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid(IID.ShellItem2),
]
interface IShellItem2 : IShellItem
{
    #region IShellItem redeclarations

    [return: MarshalAs(UnmanagedType.Interface)]
    new object BindToHandler(IBindCtx pbc, [In] ref Guid bhid, [In] ref Guid riid);

    new IShellItem GetParent();

    [return: MarshalAs(UnmanagedType.LPWStr)]
    new string GetDisplayName(SIGDN sigdnName);

    new SFGAO GetAttributes(SFGAO sfgaoMask);

    new int Compare(IShellItem psi, SICHINT hint);

    #endregion IShellItem redeclarations

    [return: MarshalAs(UnmanagedType.Interface)]
    object GetPropertyStore(
        GPS flags,
        [In] ref Guid riid);

    [return: MarshalAs(UnmanagedType.Interface)]
    object GetPropertyStoreWithCreateObject(
        GPS flags,
        [MarshalAs(UnmanagedType.IUnknown)] object punkCreateObject,   // factory for low-rights creation of type ICreateObject
        [In] ref Guid riid);

    [return: MarshalAs(UnmanagedType.Interface)]
    object GetPropertyStoreForKeys(
        IntPtr rgKeys,
        uint cKeys,
        GPS flags,
        [In] ref Guid riid);

    [return: MarshalAs(UnmanagedType.Interface)]
    object GetPropertyDescriptionList(
        IntPtr keyType,
        [In] ref Guid riid);

    // Ensures any cached information in this item is up to date, or returns __HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND) if the item does not exist.
    void Update(IBindCtx pbc);

    //void GetProperty(IntPtr key, [In, Out] PROPVARIANT pv);

    Guid GetCLSID(IntPtr key);

    FILETIME GetFileTime(IntPtr key);

    int GetInt32(IntPtr key);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetString(IntPtr key);

    uint GetUInt32(IntPtr key);

    ulong GetUInt64(IntPtr key);

    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetBool(IntPtr key);
}

public struct FILETIME
{
    public int dwLowDateTime;

    public int dwHighDateTime;
}

/// <summary>
/// GetPropertyStoreFlags.  GPS_*.
/// </summary>
/// <remarks>
/// These are new for Vista, but are used in downlevel components
/// </remarks>
internal enum GPS
{
    // If no flags are specified (GPS_DEFAULT), a read-only property store is returned that includes properties for the file or item.
    // In the case that the shell item is a file, the property store contains:
    //     1. properties about the file from the file system
    //     2. properties from the file itself provided by the file's property handler, unless that file is offline,
    //         see GPS_OPENSLOWITEM
    //     3. if requested by the file's property handler and supported by the file system, properties stored in the
    //     alternate property store.
    //
    // Non-file shell items should return a similar read-only store
    //
    // Specifying other GPS_ flags modifies the store that is returned
    DEFAULT = 0x00000000,

    HANDLERPROPERTIESONLY = 0x00000001,   // only include properties directly from the file's property handler
    READWRITE = 0x00000002,   // Writable stores will only include handler properties
    TEMPORARY = 0x00000004,   // A read/write store that only holds properties for the lifetime of the IShellItem object
    FASTPROPERTIESONLY = 0x00000008,   // do not include any properties from the file's property handler (because the file's property handler will hit the disk)
    OPENSLOWITEM = 0x00000010,   // include properties from a file's property handler, even if it means retrieving the file from offline storage.
    DELAYCREATION = 0x00000020,   // delay the creation of the file's property handler until those properties are read, written, or enumerated
    BESTEFFORT = 0x00000040,   // For readonly stores, succeed and return all available properties, even if one or more sources of properties fails. Not valid with GPS_READWRITE.
    NO_OPLOCK = 0x00000080,   // some data sources protect the read property store with an oplock, this disables that
    MASK_VALID = 0x000000FF,
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[BestFitMapping(false)]
internal class WIN32_FIND_DATAW
{
    public FileAttributes dwFileAttributes;
    public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
    public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
    public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
    public int nFileSizeHigh;
    public int nFileSizeLow;
    public int dwReserved0;
    public int dwReserved1;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string cFileName = null!;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
    public string cAlternateFileName = null!;
}

[Flags]
enum SLGP
{
    SHORTPATH = 0x1,
    UNCPRIORITY = 0x2,
    RAWPATH = 0x4
}

[
    ComImport,
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
    Guid(IID.ShellLink),
]
internal interface IShellLinkW
{
    void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, [In, Out] WIN32_FIND_DATAW pfd, SLGP fFlags);

    IntPtr GetIDList();

    void SetIDList(IntPtr pidl);

    void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxName);

    void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

    void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

    void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

    void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

    void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

    short GetHotKey();

    void SetHotKey(short wHotKey);

    uint GetShowCmd();

    void SetShowCmd(uint iShowCmd);

    void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

    void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

    void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

    void Resolve(IntPtr hwnd, uint fFlags);

    void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
}

internal static class NativeMethods2
{
    internal static class ExternDll
    {
        public const string Shell32 = "shell32.dll";
    }

    /// <summary>
    /// SHAddToRecentDocuments flags.  SHARD_*
    /// </summary>
    internal enum SHARD
    {
        PIDL = 0x00000001,
        PATHA = 0x00000002,
        PATHW = 0x00000003,
        APPIDINFO = 0x00000004, // indicates the data type is a pointer to a SHARDAPPIDINFO structure
        APPIDINFOIDLIST = 0x00000005, // indicates the data type is a pointer to a SHARDAPPIDINFOIDLIST structure
        LINK = 0x00000006, // indicates the data type is a pointer to an IShellLink instance
        APPIDINFOLINK = 0x00000007, // indicates the data type is a pointer to a SHARDAPPIDINFOLINK structure
    }

    [DllImport(ExternDll.Shell32, EntryPoint = "SHAddToRecentDocs")]
    private static extern void SHAddToRecentDocsString(SHARD uFlags, [MarshalAs(UnmanagedType.LPWStr)] string pv);

    // This overload is required.  There's a cast in the Shell code that causes the wrong vtbl to be used
    // if we let the marshaller convert the parameter to an IUnknown.
    [DllImport(ExternDll.Shell32, EntryPoint = "SHAddToRecentDocs")]
    private static extern void SHAddToRecentDocs_ShellLink(SHARD uFlags, IShellLinkW pv);

    internal static void SHAddToRecentDocs(string path)
    {
        SHAddToRecentDocsString(SHARD.PATHW, path);
    }

    // Win7 only.
    internal static void SHAddToRecentDocs(IShellLinkW shellLink)
    {
        SHAddToRecentDocs_ShellLink(SHARD.LINK, shellLink);
    }

    // Vista only
    [DllImport(ExternDll.Shell32)]
    internal static extern HRESULT SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);

    // Vista only.  Also inconsistently doced on MSDN.  It was available in some versions of the SDK, and it mentioned on several pages, but isn't specifically doced.
    [DllImport(ExternDll.Shell32)]
    internal static extern HRESULT SHGetFolderPathEx([In] ref Guid rfid, KF_FLAG dwFlags, [In, Optional] IntPtr hToken, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath, uint cchPath);

    /// <summary>
    /// Sets the User Model AppID for the current process, enabling Windows to retrieve this ID
    /// </summary>
    /// <param name="AppID"></param>
    [DllImport(ExternDll.Shell32, PreserveSig = false)]
    internal static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

    /// <summary>
    /// Retrieves the User Model AppID that has been explicitly set for the current process via SetCurrentProcessExplicitAppUserModelID
    /// </summary>
    /// <param name="AppID"></param>
    [DllImport(ExternDll.Shell32)]
    internal static extern HRESULT GetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] out string AppID);
}

[
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid(IID.FileOpenDialog),
]
internal interface IFileOpenDialog : IFileDialog
{
    #region IFileDialog redeclarations

    #region IModalDialog redeclarations

    [PreserveSig]
    new HRESULT Show(IntPtr parent);

    #endregion IModalDialog redeclarations

    new void SetFileTypes(uint cFileTypes, [In] COMDLG_FILTERSPEC[] rgFilterSpec);

    new void SetFileTypeIndex(uint iFileType);

    new uint GetFileTypeIndex();

    new uint Advise(IFileDialogEvents pfde);

    new void Unadvise(uint dwCookie);

    new void SetOptions(FOS fos);

    new FOS GetOptions();

    new void SetDefaultFolder(IShellItem psi);

    new void SetFolder(IShellItem psi);

    new IShellItem GetFolder();

    new IShellItem GetCurrentSelection();

    new void SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    new void GetFileName();

    new void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

    new void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);

    new void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

    new IShellItem GetResult();

    new void AddPlace(IShellItem psi, FDAP fdcp);

    new void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

    new void Close([MarshalAs(UnmanagedType.Error)] int hr);

    new void SetClientGuid([In] ref Guid guid);

    new void ClearClientData();

    new void SetFilter([MarshalAs(UnmanagedType.Interface)] object pFilter);

    #endregion IFileDialog redeclarations

    IShellItemArray GetResults();

    IShellItemArray GetSelectedItems();
}

[
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid(IID.ShellItemArray),
]
internal interface IShellItemArray
{
    [return: MarshalAs(UnmanagedType.Interface)]
    object BindToHandler(IBindCtx pbc, [In] ref Guid rbhid, [In] ref Guid riid);

    [return: MarshalAs(UnmanagedType.Interface)]
    object GetPropertyStore(int flags, [In] ref Guid riid);

    [return: MarshalAs(UnmanagedType.Interface)]
    object GetPropertyDescriptionList([In] ref PKEY keyType, [In] ref Guid riid);

    uint GetAttributes(SIATTRIBFLAGS dwAttribFlags, uint sfgaoMask);

    uint GetCount();

    IShellItem GetItemAt(uint dwIndex);

    [return: MarshalAs(UnmanagedType.Interface)]
    object EnumItems();
}

/// <summary>ShellItem attribute flags.  SIATTRIBFLAGS_*</summary>
internal enum SIATTRIBFLAGS
{
    AND = 0x00000001,
    OR = 0x00000002,
    APPCOMPAT = 0x00000003,
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct PKEY
{
    /// <summary>fmtid</summary>
    private readonly Guid _fmtid;

    /// <summary>pid</summary>
    private readonly uint _pid;

    private PKEY(Guid fmtid, uint pid)
    {
        _fmtid = fmtid;
        _pid = pid;
    }

    /// <summary>PKEY_Title</summary>
    public static readonly PKEY Title = new PKEY(new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9"), 2);

    /// <summary>PKEY_AppUserModel_ID</summary>
    public static readonly PKEY AppUserModel_ID = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);

    /// <summary>PKEY_AppUserModel_IsDestListSeparator</summary>
    public static readonly PKEY AppUserModel_IsDestListSeparator = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 6);

    /// <summary>PKEY_AppUserModel_RelaunchCommand</summary>
    public static readonly PKEY AppUserModel_RelaunchCommand = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 2);

    /// <summary>PKEY_AppUserModel_RelaunchDisplayNameResource</summary>
    public static readonly PKEY AppUserModel_RelaunchDisplayNameResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 4);

    /// <summary>PKEY_AppUserModel_RelaunchIconResource</summary>
    public static readonly PKEY AppUserModel_RelaunchIconResource = new PKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 3);
}

[
    ComImport,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid(IID.FileDialog2),
]
internal interface IFileDialog2 : IFileDialog
{
    #region IFileDialog redeclarations

    #region IModalWindow redeclarations

    [PreserveSig]
    new HRESULT Show(IntPtr parent);

    #endregion IModalWindow redeclarations

    new void SetFileTypes(uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] COMDLG_FILTERSPEC[] rgFilterSpec);

    new void SetFileTypeIndex(uint iFileType);

    new uint GetFileTypeIndex();

    new uint Advise(IFileDialogEvents pfde);

    new void Unadvise(uint dwCookie);

    new void SetOptions(FOS fos);

    new FOS GetOptions();

    new void SetDefaultFolder(IShellItem psi);

    new void SetFolder(IShellItem psi);

    new IShellItem GetFolder();

    new IShellItem GetCurrentSelection();

    new void SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);

    [return: MarshalAs(UnmanagedType.LPWStr)]
    new string GetFileName();

    new void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

    new void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);

    new void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

    new IShellItem GetResult();

    new void AddPlace(IShellItem psi, FDAP alignment);

    new void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

    new void Close([MarshalAs(UnmanagedType.Error)] int hr);

    new void SetClientGuid([In] ref Guid guid);

    new void ClearClientData();

    new void SetFilter([MarshalAs(UnmanagedType.Interface)] object pFilter);

    #endregion IFileDialog redeclarations

    void SetCancelButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

    void SetNavigationRoot(IShellItem psi);
}