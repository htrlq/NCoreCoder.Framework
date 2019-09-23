using System.Reflection.Emit;

namespace NfxSample
{
    internal static class IlGeneratorExtension
    {
        public static void AddArg(this ILGenerator ilGenerator, int index)
        {
            switch (index)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    ilGenerator.Emit(OpCodes.Ldarg_S, index);
                    break;
            }
        }

        public static void AddArgs(this ILGenerator ilGenerator, int start, int count)
        {
            for (var i = start; i < start + count; i++)
            {
                switch (i)
                {
                    case 0:
                        ilGenerator.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        ilGenerator.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        ilGenerator.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        ilGenerator.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        ilGenerator.Emit(OpCodes.Ldarg_S, i);
                        break;
                }
            }
        }

        public static void AddLdcI4(this ILGenerator ilGenerator, int index)
        {
            switch (index)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    break;
                default:
                    ilGenerator.Emit(OpCodes.Ldc_I4_S, index);
                    break;
            }
        }
    }
}