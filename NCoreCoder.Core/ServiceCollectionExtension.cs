using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace NCoreCoder.Aop
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddNCoreCoderAop<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
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
            return ProxyBuilder<TService, TImplementation>.Create(serviceProvider);
        }
    }
}
