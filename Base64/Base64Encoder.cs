using System;
using System.Runtime.CompilerServices;

namespace Base64
{
    /// <summary>
    /// 
    /// </summary>
    public class Base64Encoder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char Base64ByteToUrlSafeChar(int x) =>
            (char) (((x < 26 ? 0xFF : 0) & (x + 'A')) |
                    ((x >= 26 ? 0xFF : 0) & (x < 52 ? 0xFF : 0) & (x + ('a' - 26))) |
                    ((x >= 52 ? 0xFF : 0) & (x < 62 ? 0xFF : 0) & (x + ('0' - 52))) |
                    ((x == 62 ? 0xFF : 0) & '-') |
                    ((x == 63 ? 0xFF : 0) & '_'));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char Base64ByteToChar(int x) =>
            (char) (((x < 26 ? 0xFF : 0) & (x + 'A')) |
                    ((x >= 26 ? 0xFF : 0) & (x < 52 ? 0xFF : 0) & (x + ('a' - 26))) |
                    ((x >= 52 ? 0xFF : 0) & (x < 62 ? 0xFF : 0) & (x + ('0' - 52))) |
                    ((x == 62 ? 0xFF : 0) & '+') |
                    ((x == 63 ? 0xFF : 0) & '/'));


        /// <summary>
        /// Calculates the length of the base64 encoded string for the given buffer length
        /// </summary>
        /// <param name="bufferLength">Length of the buffer</param>
        /// <param name="variant">Base64 Variant</param>
        /// <returns>Length of the base64 string</returns>
        public int EncodedLength(int bufferLength, Variant variant)
        {
            if (((int) variant & (int) Mask.NoPadding) == 0)
            {
                return ((bufferLength + 2) / 3) << 2;
            }

            return ((bufferLength << 2) | 2) / 3;
        }

        /// <summary>
        /// Encodes byte array into base64 string with 4 Variants
        /// 1. Standard with padding
        /// 2. Standard with no padding
        /// 3. UrlSafe with padding
        /// 4. UrlSafe with no padding
        ///
        /// Important:
        /// Dont use this method unless you need performance.
        /// This method is the same as Encode(ReadOnlySpan<byte> src, Variant variant)
        /// in every regard, except the return type. This method allows you to control
        /// the output, there for you can avoid multiple allocation when encoding large file
        /// </summary>
        /// <param name="dst">Char[] destination for the base64 encoded byte array</param>
        /// <param name="src">Byte[] source</param>
        /// <param name="variant"></param>
        /// <exception cref="OverflowException">
        ///    OverflowException is thrown when output buffer (param dst)
        ///    does not have enough memory to hold the base64 encoded byte buffer (param src).
        ///    To check the maximum length for the output buffer use EncodedLength
        /// </exception>
        public void Encode(Span<char> dst, ReadOnlySpan<byte> src, Variant variant)
        {
            if (dst.Length != EncodedLength(src.Length, variant))
            {
                throw new OverflowException("Output span does not have enough memory to contain base64 encoded byte[]");
            }


            int accLen = 0;
            int nibbles = src.Length / 3;
            int remainder = src.Length - 3 * nibbles;
            int b64Pos = 0;
            int binPos = 0;
            int acc = 0;
            int b64Len = nibbles * 4;

            if (remainder != 0)
            {
                // With Padding
                if (((int) variant & (int) Mask.NoPadding) == 0)
                {
                    b64Len += 4;
                }
                // With no padding
                else
                {
                    b64Len += 2 + (remainder >> 1);
                }
            }

            // URL Safe variant
            if (((int) variant & (int) Mask.UrlSafe) != 0)
            {
                while (binPos < src.Length)
                {
                    acc = (acc << 8) + src[binPos++];
                    accLen += 8;
                    while (accLen >= 6)
                    {
                        accLen -= 6;
                        dst[b64Pos++] = Base64ByteToUrlSafeChar((acc >> accLen) & 0x3F);
                    }
                }

                if (accLen > 0)
                {
                    dst[b64Pos++] = Base64ByteToUrlSafeChar((acc << (6 - accLen)) & 0x3F);
                }
            }
            // Standard option
            else
            {
                while (binPos < src.Length)
                {
                    acc = (acc << 8) + src[binPos++];
                    accLen += 8;
                    while (accLen >= 6)
                    {
                        accLen -= 6;
                        dst[b64Pos++] = Base64ByteToChar((acc >> accLen) & 0x3F);
                    }
                }

                if (accLen > 0)
                {
                    dst[b64Pos++] = Base64ByteToChar((acc << (6 - accLen)) & 0x3F);
                }
            }

            while (b64Pos < b64Len)
            {
                dst[b64Pos++] = '=';
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
        public string Encode(ReadOnlySpan<byte> bytes, Variant variant)
        {
            int b64MaxLen = EncodedLength(bytes.Length, variant);
            char[] dst = new char[b64MaxLen];

            Encode(dst, bytes, variant);

            return new String(dst);
        }
    }
}