using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NCoreCoder.Aop
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AopAttribute:Attribute
    {
#if DEBUG
        public bool IsBefore { get; set; }
        public bool IsAfter { get; set; }
#endif

        protected virtual Task AfterAsync(MethodInfo targetMethod, object[] args)
        {
#if DEBUG
            IsBefore = true;
#endif
            return Task.CompletedTask;
        }

        protected virtual Task BeforeAsync(MethodInfo targetMethod, object[] args)
        {
#if DEBUG
            IsAfter = true;
#endif
            return Task.CompletedTask;
        }

        public async Task<object> ExecuteAsync(MethodInfo targetMethod, object instance, object[] args)
        {
            object result = null;

            await BeforeAsync(targetMethod, args);
            result = targetMethod.Invoke(instance, args);
            await AfterAsync(targetMethod, args);

            return result;
        }
    }
}
