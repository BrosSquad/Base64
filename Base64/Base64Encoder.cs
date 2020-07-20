using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Base64
{
    public class Base64Encoder : Base64, IEncoder
    {
        /// <summary>
        /// Base64 characters table in original mode ( A-Z a-z + / )
        /// </summary>
        private static readonly byte[] TableOriginal =
        {
            65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 97,
            98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
            120, 121, 122, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 43, 47,
        };

        /// <summary>
        /// Base64 characters table in url safe mode ( A-Z a-z - _ )
        /// </summary>
        private static readonly byte[] TableUrlSafe =
        {
            65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 97,
            98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119,
            120, 121, 122, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 45, 95,
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Encode(ref Span<byte> dst, ref ReadOnlySpan<byte> src, Variant variant)
        {
            if (dst.Length != EncodedLength(src.Length, variant))
            {
                throw new OverflowException("Output span does not have enough memory to contain base64 encoded byte[]");
            }

            bool isUrlSafe = ((int) variant & (int) Mask.UrlSafe) > 0;
            bool hasPadding = ((int) variant & (int) Mask.NoPadding) == 0;

            fixed (byte* srcBytes = src)
            fixed (byte* destBytes = dst)
            fixed (byte* table = isUrlSafe ? TableUrlSafe : TableOriginal)
            {
                byte* srcPointer = srcBytes;
                byte* destPointer = destBytes;
                byte* end = srcBytes + src.Length;
                for (; end - srcPointer > 2; srcPointer += 3)
                {
                    *destPointer++ = table[srcPointer[0] >> 2];
                    *destPointer++ = table[((srcPointer[0] & 0x03) << 4) | (srcPointer[1] >> 4)];
                    *destPointer++ = table[((srcPointer[1] & 0x0f) << 2) | (srcPointer[2] >> 6)];
                    *destPointer++ = table[srcPointer[2] & 0x3f];
                }


                if (end - srcPointer < 1) return;

                *destPointer++ = table[srcPointer[0] >> 2];

                if (end - srcPointer == 1)
                {
                    *destPointer++ = table[(srcPointer[0] & 0x03) << 4];
                    if (hasPadding)
                        *destPointer++ = 61; // =
                }
                else
                {
                    *destPointer++ = table[((srcPointer[0] & 0x03) << 4) |
                                           (srcPointer[1] >> 4)];
                    *destPointer++ = table[(srcPointer[1] & 0x0f) << 2];
                }

                if (hasPadding)
                    *destPointer = 61; // =
            }
        }

        /// <summary>
        /// Encodes byte array into base64 string with 4 Variants
        /// 1. Standard with padding
        /// 2. Standard with no padding
        /// 3. UrlSafe with padding
        /// 4. UrlSafe with no padding
        /// </summary>
        /// <param name="bytes">Input buffer (byte array)</param>
        /// <param name="variant">Base64 Variant</param>
        /// <returns>Base64 Encoded string</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Encode(ReadOnlySpan<byte> bytes, Variant variant)
        {
            Span<byte> output = new byte[EncodedLength(bytes.Length, variant)];
            Encode(ref output, ref bytes, variant);
            return Encoding.ASCII.GetString(output);
        }
    }
}