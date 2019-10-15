using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Code.Analyzer
{
    public abstract class BaseAnalyzer
    {
        public abstract SyntaxKind Kind { get; }
        public abstract DiagnosticDescriptor[] Descriptors { get; }
        public abstract void Execute(SyntaxNodeAnalysisContext context);
    }

    public class ClassAnalyzer : BaseAnalyzer
    {
        public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        public override DiagnosticDescriptor[] Descriptors => throw new System.NotImplementedException();

        public override void Execute(SyntaxNodeAnalysisContext context)
        {
            var classSyntax = context.Node as ClassDeclarationSyntax;
        }
    }
}
