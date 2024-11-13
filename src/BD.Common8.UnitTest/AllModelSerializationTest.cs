using Tools.Build.Commands;

namespace BD.Common8.UnitTest;

/// <summary>
/// 所有模型类的序列化测试
/// </summary>
public sealed class AllModelSerializationTest
{
    ImmutableDictionary<ModelTypeType, ImmutableArray<Type>> allModelTypes;

    [SetUp]
    public void Setup()
    {
        var projectNames = IBuildCommand.GetProjectNamesCore(IBuildCommand.slnFileName_Common8);
        allModelTypes = projectNames
            .Where(static x => !string.IsNullOrWhiteSpace(x) &&
                !x.Contains("SourceGenerator") && // 不测试源生成器
                !x.Contains("AspNetCore") && // 不依赖 ASP.NET Core 保证测试跨平台兼容性
                x != "BD.Common8.Bcl.Compat" &&
                x != "BD.Common8.Http.ClientFactory.Server" &&
                x != "BD.Common8.Pinyin.CoreFoundation" &&
                true
                )
            .Select(static x => Assembly.Load(x).GetTypes())
            .SelectMany(static x => x)
            .Where(x => !x.IsAbstract && // 不能是抽象类
                x.IsClass && // 必须是类
                !string.IsNullOrWhiteSpace(x.Namespace) && x.Namespace.Contains("Models") && // 命名空间必须存在 Models
                !x.GetCustomAttributes<ObsoleteAttribute>().Any() && // 忽略标记为 Obsolete
                !string.IsNullOrWhiteSpace(x.FullName) && // 必须有完整名称
                true)
            .GroupBy(static x =>
            {
                if (x.IsGenericType)
                    return ModelTypeType.Generic;
                if (x.IsNested)
                    return ModelTypeType.Nested;
                return ModelTypeType.Normal;
            }).ToImmutableDictionary(static x => x.Key, static x => x.ToImmutableArray());
    }

    /// <summary>
    /// 模型 <see cref="Type"/> 类型
    /// </summary>
    enum ModelTypeType : byte
    {
        /// <summary>
        /// 普通类型
        /// </summary>
        Normal,

        /// <summary>
        /// 嵌套类型
        /// </summary>
        Nested,

        /// <summary>
        /// 泛型类型
        /// </summary>
        Generic,
    }

    /// <inheritdoc cref="Activator.CreateInstance(Type)"/>
    static object? CreateInstance(Type type)
    {
        try
        {
            var result = Activator.CreateInstance(type);
            return result;
        }
        catch
        {
            try
            {
                var result = SystemTextJsonSerializer.Deserialize("{}"u8, type);
                return result;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 对模型类实例设置随机值
    /// </summary>
    /// <param name="model"></param>
    static object? SetRandomValue(object? model)
    {
        if (model != null)
        {
            var properties = model.GetType().GetProperties();
            foreach (var p in properties)
            {
                var randomValue = ProjectUtils.GeneratorRandomValueByType(p.PropertyType);
                p.SetMethod?.Invoke(model, [randomValue]);
            }
        }

        return model;
    }

    [Test]
    public void WriteLine()
    {
        foreach (var modelTypes in allModelTypes)
        {
            TestContext.WriteLine($"{modelTypes.Key} Count: {modelTypes.Value.Length}");
            foreach (var modelType in modelTypes.Value)
            {
                TestContext.WriteLine(modelType.FullName);
            }
            TestContext.WriteLine();
        }
    }

    record class ModelSerializationResult
    {
        public required Type ModelType { get; init; }

        public object? ModelInstance { get; set; }

        public string? ModelString { get; set; }

        public byte[]? ModelBytes { get; set; }

        public object? DeserializeInstance { get; set; }
    }

    [Test]
    public void Json()
    {
        IEnumerable<Type> modelTypes = [];

        if (allModelTypes.TryGetValue(ModelTypeType.Normal, out var modelTypes_0))
        {
            modelTypes = modelTypes.Concat(modelTypes_0);
        }
        if (allModelTypes.TryGetValue(ModelTypeType.Generic, out var modelTypes_2))
        {
            // TODO 处理泛型模型类
        }
        if (allModelTypes.TryGetValue(ModelTypeType.Nested, out var modelTypes_1))
        {
            modelTypes = modelTypes.Concat(modelTypes_1);
        }

        if (modelTypes.Any())
        {
            var items = modelTypes.Select(static x =>
            {
                return new ModelSerializationResult
                {
                    ModelType = x,
                    ModelInstance = SetRandomValue(CreateInstance(x)),
                };
            }).Where(x => x.ModelInstance != null).ToArray();

            foreach (var item in items)
            {
                try
                {
                    item.ModelString = SystemTextJsonSerializer.Serialize(item.ModelInstance, item.ModelType, options: JsonSerializerCompatOptions.WriteIndented);
                }
                catch (Exception ex)
                {
                    throw new AggregateException(new ApplicationException($"序列化失败，{item.ModelType.FullName}"), ex);
                }

                try
                {
                    item.ModelInstance = SystemTextJsonSerializer.Deserialize(item.ModelString!, item.ModelType, options: JsonSerializerCompatOptions.WriteIndented);
                }
                catch (Exception ex)
                {
                    throw new AggregateException(new ApplicationException($"反序列化失败，{item.ModelType.FullName}"), ex);
                }
            }
        }
    }

    [Test]
    public void MemoryPack()
    {
        // TODO
    }
}
