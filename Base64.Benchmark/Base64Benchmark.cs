using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Base64.Benchmark
{
    public class Base64Benchmark
    {
        private const int N = 256;
        private readonly byte[] _data;
        private readonly byte[] _output;

        private readonly Base64EncoderConstant _base64EncoderConstant;
        private readonly Base64Encoder _base64Encoder;

        public Base64Benchmark()
        {
            _base64EncoderConstant = new Base64EncoderConstant();
            _base64Encoder = new Base64Encoder();
            ;
            _data = new byte[N];
            new Random().NextBytes(_data);
            _output = new byte[System.Buffers.Text.Base64.GetMaxEncodedToUtf8Length(N)];
        }

        [Benchmark]
        public void DotNetBase64Encoding()
        {
            System.Buffers.Text.Base64.EncodeToUtf8(_data, _output, out _, out _);
            Encoding.UTF8.GetString(_output);
        }
        
        [Benchmark]
        public void DotNetBase64UrlEncoding()
        {
            System.Buffers.Text.Base64.EncodeToUtf8(_data, _output, out _, out _);
            Encoding.UTF8.GetString(_output).Replace('/', '_').Replace('+', '-');
        }
        
        [Benchmark]
        public void Base64LibConstantTime() => _base64EncoderConstant.Encode(_data, Variant.Original);

        [Benchmark]
        public void Base64Lib() => _base64Encoder.Encode(_data, Variant.Original);

        [Benchmark]
        public void Base64LibWithoutEncoding()
        {
            Span<byte> output = new byte[N];
            ReadOnlySpan<byte> data = _data;
            _base64Encoder.Encode(ref output, ref data, Variant.Original);
        }

        [Benchmark]
        public void ConvertToBase64() => Convert.ToBase64String(_data);

        
        [Benchmark]
        public void ConvertToBase64UrlSafe() => Convert.ToBase64String(_data)
            .Replace('/', '_')
            .Replace('+', '-');
        
        [Benchmark]
        public void Base64LibUrlSafe() => _base64Encoder.Encode(_data, Variant.UrlSafe);
        
    }
}