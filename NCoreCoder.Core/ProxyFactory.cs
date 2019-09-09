using System.Collections.Concurrent;
using System.Reflection;

namespace NCoreCoder.Aop
{
    internal class ProxyFactory : IProxyFactory
    {
        private ConcurrentDictionary<MethodInfo, AopAttribute> dictionary = new ConcurrentDictionary<MethodInfo, AopAttribute>();

        public bool TryGetAttribute(MethodInfo targetMethod, out AopAttribute aopAttribute)
        {
            if (dictionary.TryGetValue(targetMethod, out aopAttribute))
            {
                return aopAttribute != null;
            }
            else
            {
                aopAttribute = targetMethod.GetCustomAttribute<AopAttribute>();

                dictionary.TryAdd(targetMethod, aopAttribute);

                return aopAttribute != null;
            }
        }
    }
}
