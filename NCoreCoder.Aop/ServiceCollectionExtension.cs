using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NCoreCoder.Aop.Jit;

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

        public static IServiceProvider BuilderJit(this IServiceCollection services)
        {
            var builder = new JitAopBuilder(services);

            return builder.BuilderServiceProvider();
        }
    }

    public class JitAopBuilder
    {
        private List<ServiceDescriptor> _serviceDescriptors = new List<ServiceDescriptor>();
        private TypeBuilderFactory typeBuilderFactory = new TypeBuilderFactory();

        public JitAopBuilder(IServiceCollection services)
        {
            _serviceDescriptors.AddRange(services.ToArray());
        }

        public void Add<TSource, TTarget>(ServiceLifetime serviceLifetime)
        {
            if (typeof(TTarget).GetInterfaces().Length == 0)
                throw new Exception($"Not inherit interface");

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var attribute = sourceType.GetCustomAttribute<AopActorsAttribute>();
            var proxyType = typeBuilderFactory.CreateType(attribute == null ? typeof(DefaultAopActors) : attribute.ActorsType, sourceType, targetType);

            _serviceDescriptors.Add(new ServiceDescriptor(typeof(TTarget),typeof(TTarget), serviceLifetime));
            _serviceDescriptors.Add(new ServiceDescriptor(sourceType, proxyType, serviceLifetime));
        }

        public IServiceProvider BuilderServiceProvider()
        {
            var services = new ServiceCollection();
            services.TryAddSingleton<DefaultAopActors>();

            foreach (var descriptor in _serviceDescriptors)
            {
                if (
                    descriptor.ServiceType != descriptor.ImplementationType &&
                    descriptor.ImplementationType?.GetInterfaces().Length > 0 &&
                    descriptor.ImplementationType?.GetCustomAttribute<JitInjectAttribute>() is JitInjectAttribute attribute &&
                    attribute != null
                )
                {
                    if (!typeBuilderFactory.IsExits(descriptor.ServiceType))
                    {
                        var _attribute = descriptor.ServiceType.GetCustomAttribute<AopActorsAttribute>();
                        var proxyType = typeBuilderFactory.CreateType(_attribute == null ? typeof(DefaultAopActors) : _attribute.ActorsType, descriptor.ServiceType, descriptor.ImplementationType);

                        services.Add(new ServiceDescriptor(descriptor.ServiceType, proxyType, descriptor.Lifetime));
                        services.Add(new ServiceDescriptor(descriptor.ImplementationType, descriptor.ImplementationType,
                            descriptor.Lifetime));
                    }
                    //_serviceDescriptors.Add(new ServiceDescriptor(typeof(TTarget), typeof(TTarget), serviceLifetime));
                    //_serviceDescriptors.Add(new ServiceDescriptor(typeof(AopActors), typeof(AopActors), serviceLifetime));
                    //_serviceDescriptors.Add(new ServiceDescriptor(sourceType, proxyType, serviceLifetime));
                }
                else
                    services.Add(descriptor);
            }

#if _Nfx
            typeBuilderFactory.Save();
#endif

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

            var typeBuilderFactory = new TypeBuilderFactory();
            _services.TryAddSingleton<DefaultAopActors>();

            foreach (var descriptor in services)
            {
                if (descriptor.ServiceType != descriptor.ImplementationType &&
                    descriptor.ImplementationType?.GetInterfaces().Length > 0 &&
                    descriptor.ImplementationType?.GetCustomAttribute<JitInjectAttribute>() is JitInjectAttribute attribute &&
                    attribute != null
                )
                {
                    if (!typeBuilderFactory.IsExits(descriptor.ServiceType))
                    {
                        var _attribute = descriptor.ServiceType.GetCustomAttribute<AopActorsAttribute>();
                        var proxyType = typeBuilderFactory.CreateType(
                            _attribute == null ? typeof(DefaultAopActors) : _attribute.ActorsType,
                            descriptor.ServiceType, descriptor.ImplementationType);

                        _services.Add(new ServiceDescriptor(descriptor.ServiceType, proxyType, descriptor.Lifetime));
                        _services.Add(new ServiceDescriptor(descriptor.ImplementationType,
                            descriptor.ImplementationType,
                            descriptor.Lifetime));
                    }

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
