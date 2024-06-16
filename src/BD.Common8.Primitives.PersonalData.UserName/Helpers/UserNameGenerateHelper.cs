namespace BD.Common8.Helpers;

/// <summary>
/// 用户名生成助手类
/// </summary>
[BinaryResource(
"""
[
  {
    "Path": "..\\..\\..\\res\\f4_known_name"
  }
]
""",
"""
    static readonly Lazy<string[]> f4_known_name = new(() =>
    {
        Span<byte> bytes = {F4KnownName}();
        try
        {
            var f4_known_name = MemoryPackSerializer.Deserialize<string[]>(bytes);
            return f4_known_name!;
        }
        finally
        {
            bytes.Clear();
        }
    }, LazyThreadSafetyMode.ExecutionAndPublication);
""")]
public static partial class UserNameGenerateHelper
{
    /// <summary>
    /// 生成一个英文用户名
    /// </summary>
    /// <param name="randomCharPaddingMinValue">随机字符填充最小值（注意：该值不代表整个字符串长度）</param>
    /// <param name="randomCharPaddingMaxValue">随机字符填充最大值（注意：该值不代表整个字符串长度）</param>
    /// <returns></returns>
    public static string GenerateEnglishUserName(int randomCharPaddingMinValue = 4, int randomCharPaddingMaxValue = 6)
    {
        string? givenname = null;
        string? surname = null;
        var length = f4_known_name.Value.Length;
        switch (length)
        {
            case 0:
                break;
            case 1:
                givenname = surname = f4_known_name.Value[1];
                break;
            default:
                {
                    var givenIndex = Random2.Next(f4_known_name.Value.Length);
                    var surIndex = Random2.Next(f4_known_name.Value.Length);
                    for (int i = 0; i < byte.MaxValue; i++)
                    {
                        if (givenIndex != surIndex)
                        {
                            break;
                        }
                        surIndex = Random2.Next(f4_known_name.Value.Length);
                    }
                    givenname = f4_known_name.Value[givenIndex];
                    surname = f4_known_name.Value[surIndex];
                }
                break;
        }
        return $"{givenname}{surname}{DateTime.Today.Year - 2000}{Random2.GenerateRandomNum(Random2.Next(randomCharPaddingMinValue, randomCharPaddingMaxValue))}";
    }
}
