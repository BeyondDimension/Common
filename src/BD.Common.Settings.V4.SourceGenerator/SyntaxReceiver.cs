using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BD.Common.Settings.SourceGenerator;

public sealed class SyntaxReceiver : ISyntaxReceiver
{
    internal const string AttributeName = "SettingsV4Generation";
    internal const string AttributeTypeName = $"{AttributeName}Attribute";

    public List<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax)
        {
            foreach (var attributeList in
            typeDeclarationSyntax.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    if (attribute.Name.ToString() == AttributeName ||
                        attribute.Name.ToString() == AttributeTypeName)
                    {
                        Candidates.Add(typeDeclarationSyntax);
                    }
                }
            }
        }
    }
}
