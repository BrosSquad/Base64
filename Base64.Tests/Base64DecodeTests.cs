namespace Base64.Tests
{
    public class Base64DecodeTests : AbstractBase64DecoderTest
    {
        public Base64DecodeTests()
        {
            _encoder = new Base64Encoder();
            _decoder = new Base64Decoder();
        }
    }
}