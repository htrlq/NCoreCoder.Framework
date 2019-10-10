using System;
using System.Threading.Tasks;
using NCoreCoder.Aop;

namespace NfxSample
{
    [JitInject]
    public class TestClass: ITestClass
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
            return Task.Run(()=>{});
        }

        public ValueTask<int> ReturnValueTask()
        {
            return new ValueTask<int>(100);
        }
    }

    public interface ITestClass
    {
        ITest Test { get; set; }
        string Hello();
        Task<int> ResultIntAsync();
        Task ReturnAsync();
        ValueTask<int> ReturnValueTask();
    }

    public class TestClassInject : ITestClass
    {
        public ITest Test
        {
            get { return _intance.Test; }
            set { _intance.Test = value; }
        }

        private TestClass _intance;

        public TestClassInject(TestClass intance)
        {
            _intance = intance;
        }

        public string Hello()
        {
            throw new NotImplementedException();
        }

        public Task<int> ResultIntAsync()
        {
            throw new NotImplementedException();
        }

        public Task ReturnAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<int> ReturnValueTask()
        {
            throw new NotImplementedException();
        }
    }
}