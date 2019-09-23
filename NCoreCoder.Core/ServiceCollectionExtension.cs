using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace NCoreCoder.Aop
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDynamicAop<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TService : class
            where TImplementation : class, TService
        {
            services.TryAddSingleton<IProxyFactory, ProxyFactory>();

            if (serviceLifetime == ServiceLifetime.Singleton)
            {
                services.TryAddSingleton<TImplementation>();
                services.TryAddSingleton(serviceProvider =>
                {
                    return CreateInstance<TService, TImplementation>(serviceProvider);
                });
            }

            if (serviceLifetime == ServiceLifetime.Transient)
            {
                services.TryAddTransient<TImplementation>();
                services.TryAddTransient(serviceProvider =>
                {
                    return CreateInstance<TService, TImplementation>(serviceProvider);
                });
            }

            if (serviceLifetime == ServiceLifetime.Scoped)
            {
                services.TryAddScoped<TImplementation>();
                services.TryAddScoped(serviceProvider =>
                {
                    using (var scopeService = serviceProvider.CreateScope())
                    {
                        return CreateInstance<TService, TImplementation>(scopeService.ServiceProvider);
                    }
                });
            }

            return services;
        }

        private static TService CreateInstance<TService, TImplementation>(IServiceProvider serviceProvider)
            where TService : class
            where TImplementation : class, TService
        {
            return ProxyGenerator<TService, TImplementation>.Create(serviceProvider);
        }

        public static IServiceCollection AddJitAop<TSource, TTarget>(this IServiceCollection services, ServiceLifetime serviceLifetime)
            where TTarget : class, TSource
        {
            if (typeof(TTarget).GetInterfaces().Length == 0)
                throw new Exception($"Not inherit interface");

            services.TryAddSingleton<TTarget>();

            var typeBuilderFactory = TypeBuilderFactory.Instance;

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var proxyType = typeBuilderFactory.CreateType(sourceType, targetType);

            if (serviceLifetime == ServiceLifetime.Singleton)
            {
                services.TryAddSingleton<AopActors>();
                services.TryAddSingleton(sourceType, proxyType);
            }

            if (serviceLifetime == ServiceLifetime.Scoped)
            {
                services.TryAddScoped<AopActors>();
                services.TryAddScoped(sourceType, proxyType);
            }

            if (serviceLifetime == ServiceLifetime.Transient)
            {
                services.TryAddTransient<AopActors>();
                services.TryAddTransient(sourceType, proxyType);
            }

            return services;
        }
    }
}
