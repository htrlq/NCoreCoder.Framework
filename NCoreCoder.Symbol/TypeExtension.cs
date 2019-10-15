using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;

namespace NCoreCoder.Symbol
{
    public static class TypeExtension
    {
        public static INamedTypeSymbol ToSymbol(this SyntaxNodeAnalysisContext context,Type type)
        {
            return context.Compilation.GetTypeByMetadataName(type.FullName);
        }

        public static INamedTypeSymbol ToSymbol<T>(this SyntaxNodeAnalysisContext context)
        {
            return context.ToSymbol(typeof(T));
        }

        public static bool InterfaceInherit(this SyntaxNodeAnalysisContext context, INamedTypeSymbol source, Type type)
        {
            return source.AllInterfaces.Any(_interface => _interface == context.Compilation.GetTypeByMetadataName(type.FullName));
        }

        public static bool InterfaceInherit<T>(this SyntaxNodeAnalysisContext context, INamedTypeSymbol source)
        {
            return source.AllInterfaces.Any(_interface => _interface == context.Compilation.GetTypeByMetadataName(typeof(T).FullName));
        }
    }
}
