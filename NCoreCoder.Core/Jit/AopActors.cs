using System.Threading.Tasks;

namespace NCoreCoder.Aop
{
    public class AopActors
    {
        public object Execute(AopContext context)
        {
            return context.Execute();
        }

        public Task<TResult> ExecuteAsync<TResult>(AopContext context)
        {
            return context.ExecuteAsync<TResult>();
        }

        public Task InvokeAsync(AopContext context)
        {
            return context.InvokeAsync();
        }
    }
}