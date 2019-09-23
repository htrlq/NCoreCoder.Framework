using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCoreCoder.Aop;
using Xunit;

namespace NCoreCoder.TestProject
{
    public class TestJit
    {
        [Fact]
        public async void Singleton()
        {
            var instance =
                new DependencyInjection()
                .ConfigService(services =>
                {
                    services.AddSingleton<ITest, Test>();
                    services.AddJitAop<IJitService, JitService>(ServiceLifetime.Singleton);
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
                .ConfigService(services =>
                {
                    services.AddTransient<ITest, Test>();
                    services.AddJitAop<IJitService, JitService>(ServiceLifetime.Transient);
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
                .ConfigService(services =>
                {
                    services.AddScoped<ITest, Test>();
                    services.AddJitAop<IJitService, JitService>(ServiceLifetime.Scoped);
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

    public interface IJitService
    {
        void TestVoid();
        int TestInt();
        Task TestAsync();
        Task<int> TestIntAsync();
    }

    internal class JitService : IJitService
    {
        public ITest Test { get; }

        public JitService(ITest test)
        {
            Test = test;
        }

        [TestJit]
        public Task TestAsync()
        {
            return Test.TestAsync();
        }

        [TestJit]
        public int TestInt()
        {
            return Test.TestInt();
        }

        [TestJit]
        public Task<int> TestIntAsync()
        {
            return Test.TestIntAsync();
        }

        [TestJit]
        public void TestVoid()
        {
            Test.TestVoid();
        }
    }
}
