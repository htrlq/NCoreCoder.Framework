using Microsoft.Extensions.DependencyInjection;
using NCoreCoder.Aop;
using Xunit;

namespace NCoreCoder.TestProject
{
    public class TestTransient : UnitTestBase
    {
        protected override void ConfigService(IServiceCollection services)
        {
            services.AddNCoreCoderAop<ITest, Test>(ServiceLifetime.Transient);
        }

        [Fact]
        public void TestVoid()
        {
            var test = GetRequriedService<ITest>();

            test.TestVoid();
        }

        [Fact]
        public void TestInt()
        {
            var test = GetRequriedService<ITest>();

            var result = test.TestInt();

            Assert.Equal<int>(1000, result);
        }

        [Fact]
        public async void TestAsync()
        {
            var test = GetRequriedService<ITest>();

            await test.TestAsync();
        }

        [Fact]
        public async void TestIntAsync()
        {
            var test = GetRequriedService<ITest>();

            var result = await test.TestIntAsync();

            Assert.Equal<int>(900, result);
        }

        [Fact]
        public async void Test()
        {
            var test = GetRequriedService<ITest>();

            test.TestVoid();

            var resultInt = test.TestInt();
            Assert.Equal<int>(1000, resultInt);

            await test.TestAsync();

            var asyncInt = await test.TestIntAsync();
            Assert.Equal<int>(900, asyncInt);
        }
    }
    public class TestSingle: UnitTestBase
    {
        protected override void ConfigService(IServiceCollection services)
        {
            services.AddNCoreCoderAop<ITest, Test>();
        }

        [Fact]
        public void TestVoid()
        {
            var test = GetRequriedService<ITest>();

            test.TestVoid();
        }

        [Fact]
        public void TestInt()
        {
            var test = GetRequriedService<ITest>();

            var result = test.TestInt();

            Assert.Equal<int>(1000, result);
        }

        [Fact]
        public async void TestAsync()
        {
            var test = GetRequriedService<ITest>();

            await test.TestAsync();
        }

        [Fact]
        public async void TestIntAsync()
        {
            var test = GetRequriedService<ITest>();

            var result = await test.TestIntAsync();

            Assert.Equal<int>(900, result);
        }

        [Fact]
        public async void Test()
        {
            var test = GetRequriedService<ITest>();

            test.TestVoid();

            var resultInt = test.TestInt();
            Assert.Equal<int>(1000, resultInt);

            await test.TestAsync();

            var asyncInt = await test.TestIntAsync();
            Assert.Equal<int>(900, asyncInt);
        }
    }

    public class TestScope : UnitTestBase
    {
        protected override void ConfigService(IServiceCollection services)
        {
            services.AddNCoreCoderAop<ITest, Test>(ServiceLifetime.Scoped);
        }

        [Fact]
        public void TestVoid()
        {
            var test = GetRequriedService<ITest>();

            test.TestVoid();
        }

        [Fact]
        public void TestInt()
        {
            var test = GetRequriedService<ITest>();

            var result = test.TestInt();

            Assert.Equal<int>(1000, result);
        }

        [Fact]
        public async void TestAsync()
        {
            var test = GetRequriedService<ITest>();

            await test.TestAsync();
        }

        [Fact]
        public async void TestIntAsync()
        {
            var test = GetRequriedService<ITest>();

            var result = await test.TestIntAsync();

            Assert.Equal<int>(900, result);
        }

        [Fact]
        public async void Test()
        {
            var test = GetRequriedService<ITest>();

            test.TestVoid();

            var resultInt = test.TestInt();
            Assert.Equal<int>(1000, resultInt);

            await test.TestAsync();

            var asyncInt = await test.TestIntAsync();
            Assert.Equal<int>(900, asyncInt);
        }
    }
}
