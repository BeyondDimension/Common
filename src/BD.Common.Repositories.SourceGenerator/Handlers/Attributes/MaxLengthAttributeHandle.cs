//namespace BD.Common.Repositories.SourceGenerator.Handlers.Attributes;

//sealed class MaxLengthAttributeHandle : AttributeHandle
//{
//    protected override string ClassFullName => TypeFullNames.MaxLength;

//    protected override void WriteCore(AttributeHandleArguments args)
//    {
//        var template =
//"""
//    [MaxLength({0})]

//"""u8;
//        var ctorArgs = args.Attribute.ConstructorArguments;
//        var value = ctorArgs.FirstOrDefault().Value;
//        args.Stream.WriteFormat(template, value);
//    }
//}
