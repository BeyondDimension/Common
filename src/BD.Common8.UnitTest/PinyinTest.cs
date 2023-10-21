using Microsoft.International.Converters.PinYinConverter;
using ChnCharInfoAssemblyResource = Microsoft.International.Converters.PinYinConverter.AssemblyResource;
using Common8SR = BD.Common8.Resources.SR;

namespace BD.Common8.UnitTest;

/// <summary>
/// 汉语拼音测试
/// </summary>
public class PinyinTest
{
    /// <summary>
    /// 测试 ChnCharInfo
    /// </summary>
    [Test]
    public void ChnCharInfoTest()
    {
        ChineseChar @char = new('测');
        TestContext.WriteLine(string.Join(", ", @char.Pinyins.Where(x => x != null)));

        static void WriteLineChnCharInfoAssemblyResource()
        {
            TestContext.WriteLine(ChnCharInfoAssemblyResource.CHARACTER_NOT_SUPPORTED);
            TestContext.WriteLine(ChnCharInfoAssemblyResource.EXCEED_BORDER_EXCEPTION);
            TestContext.WriteLine(ChnCharInfoAssemblyResource.INDEX_OUT_OF_RANGE);
        }
        WriteLineChnCharInfoAssemblyResource();

        ChnCharInfoAssemblyResource.Culture = new("en-US");
        WriteLineChnCharInfoAssemblyResource();

        TestContext.WriteLine(Common8SR.DayOfWeek_L1);

        var services = new ServiceCollection();
        services.AddPinyinChnCharInfo();
        using var provider = services.BuildServiceProvider();
        var pinyinHelper = provider.GetRequiredService<IPinyin>();
        var arr = pinyinHelper.GetPinyinArray("汉语拼音测试");
        TestContext.WriteLine(string.Join(", ", arr));
    }
}