using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NfxSample
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class TestJitAttribute : JitAopAttribute
    {
        public override void Before(MethodInfo method, object instance, params object[] param)
        {
            Console.WriteLine($"Before Name:{method.Name}");
        }

        public override void After(MethodInfo method, object instance, params object[] param)
        {
            Console.WriteLine($"After Name:{method.Name}");
        }

        public override Task BeforeAsync(MethodInfo method, object instance, params object[] param)
        {
            Console.WriteLine($"BeforeAsync Name:{method.Name}");
            return Task.CompletedTask;
        }

        public override Task AfterAsync(MethodInfo method, object instance, params object[] param)
        {
            Console.WriteLine($"AfterAsync Name:{method.Name}");
            return Task.CompletedTask;
        }
    }
}