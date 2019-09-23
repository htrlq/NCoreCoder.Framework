using System;
using System.Reflection;

namespace NCoreCoder.Aop
{
    public interface IProxyFactory
    {
        bool TryGetAop(MethodInfo targetMethod, out DynamicAttribute dynamicAttribute);
        bool TryGetInject(Type targetType, out InjectFlow injectFlow);
    }
}
