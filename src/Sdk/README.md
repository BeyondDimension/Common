### SDK 文件说明

#### 用于 NuGet 包兼容的 TargetFramework 占位符

- net35/\_.\_
- net40/\_.\_
- netstandard1.0/\_.\_

#### 配置 NuGet 包生成的全局参数
- GeneratePackage.props
```
<Import Project="$(MSBuildThisFileDirectory)..\Sdk\GeneratePackage.props" />
```
将在 Release 时生成 NuGet 包

#### 源生成配置
- BD.Common8.SourceGenerator.props
- BD.Common8.SourceGenerator.Reference.props
