namespace SFA.DAS.Encoding
{
    public interface IEncodingService
    {
        string Encode(long value, EncodingType encodingType);
        long Decode(string value, EncodingType encodingType);
        bool TryDecode(string encodedValue, EncodingType encodingType, out long decodedValue);
    }
}