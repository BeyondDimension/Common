using Humanizer;

const string to_ = "&to=";
const string route = "https://api.translator.azure.cn/translate?api-version=3.0&from=zh-Hans";

try
{
    ReadAzureTranslationKey();
    var cfg = GeneratorConfig.Instance;
    var dict = cfg.Translates.
        Where(x => string.IsNullOrWhiteSpace(x.Value)).
        ToDictionary(x => x.Key, y => "");
    if (dict.Any())
    {
        const string lang = "en";
        const string url = route + to_ + lang;
        foreach (var kv in dict)
        {
            try
            {
                Console.Write(kv.Key);
                Console.Write(" => ");
                var translationResults = await Translatecs.TranslateTextAsync(url, kv.Key);
                var translationResult = translationResults.
                    First(x => x != null).Translations.
                    First(x => x.To.Equals(lang, StringComparison.OrdinalIgnoreCase));
                var value = translationResult.Text.Pascalize();
                Console.WriteLine(value);
                cfg.Translates[kv.Key] = value;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                byte counter = 0;
                StringBuilder builder = new();
                do
                {
                    if (counter++ == sbyte.MaxValue) break;
                    builder.AppendLine(ex.ToString());
                    builder.AppendLine();
                    ex = ex.InnerException!;
                } while (ex != null);
                var errorString = builder.ToString();
                Console.Error.WriteLine(errorString);
                Console.Error.Flush();
            }
        }
    }
    GeneratorConfig.Save(true);
    Console.WriteLine(GeneratorConfig.FilePath.Value);
    Console.WriteLine("OK");
}
catch (Exception ex)
{
    byte counter = 0;
    StringBuilder builder = new();
    do
    {
        if (counter++ == sbyte.MaxValue) break;
        builder.AppendLine(ex.ToString());
        builder.AppendLine();
        ex = ex.InnerException!;
    } while (ex != null);
    var errorString = builder.ToString();
    Console.Error.WriteLine(errorString);
    Console.Error.Flush();
}
finally
{
    Console.ReadLine();
}

static void ReadAzureTranslationKey()
{
    if (Translatecs.Settings != null) return;
    var azure_translation_key = Path.Combine(Utils.ProjPath, "azure-translation-key.pfx");
    if (!File.Exists(azure_translation_key)) throw new FileNotFoundException(azure_translation_key);
    var text = File.ReadAllText(azure_translation_key);
    var items = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    if (items.Length != 3) throw new ArgumentOutOfRangeException();
    Translatecs.Settings = new TranslatecsSettings()
    {
        Key = items[0],
        Endpoint = items[1],
        Region = items[2],
    };
}