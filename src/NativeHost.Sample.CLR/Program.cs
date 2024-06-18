using BD.Common8.NativeHost;

var dllPath = Path.Combine(ProjPath, "src", "artifacts", "bin", "AssemblyLoad.Sample.EntryPoint", "debug", "AssemblyLoad.Sample.EntryPoint.exe");
var dllBytes = File.ReadAllBytes(dllPath);

NativeHost.ExecuteInDefaultAppDomain(dllBytes);