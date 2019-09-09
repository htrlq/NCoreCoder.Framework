using System.Reflection;

namespace NCoreCoder.Aop
{
    public interface IProxyFactory
    {
        bool TryGetAttribute(MethodInfo targetMethod, out AopAttribute aopAttribute);
    }
}
