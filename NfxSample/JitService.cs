using System.Threading.Tasks;

namespace NfxSample
{
    internal class JitService : IJitService
    {
        public ITest Test { get; }

        public JitService(ITest test)
        {
            Test = test;
        }

        [TestJit]
        public Task TestAsync()
        {
            return Test.TestAsync();
        }

        [TestJit]
        public int TestInt()
        {
            return Test.TestInt();
        }

        [TestJit]
        public Task<int> TestIntAsync()
        {
            return Test.TestIntAsync();
        }

        [TestJit]
        public void TestVoid()
        {
            Test.TestVoid();
        }
    }
}