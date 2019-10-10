using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
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

            //services.AddAutoInject(Assembly.GetCallingAssembly());
            services.AddSingleton<ITest, Test>();
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

            var _result = await testClass.ReturnValueTask();

            var test = testClass.Test;

            test.Hello();
        }
    }

    public interface ITest
    {
        void Hello();
    }

    [JitInject]
    internal class Test:ITest
    {
        public void Hello()
        {
            Console.WriteLine("Hello");
        }
    }
}
