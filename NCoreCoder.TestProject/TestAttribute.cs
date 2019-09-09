using NCoreCoder.Aop;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NCoreCoder.TestProject
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class TestAttribute:AopAttribute
    {
        protected override Task BeforeAsync(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine($"BeforeAsync {targetMethod.Name}");
            return base.BeforeAsync(targetMethod, args);
        }

        protected override Task AfterAsync(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine($"AfterAsync {targetMethod.Name}");

            return base.AfterAsync(targetMethod, args);
        }
    }
}
