using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Code.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CodeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CodeAnalyzer";

        private BaseAnalyzer[] analyzers = new BaseAnalyzer[]
        {
            new InterfaceAnalyzer()
        };

        public CodeAnalyzer()
        {
            var supportedDiagnostics = new List<DiagnosticDescriptor>();

            foreach(var analyzer in analyzers)
            {
                supportedDiagnostics.AddRange(analyzer.Descriptors);
            }

            SupportedDiagnostics = ImmutableArray.Create(supportedDiagnostics.ToArray());
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Execute, SyntaxKind.InterfaceDeclaration);
            context.RegisterSyntaxNodeAction(Class, SyntaxKind.ClassDeclaration);
        }

        private void Execute(SyntaxNodeAnalysisContext context)
        {
            foreach (var ansyzer in analyzers.Where(_analyzer=>_analyzer.Kind == context.Node.Kind()))
            {
                ansyzer.Execute(context);
            }
        }

        private void Class(SyntaxNodeAnalysisContext context)
        {
            var jitAopAttribute = context.Compilation.GetTypeByMetadataName("NCoreCoder.Aop.JitAopAttribute");

            if (IsInheritInterface(context) && IsAopActorsAttribute(context))
            {
                foreach(var method in GetMethods(context))
                {
                    var attri = method.GetAttributes();
                    if (!method.GetAttributes().Any(
                        _attribute =>
                            _attribute.AttributeClass.Equals(jitAopAttribute) ||
                            _attribute.AttributeClass.AllInterfaces.Any(_attri =>
                                _attri.Equals(jitAopAttribute)
                            )
                       )
                    )
                    {
                        var descriptor = new DiagnosticDescriptor("E001", "实体类", $"{method.Name} 方法建议打上继承自:NCoreCoder.Aop.JitAopAttribute的特性", "Warning", DiagnosticSeverity.Warning, true);
                        var diagnostic = Diagnostic.Create(descriptor, method.Locations.FirstOrDefault());

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
            var classSyntax = context.Node as ClassDeclarationSyntax;
            var attributeSymbol = context.Compilation.GetTypeByMetadataName("NCoreCoder.Aop.Jit.AopActorsAttribute");

            var symbol = context.SemanticModel
                .GetDeclaredSymbol(classSyntax);

            foreach(var _interface in symbol.AllInterfaces)
            {
                if (_interface
                    .GetAttributes()
                    .FirstOrDefault(_attribute => _attribute.AttributeClass.Equals(attributeSymbol)) is AttributeData attribute && attribute != null)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<IMethodSymbol> GetMethods(SyntaxNodeAnalysisContext context)
        {
            var classSyntax = context.Node as ClassDeclarationSyntax;

            var symbol = context.SemanticModel
                .GetDeclaredSymbol(classSyntax);

            return symbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(_method=>_method.MethodKind != MethodKind.Constructor);
        }
    }
}
