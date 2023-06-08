//namespace BD.Common.Repositories.SourceGenerator.Handlers.Attributes;

//sealed class CommentAttributeHandle : AttributeHandle
//{
//    protected override string ClassFullName => TypeFullNames.Comment;

//    protected override void WriteCore(AttributeHandleArguments args)
//    {
//        var template =
//"""
//    [Comment("{0}")]

//"""u8;
//        var ctorArgs = args.Attribute.ConstructorArguments;
//        var value = ctorArgs.FirstOrDefault().Value;
//        args.Stream.WriteFormat(template, value);
//    }
//}
