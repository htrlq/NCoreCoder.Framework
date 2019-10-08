using System;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;

namespace NCoreCoder.Aop
{
    public interface IAopActors
    {
        object Execute(Func<AopContext, object> invoke, AopContext context);
        Task<TResult> ExecuteAsync<TResult>(Func<AopContext, Task<TResult>> invoke, AopContext context);
        Task InvokeAsync(Func<AopContext, Task> invoke, AopContext context);
    }

    public class DefaultAopActors : IAopActors
    {
        public object Execute(Func<AopContext, object> invoke, AopContext context)
        {
            if (context.MethodInfo.GetCustomAttribute<JitAopAttribute>() != null)
                return context.Execute();

            return invoke(context);
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<AopContext, Task<TResult>> invoke, AopContext context)
        {
            if (context.MethodInfo.GetCustomAttribute<JitAopAttribute>() != null)
                return await context.ExecuteAsync<TResult>();

            return await invoke(context);
        }

        public async Task InvokeAsync(Func<AopContext, Task> invoke, AopContext context)
        {
            if (context.MethodInfo.GetCustomAttribute<JitAopAttribute>() != null)
                await context.InvokeAsync();
            else
                await invoke(context);
        }
    }
}