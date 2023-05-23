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

var areas = Area.Values;
foreach (var item in areas)
{
    Console.WriteLine($"{item.Name}, {item.Id}");
}