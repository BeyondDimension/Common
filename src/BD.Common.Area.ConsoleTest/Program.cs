using BD.Common.Models;
//using MessagePack;

//string resPath = Path.Combine(Utils.ProjPath, "src", "BD.Common.Area", "Resources")!;
//ConvertMP2();

//void ConvertMP2()
//{
//    var mpo = Directory.EnumerateFiles(resPath, $"*{FileEx.MPO}");
//    var filePath = mpo.SingleOrDefault()!;
//    var value = File.ReadAllBytes(filePath);
//    var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
//    var result = MessagePackSerializer.Deserialize<Area[]>(value, lz4Options);
//    var fileNameWithoutEx = Path.GetFileNameWithoutExtension(filePath).Split('.').First();
//    var newValue = Serializable.SMP2(result);
//    File.WriteAllBytes(Path.Combine(resPath, fileNameWithoutEx), newValue);
//    //IOPath.FileTryDelete(filePath);
//}

const string type = "BD.Common.Settings.SettingsProperty<string, BD.Common.UnitTest.UISettings_>";
const string SettingsProperty = "BD.Common.Settings.SettingsProperty";
const string SettingsStructProperty = "BD.Common.Settings.SettingsStructProperty";

var a = type[(SettingsProperty.Length + 1)..^1];
//a.Split(',')
//{string[2]}
//    [0]: "string"
//    [1]: " BD.Common.UnitTest.UISettings_"
//a.Split(',')

var b = "<ThemeAccent>k__BackingField";
const string d = "k__BackingField";
var c = b.Substring(1, b.Length - d.Length - 2);

var className = "BD.Common.UnitTest.UISettings_";
className = className[..^className.LastIndexOf('.')];

var se = new BD.Common.UnitTest.UISettings_
{
    Language = "en-US",
    ThemeAccent = "aaa",
};

var json_str = JsonSerializer.Serialize(se, BD.Common.UnitTest.UISettingsContext.Instance.UISettings_);

var areas = Area.Values;
foreach (var item in areas)
{
    Console.WriteLine($"{item.Name}, {item.Id}");
}