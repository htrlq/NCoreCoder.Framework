using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;

namespace NCoreCoder.Aop
{
    public class AopContext
    {
        public IServiceProvider ServiceProvider { get; }
        public MethodInfo MethodInfo { get; }
        public object Instance { get; }
        public object[] Args { get; }

        public AopContext(IServiceProvider serviceProvider, object instance, MethodInfo methodInfo, object[] args)
        {
            //typeof(AopActors),
            //targetType,
            //typeof(IServiceProvider),

            ServiceProvider = serviceProvider;

            MethodInfo = methodInfo;
            Instance = instance;
            Args = args;
        }

        public object Execute()
        {
            var aopAttribute = MethodInfo.GetReflector().GetCustomAttribute<JitAopAttribute>();

            aopAttribute.Before(MethodInfo.GetReflector(), Args);

            try
            {
                var result = MethodInfo.GetReflector().Invoke(Instance, Args);
                aopAttribute.After(MethodInfo.GetReflector(), null, Instance, Args);

                return result;
            }
            catch (Exception e)
            {
                aopAttribute.After(MethodInfo.GetReflector(), e, Instance, Args);

                throw;
            }
        }

        public async Task<TResult> ExecuteAsync<TResult>()
        {
            var aopAttribute = MethodInfo.GetReflector().GetCustomAttribute<JitAopAttribute>();

            await aopAttribute.BeforeAsync(MethodInfo.GetReflector(), Args);

            try
            {
                var result = await (Task<TResult>)MethodInfo.GetReflector().Invoke(Instance, Args);
                await aopAttribute.AfterAsync(MethodInfo.GetReflector(), null, Instance, Args);

                return result;
            }
            catch (Exception e)
            {
                await aopAttribute.AfterAsync(MethodInfo.GetReflector(), e, Instance, Args);
                throw;
            }
        }

        public async Task InvokeAsync()
        {
            var aopAttribute = MethodInfo.GetReflector().GetCustomAttribute<JitAopAttribute>();

            await aopAttribute.BeforeAsync(MethodInfo.GetReflector(), Args);

            try
            {
                await (Task)MethodInfo.GetReflector().Invoke(Instance, Args);
                await aopAttribute.AfterAsync(MethodInfo.GetReflector(), null, Instance, Args);
            }
            catch (Exception e)
            {
                await aopAttribute.AfterAsync(MethodInfo.GetReflector(), e, Instance, Args);
                throw;
            }
        }
    }
}