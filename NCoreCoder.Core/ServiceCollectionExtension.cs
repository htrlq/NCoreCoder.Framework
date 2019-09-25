using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

            services.TryAddSingleton<AopActors>();

            var typeBuilderFactory = TypeBuilderFactory.Instance;

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var proxyType = typeBuilderFactory.CreateType(sourceType, targetType);

            if (serviceLifetime == ServiceLifetime.Singleton)
            {
                services.TryAddSingleton<TTarget>();
                services.TryAddSingleton(sourceType, proxyType);
            }

            if (serviceLifetime == ServiceLifetime.Scoped)
            {
                services.TryAddScoped<TTarget>();
                services.TryAddScoped(sourceType, proxyType);
            }

            if (serviceLifetime == ServiceLifetime.Transient)
            {
                services.TryAddTransient<TTarget>();
                services.TryAddTransient(sourceType, proxyType);
            }

            return services;
        }

        public static IServiceProvider BuilderJit(this IServiceCollection services)
        {
            var builder = new JitAopBuilder(services);

            return builder.BuilderServiceProvider();
        }
    }

    public class JitAopBuilder
    {
        private List<ServiceDescriptor> _serviceDescriptors = new List<ServiceDescriptor>();

        public JitAopBuilder(IServiceCollection services)
        {
            _serviceDescriptors.AddRange(services.ToArray());
        }

        public void Add<TSource, TTarget>(ServiceLifetime serviceLifetime)
        {
            if (typeof(TTarget).GetInterfaces().Length == 0)
                throw new Exception($"Not inherit interface");

            var typeBuilderFactory = TypeBuilderFactory.Instance;

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var proxyType = typeBuilderFactory.CreateType(sourceType, targetType);

            _serviceDescriptors.Add(new ServiceDescriptor(typeof(TTarget),typeof(TTarget), serviceLifetime));
            _serviceDescriptors.Add(new ServiceDescriptor(sourceType, proxyType, serviceLifetime));
        }

        public IServiceProvider BuilderServiceProvider()
        {
            var services = new ServiceCollection();

            services.TryAddSingleton<AopActors>();

            foreach (var descriptor in _serviceDescriptors)
            {
                if (descriptor.ImplementationType?.GetInterfaces().Length > 0 &&
                    descriptor.ImplementationType?.GetCustomAttribute<JitInjectAttribute>() is JitInjectAttribute attribute &&
                    attribute != null
                )
                {
                    var typeBuilderFactory = TypeBuilderFactory.Instance;

                    var proxyType = typeBuilderFactory.CreateType(descriptor.ServiceType, descriptor.ImplementationType);

                    services.Add(new ServiceDescriptor(descriptor.ServiceType, proxyType, descriptor.Lifetime));
                    services.Add(new ServiceDescriptor(descriptor.ImplementationType, descriptor.ImplementationType,
                        descriptor.Lifetime));
                    //_serviceDescriptors.Add(new ServiceDescriptor(typeof(TTarget), typeof(TTarget), serviceLifetime));
                    //_serviceDescriptors.Add(new ServiceDescriptor(typeof(AopActors), typeof(AopActors), serviceLifetime));
                    //_serviceDescriptors.Add(new ServiceDescriptor(sourceType, proxyType, serviceLifetime));
                }
                else
                    services.Add(descriptor);
            }

            return services.BuildServiceProvider();
        }
    }

    public class JitServiceProviderFactory : IServiceProviderFactory<JitAopBuilder>
    {
        public JitAopBuilder CreateBuilder(IServiceCollection services)
        {
            return new JitAopBuilder(services);
        }

        public IServiceProvider CreateServiceProvider(JitAopBuilder containerBuilder)
        {
            return containerBuilder.BuilderServiceProvider();
        }
    }

    public class JitNewServiceProviderFactory : IServiceProviderFactory<ServiceCollection>
    {
        public ServiceCollection CreateBuilder(IServiceCollection services)
        {
            var _services = new ServiceCollection();

            _services.TryAddSingleton<AopActors>();

            foreach (var descriptor in services)
            {
                if (descriptor.ImplementationType?.GetInterfaces().Length > 0 &&
                    descriptor.ImplementationType?.GetCustomAttribute<JitInjectAttribute>() is JitInjectAttribute attribute &&
                    attribute != null
                )
                {
                    var typeBuilderFactory = TypeBuilderFactory.Instance;

                    var proxyType = typeBuilderFactory.CreateType(descriptor.ServiceType, descriptor.ImplementationType);

                    _services.Add(new ServiceDescriptor(descriptor.ServiceType, proxyType, descriptor.Lifetime));
                    _services.Add(new ServiceDescriptor(descriptor.ImplementationType, descriptor.ImplementationType,
                        descriptor.Lifetime));
                    //_serviceDescriptors.Add(new ServiceDescriptor(typeof(TTarget), typeof(TTarget), serviceLifetime));
                    //_serviceDescriptors.Add(new ServiceDescriptor(typeof(AopActors), typeof(AopActors), serviceLifetime));
                    //_serviceDescriptors.Add(new ServiceDescriptor(sourceType, proxyType, serviceLifetime));
                }
                else
                    _services.Add(descriptor);
            }

            return _services;
        }

        public IServiceProvider CreateServiceProvider(ServiceCollection containerBuilder)
        {
            return containerBuilder.BuildServiceProvider();
        }
    }//ServiceCollection
}
