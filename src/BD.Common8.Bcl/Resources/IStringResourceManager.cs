namespace System.Resources;

public interface IStringResourceManager
{
    string GetString(string name, CultureInfo? culture = null);

    string[] SupportedUICultures { get; }

    string? GetCultureName(CultureInfo? culture = null);
}