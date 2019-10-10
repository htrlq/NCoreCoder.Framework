using AspectCore.Extensions.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NCoreCoder.Aop
{
#if NETSTANDARD
    public class ProxyGenerator<TService, TImplementation> : DispatchProxy
        where TImplementation:TService
    {
        private TService _instance;
        private IProxyFactory _proxyFactory = null;

        private void SetFactory(IProxyFactory proxyFactory)
        {
            _proxyFactory = proxyFactory;
        }

        private void Inject(Type serviceType, IServiceProvider serviceProvider)
        {
            if (_proxyFactory.TryGetInject(serviceType, out InjectFlow injectFlow))
            {
                injectFlow.Inject(_instance, serviceProvider);
            }
        }

        public static TService Create(IServiceProvider serviceProvider)
        {
            object proxy = Create<TService, ProxyGenerator<TService, TImplementation>>();

            var instance = serviceProvider.GetRequiredService<TImplementation>();
            var factory = serviceProvider.GetRequiredService<IProxyFactory>();

            ((ProxyGenerator<TService,TImplementation>)proxy)._instance = instance;
            ((ProxyGenerator<TService, TImplementation>)proxy).SetFactory(factory);
            ((ProxyGenerator<TService, TImplementation>)proxy).Inject(typeof(TService),serviceProvider);

            return (TService)proxy;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (_proxyFactory.TryGetAop(targetMethod, out DynamicAttribute aopAttribute))
            {
                var task = aopAttribute.ExecuteAsync(targetMethod.GetReflector(), _instance, args);

                var awaiter = task.ConfigureAwait(true).GetAwaiter();
                var result = awaiter.GetResult();

                return result;
            }

            return targetMethod.Invoke(_instance, args);
        }
    }
#endif
}
