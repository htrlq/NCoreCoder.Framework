using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace NCoreCoder.Analyzer
{
    public abstract class BaseAnalyzerContext
    {
        public abstract SyntaxKind Kind { get; }
        public abstract DiagnosticDescriptor[] Descriptors { get; }
        public abstract void Execute(SyntaxNodeAnalysisContext context);
    }
}
