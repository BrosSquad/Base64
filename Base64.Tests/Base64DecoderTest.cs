using System;
using System.Security.Cryptography;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;

namespace Base64.Tests
{
    public class Base64DecoderTest
    {
        [Fact]
        public void Should_Decode_Original_Mode_To_String()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(bytes);
            string b64 = Convert.ToBase64String(bytes);

            var base64 = new Base64Decoder();

            byte[] buffer = base64.Decode(b64, Variant.Original)
                .ToArray();

            buffer.Should()
                .HaveSameCount(bytes)
                .And
                .BeEquivalentTo(bytes);
        }

        [Fact]
        public void Should_Decode_Original_Mode_NoPadding_To_String()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(bytes);
            string b64 = new Base64Encoder().Encode(bytes, Variant.OriginalNoPadding);

            var base64 = new Base64Decoder();

            byte[] buffer = base64.Decode(b64, Variant.OriginalNoPadding)
                .ToArray();

            buffer.Should()
                .HaveSameCount(bytes)
                .And
                .BeEquivalentTo(bytes);
        }

        [Fact]
        public void Should_Decode_UrlSafe_Mode_To_String()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(bytes);
            string b64 = new Base64Encoder().Encode(bytes, Variant.UrlSafe);

            var outputMemory = new Base64Decoder().Decode(b64, Variant.UrlSafe);

            var outputBuffer = outputMemory.ToArray();

            outputBuffer
                .Should()
                .HaveCount(64)
                .And
                .BeEquivalentTo(bytes);
        }

        [Fact]
        public void Should_Decode_UrlSafe_Mode_NoPadding_To_String()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(bytes);
            string b64 = new Base64Encoder().Encode(bytes, Variant.UrlSafeNoPadding);

            var base64 = new Base64Decoder();

            byte[] buffer = base64.Decode(b64, Variant.UrlSafeNoPadding)
                .ToArray();

            buffer.Should()
                .HaveSameCount(bytes)
                .And
                .BeEquivalentTo(bytes);
        }

        [Fact]
        public void Should_Give_Right_Number_Of_Bytes_For_Encoded_String()
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            Base64Encoder encoder = new Base64Encoder();
            Base64Decoder decoder = new Base64Decoder();

            Random random = new Random();
            
            for (int i = 0; i < 10_000; i++)
            {
                int length = random.Next(32, 128);
                byte[] bytes = new byte[length];
                provider.GetNonZeroBytes(bytes);
                Variant variant = random.Next(0, 4) switch
                {
                    0 => Variant.Original,
                    1 => Variant.UrlSafe,
                    2 => Variant.OriginalNoPadding,
                    _ => Variant.UrlSafeNoPadding,
                };

                string base64 = encoder.Encode(bytes, variant);

                decoder.EncodedLengthToBytes(base64).Should()
                    .Be(length);

                Array.Clear(bytes, 0, bytes.Length);
            }
        }
    }
}