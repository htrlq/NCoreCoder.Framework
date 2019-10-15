using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCoreCoder.Symbol
{
    public class SymbolType
    {
        private SyntaxNodeAnalysisContext Context { get; }

        public SymbolType(SyntaxNodeAnalysisContext context)
        {
            Context = context;
        }

        public bool IsInterface => Context.Node.IsKind(SyntaxKind.InterfaceDeclaration);
        public bool IsClass => Context.Node.IsKind(SyntaxKind.ClassDeclaration);

        public IEnumerable<AttributeSymbolType> GetAttributes()
        {
            if (IsInterface)
                return Context.GetInterfaceAttributes();

            if (IsClass)
                return Context.GetClassAttributes();

            throw new NotImplementedException();
        }

        public bool IsAttribute<TAttribute>()
            where TAttribute:Attribute
        {
            if (IsInterface)
                return Context.GetInterfaceAttributes().Any(_attribute => _attribute.Type == Context.ToSymbol<TAttribute>());

            if (IsClass)
                return Context.GetClassAttributes().Any(_attribute => _attribute.Type == Context.ToSymbol<TAttribute>());

            throw new NotImplementedException();
        }
    }
}
