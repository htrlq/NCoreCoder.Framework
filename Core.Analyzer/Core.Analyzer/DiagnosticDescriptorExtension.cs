using Microsoft.CodeAnalysis;

namespace NCoreCoder.Analyzer
{
    public static class DiagnosticDescriptorExtension
    {
        public static DiagnosticDescriptor InterfaceName => Create("I001", "接口命名错误", "接口必须是I开头，且第二个字母大写的命名", DiagnosticSeverity.Error);
        public static DiagnosticDescriptor InterfaceAttribute => Create("I002", "接口提示", "接口默认执行器:NCoreCoder.Aop.DefaultAopActors", DiagnosticSeverity.Warning);
        public static DiagnosticDescriptor InterfaceInherit => Create("I003", "接口提示", "AopActorsAttribute参数的Type必须继承自IAopActors", DiagnosticSeverity.Error);
        public static DiagnosticDescriptor InterfaceDefaultAopActors => Create("I004", "接口提示", "接口默认执行器是NCoreCoder.Aop.DefaultAopActor, 不用特意指定:NCoreCoder.Aop.DefaultAopActor", DiagnosticSeverity.Warning);

        public static DiagnosticDescriptor ClassMethod => Create("M001", "实体类", "{0}方法上建议打上继承自:NCoreCoder.Aop.JitAopAttribute的特性", DiagnosticSeverity.Warning);

        private static DiagnosticDescriptor Create(string id, string title, string message, DiagnosticSeverity level = DiagnosticSeverity.Error, string helpLinkUri = null)
        {
            var category = level.ToString();

            return new DiagnosticDescriptor(id, title, message, category, level, true, helpLinkUri: helpLinkUri);
        }
    }
}
