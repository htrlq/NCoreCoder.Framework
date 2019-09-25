using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;

namespace NCoreCoder.Aop
{
    public class AopActors
    {
        public object ProxyExecute(AopContext context)
        {
            return context.Execute();
        }

        public Task<TResult> ProxyExecuteAsync<TResult>(AopContext context)
        {
            return context.ExecuteAsync<TResult>();
        }

        public Task ProxyInvokeAsync(AopContext context)
        {
            return context.InvokeAsync();
        }

        public object Execute(AopContext context)
        {
            return context.MethodInfo.GetReflector().Invoke(context.Instance, context.Args);
        }

        public async Task<TResult> ExecuteAsync<TResult>(AopContext context)
        {
            var result = await (Task<TResult>) context.MethodInfo.GetReflector().Invoke(context.Instance, context.Args);
            return await Task.FromResult<TResult>((TResult)result);
        }

        public async Task InvokeAsync(AopContext context)
        {
            await (Task)context.MethodInfo.GetReflector().Invoke(context.Instance, context.Args);
        }
    }
}