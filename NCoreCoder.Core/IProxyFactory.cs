using System;
using System.Reflection;

namespace NCoreCoder.Aop
{
    public interface IProxyFactory
    {
        bool TryGetAop(MethodInfo targetMethod, out AopAttribute aopAttribute);
        bool TryGetInject(Type targetType, out InjectFlow injectFlow);
    }
}
