using BD.Common8.Models;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.International.Converters.PinYinConverter;

// https://benchmarkdotnet.org/articles/guides/getting-started.html

namespace BD.Common8.Benchmark;

[InProcess]
[BinaryResource(
"""
[
  {
    "Path": "..\\Microsoft Visual Studio International Pack\\Simplified Chinese Pin-Yin Conversion Library\\ChnCharInfo\\CharDictionary",
    "Name": "_CharDictionary"
  }
]
""")]
public partial class BinaryResourceStreamTest
{
    CharDictionary? dict1;
    CharDictionary? dict2;

    [Benchmark]
    public unsafe void BinaryResource_Stream_SourceGenerator()
    {
        var bytes = _CharDictionary();
        fixed (byte* ptr = bytes)
        {
            using UnmanagedMemoryStream stream = new(ptr, bytes.Length);
            using BinaryReader binaryReader = new BinaryReader(stream);
            dict1 = CharDictionary.Deserialize(binaryReader);
            new Span<byte>(ptr, bytes.Length).Clear();
        }
    }

    [Benchmark(Baseline = true)]
    public void BinaryResource_Stream_ManifestResource()
    {
        Assembly executingAssembly = typeof(BinaryResourceStreamTest).Assembly;
        using var manifestResourceStream = executingAssembly.GetManifestResourceStream("Microsoft.International.Converters.PinYinConverter.CharDictionary.resources");
        using BinaryReader binaryReader = new BinaryReader(manifestResourceStream!);
        dict2 = CharDictionary.Deserialize(binaryReader);
    }
}

[InProcess]
[BinaryResource(
"""
[
  {
    "Path": "..\\..\\res\\AMap_adcode_citycode_20210406"
  }
]
""")]
public partial class BinaryResourceMemoryPackTest
{
    District[]? all1;
    District[]? all2;

    [Benchmark]
    public void BinaryResource_MemoryPack_SourceGenerator()
    {
        Span<byte> bytes = AMapAdcodeCitycode20210406();
        try
        {
            all1 = MemoryPackSerializer.Deserialize<District[]>(bytes);
            ArgumentNullException.ThrowIfNull(all1);
        }
        finally
        {
            bytes.Clear();
        }
    }

    [Benchmark(Baseline = true)]
    public void BinaryResource_MemoryPack_ManifestResource()
    {
        var all2 = MemoryPackSerializer.Deserialize<District[]>(Properties.Resources.AMap_adcode_citycode_20210406);
        ArgumentNullException.ThrowIfNull(all2);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<BinaryResourceStreamTest>();
        BenchmarkRunner.Run<BinaryResourceMemoryPackTest>();
    }
}