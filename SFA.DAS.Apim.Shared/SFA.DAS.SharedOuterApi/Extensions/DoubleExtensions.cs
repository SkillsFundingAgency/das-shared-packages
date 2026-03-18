namespace SFA.DAS.SharedOuterApi.Extensions;

public static class DoubleExtensions
{
    // This gives metre accuracy for geo coords
    private const int Accuracy = 100000;
    
    public static double? ToGeoWithMetreAccuracy(this double? value)
    {
        return value switch
        {
            null => null,
            0 => null,
            _ => ToGeoWithMetreAccuracy(value.Value)
        };
    }
    
    public static double ToGeoWithMetreAccuracy(this double value)
    {
        return double.Truncate(value * Accuracy) / Accuracy;
    }
}