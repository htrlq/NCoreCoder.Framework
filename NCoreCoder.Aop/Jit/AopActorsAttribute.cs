using System;
using System.Collections.Generic;
using System.Text;

namespace NCoreCoder.Aop.Jit
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class AopActorsAttribute: Attribute
    {
        public Type ActorsType { get; }

        public AopActorsAttribute(Type actorsType)
        {
            ActorsType = actorsType;
        }
    }
}
