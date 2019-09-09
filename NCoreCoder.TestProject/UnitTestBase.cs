using Microsoft.Extensions.DependencyInjection;
using System;

namespace NCoreCoder.TestProject
{
    public class UnitTestBase
    {
        private IServiceProvider ServiceProvider { get; }

        public UnitTestBase()
        {
            var Collection = new ServiceCollection();

            ConfigService(Collection);

            ServiceProvider = Collection.BuildServiceProvider();
        }

        protected virtual void ConfigService(IServiceCollection services)
        {

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
