using AspectCore.Extensions.Reflection;
using NCoreCoder.Aop;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NCoreCoder.TestProject
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class TestAttribute:AopAttribute
    {
        protected override Task BeforeAsync(MethodReflector targetMethod, object[] args)
        {
            Console.WriteLine($"BeforeAsync {targetMethod.Name}");
            return base.BeforeAsync(targetMethod, args);
        }

        protected override Task AfterAsync(MethodReflector targetMethod, object[] args)
        {
            Console.WriteLine($"AfterAsync {targetMethod.Name}");

            return base.AfterAsync(targetMethod, args);
        }
    }
}
