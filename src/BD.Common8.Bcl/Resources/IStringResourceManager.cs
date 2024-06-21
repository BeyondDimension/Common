namespace System.Resources;

public interface IStringResourceManager
{
    string GetString(string name, CultureInfo? culture = null);

    IReadOnlyList<string> SupportedUICultures { get; }

    string? GetCultureName(CultureInfo? culture = null);
}