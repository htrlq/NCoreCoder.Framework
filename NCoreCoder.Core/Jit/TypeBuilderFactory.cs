//#define _Nfx
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NCoreCoder.Aop
{
    internal class TypeBuilderFactory
    {
        private readonly AssemblyName _assemblyName;
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, Type> _dictionary;

        public static TypeBuilderFactory Instance => new TypeBuilderFactory();

        public TypeBuilderFactory()
        {
            _assemblyName = new AssemblyName("NCoreCoder.Aop");
            _assemblyName.Version = new Version(0, 1);
#if _Nfx
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndSave);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule($"Proxy_Module", "1.dll");
#else
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule($"Proxy_Module");
#endif
            _dictionary = new ConcurrentDictionary<Type, Type>();
        }

        public Type CreateType(Type sourceType,Type targetType)
        {
            if (_dictionary.TryGetValue(sourceType, out Type resultType))
                return resultType;

            var typeBuilder = _moduleBuilder.DefineType($"NCoreCoder.Aop.Proxy_{targetType.Name}", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeof(object), new []{ sourceType });

            var types = new Type[]
            {
                typeof(AopActors),
                typeof(IServiceProvider),
                targetType,
            };

            var paramArray = typeBuilder.AddRangeParam(FieldAttributes.Private,
                types
            );

            // ReSharper disable once UseMethodAny.2
            //var context = typeBuilder.DefineField("_context", typeof(AopActors), FieldAttributes.Private);
            //var methods = typeBuilder.DefineField("_methods", typeof(MethodInfo[]), FieldAttributes.Private);
            //var instance = typeBuilder.DefineField("_instance", targetType, FieldAttributes.Private);
            //var serviceProvider = typeBuilder.DefineField("_serviceProvider", typeof(IServiceProvider), FieldAttributes.Private);

            targetType.InjectConstructor(targetType, typeBuilder, paramArray);

            targetType.InjectMethod(typeBuilder,
                paramArray
            );

#if _Nfx

            var proxyType = typeBuilder.CreateType();
#else
            var proxyType = typeBuilder.CreateTypeInfo().AsType();
#endif
            _dictionary.TryAdd(sourceType, proxyType);

#if _Nfx
            _assemblyBuilder.Save("1.dll");
#endif

            return proxyType;
        }
    }

    internal static class TypeBuilderExtension
    {
        public static FieldBuilder[] AddRangeParam(this TypeBuilder typeBuilder, FieldAttributes fieldAttributes,
            params Type[] types)
        {
            var result = types.Select(type =>
            {
                return typeBuilder.DefineField($"_{type.Name}", type, fieldAttributes);
            }).ToArray();

            return result;
        }
    }
}