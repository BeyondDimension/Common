### 压缩与打包(归档)
在 Linux 上通常压缩与打包是分开作为两个功能，而在 Windows 上通常是合在一起的  
**压缩**是指利用算法将文件进行处理，已达到保留最大文件信息，而让文件体积变小的目的。其基本原理为，通过查找文件内的重复字节，建立一个相同字节的词典文件，并用一个代码表示。比如说，在压缩文件中，有不止一处出现了 "C语言中文网"，那么，在压缩文件时，这个词就会用一个代码表示并写入词典文件，这样就可以实现缩小文件体积的目的。由于计算机处理的信息是以二进制的形式表示的，因此，压缩软件就是把二进制信息中相同的字符串以特殊字符标记，只要通过合理的数学计算，文件的体积就能够被大大压缩。把一个或者多个文件用压缩软件进行压缩，形成一个文件压缩包，既可以节省存储空间，有方便在网络上传送。  
**打包**，也称为归档，指的是一个文件或目录的集合，而这个集合被存储在一个文件中。归档文件没有经过压缩，因此，它占用的空间是其中所有文件和目录的总和。通常，归档总是会和系统（数据）备份联系在一起。 

#### Windows 上常用的一些格式
- **7z** 是一种全新的压缩格式，它拥有极高的压缩比。  
使用包 Squid-Box.SevenZipSharp.Lite 与 7-Zip.NativeAssets.Win32 在 C# 中操作，由于本机库仅支持 Windows，尚未测试其他平台本机库是否兼容  
- **Zip** 文件格式是一种数据压缩和文档储存的文件格式，原名 Deflate，发明者为菲尔·卡茨（Phil Katz），他于1989年1月公布了该格式的资料。  
使用 .NET Framework 4.5 中新增的 ```System.IO.Compression.ZipArchive``` 类进行操作  
[.NET 基础知识 - 如何：压缩和解压缩文件](https://learn.microsoft.com/zh-cn/dotnet/standard/io/how-to-compress-and-extract-files)  

#### Linux 上常用的一些格式
- **tar** Unix 和类 Unix 系统上的压缩打包工具，可以将多个文件合并为一个文件，打包后的文件后缀亦为 tar  
将打包后的一个文件进行压缩，例如采用 gzip 压缩，文件后缀名即为 tar.gz 又可简写为 .tgz  
使用包 SharpZipLib 或 .NET 7 中新增的 ```System.Formats.Tar``` 进行操作
- **Zstandard/zstd** 是 Facebook 在 2016 年开源的新无损压缩算法，是针对 zlib 级别的实时压缩方案，以及更好的压缩比。它由一个非常快的熵阶段，由 Huff0 和 FSE 库提供  
使用包 ZstdNet 进行操作

#### Web 中常用的压缩格式，这些格式由 .NET 基础类库实现的可跨平台使用
- **Gzip** 是一种用于文件压缩与解压缩的文件格式。它基于 Deflate 算法，可将文件压缩地更小，从而实现更快的网络传输。  
https://developer.mozilla.org/zh-CN/docs/Glossary/GZip_compression  
使用 ```System.IO.Compression.GZipStream``` 类型进行操作  
- **Brotli/Br** 是 Google 发明的一种通用的无损压缩算法。  
https://developer.mozilla.org/zh-CN/docs/Glossary/brotli_compression  
使用 .NET Core 2.1 中新增的 ```System.IO.Compression.BrotliStream``` 类型 或在 .NET Framework 中使用包 Brotli.NET 进行操作  


#### 参考文献
[Linux(四)：Linux的打包和压缩详解 作者：糖拌西红柿](https://www.cnblogs.com/TheGCC/p/14228439.html)  