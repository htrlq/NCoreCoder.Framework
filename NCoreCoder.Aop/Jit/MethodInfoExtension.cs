using System;
using System.Linq;
using System.Reflection;

namespace NCoreCoder.Aop
{
    internal static class MethodInfoExtension
    {
        public static readonly MethodInfo Invoke =
            typeof(MethodBase).GetMethod("Invoke", new[] {typeof(object), typeof(object[])});

        public static readonly MethodInfo ExecuteAsync = typeof(DefaultAopActors).GetMethod("ExecuteAsync");
        public static readonly MethodInfo InvokeAsync = typeof(DefaultAopActors).GetMethod("InvokeAsync");
        public static readonly MethodInfo Execute = typeof(DefaultAopActors).GetMethod("Execute");

        public static readonly MethodInfo GetTypeFromHandle =
            typeof(Type).GetMethod("GetTypeFromHandle", new[] {typeof(RuntimeTypeHandle)});

        public static readonly MethodInfo BuilderDelegate = typeof(ExpressionExtension).GetMethod("BuilderDelegate",
            new[]
            {
                typeof(Type)
            });

        public static Type[] GetParameterTypes(this MethodInfo methodInfo)
        {
            var result = methodInfo.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

            return result;
        }
    }
}