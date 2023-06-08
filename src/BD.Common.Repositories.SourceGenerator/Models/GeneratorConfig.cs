namespace BD.Common.Repositories.SourceGenerator.Models;

public sealed record class GeneratorConfig(ImmutableDictionary<string, string> Translates)
{
    const string fileName = "GeneratorConfig.Repositories.json";

    static readonly Lazy<GeneratorConfig> instance = new(() =>
    {
        var projPath = ProjPathHelper.GetProjPath(null);
        using var stream = new FileStream(
            Path.Combine(projPath, "src", fileName),
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);
        var generatorConfig = JsonSerializer.Deserialize<GeneratorConfig>(stream);
        if (generatorConfig == null)
            throw new ArgumentNullException(nameof(generatorConfig));
        return generatorConfig;
    });

    public static GeneratorConfig Instance => instance.Value;

    public static string Translate(string input)
    {
        if (Instance.Translates.TryGetValue(input, out var result))
            return result;
        return input;
    }
}
