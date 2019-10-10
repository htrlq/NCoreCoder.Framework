using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;
using AspectCore.Extensions.Reflection.Emit;

namespace NCoreCoder.Aop
{
    internal static class EmitExtension
    {
        private static string[] ignoreMethods = new[]
        {
            "ToString",
            "Equals",
            "GetHashCode",
            "MemberwiseClone",
            "GetType",
            "Finalize"
        };

        public static void InjectConstructorNoProperty(this Type sourceType,
            Type targetType, TypeBuilder typeBuilder,
            params FieldBuilder[] paramArray
         )
        {
            if (sourceType.GetConstructors().Length > 1)
                throw new Exception($"Constructor Count > 1");

            var ctorMethod = typeof(object).GetConstructor(Type.EmptyTypes);
            var argsList = paramArray.Select(_field => _field.FieldType).ToList();

            var method = typeBuilder.DefineConstructor(MethodAttributes.Public, typeof(object).GetReflector().GetMemberInfo().GetConstructors().Single().CallingConvention, argsList.ToArray());
            // ReSharper disable once PossibleNullReferenceException

            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);

            //ReSharper disable once AssignNullToNotNullAttribute
            ilGenerator.Emit(OpCodes.Call, ctorMethod);
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Nop);

            for (var index = 0; index < argsList.Count; index++)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.EmitLoadArg(index + 1);
                ilGenerator.Emit(OpCodes.Stfld, paramArray[index]);
            }

            ilGenerator.Emit(OpCodes.Ret);
        }

        public static void InjectConstructor(this Type sourceType,
            Type targetType, TypeBuilder typeBuilder,
            MemberInfo[] paramArray
        )
        {
            if (sourceType.GetConstructors().Length > 1)
                throw new Exception($"Constructor Count > 1");

            var ctorMethod = typeof(object).GetConstructor(Type.EmptyTypes);
            var argsList = paramArray.Select(_field => _field.ReflectedType).ToList();

            var method = typeBuilder.DefineConstructor(MethodAttributes.Public, typeof(object).GetReflector().GetMemberInfo().GetConstructors().Single().CallingConvention, argsList.ToArray());
            // ReSharper disable once PossibleNullReferenceException

            var ilGenerator = method.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            
            //ReSharper disable once AssignNullToNotNullAttribute
            ilGenerator.Emit(OpCodes.Call, ctorMethod);
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Nop);

            for (var index = 0; index < argsList.Count; index++)
            {
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.EmitLoadArg(index + 1);

                if (paramArray[index] is FieldBuilder field)
                    ilGenerator.Emit(OpCodes.Stfld, field);

                if (paramArray[index] is PropertyBuilder property)
                {
                    var _field = sourceType.GetField($"_{property.Name}");
                    ilGenerator.Emit(OpCodes.Stfld, _field);
                }
            }

            ilGenerator.Emit(OpCodes.Ret);
        }

        public static void InjectMethod(this Type targetType, TypeBuilder typeBuilder,
            FieldBuilder actors,FieldBuilder serviceProvider, FieldBuilder instance)
        {
            var methodinfos = targetType
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(_method => !ignoreMethods.Contains(_method.Name) && _method.GetBindingProperty() == null)
                .ToArray();

            for (var i = 0; i < methodinfos.Length; i++)
            {
                var methodInfo = methodinfos[i];

                var paramTypes = methodInfo
                    .GetParameters()
                    // ReSharper disable once InconsistentNaming
                    .Select(_parameter => _parameter.ParameterType)
                    .ToArray();

                var attributes = MethodAttributes.HideBySig | MethodAttributes.Virtual;

                if (methodInfo.Attributes.HasFlag(MethodAttributes.Public))
                {
                    attributes = attributes | MethodAttributes.Public;
                }

                if (methodInfo.Attributes.HasFlag(MethodAttributes.Family))
                {
                    attributes = attributes | MethodAttributes.Family;
                }

                if (methodInfo.Attributes.HasFlag(MethodAttributes.FamORAssem))
                {
                    attributes = attributes | MethodAttributes.FamORAssem;
                }

                var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, attributes, CallingConventions.Standard, methodInfo.ReturnType, paramTypes);

                var ilGenerator = methodBuilder.GetILGenerator();

                ilGenerator.EmitProxyMethod(methodInfo, actors, serviceProvider, instance);
            }
        }

        public static void InjectProperty(this Type targetType, TypeBuilder typeBuilder, FieldBuilder instance)
        {
            const MethodAttributes InterfaceMethodAttributes =
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.NewSlot | MethodAttributes.Virtual;

            foreach (var property in targetType.GetProperties())
            {
                var propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, Type.EmptyTypes);

                if (property.CanRead)
                {
                    var getMethodBuild = typeBuilder.DefineMethod(property.GetMethod.Name,
                        InterfaceMethodAttributes, property.GetMethod.CallingConvention, property.GetMethod.ReturnType, property.GetMethod.GetParameterTypes());

                    var ilGenerator = getMethodBuild.GetILGenerator();

                    var result = ilGenerator.DeclareLocal(property.PropertyType);
                    var ret = ilGenerator.DefineLabel();
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, instance);
                    ilGenerator.Emit(OpCodes.Callvirt, property.GetMethod);
                    ilGenerator.Emit(OpCodes.Stloc, result);
                    ilGenerator.Emit(OpCodes.Br_S, ret);
                    ilGenerator.MarkLabel(ret);
                    ilGenerator.Emit(OpCodes.Ldloc, result);
                    ilGenerator.Emit(OpCodes.Ret);

                    propertyBuilder.SetGetMethod(getMethodBuild);
                }

                if (property.CanWrite)
                {
                    var setMethodBuild = typeBuilder.DefineMethod(property.SetMethod.Name,
                        InterfaceMethodAttributes, property.SetMethod.CallingConvention, property.SetMethod.ReturnType, property.SetMethod.GetParameterTypes());

                    var ilGenerator = setMethodBuild.GetILGenerator();

                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, instance);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Callvirt, property.SetMethod);
                    ilGenerator.Emit(OpCodes.Ret);

                    propertyBuilder.SetSetMethod(setMethodBuild);
                }
            }
        }
    }
}