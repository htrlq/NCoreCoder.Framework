using System.Reflection;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;

namespace NCoreCoder.Aop
{
    public interface IAopActors
    {
        object Execute(IAopContext context);
        Task<TResult> ExecuteAsync<TResult>(IAopContext context);
        Task InvokeAsync(IAopContext context);
    }

    public class DefaultAopActors: IAopActors
    {
        public object Execute(IAopContext context)
        {
            if (context.MethodInfo.GetReflector().GetCustomAttribute<JitAopAttribute>() != null)
                return context.Execute();

            return context.MethodInfo.GetReflector().Invoke(context.Instance, context.Args);
        }

        public async Task<TResult> ExecuteAsync<TResult>(IAopContext context)
        {
            if (context.MethodInfo.GetReflector().GetCustomAttribute<JitAopAttribute>() != null)
                return await context.ExecuteAsync<TResult>();

            var result = await (Task<TResult>) context.MethodInfo.GetReflector().Invoke(context.Instance, context.Args);
            return await Task.FromResult<TResult>((TResult)result);
        }

        public async Task InvokeAsync(IAopContext context)
        {
            if (context.MethodInfo.GetReflector().GetCustomAttribute<JitAopAttribute>() != null)
                await context.InvokeAsync();
            else
                await (Task)context.MethodInfo.GetReflector().Invoke(context.Instance, context.Args);
        }
    }
}