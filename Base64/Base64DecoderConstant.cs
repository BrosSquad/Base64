using System;
using System.Runtime.CompilerServices;

namespace Base64
{
    /// <summary>
    /// 
    /// </summary>
    public class Base64DecoderConstant : Base64Constant, IDecoder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte Base64CharToByte(byte c, bool urlSafe)
        {
            int c1 = urlSafe ? '-' : '+';
            int c2 = urlSafe ? '_' : '/';
            int x =
                (ConstantGreaterThenOrEquals(c, 'A') & ConstantLessThenOrEquals(c, 'Z') & (c - 'A')) |
                (ConstantGreaterThenOrEquals(c, 'a') & ConstantLessThenOrEquals(c, 'z') & (c - ('a' - 26))) |
                (ConstantGreaterThenOrEquals(c, '0') & ConstantLessThenOrEquals(c, '9') & (c - ('0' - 52))) |
                (ConstantEquals(c, c1) & 62) |
                (ConstantEquals(c, c2) & 63);
            return (byte) (x | (ConstantEquals(x, 0) & (ConstantEquals(c, 'A') ^ 0xFF)));
        }


        public void Decode(Span<byte> dst, ReadOnlySpan<byte> base64, Variant variant)
        {
            int accLen = 0, b64Pos = 0, binPos = 0;
            ulong acc = 0;
            bool isUrlSafe = ((int) variant & (int) Mask.UrlSafe) > 0;


            while (b64Pos < base64.Length)
            {
                byte c = base64[b64Pos];
                byte d = Base64CharToByte(c, isUrlSafe);

                if (d == 0xFF)
                {
                    break;
                }

                acc = (acc << 6) + d;
                accLen += 6;
                if (accLen >= 8)
                {
                    accLen -= 8;
                    dst[binPos++] = (byte) ((acc >> accLen) & 0xFF);
                }

                b64Pos++;
            }

            // Check for padding
            if (((int) variant & (int) Mask.NoPadding) == 0)
            {
                int paddingLen = accLen / 2;
                byte c;
                while (paddingLen > 0)
                {
                    c = base64[b64Pos];

                    if (c == 61) // =
                    {
                        paddingLen--;
                    }

                    b64Pos++;
                }
            }
            else if (b64Pos != base64.Length)
            {
                throw new Exception("Cannot decode base64");
            }
        }


        /// <summary>
        /// Decodes base64 string into buffer (byte array) with variants
        /// 1. Standard with padding
        /// 2. Standard with no padding
        /// 3. UrlSafe with padding
        /// 4. UrlSafe with no padding
        /// </summary>
        /// <param name="base64">Base64 Encoded string</param>
        /// <param name="variant">Variant used in encoding</param>
        /// <returns>Return Memory&gt;byte&lt; with underlying byte buffer or null on error</returns>
        public Memory<byte> Decode(ReadOnlySpan<byte> base64, Variant variant)
        {
            byte[] data = new byte[EncodedLengthToBytes(base64)];
            Decode(data, base64, variant);
            return data;
        }
    }
}