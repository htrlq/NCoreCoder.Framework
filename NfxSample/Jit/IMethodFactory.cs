using System;
using System.Reflection;

namespace NfxSample
{
    public interface IMethodFactory
    {
        MethodInfo[] GetMethodInfos<TSource>();
    }
}