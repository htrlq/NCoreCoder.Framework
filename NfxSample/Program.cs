using System;
using System.Linq;
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
            services.AddSingleton<ITestClass, TestClass>();

            var serviceProvider = services.BuilderJit();
            var instance = serviceProvider.GetRequiredService<ITestClass>();

            Test(instance);

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
}
