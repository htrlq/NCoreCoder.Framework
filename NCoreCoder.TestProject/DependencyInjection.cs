using Microsoft.Extensions.DependencyInjection;
using System;

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

        public TService GetRequriedService<TService>()
        {
            return ServiceProvider.GetRequiredService<TService>();
        }

        public IServiceScope CreateScop()
        {
            return ServiceProvider.CreateScope();
        }
    }
}
