﻿using System.Threading.Tasks;

namespace NCoreCoder.TestProject
{
    public interface ITest
    {
        [TestDynamic]
        void TestVoid();
        [TestDynamic]
        int TestInt();
        [TestDynamic]
        Task TestAsync();
        [TestDynamic]
        Task<int> TestIntAsync();
    }
}
