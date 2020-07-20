using System;

namespace Base64
{
    public interface IDecoder
    {
        Memory<byte> Decode(ReadOnlySpan<byte> base64, Variant variant);
        void Decode(Span<byte> dst, ReadOnlySpan<byte> base64, Variant variant);
        int EncodedLengthToBytes(ReadOnlySpan<byte> base64);
    }
}