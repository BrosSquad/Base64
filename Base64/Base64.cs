using System;
using System.Runtime.CompilerServices;

namespace Base64
{
    public abstract class Base64
    {
        /// <summary>
        /// Calculates number of bytes that will be used in byte buffer when decoding
        /// </summary>
        /// <param name="base64">Base64 encoded string</param>
        /// <param name="variant">Variant of base64 used in encoding</param>
        /// <returns>Number of bytes in original buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EncodedLengthToBytes(ReadOnlySpan<byte> base64)
        {
            int count = 0;

            if (base64[^1] == 61)
            {
                count++;
                if (base64[^2] == 61)
                {
                    count++;
                }
            }

            return ((3 * base64.Length) >> 2) - count;
        }
        
        /// <summary>
        /// Calculates the length of the base64 encoded string for the given buffer length
        /// </summary>
        /// <param name="bufferLength">Length of the buffer</param>
        /// <param name="variant">Base64 Variant</param>
        /// <returns>Length of the base64 string</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EncodedLength(int bufferLength, Variant variant)
        {
            if (((int) variant & (int) Mask.NoPadding) == 0)
            {
                return ((bufferLength + 2) / 3) << 2;
            }

            return ((bufferLength << 2) | 2) / 3;
        }
    }
}