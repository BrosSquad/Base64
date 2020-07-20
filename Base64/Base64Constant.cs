using System.Runtime.CompilerServices;

namespace Base64
{
    /// <summary>
    /// Constant time base64 encoding and decoding
    /// </summary>
    public abstract class Base64Constant : Base64
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int ConstantEquals(int x, int y)
        {
            return (((0 - (x ^ y)) >> 8) & 0xFF) ^ 0xFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int ConstantGreaterThan(int x, int y)
        {
            return ((y - x) >> 8) & 0xFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int ConstantGreaterThenOrEquals(int x, int y)
        {
            return ConstantGreaterThan(y, x) ^ 0xFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int ConstantLessThen(int x, int y)
        {
            return ConstantGreaterThan(y, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int ConstantLessThenOrEquals(int x, int y)
        {
            return ConstantGreaterThenOrEquals(y, x);
        }
    }
}