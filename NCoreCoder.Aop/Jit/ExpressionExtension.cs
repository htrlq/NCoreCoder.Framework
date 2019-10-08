using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NCoreCoder.Aop
{
    public static class ExpressionExtension
    {
        private static ConcurrentDictionary<Type,Delegate> _delegates = new ConcurrentDictionary<Type, Delegate>();
        private static Delegate BuilderFunc(Type returnType)
        {
            var contextParameter = Expression.Parameter(typeof(AopContext), "context");
            var methodInfo = Expression.Property(contextParameter, "MethodInfo");
            var instance = Expression.Property(contextParameter, "Instance");
            var args = Expression.Property(contextParameter, "Args");

            var resultLabel = Expression.Label(typeof(object));
            var resultVariable = Expression.Variable(typeof(object));

            var method = Expression.Call(methodInfo,
                typeof(MethodBase).GetMethod("Invoke", new[] { typeof(object), typeof(object[]) }), instance, args);
            var setResult = Expression.Assign(resultVariable, method);
            var ret = Expression.Return(resultLabel, resultVariable);
            var defaultLabel = Expression.Label(resultLabel, Expression.Constant(default(object), typeof(object)));
            var body = Expression.Block(
                new ParameterExpression[] { resultVariable },
                setResult,
                ret,
                defaultLabel
            );

            return Expression.Lambda(body, contextParameter).Compile();
        }

        private static Delegate BuilderTaskFunc(Type returnType)
        {
            var contextParameter = Expression.Parameter(typeof(AopContext), "context");
            var methodInfo = Expression.Property(contextParameter, "MethodInfo");
            var instance = Expression.Property(contextParameter, "Instance");
            var args = Expression.Property(contextParameter, "Args");

            var resultLabel = Expression.Label(typeof(object));
            var resultVariable = Expression.Variable(typeof(object));

            var method = Expression.Call(methodInfo,
                typeof(MethodBase).GetMethod("Invoke", new[] { typeof(object), typeof(object[]) }), instance, args);
            var setResult = Expression.Assign(resultVariable, method);
            var ret = Expression.Return(resultLabel, resultVariable);
            var defaultLabel = Expression.Label(resultLabel, Expression.Constant(default(object), typeof(object)));
            var body = Expression.Block(
                new ParameterExpression[] { resultVariable },
                setResult,
                ret,
                defaultLabel
            );
            var convert = Expression.Convert(body, returnType);

            return Expression.Lambda(convert, contextParameter).Compile();
        }

        public static Delegate BuilderDelegate(Type returnType)
        {
            var isAsync = returnType == typeof(Task) || returnType.BaseType == typeof(Task);

            if (_delegates.TryGetValue(returnType, out Delegate _delegate))
                return _delegate;

            if (isAsync)
            {
                var asyncDelegate = BuilderTaskFunc(returnType);

                _delegates.TryAdd(returnType, asyncDelegate);

                return asyncDelegate;
            }

            var syncDelegate = BuilderFunc(returnType);

            _delegates.TryAdd(returnType, syncDelegate);

            return syncDelegate;
        }

        public static Type CreateFuncType(Type returnType)
        {
            return typeof(Func<,>).MakeGenericType(typeof(AopContext), returnType);
        }
    }
}