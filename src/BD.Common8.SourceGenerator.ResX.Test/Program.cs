#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

using BD.Common8.Resources;
using static BD.Common8.SourceGenerator.ResX.Test.Helpers.ResXHelper;

SR.Culture = new CultureInfo("en");
Console.WriteLine(SR.DayOfWeek_S1);
SR.Culture = new CultureInfo("es");
Console.WriteLine(SR.DayOfWeek_S1);
SR.Culture = new CultureInfo("ru");
Console.WriteLine(SR.DayOfWeek_S1);
SR.Culture = new CultureInfo("ja");
Console.WriteLine(SR.DayOfWeek_S1);
SR.Culture = new CultureInfo("zh-CN");
Console.WriteLine(SR.DayOfWeek_S1);
SR.Culture = new CultureInfo("zh-HK");
Console.WriteLine(SR.DayOfWeek_S1);

var i18nPath = Path.Combine(ProjPath, "res", "i18n");
var values = RemoveResXCommentSatelliteAssemblies(i18nPath);
WriteProps(i18nPath, values);

Console.WriteLine("OK");
Console.ReadLine();
