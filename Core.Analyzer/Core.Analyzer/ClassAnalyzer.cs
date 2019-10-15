using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NCoreCoder.Aop;
using NCoreCoder.Symbol;
using System.Collections.Generic;
using System.Linq;

namespace NCoreCoder.Analyzer
{
    public class ClassContext : BaseAnalyzerContext
    {
        public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        public override DiagnosticDescriptor[] Descriptors => new DiagnosticDescriptor[]
        {
            DiagnosticDescriptorExtension.ClassMethod
        };

        public override void Execute(SyntaxNodeAnalysisContext context)
        {
            var jitAopAttribute = context.Compilation.GetTypeByMetadataName("NCoreCoder.Aop.JitAopAttribute");

            if (IsInheritInterface(context) && IsAopActorsAttribute(context))
            {
                foreach (var method in GetMethods(context))
                {
                    if (!method.GetAttributes().Any(
                        _attribute =>
                            _attribute.AttributeClass.Equals(jitAopAttribute) ||
                            _attribute.AttributeClass.AllInterfaces.Any(_attri =>
                                _attri.Equals(jitAopAttribute)
                            )
                        )
                    )
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptorExtension.ClassMethod, context.Node.GetLocation(), method.Name);

                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private bool IsInheritInterface(SyntaxNodeAnalysisContext context)
        {
            var classSyntax = context.Node as ClassDeclarationSyntax;
            if (classSyntax.BaseList == null)
                return false;

            var symbol = context.SemanticModel
                .GetDeclaredSymbol(classSyntax);

            return symbol.AllInterfaces.Any();
        }

        private bool IsAopActorsAttribute(SyntaxNodeAnalysisContext context)
        {
            var typeInfo = new SymbolType(context);

            return typeInfo.IsAttribute<JitInjectAttribute>();
        }

        private IEnumerable<IMethodSymbol> GetMethods(SyntaxNodeAnalysisContext context)
        {
            var classSyntax = context.Node as ClassDeclarationSyntax;
            var symbol = context.SemanticModel
                .GetDeclaredSymbol(classSyntax);

            return symbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(_method => _method.MethodKind != MethodKind.Constructor);
        }
    }
}
