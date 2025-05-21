namespace Tools.Build;

static partial class Constants // nuget.org
{
    internal const string? token_nuget = null;

    internal static readonly DateTimeOffset? token_nuget_expire = null;

    internal const string? url_nuget_push_nuget = "https://api.nuget.org/v3/index.json";
}

static partial class Constants // github.com
{
    internal const string? token_github = null;

    internal static readonly DateTimeOffset? token_github_expire = null;

    internal const string? url_nuget_push_github = "https://nuget.pkg.github.com/BeyondDimension";
}

static partial class Constants // SPP.ApiService.BaGet
{
    internal const string? token_local = null;

    internal static readonly DateTimeOffset? token_local_expire = null;

    internal const string? url_nuget_push_local = null;
}