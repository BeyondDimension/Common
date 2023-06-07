namespace BD.Common.Repositories.SourceGenerator.Annotations;

/// <summary>
/// 配置式后台管理系统 CURD 增量源码生成特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class GenerateRepositoriesAttribute : Attribute
{

}
