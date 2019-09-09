using System.Threading.Tasks;

namespace NCoreCoder.TestProject
{
    public interface ITest
    {
        [Test]
        void TestVoid();
        [Test]
        int TestInt();
        [Test]
        Task TestAsync();
        [Test]
        Task<int> TestIntAsync();
    }
}
