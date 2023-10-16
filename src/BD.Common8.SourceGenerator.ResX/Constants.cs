using static BD.Common8.SourceGenerator.ResX.Templates.DesignerTemplate;

namespace BD.Common8.SourceGenerator.ResX;

/// <summary>
/// 常量定义
/// </summary>
static partial class Constants
{
    /// <summary>
    /// AnalyzerConfigOptionKey 固定前缀
    /// </summary>
    const string Key_Prefix = "build_metadata.AdditionalFiles.";

    /// <summary>
    /// AnalyzerConfigOptionKey <see cref="SourceModel.IsPublic"/>
    /// </summary>
    public const string Key_IsPublic_WithoutPrefix = "BD_Common8_ResX_IsPublic";

    /// <summary>
    /// AnalyzerConfigOptionKey <see cref="SourceModel.IsPublic"/>
    /// </summary>
    public const string Key_IsPublic = Key_Prefix +
        Key_IsPublic_WithoutPrefix;

    /// <summary>
    /// AnalyzerConfigOptionKey <see cref="SourceModel.Namespace"/>
    /// </summary>
    public const string Key_CustomNamespace_WithoutPrefix =
        "BD_Common8_ResX_Namespace";

    /// <summary>
    /// AnalyzerConfigOptionKey <see cref="SourceModel.Namespace"/>
    /// </summary>
    public const string Key_CustomNamespace = Key_Prefix +
        Key_CustomNamespace_WithoutPrefix;

    /// <summary>
    /// AnalyzerConfigOptionKey <see cref="SourceModel.TypeName"/>
    /// </summary>
    public const string Key_CustomCustomTypeName_WithoutPrefix =
        "BD_Common8_ResX_CustomTypeName";

    /// <summary>
    /// AnalyzerConfigOptionKey <see cref="SourceModel.TypeName"/>
    /// </summary>
    public const string Key_CustomCustomTypeName = Key_Prefix +
        Key_CustomCustomTypeName_WithoutPrefix;

    /// <summary>
    /// AnalyzerConfigOptionKey <see cref="SourceModel.ResourceBaseName"/>
    /// </summary>
    public const string Key_CustomResourceBaseName_WithoutPrefix =
        "BD_Common8_ResX_CustomResourceBaseName";

    /// <summary>
    /// AnalyzerConfigOptionKey <see cref="SourceModel.ResourceBaseName"/>
    /// </summary>
    public const string Key_CustomResourceBaseName = Key_Prefix +
        Key_CustomResourceBaseName_WithoutPrefix;

    /// <summary>
    /// 默认值 <see cref="SourceModel.IsPublic"/>
    /// </summary>
    public const bool Default_IsPublic = true;

    /// <summary>
    /// 获取 <see cref="SourceModel.Namespace"/> 默认值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetDefaultNamespace(string filePath)
    {
        var fileNameWithoutEx = Path.GetFileNameWithoutExtension(filePath);
        fileNameWithoutEx = fileNameWithoutEx switch
        {
            "BD.Common8.Bcl" => "BD.Common8",
            _ => fileNameWithoutEx,
        };
        return fileNameWithoutEx.EndsWith(".Resources") ?
            fileNameWithoutEx : $"{fileNameWithoutEx}.Resources";
    }

    /// <summary>
    /// 获取 <see cref="SourceModel.ResourceBaseName"/> 默认值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetDefaultResourceBaseName(string filePath)
    {
        var fileNameWithoutEx = Path.GetFileNameWithoutExtension(filePath);
        return $"FxResources.{fileNameWithoutEx}";
    }
}
