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
                                /*
                                ilGenerator.Emit(OpCodes.Nop);

                                var method = ilGenerator.DeclareLocal(typeof(MethodInfo));
                                var parameters = ilGenerator.DeclareLocal(typeof(object[]));

                                var _returnType = methodInfo.ReturnType;
                                var isAsync = _returnType == typeof(Task) || _returnType.BaseType == typeof(Task);

                                ilGenerator.AddLdcI4(paramTypes.Length);
                                ilGenerator.Emit(OpCodes.Newarr, typeof(object));

                                for (var j = 0; j < paramTypes.Length; j++)
                                {
                                    ilGenerator.Emit(OpCodes.Dup);
                                    ilGenerator.AddLdcI4(j);
                                    ilGenerator.EmitLoadArg(j + 1);

                                    var parameterType = paramTypes[j];
                                    if (parameterType.GetTypeInfo().IsValueType || parameterType.IsGenericParameter)
                                    {
                                        ilGenerator.Emit(OpCodes.Box, parameterType);
                                    }
                                    ilGenerator.Emit(OpCodes.Stelem_Ref);
                                }

                                ilGenerator.Emit(OpCodes.Stloc, parameters);

                                //methodInfo
                                ilGenerator.EmitMethod(methodInfo);
                                ilGenerator.Emit(OpCodes.Stloc_0, method);

                                //_context
                                ilGenerator.Emit(OpCodes.Ldarg_0);
                                ilGenerator.Emit(OpCodes.Ldflda, paramArray[0]);

                                for (int index = 1; index < paramArray.Length; index++)
                                {
                                    ilGenerator.Emit(OpCodes.Ldarg_0);
                                    ilGenerator.Emit(OpCodes.Ldfld, paramArray[index]);
                                }

                                //methodInfo
                                ilGenerator.Emit(OpCodes.Ldloc_0, method);
                                //args
                                ilGenerator.Emit(OpCodes.Ldloc, parameters);

                                //new AopContext(serviceProvider,methods,instance,args);
                                ilGenerator.Emit(OpCodes.Newobj,
                                typeof(AopContext).GetReflector().GetMemberInfo().GetConstructors().FirstOrDefault()
                                );

                                if (isAsync)
                                {
                                    //_context.Execute(method, instance, params);

                                    if (_returnType.IsGenericType)
                                    {
                                        var typeInfo = _returnType.GetTypeInfo();
                                        var genericTypeDefinition = typeInfo.GetGenericArguments().Single();

                                        ilGenerator.Emit(OpCodes.Callvirt,
                                            actorsType.GetMethod($"ExecuteAsync", new[]
                                            {
                                            typeof(AopContext)
                                            }).MakeGenericMethod(genericTypeDefinition)
                                        );
                                    }
                                    else
                                    {
                                        ilGenerator.Emit(OpCodes.Callvirt,
                                            actorsType.GetMethod($"InvokeAsync", new[]
                                            {
                                            typeof(AopContext),
                                            })
                                        );
                                    }
                                }
                                else
                                {
                                    //_context.Execute(method, instance, params);
                                    // ReSharper disable once AssignNullToNotNullAttribute
                                    ilGenerator.Emit(OpCodes.Callvirt, actorsType.GetMethod($"Execute", new[]
                                    {
                                    typeof(AopContext),
                                    }));
                                }

                                if (_returnType == typeof(void))
                                    ilGenerator.Emit(OpCodes.Pop);

                                if (_returnType != typeof(void) && _returnType.IsValueType)
                                    ilGenerator.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);

                                if (!isAsync && methodInfo.ReturnType.IsGenericType)
                                    ilGenerator.Emit(OpCodes.Castclass, methodInfo.ReturnType);

                                ilGenerator.Emit(OpCodes.Ret);
                                */
                ilGenerator.EmitProxyMethod(methodInfo, actors, serviceProvider, instance);
            }
        }
    }
}