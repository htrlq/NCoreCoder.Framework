using System;
using System.Linq;
using System.Reflection;

namespace NfxSample
{
    internal class MethodFactory : IMethodFactory
    {
        private readonly string[] _ignoreMethods = new[]
        {
            "ToString",
            "Equals",
            "GetHashCode",
            "MemberwiseClone",
            "GetType"
        };

        public MethodInfo[] GetMethodInfos<TSource>()
        {
            var methods = typeof(TSource)
                .GetType()
                .BaseType
                .GetMethods()
                // ReSharper disable once InconsistentNaming
                .Where(_method => !_ignoreMethods.Contains(_method.Name))
                .OrderBy(_method => _method.Name)
                .ToArray();

            return methods;
        }
    }
}