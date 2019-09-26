using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;

namespace NCoreCoder.Aop
{
    //public class AopContext
    //{
    //    public object Execute(MethodInfo method, object instance, params object[] args)
    //    {
    //        var aopAttribute = method.GetCustomAttribute<JitAopAttribute>();

    //        aopAttribute.Before(method, args);
    //        var methodResult = method.Invoke(instance, args);

    //        if (methodResult is Task methodTask)
    //        {
    //            object result = null;

    //            var task = methodTask.ContinueWith((_task) =>
    //            {
    //                var property = _task.GetType().GetProperty("Result");

    //                result = property.GetValue(_task);

    //                aopAttribute.After(method, instance, args);
    //            });

    //            task.ConfigureAwait(false).GetAwaiter().GetResult();

    //            if (result == null)
    //                return result;

    //            var fromResultInvoke = typeof(Task).GetMethod("FromResult").MakeGenericMethod(result.GetType());
    //            var taskResult = fromResultInvoke.Invoke(null, new []{ result });

    //            return taskResult;
    //        }
    //        else
    //        {
    //            aopAttribute.After(method, args);

    //            return methodResult;
    //        }
    //    }

    //    public async Task<object> ExecuteAsync(MethodInfo method, object instance, params object[] args)
    //    {
    //        var aopAttribute = method.GetCustomAttribute<JitAopAttribute>();

    //        aopAttribute.Before(method, instance, args);
    //        var methodResult = (Task)method.Invoke(instance, args);

    //        object result = null;

    //        await methodResult.ContinueWith((_task) =>
    //        {
    //            var property = _task.GetType().GetProperty("Result");

    //            result = property.GetValue(_task);

    //            aopAttribute.After(method, instance, args);
    //        });

    //        return result;
    //    }
    //}
    public  interface  IAopContext
    {
        IServiceProvider ServiceProvider { get; }
        MethodInfo MethodInfo { get; }
        object Instance { get; }
        object[] Args { get; }
        object Execute();
        Task<TResult> ExecuteAsync<TResult>();
        Task InvokeAsync();
    }

    public class AopContext: IAopContext
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
            var result = MethodInfo.GetReflector().Invoke(Instance, Args);
            aopAttribute.After(MethodInfo.GetReflector(), Args);

            return result;
        }

        public async Task<TResult> ExecuteAsync<TResult>()
        {
            var aopAttribute = MethodInfo.GetReflector().GetCustomAttribute<JitAopAttribute>();

            await aopAttribute.BeforeAsync(MethodInfo.GetReflector(), Args);
            var result = await (Task<TResult>)MethodInfo.GetReflector().Invoke(Instance, Args);
            await aopAttribute.AfterAsync(MethodInfo.GetReflector(), Instance, Args);

            return result;
        }

        public async Task InvokeAsync()
        {
            var aopAttribute = MethodInfo.GetReflector().GetCustomAttribute<JitAopAttribute>();

            await aopAttribute.BeforeAsync(MethodInfo.GetReflector(), Args);
            await (Task)MethodInfo.GetReflector().Invoke(Instance, Args);
            await aopAttribute.AfterAsync(MethodInfo.GetReflector(), Instance, Args);
        }
    }
}