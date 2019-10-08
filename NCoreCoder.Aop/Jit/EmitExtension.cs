using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
            "GetType"
        };

        public static void InjectConstructor(this Type sourceType,
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

        public static void InjectMethod(this Type targetType, TypeBuilder typeBuilder,
            FieldBuilder actors,FieldBuilder serviceProvider, FieldBuilder instance)
        {
            var methodinfos = targetType
                .GetTypeInfo()
                .GetMethods()
                .Where(_method => !ignoreMethods.Contains(_method.Name) && _method.IsVirtual)
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
    }
}