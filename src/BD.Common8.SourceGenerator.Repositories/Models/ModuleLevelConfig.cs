using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace BD.Common8.SourceGenerator.Repositories.Models;

#pragma warning disable RS1035 // 不要使用禁用于分析器的 API

public sealed record class ModuleLevelConfig(Dictionary<string, string> RelativeSourcePath)
{
    public const string ModuleConfigName = "ModuleLevelConfig.json";
}

public static class GeneratorConfigExtensions
{
    public static ModuleLevelConfig? GetModuleLevelConfig(this GeneratorConfig generatorConfig, ISymbol symbol)
    {
        if (generatorConfig == null)
            throw new ArgumentNullException(nameof(generatorConfig));

        var moduleFolder = Path.GetDirectoryName(symbol.Locations.FirstOrDefault()?.SourceTree?.FilePath);
        var configPath = Path.Combine(moduleFolder, ModuleLevelConfig.ModuleConfigName);

        if (!File.Exists(configPath))
            return null;

        using var stream = new FileStream(
            configPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);

        using var streamReader = new StreamReader(stream, Encoding.UTF8);
        using var jsonTextReader = new JsonTextReader(streamReader);
        var moduleConfig = JsonSerializer.CreateDefault().Deserialize<ModuleLevelConfig>(jsonTextReader);

        return moduleConfig;
    }
}