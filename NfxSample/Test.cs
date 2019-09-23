using System;
using System.Threading.Tasks;

namespace NfxSample
{
    public class Test : ITest
    {
        public void TestVoid()
        {
            Console.WriteLine("TestVoid");
        }

        public int TestInt()
        {
            Console.WriteLine("TestInt");
            return 1000;
        }

        public Task TestAsync()
        {
            Console.WriteLine("TestAsync");
            return Task.CompletedTask;
        }

        public Task<int> TestIntAsync()
        {
            Console.WriteLine("TestIntAsync");
            return Task.FromResult(900);
        }

    }
}