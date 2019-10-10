using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCoreCoder.Aop;
using Xunit;

namespace NCoreCoder.TestProject
{
    public class TestJitServicePropety
    {
        [Fact]
        public async void Singleton()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddSingleton<ITest, Test>();
                    services.AddSingleton<ITestClass, TestClass>();
                });

            var jitServices = instance.GetRequriedService<ITestClass>();
            await DebugAsync(jitServices);
        }

        [Fact]
        public async void Transient()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddTransient<ITest, Test>();
                    services.AddTransient<ITestClass, TestClass>();
                });

            var jitServices = instance.GetRequriedService<ITestClass>();
            await DebugAsync(jitServices);
        }

        [Fact]
        public async void Scoped()
        {
            var instance =
                new DependencyInjection()
                .BuilderService(services =>
                {
                    services.AddScoped<ITest, Test>();
                    services.AddTransient<ITestClass, TestClass>();
                });

            using (var serviceScope = instance.CreateScoped())
            {
                var jitServices = serviceScope.GetRequriedService<ITestClass>();
                await DebugAsync(jitServices);
            }
        }

        private async Task DebugAsync(ITestClass services)
        {
            var result = await services.ResultIntAsync();

            if (result != 100)
                throw new Exception($"Assert not Equals");

            await services.ReturnAsync();

            var hello = services.Hello();

            if (!hello.Equals("Hello"))
                throw new Exception($"Assert not Equals");

            var valueAsync = await services.ValueIntAsync();
            Assert.Equal<int>(1000, valueAsync);

            var stringBuild = await services.StringBuilderAsync();
            var value = stringBuild.ToString();
            Assert.Equal<string>("hello", value);

            var test = services.Test;

            await DebugTestAsync(test);
        }

        private async Task DebugTestAsync(ITest services)
        {
            services.TestVoid();
            var intResult = services.TestInt();

            if (intResult != 1000)
                throw new Exception($"Assert not Equals");

            await services.TestAsync();
            var intAsyncResult = await services.TestIntAsync();

            if (intAsyncResult != 900)
                throw new Exception($"Assert not Equals");
        }
    }

    [JitInject]
    public class TestClass : ITestClass
    {
        public ITest Test { get; set; }

        public TestClass(ITest test)
        {
            Test = test;
        }

        [JitAop]
        public string Hello()
        {
            Console.WriteLine("Hello");
            return "Hello";
        }

        public Task<int> ResultIntAsync()
        {
            Console.WriteLine("ResultIntAsync");
            return Task.FromResult(100);
        }

        public Task ReturnAsync()
        {
            Console.WriteLine("ReturnAsync");
            return Task.CompletedTask;
        }

        public ValueTask<int> ValueIntAsync()
        {
            return new ValueTask<int>(1000);
        }

        public ValueTask<StringBuilder> StringBuilderAsync()
        {
            return new ValueTask<StringBuilder>(new StringBuilder("hello"));
        }
    }

    public interface ITestClass
    {
        ITest Test { get; set; }
        string Hello();
        Task<int> ResultIntAsync();
        Task ReturnAsync();
        ValueTask<int> ValueIntAsync();
        ValueTask<StringBuilder> StringBuilderAsync();
    }
}
