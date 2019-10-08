//#define _Nfx
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NCoreCoder.Aop
{
    public class TypeBuilderFactory
    {
        private readonly AssemblyName _assemblyName;
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly ConcurrentDictionary<Type, Type> _dictionary;

        //public static TypeBuilderFactory Instance = new TypeBuilderFactory();

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

        public bool IsExits(Type sourceType)
        {
            return _dictionary.Keys.Any(_key => _key == sourceType);
        }

        public Type CreateType(Type actorsType, Type sourceType,Type targetType)
        {
            if (_dictionary.TryGetValue(sourceType, out Type resultType))
                return resultType;

            var typeName = $"NCoreCoder.Aop.Proxy_{targetType.Name}";

            if (_moduleBuilder.GetType(typeName) != null)
            {
                typeName = $"NCoreCoder.Aop.Proxy_{targetType.Name}_{DateTimeOffset.Now.Ticks}";
            }

            var typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Sealed, typeof(object), targetType.GetInterfaces());

            var types = new Type[]
            {
                actorsType,
                typeof(IServiceProvider),
                targetType,
            };

            var actors = typeBuilder.DefineField("_actors", actorsType, FieldAttributes.Private);
            var serviceProvider = typeBuilder.DefineField("_serviceProvider", typeof(IServiceProvider), FieldAttributes.Private);
            var instance = typeBuilder.DefineField("_instance", targetType, FieldAttributes.Private);

            // ReSharper disable once UseMethodAny.2
            //var context = typeBuilder.DefineField("_context", typeof(AopActors), FieldAttributes.Private);
            //var methods = typeBuilder.DefineField("_methods", typeof(MethodInfo[]), FieldAttributes.Private);
            //var instance = typeBuilder.DefineField("_instance", targetType, FieldAttributes.Private);
            //var serviceProvider = typeBuilder.DefineField("_serviceProvider", typeof(IServiceProvider), FieldAttributes.Private);

            targetType.InjectConstructor(targetType, typeBuilder, actors, serviceProvider, instance);

            targetType.InjectMethod(typeBuilder,
                actors,serviceProvider,instance
            );

#if _Nfx

            var proxyType = typeBuilder.CreateType();
#else
            var proxyType = typeBuilder.CreateTypeInfo().AsType();
#endif
            _dictionary.TryAdd(sourceType, proxyType);

            return proxyType;
        }

#if _Nfx
        public void Save()
        {
            _assemblyBuilder.Save("1.dll");
        }
#endif
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