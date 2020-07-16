using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Base64
{
    /// <summary>
    /// 
    /// </summary>
    public class Base64Decoder
    {
        /// <summary>
        /// Calculates number of bytes that will be used in byte buffer when decoding
        /// </summary>
        /// <param name="base64">Base64 encoded string</param>
        /// <param name="variant">Variant of base64 used in encoding</param>
        /// <returns>Number of bytes in original buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EncodedLengthToBytes(ReadOnlySpan<char> base64)
        {
            int count = 0;

            foreach (var c in base64[^2..])
            {
                if (c == '=') count++;
            }

            return base64.Length * 3 / 4 - count;
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
        public Memory<byte> Decode(ReadOnlySpan<char> base64, Variant variant)
        {
            byte[] data = new byte[EncodedLengthToBytes(base64)];
            int accLen = 0, b64Pos = 0, binPos = 0;
            ulong acc = 0;
            bool isUrlSafe = ((int) variant & (int) Mask.UrlSafe) > 0;


            while (b64Pos < base64.Length)
            {
                char c = base64[b64Pos];
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
                    data[binPos++] = (byte) ((acc >> accLen) & 0xFF);
                }

                b64Pos++;
            }

            // Check for padding
            if (((int) variant & (int) Mask.NoPadding) == 0)
            {
                int paddingLen = accLen / 2;

                while (paddingLen > 0)
                {
                    char c = base64[b64Pos];

                    if (c == '=')
                    {
                        paddingLen--;
                    }

                    b64Pos++;
                }
            }
            else if (b64Pos != base64.Length)
            {
                return null;
            }

            return data;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte Base64CharToByte(char c, bool urlSafe)
        {
            int y = urlSafe ? '-' : '+';
            int y1 = urlSafe ? '_' : '/';
            int x =
                (((int) c >= (int) 'A' ? 0xFF : 0) & ((int) c <= (int) 'Z' ? 0xFF : 0) & (c - 'A')) |
                (((int) c >= (int) 'a' ? 0xFF : 0) & ((int) c <= (int) 'z' ? 0xFF : 0) & (c - ('a' - 26))) |
                (((int) c >= (int) '0' ? 0xFF : 0) & ((int) c <= (int) '9' ? 0xFF : 0) & (c - ('0' - 52))) |
                (((int) c == y ? 0xFF : 0) & 62) |
                (((int) c == y1 ? 0xFF : 0) & 63);
        
            return (byte) (x | ((x == 0 ? 0xFF : 0) & (((int) c == (int) 'A' ? 0xFF : 0) ^ 0xFF)));
        }
    }
}