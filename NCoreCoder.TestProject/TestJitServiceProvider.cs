using System;
using System.Text;
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

            await DebugAsync(test);
        }

        [Fact]
        public async void Transient()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddTransient<ITest, Test>();
                    //services.AddJitAop<IJitService, JitService>(ServiceLifetime.Transient);
                    services.AddTransient<IJitService, JitService>();
                });

            var test = instance.GetRequriedService<IJitService>();

            await DebugAsync(test);
        }

        [Fact]
        public async void Scoped()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddScoped<ITest, Test>();
                    //services.AddJitAop<IJitService, JitService>(ServiceLifetime.Scoped);
                    services.AddScoped<IJitService, JitService>();
                });

            using (var serviceScope = instance.CreateScoped())
            {
                var test = serviceScope.GetRequriedService<IJitService>();

                await DebugAsync(test);
            }
        }

        private async Task DebugAsync(IJitService test)
        {
            test.TestVoid();

            var resultInt = test.TestInt();
            Assert.Equal<int>(1000, resultInt);

            await test.TestAsync();

            var asyncInt = await test.TestIntAsync();
            Assert.Equal<int>(900, asyncInt);

            var valueAsync = await test.ValueIntAsync();
            Assert.Equal<int>(1000,valueAsync);

            var stringBuild = await test.StringBuilderAsync();
            var value = stringBuild.ToString();
            Assert.Equal<string>("hello", value);
        }
    }

    public interface IJitService
    {
        void TestVoid();
        int TestInt();
        Task TestAsync();
        Task<int> TestIntAsync();
        ValueTask<int> ValueIntAsync();
        ValueTask<StringBuilder> StringBuilderAsync();
    }

    [JitInject]
    internal class JitService : IJitService
    {
        public ITest Test { get; }

        public JitService(ITest test)
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

        public ValueTask<int> ValueIntAsync()
        {
            return new ValueTask<int>(1000);
        }

        public ValueTask<StringBuilder> StringBuilderAsync()
        {
            return new ValueTask<StringBuilder>(new StringBuilder("hello"));
        }

        public void TestVoid()
        {
            Test.TestVoid();
        }
    }
}
