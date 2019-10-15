using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NCoreCoder.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CodeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CodeAnalyzer";

        private BaseAnalyzerContext[] contexts = new BaseAnalyzerContext[]
        {
            new InterfaceContext(),
            new ClassContext()
        };

        public CodeAnalyzer()
        {
            var supportedDiagnostics = new List<DiagnosticDescriptor>();

            foreach(var analyzer in contexts)
            {
                supportedDiagnostics.AddRange(analyzer.Descriptors);
            }

            SupportedDiagnostics = ImmutableArray.Create(supportedDiagnostics.ToArray());
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Execute, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(Execute, SyntaxKind.ClassDeclaration);
        }

        private void Execute(SyntaxNodeAnalysisContext context)
        {
            foreach (var ansyzer in contexts.Where(_analyzer=>_analyzer.Kind == context.Node.Kind()))
            {
                ansyzer.Execute(context);
            }
        }
    }
}
