### SDK 文件说明

#### 用于 NuGet 包兼容的 TargetFramework 占位符

- net35/\_.\_
- net40/\_.\_
- netstandard1.0/\_.\_

#### 配置 NuGet 包生成的全局参数
- GeneratePackage.props

#### 引用此项目需要的 SDK 配置
实现项目引用，源码引用，全局命名空间，包引用
- BD.Common8.Sdk.targets

#### 项目配置
将 csproj 中的参数放在 props 上以便源码引用实现类似 ILMerge 将多个项目合并为单个程序集
- $(MSBuildProjectName).props