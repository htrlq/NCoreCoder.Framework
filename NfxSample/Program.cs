using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NfxSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = new DependencyInjection()
                .ConfigService(services =>
            {
                services.AddSingleton<ITest, Test>();
                services.AddJitAop<IJitService, JitService>(ServiceLifetime.Singleton);
            });

            var service = instance.GetRequriedService<IJitService>();

            var result = service.TestInt();
        }
    }

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddJitAop<TSource, TTarget>(this IServiceCollection services, ServiceLifetime serviceLifetime)
            where TTarget : class, TSource
        {
            services.TryAddSingleton<AopActors>();
            services.TryAddSingleton<IMethodFactory, MethodFactory>();

            if (typeof(TTarget).GetInterfaces().Length == 0)
                throw new Exception($"Not inherit interface");

            services.TryAddSingleton<TTarget>();

            var typeBuilderFactory = TypeBuilderFactory.Instance;

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var proxyType = typeBuilderFactory.CreateType(sourceType, targetType);

            if (serviceLifetime == ServiceLifetime.Singleton)
                services.TryAddSingleton(sourceType, proxyType);

            if (serviceLifetime == ServiceLifetime.Scoped)
                services.TryAddScoped(sourceType, proxyType);

            if (serviceLifetime == ServiceLifetime.Transient)
                services.TryAddTransient(sourceType, proxyType);

            return services;
        }
    }
}
