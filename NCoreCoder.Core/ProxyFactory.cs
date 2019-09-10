using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NCoreCoder.Aop
{
    internal class ProxyFactory : IProxyFactory
    {
        private ConcurrentDictionary<MethodInfo, AopAttribute> attributes = new ConcurrentDictionary<MethodInfo, AopAttribute>();
        private ConcurrentDictionary<Type, InjectFlow> injectFlows = new ConcurrentDictionary<Type, InjectFlow>();

        public bool TryGetAop(MethodInfo targetMethod, out AopAttribute aopAttribute)
        {
            if (attributes.TryGetValue(targetMethod, out aopAttribute))
            {
                return aopAttribute != null;
            }
            else
            {
                aopAttribute = targetMethod.GetCustomAttribute<AopAttribute>();

                attributes.TryAdd(targetMethod, aopAttribute);

                return aopAttribute != null;
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
