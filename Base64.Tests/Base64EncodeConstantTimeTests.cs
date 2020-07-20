namespace Base64.Tests
{
    public class Base64EncodeConstantTimeTests : AbstractBase64EncoderTest
    {
        public Base64EncodeConstantTimeTests()
        {
            _encoder = new Base64EncoderConstant();
            _decoder = new Base64DecoderConstant();
        }
    }
}