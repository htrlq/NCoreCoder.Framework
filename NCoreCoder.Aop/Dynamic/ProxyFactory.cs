using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NCoreCoder.Aop
{
    internal class ProxyFactory : IProxyFactory
    {
        private ConcurrentDictionary<MethodInfo, DynamicAttribute> attributes = new ConcurrentDictionary<MethodInfo, DynamicAttribute>();
        private ConcurrentDictionary<Type, InjectFlow> injectFlows = new ConcurrentDictionary<Type, InjectFlow>();

        public bool TryGetAop(MethodInfo targetMethod, out DynamicAttribute dynamicAttribute)
        {
            if (attributes.TryGetValue(targetMethod, out dynamicAttribute))
            {
                return dynamicAttribute != null;
            }
            else
            {
                dynamicAttribute = targetMethod.GetCustomAttribute<DynamicAttribute>();

                attributes.TryAdd(targetMethod, dynamicAttribute);

                return dynamicAttribute != null;
            }
        }

        public bool TryGetInject(Type targetType, out InjectFlow injectFlow)
        {
            if (injectFlows.TryGetValue(targetType, out injectFlow))
            {
                return injectFlow != null;
            }
            else
            {
                injectFlow = new InjectFlow(targetType);

                injectFlows.TryAdd(targetType, injectFlow);

                return injectFlow != null;
            }
        }
    }
}
