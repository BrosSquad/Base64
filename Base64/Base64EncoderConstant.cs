using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Base64
{
    /// <summary>
    /// 
    /// </summary>
    public class Base64EncoderConstant : Base64Constant, IEncoder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte Base64ByteToChar(int x, bool isUrlSafe)
        {
            int c1 = isUrlSafe ? '-' : '+';
            int c2 = isUrlSafe ? '_' : '/';

            return (byte) ((ConstantLessThen(x, 26) & (x + 'A')) |
                           (ConstantGreaterThenOrEquals(x, 26) & ConstantLessThen(x, 52) & (x + ('a' - 26))) |
                           (ConstantGreaterThenOrEquals(x, 52) & ConstantLessThen(x, 62) & (x + ('0' - 52))) |
                           (ConstantEquals(x, 62) & c1) |
                           (ConstantEquals(x, 63) & c2));
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
        /// This method is the same as Encode(ReadOnlySpan src, Variant variant)
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
        public void Encode(ref Span<byte> dst, ref ReadOnlySpan<byte> src, Variant variant)
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
                        dst[b64Pos++] = Base64ByteToChar((acc >> accLen) & 0x3F, true);
                    }
                }

                if (accLen > 0)
                {
                    dst[b64Pos++] = Base64ByteToChar((acc << (6 - accLen)) & 0x3F, true);
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
                        dst[b64Pos++] = Base64ByteToChar((acc >> accLen) & 0x3F, false);
                    }
                }

                if (accLen > 0)
                {
                    dst[b64Pos++] = Base64ByteToChar((acc << (6 - accLen)) & 0x3F, false);
                }
            }

            while (b64Pos < b64Len)
            {
                dst[b64Pos++] = 61; // =
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
            Span<byte> dst = new byte[EncodedLength(bytes.Length, variant)];
            Encode(ref dst, ref bytes, variant);

            return Encoding.ASCII.GetString(dst);
        }
    }
}