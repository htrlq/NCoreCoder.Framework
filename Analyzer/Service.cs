using NCoreCoder.Aop;

namespace Analyzer
{
    [JitInject]
    public class Service:IService
    {
        public void Hello()
        { }
    }
}
