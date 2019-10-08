using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCoreCoder.Aop;
using Xunit;

namespace NCoreCoder.TestProject
{
    public class TestJitServiceProviderNew
    {
        [Fact]
        public async void Singleton()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddSingleton<ITest, Test>();
                    services.AddSingleton<IJitServices, JitServices>();
                    services.AddSingleton<IJitServicesNew, JitServices>();
                });

            var jitServices = instance.GetRequriedService<IJitServices>();
            await DebugAsync(jitServices);

            var jitServicesNew = instance.GetRequriedService<IJitServicesNew>();
            await DebugAsync(jitServicesNew);
        }

        [Fact]
        public async void Transient()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddTransient<ITest, Test>();
                    services.AddTransient<IJitServices, JitServices>();
                    services.AddTransient<IJitServicesNew, JitServices>();
                });

            var jitServices = instance.GetRequriedService<IJitServices>();
            await DebugAsync(jitServices);

            var jitServicesNew = instance.GetRequriedService<IJitServicesNew>();
            await DebugAsync(jitServicesNew);
        }

        [Fact]
        public async void Scoped()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddScoped<ITest, Test>();
                    services.AddScoped<IJitServices, JitServices>();
                    services.AddScoped<IJitServicesNew, JitServices>();
                });

            using (var serviceScope = instance.CreateScoped())
            {
                var jitServices = serviceScope.GetRequriedService<IJitServices>();
                await DebugAsync(jitServices);

                var jitServicesNew = serviceScope.GetRequriedService<IJitServicesNew>();
                await DebugAsync(jitServicesNew);
            }
        }

        private async Task DebugAsync(IJitServices services)
        {
            services.TestVoid();

            var resultInt = services.TestInt();
            Assert.Equal<int>(1000, resultInt);

            await services.TestAsync();

            var asyncInt = await services.TestIntAsync();
            Assert.Equal<int>(900, asyncInt);
        }
    }

    public interface IJitServices
    {
        void TestVoid();
        int TestInt();
        Task TestAsync();
        Task<int> TestIntAsync();
    }

    public interface IJitServicesNew: IJitServices
    {
    }

    [JitInject]
    internal class JitServices : IJitServicesNew
    {
        public ITest Test { get; }

        public JitServices(ITest test)
        {
            Test = test;
        }

        public Task TestAsync()
        {
            return Test.TestAsync();
        }

        public int TestInt()
        {
            return Test.TestInt();
        }

        public Task<int> TestIntAsync()
        {
            return Test.TestIntAsync();
        }

        public void TestVoid()
        {
            Test.TestVoid();
        }
    }
}
