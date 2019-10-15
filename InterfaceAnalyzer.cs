using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Code.Analyzer
{
    public class InterfaceAnalyzer : BaseAnalyzer
    {
        public override SyntaxKind Kind => SyntaxKind.InterfaceDeclaration;

        public override DiagnosticDescriptor[] Descriptors => new DiagnosticDescriptor[]
        {
            DiagnosticDescriptorExtension.InterfaceName,
            DiagnosticDescriptorExtension.InterfaceAttribute,
            DiagnosticDescriptorExtension.InterfaceInherit,
            DiagnosticDescriptorExtension.InterfaceDefaultAopActors
        };

        public override void Execute(SyntaxNodeAnalysisContext context)
        {
            if (!CheckInterfaceName(context))
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptorExtension.InterfaceName, context.Node.GetLocation()));

            if (!CheckInterfaceCtorAttribute(context))
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptorExtension.InterfaceAttribute, context.Node.GetLocation()));

            if (!CheckInterfaceCtorAttributeDefault(context))
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptorExtension.InterfaceDefaultAopActors, context.Node.GetLocation()));
            else
            {
                if (!CheckInterfaceCtorAttributeInherit(context))
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptorExtension.InterfaceInherit, context.Node.GetLocation()));
            }
        }

        private bool CheckInterfaceName(SyntaxNodeAnalysisContext context)
        {
            var _interface = context.Node as InterfaceDeclarationSyntax;

            if (_interface.ChildTokens().Any(_token => _token.Kind() == SyntaxKind.IdentifierToken))
            {
                var interfaceName = _interface.ChildTokens().FirstOrDefault(_token => _token.Kind() == SyntaxKind.IdentifierToken);

                var name = interfaceName.ValueText;

                return Regex.IsMatch(name, "(I[A-Z]{1})");
            }

            return false;
        }

        private bool CheckInterfaceCtorAttribute(SyntaxNodeAnalysisContext context)
        {
            var _interface = context.Node as InterfaceDeclarationSyntax;

            if (_interface.AttributeLists == null)
                return false;

            var attributeSymbol = context.Compilation.GetTypeByMetadataName("NCoreCoder.Aop.Jit.AopActorsAttribute");

            foreach (var attributeType in _interface.AttributeLists)
            {
                foreach (var baseType in attributeType.Attributes)
                {
                    var type = context.SemanticModel.GetTypeInfo(baseType).Type;

                    if (type.Equals(attributeSymbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckInterfaceCtorAttributeDefault(SyntaxNodeAnalysisContext context)
        {
            var _interface = context.Node as InterfaceDeclarationSyntax;

            if (_interface.AttributeLists == null)
                return false;

            var defaultAopActors = context.Compilation.GetTypeByMetadataName("NCoreCoder.Aop.DefaultAopActors");
            var symbol = ReturnInterfaceCtorAttribute(context, _interface);

            return !defaultAopActors.Equals(symbol);
        }

        private bool CheckInterfaceCtorAttributeInherit(SyntaxNodeAnalysisContext context)
        {
            var _interface = context.Node as InterfaceDeclarationSyntax;

            if (_interface.AttributeLists == null)
                return false;

            var aopActorsInterface = context.Compilation.GetTypeByMetadataName("NCoreCoder.Aop.IAopActors");
            var symbol = ReturnInterfaceCtorAttribute(context, _interface);

            return symbol.Interfaces.Any(_symBol => _symBol.Equals(aopActorsInterface));
        }

        private INamedTypeSymbol ReturnInterfaceCtorAttribute(SyntaxNodeAnalysisContext context, InterfaceDeclarationSyntax _interface)
        {
            var attributeSymbol = context.Compilation.GetTypeByMetadataName("NCoreCoder.Aop.Jit.AopActorsAttribute");

            if (context.SemanticModel
                .GetDeclaredSymbol(_interface)
                .GetAttributes()
                .FirstOrDefault(_attribute => _attribute.AttributeClass.Equals(attributeSymbol)) is AttributeData attribute && attribute != null)
            {
                var args = attribute.ConstructorArguments;

                if (args.Length == 1)
                {
                    var value = (INamedTypeSymbol)args[0].Value;

                    return value;
                }
            }

            return null;
        }
    }
}
