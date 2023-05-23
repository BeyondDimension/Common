namespace BD.Common.Settings.Models;

/// <summary>
/// 设置元数据
/// </summary>
/// <param name="ClassFilePath">类文件路径</param>
/// <param name="InterfaceFilePath">接口文件路径</param>
/// <param name="Usings">可选的生成 using 语句</param>
public sealed record class SettingsMetadata(string ClassFilePath, string InterfaceFilePath, SettingsProperty[] Properties, string Usings = "")
{

}
