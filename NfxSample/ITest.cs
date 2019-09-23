using System.Threading.Tasks;

namespace NfxSample
{
    public interface ITest
    {
        void TestVoid();
        int TestInt();
        Task TestAsync();
        Task<int> TestIntAsync();
    }
}