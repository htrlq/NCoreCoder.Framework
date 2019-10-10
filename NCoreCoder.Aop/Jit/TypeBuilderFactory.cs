using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
#if NETSTANDARD
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule($"Proxy_Module");
#else
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndSave);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule($"Proxy_Module", "1.dll");
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

            var actors = typeBuilder.DefineField("_actors", actorsType, FieldAttributes.Private);
            var serviceProvider = typeBuilder.DefineField("_serviceProvider", typeof(IServiceProvider), FieldAttributes.Private);
            var instance = typeBuilder.DefineField("_instance", targetType, FieldAttributes.Private);

           targetType.InjectConstructor(targetType, typeBuilder, actors, serviceProvider, instance);

            targetType.InjectMethod(typeBuilder,
                actors,serviceProvider,instance
            );

            targetType.InjectProperty(typeBuilder, instance);

#if NETSTANDARD
            var proxyType = typeBuilder.CreateTypeInfo().AsType();
#else
            var proxyType = typeBuilder.CreateType();
#endif
            _dictionary.TryAdd(sourceType, proxyType);

            return proxyType;
        }

#if NETSTANDARD
#else
        public void Save()
        {
            _assemblyBuilder.Save("1.dll");
        }
#endif
    }
}