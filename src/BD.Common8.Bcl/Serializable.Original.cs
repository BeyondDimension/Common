#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable IDE0161 // 转换为文件范围限定的 namespace
namespace System
{
    public static partial class Serializable // Original(使用原键名)
    {
#if !NO_NEWTONSOFT_JSON
        static readonly Lazy<NewtonsoftJsonSerializerSettings> mIgnoreJsonPropertyContractResolverWithStringEnumConverterSettings = new(GetIgnoreJsonPropertyContractResolverWithStringEnumConverterSettings);

        static NewtonsoftJsonSerializerSettings GetIgnoreJsonPropertyContractResolverWithStringEnumConverterSettings() => new()
        {
            ContractResolver = new IgnoreJsonPropertyContractResolver(),
            Converters = new List<NewtonsoftJsonConverter>
            {
                new StringEnumConverter(),
            },
        };

        /// <summary>
        /// 使用原键名的序列化设置
        /// </summary>
        public static NewtonsoftJsonSerializerSettings IgnoreJsonPropertyContractResolverWithStringEnumConverterSettings => mIgnoreJsonPropertyContractResolverWithStringEnumConverterSettings.Value;

        /// <summary>
        /// 序列化 JSON 模型，使用原键名
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SJSON_Original(object? value, NewtonsoftJsonFormatting formatting = NewtonsoftJsonFormatting.Indented)
            => NewtonsoftJsonConvert.SerializeObject(value, formatting, IgnoreJsonPropertyContractResolverWithStringEnumConverterSettings);

        /// <summary>
        /// 反序列化 JSON 模型，使用原键名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: MaybeNull]
        public static T DJSON_Original<T>(string value)
            => NewtonsoftJsonConvert.DeserializeObject<T>(value, IgnoreJsonPropertyContractResolverWithStringEnumConverterSettings);
#endif
    }
}

#if !NO_NEWTONSOFT_JSON
namespace Newtonsoft.Json.Serialization
{
    /// <summary>
    /// 将忽略 <see cref="NewtonsoftJsonProperty"/> 属性
    /// </summary>
    public sealed class IgnoreJsonPropertyContractResolver(bool useCamelCase = false) : DefaultContractResolver
    {
        readonly bool useCamelCase = useCamelCase;

        /// <summary>
        /// 创建属性列表，并根据成员序列化方式忽略特定属性
        /// </summary>
        protected override IList<NewtonsoftJsonPropertyClass> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperties(type, memberSerialization);
            foreach (var item in result)
            {
                item.PropertyName = item.UnderlyingName == null ? null :
                    (useCamelCase ?
#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
                        JsonNamingPolicy.CamelCase.ConvertName(item.UnderlyingName) :
#else
                        ToCamelCase(item.UnderlyingName) :
#endif
                        item.UnderlyingName);
            }
            return result;
        }

#if !(NETFRAMEWORK && !NET461_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
#else
        static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }
                bool hasNext = i + 1 < chars.Length;
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // if the next character is a space, which is not considered uppercase 
                    // (otherwise we wouldn't be here...)
                    // we want to ensure that the following:
                    // 'FOO bar' is rewritten as 'foo bar', and not as 'foO bar'
                    // The code was written in such a way that the first word in uppercase
                    // ends when if finds an uppercase letter followed by a lowercase letter.
                    // now a ' ' (space, (char)32) is considered not upper
                    // but in that case we still want our current character to become lowercase
                    if (char.IsSeparator(chars[i + 1]))
                    {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
        }
#endif

    }
}
#endif