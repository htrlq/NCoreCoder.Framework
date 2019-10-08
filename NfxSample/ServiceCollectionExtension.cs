using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NfxSample
{
    internal static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAutoInject(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var allTuples = assembly
                    .GetTypes()
                    .Where(_type => CustomAttributeExtensions.GetCustomAttribute<AutoInjectAttribute>((MemberInfo) _type) != null)
                    .Select(_type => (_type.GetCustomAttribute<AutoInjectAttribute>().ServiceType, _type,
                        _type.GetCustomAttribute<AutoInjectAttribute>().ServiceLifetime));

                foreach (var (servierType,implType,serviceLifetime) in allTuples)
                {
                    services.Add(new ServiceDescriptor(servierType, implType, serviceLifetime));
                }
            }

            return services;
        }
    }


    public class AutoInjectAttribute : Attribute
    {
        public Type ServiceType { get; }
        public ServiceLifetime ServiceLifetime { get; }

        public AutoInjectAttribute(Type serviceType, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;
            ServiceLifetime = serviceLifetime;
        }
    }

    public class ScopedAttribute : AutoInjectAttribute
    {
        public ScopedAttribute(Type serviceType) : base(serviceType, ServiceLifetime.Scoped)
        {
        }
    }

    public class SingletonAttribute : AutoInjectAttribute
    {
        public SingletonAttribute(Type serviceType) : base(serviceType, ServiceLifetime.Singleton)
        {
        }
    }

    public class TransientAttribute : AutoInjectAttribute
    {
        public TransientAttribute(Type serviceType) : base(serviceType, ServiceLifetime.Transient)
        {
        }
    }
}