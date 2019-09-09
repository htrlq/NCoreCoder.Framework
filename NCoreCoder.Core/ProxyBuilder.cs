using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NCoreCoder.Aop
{
    public class ProxyBuilder<TService, TImplementation> : DispatchProxy
        where TImplementation:TService
    {
        private TService _instance;
        private IProxyFactory _proxyFactory = null;

        public void SetFactory(IProxyFactory proxyFactory)
        {
            _proxyFactory = proxyFactory;
        }

        public static TService Create(IServiceProvider serviceProvider)
        {
            object proxy = Create<TService, ProxyBuilder<TService, TImplementation>>();

            var instance = serviceProvider.GetRequiredService<TImplementation>();
            var factory = serviceProvider.GetRequiredService<IProxyFactory>();

            ((ProxyBuilder<TService,TImplementation>)proxy)._instance = instance;
            ((ProxyBuilder<TService, TImplementation>)proxy).SetFactory(factory);

            return (TService)proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (_proxyFactory.TryGetAttribute(targetMethod, out AopAttribute aopAttribute))
            {
                var task = aopAttribute.ExecuteAsync(targetMethod, _instance, args);

                var awaiter = task.ConfigureAwait(true).GetAwaiter();
                var result = awaiter.GetResult();

#if DEBUG
                if (!(aopAttribute.IsBefore && aopAttribute.IsAfter))
                {
                    aopAttribute.IsBefore = false;
                    aopAttribute.IsAfter = false;

                    throw new Exception($"Execute Error");
                }
#endif

                return result;
            }

            return targetMethod.Invoke(_instance, args);
        }
    }
}
