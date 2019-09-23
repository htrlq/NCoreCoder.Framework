using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NfxSample
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JitAopAttribute : Attribute
    {
        public virtual void Before(MethodInfo method, object instance, params object[] param)
        {
            //Console.WriteLine($"Before Name:{method.Name}");
        }

        public virtual void After(MethodInfo method, object instance, params object[] param)
        {
            //Console.WriteLine($"After Name:{method.Name}");
        }

        public virtual Task BeforeAsync(MethodInfo method, object instance, params object[] param)
        {
            //Console.WriteLine($"BeforeAsync Name:{method.Name}");
            return Task.CompletedTask;
        }

        public virtual Task AfterAsync(MethodInfo method, object instance, params object[] param)
        {
            //Console.WriteLine($"AfterAsync Name:{method.Name}");
            return Task.CompletedTask;
        }
    }
}