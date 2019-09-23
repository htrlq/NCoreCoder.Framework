using System.Reflection.Emit;

namespace NCoreCoder.Aop
{
    internal static class IlGeneratorExtension
    {
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