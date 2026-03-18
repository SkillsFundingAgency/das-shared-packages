using SFA.DAS.SharedOuterApi.Extensions;

namespace SFA.DAS.SharedOuterApi.UnitTests.Extensions;

public class WhenConvertingToGeoWithMetreAccuracy
{
    [Test]
    public void ToGeoWithMetreAccuracy_Should_Return_Null_When_Passed_Null()
    {
        // arrange
        double? value = null;
        
        // act
        var result = value.ToGeoWithMetreAccuracy();
        
        // assert
        result.Should().BeNull();
    }
    
    [Test]
    public void ToGeoWithMetreAccuracy_Should_Return_Null_When_Passed_Zero()
    {
        // arrange
        double? value = 0d;
        
        // act
        var result = value.ToGeoWithMetreAccuracy();
        
        // assert
        result.Should().BeNull();
    }
    
    [Test]
    public void ToGeoWithMetreAccuracy_Should_Truncate_To_5_Decimal_Places()
    {
        // arrange
        double? value = 10.0292321d;
        
        // act
        var result = value.ToGeoWithMetreAccuracy();
        
        // assert
        result.Should().Be(10.02923d);
    }
    
    [Test]
    public void ToGeoWithMetreAccuracy_Should_Not_Round()
    {
        // arrange
        double? value = 10.000009d;
        
        // act
        var result = value.ToGeoWithMetreAccuracy();
        
        // assert
        result.Should().Be(10d);
    }
}