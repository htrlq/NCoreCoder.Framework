using System;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;

namespace NCoreCoder.Aop
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JitAopAttribute : Attribute
    {
        public virtual void Before(MethodReflector method, object instance, params object[] param)
        {
            Console.WriteLine($"Before Name:{method.Name}");
        }

        public virtual void After(MethodReflector method, Exception exception, object instance, params object[] param)
        {
            Console.WriteLine($"After Name:{method.Name}");
        }

        public virtual Task BeforeAsync(MethodReflector method, object instance, params object[] param)
        {
            Console.WriteLine($"BeforeAsync Name:{method.Name}");
#if NETSTANDARD
            return Task.CompletedTask;    
#else
            return Task.FromResult<object>(null);
#endif
        }

        public virtual Task AfterAsync(MethodReflector method, Exception exception, object instance, params object[] param)
        {
            Console.WriteLine($"AfterAsync Name:{method.Name}");
#if NETSTANDARD
            return Task.CompletedTask;    
#else
            return Task.FromResult<object>(null);
#endif
        }
    }
}