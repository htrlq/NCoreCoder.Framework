using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCoreCoder.Aop;
using Xunit;

namespace NCoreCoder.TestProject
{
    public class TestJitServiceProvider
    {
        [Fact]
        public async void Singleton()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddSingleton<ITest, Test>();
                    services.AddSingleton<IJitService, JitService>();
                });

            var test = instance.GetRequriedService<IJitService>();

            test.TestVoid();

            var resultInt = test.TestInt();
            Assert.Equal<int>(1000, resultInt);

            await test.TestAsync();

            var asyncInt = await test.TestIntAsync();
            Assert.Equal<int>(900, asyncInt);
        }

        [Fact]
        public async void Transient()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddTransient<ITest, Test>();
                    services.AddTransient<IJitService, JitService>();
                });

            var test = instance.GetRequriedService<IJitService>();

            test.TestVoid();

            var resultInt = test.TestInt();
            Assert.Equal<int>(1000, resultInt);

            await test.TestAsync();

            var asyncInt = await test.TestIntAsync();
            Assert.Equal<int>(900, asyncInt);
        }

        [Fact]
        public async void Scoped()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddScoped<ITest, Test>();
                    services.AddScoped<IJitService, JitService>();
                });

            using (var serviceScope = instance.CreateScoped())
            {
                var test = serviceScope.GetRequriedService<IJitService>();

                test.TestVoid();

                var resultInt = test.TestInt();
                Assert.Equal<int>(1000, resultInt);

                await test.TestAsync();

                var asyncInt = await test.TestIntAsync();
                Assert.Equal<int>(900, asyncInt);
            }
        }
    }
}
