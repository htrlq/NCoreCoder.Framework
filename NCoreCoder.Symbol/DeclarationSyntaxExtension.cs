using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace NCoreCoder.Symbol
{

    public static class DeclarationSyntaxExtension
    {
        public static IEnumerable<AttributeSymbolType> GetInterfaceAttributes(this SyntaxNodeAnalysisContext context)
        {
            var _interface = context.Node as InterfaceDeclarationSyntax;

            return context.SemanticModel.GetDeclaredSymbol(_interface).GetAttributes().Select(_attributeData => new AttributeSymbolType(_attributeData));
        }

        public static IEnumerable<AttributeSymbolType> GetClassAttributes(this SyntaxNodeAnalysisContext context)
        {
            var _class = context.Node as ClassDeclarationSyntax;

            return context.SemanticModel.GetDeclaredSymbol(_class).GetAttributes().Select(_attributeData => new AttributeSymbolType(_attributeData));
        }
    }
}
