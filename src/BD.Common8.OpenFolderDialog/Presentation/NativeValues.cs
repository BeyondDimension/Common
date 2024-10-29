namespace MS.Win32;

/// <summary>IFileDialog options.  FOS_*</summary>
[Flags]
internal enum FOS : uint
{
    OVERWRITEPROMPT = 0x00000002,
    STRICTFILETYPES = 0x00000004,
    NOCHANGEDIR = 0x00000008,
    PICKFOLDERS = 0x00000020,
    FORCEFILESYSTEM = 0x00000040,
    ALLNONSTORAGEITEMS = 0x00000080,
    NOVALIDATE = 0x00000100,
    ALLOWMULTISELECT = 0x00000200,
    PATHMUSTEXIST = 0x00000800,
    FILEMUSTEXIST = 0x00001000,
    CREATEPROMPT = 0x00002000,
    SHAREAWARE = 0x00004000,
    NOREADONLYRETURN = 0x00008000,
    NOTESTFILECREATE = 0x00010000,
    HIDEMRUPLACES = 0x00020000,
    HIDEPINNEDPLACES = 0x00040000,
    NODEREFERENCELINKS = 0x00100000,
    DONTADDTORECENT = 0x02000000,
    FORCESHOWHIDDEN = 0x10000000,
    DEFAULTNOMINIMODE = 0x20000000,
    FORCEPREVIEWPANEON = 0x40000000,
}

/// <summary>FDE_OVERWRITE_RESPONSE.  FDEOR_*</summary>
internal enum FDEOR
{
    DEFAULT = 0x00000000,
    ACCEPT = 0x00000001,
    REFUSE = 0x00000002,
}

/// <summary>FDE_SHAREVIOLATION_RESPONSE.  FDESVR_*</summary>
internal enum FDESVR
{
    DEFAULT = 0x00000000,
    ACCEPT = 0x00000001,
    REFUSE = 0x00000002,
}

/// <summary>FileDialog AddPlace options.  FDAP_*</summary>
internal enum FDAP : uint
{
    BOTTOM = 0x00000000,
    TOP = 0x00000001,
}

/// <summary>Flags for Known Folder APIs.  KF_FLAG_*</summary>
/// <remarks>native enum was called KNOWN_FOLDER_FLAG</remarks>
[Flags]
internal enum KF_FLAG : uint
{
    DEFAULT = 0x00000000,

    // Make sure that the folder already exists or create it and apply security specified in folder definition
    // If folder can not be created then function will return failure and no folder path (IDList) will be returned
    // If folder is located on the network the function may take long time to execute
    CREATE = 0x00008000,

    // If this flag is specified then the folder path is returned and no verification is performed
    // Use this flag is you want to get folder's path (IDList) and do not need to verify folder's existence
    //
    // If this flag is NOT specified then Known Folder API will try to verify that the folder exists
    //     If folder does not exist or can not be accessed then function will return failure and no folder path (IDList) will be returned
    //     If folder is located on the network the function may take long time to execute
    DONT_VERIFY = 0x00004000,

    // Set folder path as is and do not try to substitute parts of the path with environments variables.
    // If flag is not specified then Known Folder will try to replace parts of the path with some
    // known environment variables (%USERPROFILE%, %APPDATA% etc.)
    DONT_UNEXPAND = 0x00002000,

    // Get file system based IDList if available. If the flag is not specified the Known Folder API
    // will try to return aliased IDList by default. Example for FOLDERID_Documents -
    // Aliased - [desktop]\[user]\[Documents] - exact location is determined by shell namespace layout and might change
    // Non aliased - [desktop]\[computer]\[disk_c]\[users]\[user]\[Documents] - location is determined by folder location in the file system
    NO_ALIAS = 0x00001000,

    // Initialize the folder with desktop.ini settings
    // If folder can not be initialized then function will return failure and no folder path will be returned
    // If folder is located on the network the function may take long time to execute
    INIT = 0x00000800,

    // Get the default path, will also verify folder existence unless KF_FLAG_DONT_VERIFY is also specified
    DEFAULT_PATH = 0x00000400,

    // Get the not-parent-relative default path. Only valid with KF_FLAG_DEFAULT_PATH
    NOT_PARENT_RELATIVE = 0x00000200,

    // Build simple IDList
    SIMPLE_IDLIST = 0x00000100,

    // only return the aliased IDLists, don't fallback to file system path
    ALIAS_ONLY = 0x80000000,
}