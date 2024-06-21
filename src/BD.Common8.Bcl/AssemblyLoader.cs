//namespace Costura;

//static class AssemblyLoader
//{
//    static readonly object nullCacheLock = new();

//    static readonly Dictionary<string, bool> nullCache = new();

//    static readonly Dictionary<string, string> assemblyNames = new();

//    static readonly Dictionary<string, string> symbolNames = new();

//    static int isAttached;

//    static string CultureToString(CultureInfo? culture)
//    {
//        if (culture == null)
//        {
//            return "";
//        }
//        return culture.Name;
//    }

//    static Assembly? ReadExistingAssembly(AssemblyName name)
//    {
//        var currentDomain = AppDomain.CurrentDomain;
//        var assemblies = currentDomain.GetAssemblies();
//        var array = assemblies;
//        foreach (var assembly in array)
//        {
//            var name2 = assembly.GetName();
//            if (string.Equals(name2.Name, name.Name, StringComparison.InvariantCultureIgnoreCase) &&
//                string.Equals(CultureToString(name2.CultureInfo), CultureToString(name.CultureInfo), StringComparison.InvariantCultureIgnoreCase))
//            {
//                return assembly;
//            }
//        }
//        return null;
//    }

//    static Stream LoadStream(string fullName)
//    {
//        Assembly executingAssembly = Assembly.GetExecutingAssembly();
//        if (fullName.EndsWith(".compressed"))
//        {
//            using Stream stream = executingAssembly.GetManifestResourceStream(fullName)!;
//            using BrotliStream source = new(stream, CompressionMode.Decompress);
//            MemoryStream memoryStream = new();
//            source.CopyTo(memoryStream);
//            memoryStream.Position = 0L;
//            return memoryStream;
//        }
//        return executingAssembly.GetManifestResourceStream(fullName)!;
//    }

//    static Stream? LoadStream(Dictionary<string, string> resourceNames, string name)
//    {
//        if (resourceNames.TryGetValue(name, out var value))
//        {
//            return LoadStream(value);
//        }
//        return null;
//    }

//    [RequiresUnreferencedCode("Types and members the loaded assembly depends on might be removed")]
//    static Assembly? ReadFromEmbeddedResources(Dictionary<string, string> assemblyNames, Dictionary<string, string> symbolNames, AssemblyName requestedAssemblyName)
//    {
//        if (requestedAssemblyName.Name == null)
//            return null;
//        var text = requestedAssemblyName.Name.ToLowerInvariant();
//        if (requestedAssemblyName.CultureInfo != null && !string.IsNullOrEmpty(requestedAssemblyName.CultureInfo.Name))
//        {
//            text = requestedAssemblyName.CultureInfo.Name + "." + text;
//        }
//        byte[] rawAssembly;
//        using (var stream = LoadStream(assemblyNames, text))
//        {
//            if (stream == null)
//            {
//                return null;
//            }
//            rawAssembly = stream.ToByteArray();
//        }
//        using (var stream2 = LoadStream(symbolNames, text))
//        {
//            if (stream2 != null)
//            {
//                byte[] rawSymbolStore = stream2.ToByteArray();
//                return Assembly.Load(rawAssembly, rawSymbolStore);
//            }
//        }
//        return Assembly.Load(rawAssembly);
//    }

//    static AssemblyLoader()
//    {
//        assemblyNames.Add("newtonsoft.json", "costura.newtonsoft.json.dll");
//    }

//    [RequiresUnreferencedCode("Types and members the loaded assembly depends on might be removed")]
//    public static void Attach()
//    {
//        if (Interlocked.Exchange(ref isAttached, 1) == 1)
//        {
//            return;
//        }
//        var currentDomain = AppDomain.CurrentDomain;
//        currentDomain.AssemblyResolve += (sender, e) =>
//        {
//            lock (nullCacheLock)
//            {
//                if (nullCache.ContainsKey(e.Name))
//                {
//                    return null;
//                }
//            }
//            AssemblyName assemblyName = new(e.Name);
//            var assembly = ReadExistingAssembly(assemblyName);
//            if (assembly != null)
//            {
//                return assembly;
//            }
//            assembly = ReadFromEmbeddedResources(assemblyNames, symbolNames, assemblyName);
//            if (assembly == null)
//            {
//                lock (nullCacheLock)
//                {
//                    nullCache[e.Name] = true;
//                }
//                if ((assemblyName.Flags & AssemblyNameFlags.Retargetable) != 0)
//                {
//                    assembly = Assembly.Load(assemblyName);
//                }
//            }
//            return assembly;
//        };
//    }
//}
