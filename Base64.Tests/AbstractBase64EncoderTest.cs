using System;
using System.Security.Cryptography;
using FluentAssertions;
using Xunit;

namespace Base64.Tests
{
    public abstract class AbstractBase64EncoderTest : Base64Tests
    {
        [Fact]
        public void Should_Encode_Base64_In_Original_Mode()
        {
            byte[] bytes = new byte[64];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

            provider.GetNonZeroBytes(bytes);

            string b64 = _encoder.Encode(bytes, Variant.Original);

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

            string b64 = _encoder.Encode(bytes, Variant.UrlSafe);

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

            string b64 = _encoder.Encode(bytes, Variant.OriginalNoPadding);

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

            string b64 = _encoder.Encode(bytes, Variant.UrlSafeNoPadding);

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
            Assert.Throws<OverflowException>(() =>
            {
                byte[] bytes = new byte[64];
                RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();

                provider.GetNonZeroBytes(bytes);

                Span<byte> output = new byte[10];
                var reference = new ReadOnlySpan<byte>(bytes);

                _encoder.Encode(ref output, ref reference, Variant.Original);
            });
        }
    }
}