namespace Base64.Tests
{
    public class Base64DecodeConstantTimeTests : AbstractBase64DecoderTest
    {
        public Base64DecodeConstantTimeTests()
        {
            _encoder = new Base64EncoderConstant();
            _decoder = new Base64DecoderConstant();
        }
    }
}