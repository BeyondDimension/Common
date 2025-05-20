# BD.Common8.SourceGenerator.Shared
源生成器基类库

[API browser](https://beyonddimension.github.io/Common/api/index.html)

定义源生成器模板基类与常量以及 ```Common 库```中常用的扩展函数，以及兼容 ```.NET Standard 2.0``` 的一些 API

项目配置由 ```src\SharedProject\SourceGenerator.props``` 定义

新建类库项目，删除 ```TargetFramework``` 在末尾 ```Import SourceGenerator.props``` 即可作为源生成器项目