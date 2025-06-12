using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using System.Extensions;
using System.Reflection;
using System.Runtime.Versioning;

namespace BD.Common8.UnitTest;

sealed class TFMRefTest
{
    static void TestAssembly(params IEnumerable<Type> types)
    {
        foreach (var it in types)
        {
            TestAssembly(it.Assembly);
        }
    }

    static void TestAssembly(Assembly assembly)
    {
        var targetPlatform = assembly.GetRequiredCustomAttribute<TargetPlatformAttribute>();
        if (!targetPlatform.PlatformName.StartsWith("Windows"))
        {
            throw new Exception("error TargetPlatformAttribute");
        }

        var supportedOSPlatform = assembly.GetRequiredCustomAttribute<SupportedOSPlatformAttribute>();
        if (!supportedOSPlatform.PlatformName.StartsWith("Windows"))
        {
            throw new Exception("error SupportedOSPlatformAttribute");
        }
    }

    [Test]
    public void Windows()
    {
#if WINDOWS
        Type[] types = [
            typeof(global::SteamKit2.SteamContent),
            typeof(global::Steamworks.SteamServer),
        ];
        TestAssembly(types);
#endif
    }
}
