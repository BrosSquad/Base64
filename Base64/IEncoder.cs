using System;

namespace Base64
{
    public interface IEncoder
    {
        void Encode(ref Span<byte> dst, ref ReadOnlySpan<byte> src, Variant variant);
        string Encode(ReadOnlySpan<byte> bytes, Variant variant);
        int EncodedLength(int bufferLength, Variant variant);
    }
}