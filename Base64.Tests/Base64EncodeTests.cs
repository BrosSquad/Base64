namespace Base64.Tests
{
    public class Base64EncodeTests : AbstractBase64EncoderTest
    {
        public Base64EncodeTests()
        {
            _encoder = new Base64Encoder();
            _decoder = new Base64Decoder();
        }
    }
}