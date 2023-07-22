using System.Drawing;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace BD.Common.UnitTest;

public sealed class EnumTest
{
    [Test]
    public void Enum2Test()
    {
        StringFormatFlags[] all = Enum2.GetAll<StringFormatFlags>();
        string a1 = string.Join(", ", all);

        IEnumerable<StringFormatFlags> all2 = Enum.GetValues(typeof(StringFormatFlags)).Cast<StringFormatFlags>();
        string a2 = string.Join(", ", all2);

        // GetAll 结果应当一致
        TestContext.WriteLine(a1);
        Assert.IsTrue(a1 == a2);

        string[] alls = Enum2.GetAllStrings<TypeLibFuncFlags>();
        a1 = string.Join(", ", alls);

        string[] alls2 = Enum2.GetAllStrings(typeof(TypeLibFuncFlags));
        a2 = string.Join(", ", alls2);

        // GetAllStrings 结果应当一致
        TestContext.WriteLine(a1);
        Assert.IsTrue(a1 == a2);

        TypeLibFuncFlags enumFlags = TypeLibFuncFlags.FBindable | TypeLibFuncFlags.FDefaultCollelem | TypeLibFuncFlags.FUiDefault;
        IEnumerable<TypeLibFuncFlags> split1 = Enum2.FlagsSplit(enumFlags);

        Enum enumFlags1 = enumFlags;
        IEnumerable<TypeLibFuncFlags> split2 = Enum2.FlagsSplit(enumFlags1).Cast<TypeLibFuncFlags>();

        // FlagsSplit 结果应当一致
        Assert.That(split1.SequenceEqual(split2), Is.True);

        int enumInt32 = Enum2.ConvertToInt32(enumFlags);
        TestContext.WriteLine(enumInt32);

        GetDescriptionE enumD = GetDescriptionE.A;
        Enum enumD1 = enumD;
        string desc1 = enumD.GetDescription()!;
        string desc2 = enumD1.GetDescription()!;

        // GetDescription 结果应当一致
        TestContext.WriteLine(desc2);
        Assert.IsTrue(desc1 == desc2);
    }

    enum GetDescriptionE
    {
        [Description("AAA")]
        A,

        [Description("BBB")]
        B,

        [Description("CCC")]
        C,
    }
}
