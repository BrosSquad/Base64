using System;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Base64.Tests
{
    public abstract class AbstractBase64DecoderTest : Base64Tests
    {
        [Fact]
        public void Should_Decode_Original_Mode_To_String()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(bytes);
            string b64 = Convert.ToBase64String(bytes);


            byte[] buffer = _decoder.Decode(Encoding.ASCII.GetBytes(b64), Variant.Original)
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
            string b64 = _encoder.Encode(bytes, Variant.OriginalNoPadding);


            byte[] buffer = _decoder.Decode(Encoding.ASCII.GetBytes(b64), Variant.OriginalNoPadding)
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
            string b64 = _encoder.Encode(bytes, Variant.UrlSafe);

            var outputMemory = _decoder.Decode(Encoding.ASCII.GetBytes(b64), Variant.UrlSafe);

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
            string b64 = _encoder.Encode(bytes, Variant.UrlSafeNoPadding);


            byte[] buffer = _decoder.Decode(Encoding.ASCII.GetBytes(b64), Variant.UrlSafeNoPadding)
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

                string base64 = _encoder.Encode(bytes, variant);

                _decoder.EncodedLengthToBytes(Encoding.ASCII.GetBytes(base64)).Should()
                    .Be(length);

                Array.Clear(bytes, 0, bytes.Length);
            }
        }
    }
}