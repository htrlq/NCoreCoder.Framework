using Microsoft.CodeAnalysis;
using System.Linq;

namespace NCoreCoder.Symbol
{
    public class AttributeSymbolType
    {
        public INamedTypeSymbol Type { get; }
        public INamedTypeSymbol[] Args { get; }

        public AttributeSymbolType(AttributeData attributeData)
        {
            Type = attributeData.AttributeClass;
            Args = attributeData.ConstructorArguments.Select(_arg => (INamedTypeSymbol)_arg.Value).ToArray();
        }
    }
}
