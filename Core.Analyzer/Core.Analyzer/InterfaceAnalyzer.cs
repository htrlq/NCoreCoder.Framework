using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NCoreCoder.Aop;
using NCoreCoder.Aop.Jit;
using NCoreCoder.Symbol;
using System.Linq;
using System.Text.RegularExpressions;

namespace NCoreCoder.Analyzer
{
    public class InterfaceContext : BaseAnalyzerContext
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
            else
            {
                if (!CheckInterfaceCtorAttributeDefault(context))
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptorExtension.InterfaceDefaultAopActors, context.Node.GetLocation()));
                else
                {
                    if (!CheckInterfaceCtorAttributeInherit(context))
                        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptorExtension.InterfaceInherit, context.Node.GetLocation()));
                }
            }
        }

        private bool CheckInterfaceName(SyntaxNodeAnalysisContext context)
        {
            var _interface = context.Node as InterfaceDeclarationSyntax;

            return Regex.IsMatch(_interface.Identifier.Text, "(I[A-Z]{1})");

            return false;
        }

        private bool CheckInterfaceCtorAttribute(SyntaxNodeAnalysisContext context)
        {
            var _interface = context.Node as InterfaceDeclarationSyntax;

            if (_interface.AttributeLists == null)
                return false;

            var typeInfo = new SymbolType(context);

            return typeInfo.IsAttribute<AopActorsAttribute>();
        }

        private bool CheckInterfaceCtorAttributeDefault(SyntaxNodeAnalysisContext context)
        {
            var defaultAopActors = context.ToSymbol<DefaultAopActors>();

            var typeInfo = new SymbolType(context);

            return typeInfo.IsAttribute<AopActorsAttribute>() && typeInfo.GetAttributes().Any(_attribute=> _attribute.Args[0] == defaultAopActors);
        }

        private bool CheckInterfaceCtorAttributeInherit(SyntaxNodeAnalysisContext context)
        {
            var typeInfo = new SymbolType(context);

            return typeInfo.IsAttribute<AopActorsAttribute>() && typeInfo.GetAttributes().Any(_attribute => context.InterfaceInherit<IAopActors>(_attribute.Args[0]));
        }
    }
}
