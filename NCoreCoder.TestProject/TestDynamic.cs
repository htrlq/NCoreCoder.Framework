using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCoreCoder.Aop;
using Xunit;

namespace NCoreCoder.TestProject
{
    public class TestDynamic
    {
        [Fact]
        public async void Singleton()
        {
            var instance =
                new DependencyInjection()
                .ConfigService(services =>
                {
                    services.AddDynamicAop<ITest, Test>(ServiceLifetime.Singleton);
                });

            var test = instance.GetRequriedService<ITest>();

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
                    services.AddDynamicAop<ITest, Test>(ServiceLifetime.Transient);
                });

            var test = instance.GetRequriedService<ITest>();

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
                    services.AddDynamicAop<ITest, Test>(ServiceLifetime.Scoped);
                });

            using (var serviceScope = instance.CreateScoped())
            {
                var test = serviceScope.GetRequriedService<ITest>();

                test.TestVoid();

                var resultInt = test.TestInt();
                Assert.Equal<int>(1000, resultInt);

                await test.TestAsync();

                var asyncInt = await test.TestIntAsync();
                Assert.Equal<int>(900, asyncInt);
            }
        }
    }

    public class TestPropetyInject
    {
        [Fact]
        public async void Singleton()
        {
            var instance =
                new DependencyInjection()
                .ConfigService(services =>
                {
                    services.AddDynamicAop<ITest, Test>(ServiceLifetime.Singleton);
                    services.AddDynamicAop<ITestPropetyInject, TestPropetyInjectImpl>(ServiceLifetime.Singleton);
                });

            var test = instance.GetRequriedService<ITestPropetyInject>();

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
                    services.AddDynamicAop<ITest, Test>(ServiceLifetime.Transient);
                    services.AddDynamicAop<ITestPropetyInject, TestPropetyInjectImpl>(ServiceLifetime.Transient);
                });

            var test = instance.GetRequriedService<ITestPropetyInject>();

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
                    services.AddDynamicAop<ITest, Test>(ServiceLifetime.Scoped);
                    services.AddDynamicAop<ITestPropetyInject, TestPropetyInjectImpl>(ServiceLifetime.Scoped);
                });

            var test = instance.GetRequriedService<ITestPropetyInject>();

            test.TestVoid();

            var resultInt = test.TestInt();
            Assert.Equal<int>(1000, resultInt);

            await test.TestAsync();

            var asyncInt = await test.TestIntAsync();
            Assert.Equal<int>(900, asyncInt);
        }
    }

    public interface ITestPropetyInject:ITest
    {
        [Inject(typeof(ITest))]
        ITest Test { get; set; }
    }

    public class TestPropetyInjectImpl : ITestPropetyInject
    {
        public ITest Test { get; set; }

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
