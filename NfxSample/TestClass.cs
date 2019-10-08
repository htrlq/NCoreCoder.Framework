using System;
using System.Threading.Tasks;
using NCoreCoder.Aop;

namespace NfxSample
{
    [JitInject]
    public class TestClass: ITestClass
    {
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
    }

    public interface ITestClass
    {
        string Hello();
        Task<int> ResultIntAsync();
        Task ReturnAsync();
    }
}