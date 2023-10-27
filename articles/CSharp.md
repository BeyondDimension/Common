### [C# 中的新增功能 与 重大更改](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/relationships-between-language-and-library)

#### [C# 11 中的新增功能](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-11)
- [泛型属性](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-11#generic-attributes)
- [泛型数学支持](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-11#generic-math-support)
- [字符串内插中的换行符](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-11#newlines-in-string-interpolations)
- [原始字符串文本](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-11#raw-string-literals)
- [UTF-8 字符串字面量](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-11#utf-8-string-literals)

#### [C# 11 重大更改](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/breaking-changes/compiler%20breaking%20changes%20-%20dotnet%207)

#### [C# 10 中的新增功能](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-10)
- [全局 using 指令](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-10#global-using-directives)
- [常数内插字符串](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-10#constant-interpolated-strings)
- [在同一析构中进行赋值和声明](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-10#assignment-and-declaration-in-same-deconstruction)

#### [C# 9.0 中的新增功能](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-9)
- [记录类型](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-9#record-types)
- [目标类型的 new 表达式](https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/proposals/csharp-9.0/target-typed-new)
- [静态匿名函数](https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/proposals/csharp-9.0/static-anonymous-functions)
- [Lambda 弃元参数](https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/proposals/csharp-9.0/lambda-discard-parameters)

#### [C# 8.0 中的新增功能](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-version-history#c-version-80)
- [可为 null 的引用类型（C# 引用）](https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/builtin-types/nullable-reference-types)
    - [Blog - C# 8.0 最具影响力的功能可能是可为空的引用类型 （NRT）。它允许您在代码中显式执行 null 流，并在您未按照意图执行操作时发出警告。](https://devblogs.microsoft.com/dotnet/embracing-nullable-reference-types/)
- [接口 - 默认接口成员](https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/interface#default-interface-members) 

#### [早期版本中的重大更改](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/breaking-changes)

### [源生成器](https://learn.microsoft.com/zh-cn/dotnet/csharp/roslyn-sdk/source-generators-overview)
- [.NET 正则表达式源生成器](https://learn.microsoft.com/zh-CN/dotnet/standard/base-types/regular-expression-source-generators)
    - [Blog - .NET 7 中的正则表达式改进](https://devblogs.microsoft.com/dotnet/regular-expression-improvements-in-dotnet-7/)
- [P/Invoke 源生成](https://learn.microsoft.com/zh-cn/dotnet/standard/native-interop/pinvoke-source-generation)
    - [在源生成的 P/Invoke 中使用自定义封送程序](https://learn.microsoft.com/zh-cn/dotnet/standard/native-interop/tutorial-custom-marshaller)

### 全球化/国家或地区/区域
System.Globalization.RegionInfo 构造函数支持 ISO 3166 2个字母组成的字符串与 int 类型的值构造  
其中 ThreeLetterISORegionName 可获取在 ISO 3166 中定义的由三个字母组成的国家/地区代码。  
这里面也有每个国家的货币单位以及金额小数点相关的区域信息  
System.Globalization.CultureInfo 这些信息包括区域性的名称、书写系统、使用的日历、字符串的排序顺序以及对日期和数字的格式化设置。  
构造函数中的 int 为 LCID  
LCID 是微软的 Windows Language Code Identifier 在微软的文档上有一份表格，列出了所有国家的 int  
https://learn.microsoft.com/zh-cn/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c  