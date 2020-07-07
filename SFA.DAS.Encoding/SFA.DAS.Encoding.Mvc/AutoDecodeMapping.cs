namespace SFA.DAS.Encoding.Mvc
{
    public class AutoDecodeMapping
    {
        public AutoDecodeMapping(string encodedProperty, EncodingType encodingType)
        {
            EncodedProperty = encodedProperty;
            EncodingType = encodingType;
        }

        public string EncodedProperty { get; }
        public EncodingType EncodingType { get; }
    }
}
