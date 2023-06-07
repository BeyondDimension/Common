#pragma warning disable IDE0044 // 添加只读修饰符
#pragma warning disable IDE0051 // 删除未使用的私有成员

namespace BD.Common.Repositories.SourceGenerator.ConsoleTest.Entities.Design;

[GenerateRepositories]
public class 静态资源
{
    Guid Id;

    Guid 租户Id;

    [Description("CloudFileInfo")]
    PreprocessorDirective PD_CloudFileInfo_R = PreprocessorDirective.region;

    [MaxLength(255)]
    string? 文件名;

    [MaxLength(Hashs.String.Lengths.SHA256 + 3)]
    [MinLength(Hashs.String.Lengths.SHA256)]
    string? SHA256;

    [MaxLength(Hashs.String.Lengths.SHA384 + 3)]
    [MinLength(Hashs.String.Lengths.SHA384)]
    string? SHA384;

    [MaxLength(MaxLengths.Url)]
    string? 文件路径;

    [MaxLength(MaxLengths.FileExtension)]
    string? 文件后缀名;

    byte 文件类型;

    long 文件大小;

    PreprocessorDirective PD_CloudFileInfo_E = PreprocessorDirective.endregion;

    [MaxLength(MaxLengths.Url)]
    string? 访问地址;

    bool 是否删除;

    Guid? 创建人;

    DateTimeOffset 创建时间;

    Guid? 操作人;

    DateTimeOffset 更新时间;

    public string? A { get; set; }
}
