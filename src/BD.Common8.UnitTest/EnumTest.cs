namespace BD.Common8.UnitTest;

/// <summary>
/// 提供对 <see cref="Enum"/> 类型的单元测试
/// </summary>
public sealed class EnumTest
{
    /// <summary>
    /// 测试 <see cref="Enum2"/>
    /// </summary>
    [Test]
    public void Enum2Test()
    {
        var all = Enum2.GetAll<ArchitectureFlags>();
        var a1 = string.Join(", ", all);

        var all2 = Enum.GetValues(typeof(ArchitectureFlags)).Cast<ArchitectureFlags>();
        var a2 = string.Join(", ", all2);

        // GetAll 结果应当一致
        TestContext.WriteLine(a1);
        Assert.That(a1, Is.EqualTo(a2));

        var alls = Enum2.GetAllStrings<TypeLibFuncFlags>();
        a1 = string.Join(", ", alls);

        var alls2 = Enum2.GetAllStrings(typeof(TypeLibFuncFlags));
        a2 = string.Join(", ", alls2);

        // GetAllStrings 结果应当一致
        TestContext.WriteLine(a1);
        Assert.That(a1, Is.EqualTo(a2));

        var enumFlags = TypeLibFuncFlags.FBindable | TypeLibFuncFlags.FDefaultCollelem | TypeLibFuncFlags.FUiDefault;
        var split1 = Enum2.FlagsSplit(enumFlags);

        Enum enumFlags1 = enumFlags;
        var split2 = Enum2.FlagsSplit(enumFlags1).Cast<TypeLibFuncFlags>();

        // FlagsSplit 结果应当一致
        Assert.That(split1.SequenceEqual(split2), Is.True);

        var enumInt32 = Enum2.ConvertToInt32(enumFlags);
        TestContext.WriteLine(enumInt32);

        var enumD = GetDescriptionE.A;
        Enum enumD1 = enumD;
        var desc1 = enumD.GetDescription()!;
        var desc2 = enumD1.GetDescription()!;

        // GetDescription 结果应当一致
        TestContext.WriteLine(desc2);
        Assert.That(desc1, Is.EqualTo(desc2));
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
