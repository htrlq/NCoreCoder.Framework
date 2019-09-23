using System.Threading.Tasks;

namespace NfxSample
{
    public interface IJitService
    {
        void TestVoid();
        int TestInt();
        Task TestAsync();
        Task<int> TestIntAsync();
    }
}