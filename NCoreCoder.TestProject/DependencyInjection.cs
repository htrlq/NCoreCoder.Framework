using Microsoft.Extensions.DependencyInjection;
using System;
using NCoreCoder.Aop;

namespace NCoreCoder.TestProject
{
    public class DependencyInjection
    {
        private ServiceCollection collection = new ServiceCollection();
        private IServiceProvider ServiceProvider { get; }

        public DependencyInjection()
        {
        }

        public DependencyInjection(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public DependencyInjection ConfigService(Action<IServiceCollection> func)
        {
            func.Invoke(collection);

            return new DependencyInjection(collection.BuildServiceProvider());
        }

        public DependencyInjection BuilderService(Action<IServiceCollection> func)
        {
            func.Invoke(collection);

            return new DependencyInjection(collection.BuilderJit());
        }

        public TService GetRequriedService<TService>()
        {
            return ServiceProvider.GetRequiredService<TService>();
        }

        public IServiceScope CreateScoped()
        {
            return ServiceProvider.CreateScope();
        }
    }
}
