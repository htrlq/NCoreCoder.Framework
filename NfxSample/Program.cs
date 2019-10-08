using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;
using NCoreCoder.Aop;
using Newtonsoft.Json;

namespace NfxSample
{
    class Program
    {
        static void Main()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddAutoInject(Assembly.GetCallingAssembly());
            //services.AddSingleton<ITestClass, TestClass>();

            var serviceProvider = services.BuilderJit();
            var instance = serviceProvider.GetRequiredService<ITest>();

            //Test(instance);

            Console.ReadLine();
        }

        static async void Test(ITestClass testClass)
        {
            var result = await testClass.ResultIntAsync();

            if (result != 100)
                throw new Exception($"Assert not Equals");

            await testClass.ReturnAsync();

            var hello = testClass.Hello();

            if (!hello.Equals("Hello"))
                throw new Exception($"Assert not Equals");
        }
    }

    public interface ITest
    {
        void Hello();
    }

    [Scoped(typeof(ITest))]
    internal class Test:ITest
    {
        public void Hello()
        {
            Console.WriteLine("Hello");
        }
    }
}
