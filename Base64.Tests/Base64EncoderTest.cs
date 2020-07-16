using System;
using System.Security.Cryptography;
using FluentAssertions;
using Xunit;

namespace Base64.Tests
{
    public class Base64EncoderTest
    {
        [Fact]
        public void Should_Encode_Base64_In_Original_Mode()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            provider.GetNonZeroBytes(bytes);
            var base64 = new Base64Encoder();

            string b64 = base64.Encode(bytes, Variant.Original);

            b64.Should()
                .NotBeEmpty()
                .And
                .BeEquivalentTo(Convert.ToBase64String(bytes));
        }

        [Fact]
        public void Should_Encode_Base64_In_UrlSafe_Mode()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            provider.GetNonZeroBytes(bytes);
            var base64 = new Base64Encoder();

            string b64 = base64.Encode(bytes, Variant.UrlSafe);

            b64.Should()
                .NotBeEmpty()
                .And
                .BeEquivalentTo(
                    Convert.ToBase64String(bytes)
                        .Replace('/', '_')
                        .Replace('+', '-')
                );
        }

        [Fact]
        public void Should_Encode_Base64_In_Original_Mode_With_No_Padding()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            provider.GetNonZeroBytes(bytes);
            var base64 = new Base64Encoder();

            string b64 = base64.Encode(bytes, Variant.OriginalNoPadding);

            b64.Should()
                .NotBeEmpty()
                .And
                .BeEquivalentTo(Convert.ToBase64String(bytes).TrimEnd('='));
        }

        [Fact]
        public void Should_Encode_Base64_In_UrlSafe_Mode_With_No_Padding()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            provider.GetNonZeroBytes(bytes);
            var base64 = new Base64Encoder();

            string b64 = base64.Encode(bytes, Variant.UrlSafeNoPadding);

            b64.Should()
                .NotBeEmpty()
                .And
                .BeEquivalentTo(
                    Convert.ToBase64String(bytes)
                        .TrimEnd('=')
                        .Replace('/', '_')
                        .Replace('+', '-')
                );
        }

        [Fact]
        public void Should_Throw_OverflowException_When_OutputBuffer_Is_Not_Large_Enough()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            provider.GetNonZeroBytes(bytes);
            var base64 = new Base64Encoder();

            char[] output = new char[10];
            Assert.Throws<OverflowException>(() => base64.Encode(output, bytes, Variant.Original));
        }
    }
}