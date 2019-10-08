using System;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;
using NCoreCoder.Aop;

namespace NCoreCoder.TestProject
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class TestJitAttribute : JitAopAttribute
    {
        public override void Before(MethodReflector method, object instance, params object[] param)
        {
            Console.WriteLine($"Before Name:{method.Name}");
        }

        public override void After(MethodReflector method, Exception exception, object instance, params object[] param)
        {
            Console.WriteLine($"After Name:{method.Name}");
        }

        public override Task BeforeAsync(MethodReflector method, object instance, params object[] param)
        {
            Console.WriteLine($"BeforeAsync Name:{method.Name}");
            return Task.CompletedTask;
        }

        public override Task AfterAsync(MethodReflector method, Exception exception, object instance, params object[] param)
        {
            Console.WriteLine($"AfterAsync Name:{method.Name}");
            return Task.CompletedTask;
        }
    }
}