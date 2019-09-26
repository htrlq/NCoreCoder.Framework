using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NCoreCoder.Aop
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DynamicAttribute:Attribute
    {
#if DEBUG
        public bool IsBefore { get; set; }
        public bool IsAfter { get; set; }
#endif

        protected virtual Task AfterAsync(MethodReflector targetMethod, object[] args)
        {
#if DEBUG
            IsAfter = true;
#endif
            return Task.CompletedTask;
        }

        protected virtual Task BeforeAsync(MethodReflector targetMethod, object[] args)
        {
#if DEBUG
            IsBefore = true;
#endif
            return Task.CompletedTask;
        }

        public async Task<object> ExecuteAsync(MethodReflector targetMethod, object instance, object[] args)
        {
            object result = null;

            await BeforeAsync(targetMethod, args);

            result = targetMethod.Invoke(instance, args);

            await AfterAsync(targetMethod, args);

            return result;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class InjectAttribute:Attribute
    {
        public Type SourceType { get; }

        public InjectAttribute(Type sourceType)
        {
            SourceType = sourceType;
        }
    }

    public class InjectFlow
    {
        private IEnumerable<PropertyInfo> propertys;

        public InjectFlow(Type serviceType)
        {
            propertys = serviceType
                .GetTypeInfo()
                .GetProperties()
                .Where(_property => _property.GetReflector().GetCustomAttribute<InjectAttribute>() != null);
        }

        public void Inject(object instance, IServiceProvider serviceProvider)
        {
            foreach(var property in propertys)
            {
                var attribute = property.GetReflector().GetCustomAttribute<InjectAttribute>();
                var serviceType = attribute.SourceType;
                var service = serviceProvider.GetRequiredService(serviceType);

                property.GetReflector().SetValue(instance, service);
            }
        }
    }
}
