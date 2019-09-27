using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCoreCoder.Aop;

namespace NfxSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ServiceCollection();

            service.AddSingleton<IJitService, JitService>();
            service.AddSingleton<IMyClass, MyClass>();

            var serviceProvider = service.BuilderJit();

            TypeBuilderFactory.Instance.Save();

            var myclass = serviceProvider.GetRequiredService<IJitService>();

            Task.Factory.StartNew(async () =>
            {
                myclass.TestVoid();
                var result1 = myclass.TestInt();

                await myclass.TestAsync();
                var result2 = await myclass.TestIntAsync();
            });

            Console.ReadLine();
        }
    }

    public interface IMyClass
    {
        void TestVoid();
        int TestInt();
        Task TestAsync();
        Task<int> TestIntAsync();
    }

    [JitInject]
    internal class MyClass : IMyClass
    {
        public void TestVoid()
        {
            Console.WriteLine("TestVoid");
        }

        public int TestInt()
        {
            Console.WriteLine("TestInt");

            return 100;
        }

        public Task TestAsync()
        {
            Console.WriteLine("TestAsync");

            return Task.CompletedTask;
        }

        public Task<int> TestIntAsync()
        {
            Console.WriteLine("TestIntAsync");

            return Task.FromResult(100);
        }
    }
    public interface IJitService
    {
        void TestVoid();
        int TestInt();
        Task TestAsync();
        Task<int> TestIntAsync();
    }

    [JitInject]
    internal class JitService : IJitService
    {
        public IMyClass Test { get; }

        public JitService(IMyClass test)
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
