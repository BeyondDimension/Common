namespace BD.Common8.Essentials.Enums;

/// <summary>
/// Various states of the connection to the internet.
/// </summary>
public enum NetworkAccess : byte
{
    /// <summary>The state of the connectivity is not known.</summary>
    Unknown = 0,

    /// <summary>No connectivity.</summary>
    None = 1,

    /// <summary>Local network access only.</summary>
    Local = 2,

    /// <summary>Limited internet access.</summary>
    ConstrainedInternet = 3,

    /// <summary>Local and Internet access.</summary>
    Internet = 4,
}