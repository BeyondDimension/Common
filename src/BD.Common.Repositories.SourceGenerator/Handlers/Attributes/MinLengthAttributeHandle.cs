//namespace BD.Common.Repositories.SourceGenerator.Handlers.Attributes;

//sealed class MinLengthAttributeHandle : AttributeHandle
//{
//    protected override string ClassFullName => TypeFullNames.MinLength;

//    protected override void WriteCore(AttributeHandleArguments args)
//    {
//        var template =
//"""
//    [MinLength({0})]

//"""u8;
//        var ctorArgs = args.Attribute.ConstructorArguments;
//        var value = ctorArgs.FirstOrDefault().Value;
//        args.Stream.WriteFormat(template, value);
//    }
//}
