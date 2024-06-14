// Decompiled with JetBrains decompiler
// Type: Microsoft.International.Converters.PinYinConverter.AssemblyResource
// Assembly: ChnCharInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=18f031bd02e5e291
// MVID: CDFDC3F6-7539-450B-8671-2A504BDB3DD4
// Assembly location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.dll
// XML documentation location: Microsoft Visual Studio International Pack\Simplified Chinese Pin-Yin Conversion Library\ChnCharInfo.xml

namespace Microsoft.International.Converters.PinYinConverter;

[DebuggerNonUserCode]
[CompilerGenerated]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
static partial class AssemblyResource
{
    static ResourceManager? resourceMan;
    static CultureInfo? resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
        get
        {
            if (object.ReferenceEquals(resourceMan, null))
            {
                ResourceManager temp = new ResourceManager("Microsoft.International.Converters.PinYinConverter.AssemblyResource", typeof(AssemblyResource).Assembly);
                resourceMan = temp;
            }
            return resourceMan;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo? Culture
    {
        get => resourceCulture;
        set => resourceCulture = value;
    }

    internal static string? CHARACTER_NOT_SUPPORTED => ResourceManager.GetString(nameof(CHARACTER_NOT_SUPPORTED), resourceCulture);

    internal static string? EXCEED_BORDER_EXCEPTION => ResourceManager.GetString(nameof(EXCEED_BORDER_EXCEPTION), resourceCulture);

    internal static string? INDEX_OUT_OF_RANGE => ResourceManager.GetString(nameof(INDEX_OUT_OF_RANGE), resourceCulture);
}
