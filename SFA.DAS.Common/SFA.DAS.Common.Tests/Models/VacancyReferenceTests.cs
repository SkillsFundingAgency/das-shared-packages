using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Models;
using System.Globalization;

namespace SFA.DAS.Common.Tests.Models
{
    [TestFixture]
    public class VacancyReferenceTests
    {
        [TestCase("VAC123456")]
        [TestCase("123456")]
        public void Can_Create_From_String(string value)
        {
            // act
            var result = new VacancyReference(value);

            // assert
            result.Should().NotBeNull();
            result.Should().NotBe(VacancyReference.None);
        }

        [TestCase("foo")]
        [TestCase("!123")]
        public void Invalid_String_Throws(string value)
        {
            // act
            var action = () => new VacancyReference(value);

            // assert
            action.Should().Throw<InvalidVacancyReferenceException>();
        }

        [TestCase(123456)]
        [TestCase(1)]
        public void Can_Create_From_Long(long value)
        {
            // act
            var result = new VacancyReference(value);

            // assert
            result.Should().NotBeNull();
            result.Should().NotBe(VacancyReference.None);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public void Invalid_Long_Throws(long value)
        {
            // act
            var action = () => new VacancyReference(value);

            // assert
            action.Should().Throw<InvalidVacancyReferenceException>();
        }

        [TestCase(12345, true)]
        [TestCase(54321, false)]
        public void Can_Compare_To_Long(long value, bool expectedResult)
        {
            // arrange
            var vacancyReference = new VacancyReference(12345);

            // act
            bool result = vacancyReference == value;
            bool resultInversed = vacancyReference != value;

            // assert
            result.Should().Be(expectedResult);
            resultInversed.Should().Be(!expectedResult);
        }

        [TestCase(12345, true)]
        [TestCase(54321, false)]
        public void Can_Compare_To_Long_Reversed(long value, bool expectedResult)
        {
            // arrange
            var vacancyReference = new VacancyReference(12345);

            // act
            bool result = value == vacancyReference;
            bool resultInversed = value != vacancyReference;

            // assert
            result.Should().Be(expectedResult);
            resultInversed.Should().Be(!expectedResult);
        }

        [TestCase("12345", true)]
        [TestCase("VAC12345", true)]
        [TestCase("54321", false)]
        [TestCase("VAC54321", false)]
        public void Can_Compare_To_String(string value, bool expectedResult)
        {
            // arrange
            var vacancyReference = new VacancyReference(12345);

            // act
            bool result = vacancyReference == value;
            bool resultInversed = vacancyReference != value;

            // assert
            result.Should().Be(expectedResult);
            resultInversed.Should().Be(!expectedResult);
        }

        [TestCase("12345", true)]
        [TestCase("VAC12345", true)]
        [TestCase("54321", false)]
        [TestCase("VAC54321", false)]
        public void Can_Compare_To_String_Reversed(string value, bool expectedResult)
        {
            // arrange
            var vacancyReference = new VacancyReference(12345);

            // act
            bool result = value == vacancyReference;
            bool resultInversed = value != vacancyReference;

            // assert
            result.Should().Be(expectedResult);
            resultInversed.Should().Be(!expectedResult);
        }

        [Test]
        public void Can_Assign_From_Long()
        {
            // act
            VacancyReference result = 12345;

            // assert
            result.Should().NotBeNull();
            result.Should().Be(new VacancyReference("VAC12345"));
        }

        [TestCase("12345")]
        [TestCase("VAC12345")]
        [TestCase("vac12345")]
        public void Can_Assign_From_String(string value)
        {
            // act
            VacancyReference result = value;

            // assert
            result.Should().NotBeNull();
            result.Should().Be(new VacancyReference(value));
        }

        private record TestDto(VacancyReference VacancyReference);
        private static readonly JsonSerializerSettings JsonSerializerOptions = new JsonSerializerSettings
        {
            // No PropertyNameCaseInsensitive in Newtonsoft — use ContractResolver
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
            {
                NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
            }
        };

        [TestCase("12345")]
        [TestCase("VAC12345")]
        public void Can_Deserialize_From_String(string value)
        {
            // arrange
            string json = $$"""{ "vacancyReference": "{{value}}" }""";

            // act
            var result = JsonConvert.DeserializeObject<TestDto>(json, settings: JsonSerializerOptions);

            // assert
            result.Should().NotBeNull();
            result.VacancyReference.Should().Be(new VacancyReference(value));
        }

        [Test]
        public void Can_Deserialize_From_Long()
        {
            // arrange
            const string json = """{ "vacancyReference": 12345 }""";

            // act
            var result = JsonConvert.DeserializeObject<TestDto>(json, settings: JsonSerializerOptions);

            // assert
            result.Should().NotBeNull();
            result.VacancyReference.Should().Be(new VacancyReference(12345));
        }

        [Test]
        public void Can_Deserialize_From_Null()
        {
            // arrange
            const string json = """{ "vacancyReference": null }""";

            // act
            var result = JsonConvert.DeserializeObject<TestDto>(json, settings: JsonSerializerOptions);

            // assert
            result.Should().NotBeNull();
            result.VacancyReference.Should().Be(VacancyReference.None);
        }

        [Test]
        public void Can_Deserialize_From_Empty_String()
        {
            // arrange
            const string json = """{ "vacancyReference": "" }""";

            // act
            var result = JsonConvert.DeserializeObject<TestDto>(json, settings: JsonSerializerOptions);

            // assert
            result.Should().NotBeNull();
            result.VacancyReference.Should().Be(VacancyReference.None);
        }

        [Test]
        public void Can_Compare_To_Null()
        {
            // act
            bool result = VacancyReference.None == null;

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void Can_Compare_To_Null_Reversed()
        {
            // act
            bool result = null == VacancyReference.None;

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void Can_Compare_To_String_Empty()
        {
            // act
            bool result = VacancyReference.None == "";

            // assert
            result.Should().BeTrue();
        }

        [TestCase("12345", "VAC12345", true)]
        public void Can_Compare_To_Separate_Instances(string left, string right, bool expectedResult)
        {
            // act
            bool result = new VacancyReference(left) == new VacancyReference(right);

            // assert
            result.Should().Be(expectedResult);
        }

        [Test]
        public void Can_Get_Value_As_Long()
        {
            // act
            long result = new VacancyReference("12345").Value;

            // assert
            result.Should().Be(12345);
        }

        [Test]
        public void Can_Get_Value_As_String()
        {
            // act
            string result = new VacancyReference(12345).ToString();

            // assert
            result.Should().Be("VAC12345");
        }

        [Test]
        public void Can_Get_Value_As_Short_String()
        {
            // act
            string result = new VacancyReference(12345).ToShortString();

            // assert
            result.Should().Be("12345");
        }

        [Test]
        public void None_As_String_Is_Empty()
        {
            // act
            string result = new VacancyReference(string.Empty).ToString();

            // assert
            result.Should().Be(string.Empty);
        }

        [Test]
        public void None_As_Long_Is_Zero()
        {
            // act
            long result = VacancyReference.None.Value;

            // assert
            result.Should().Be(0);
        }


        [Test]
        public void Can_Parse_String()
        {
            // act
            var result = VacancyReference.Parse("12345", CultureInfo.CurrentCulture);

            // assert
            result.Should().Be(new VacancyReference("12345"));
        }

        [Test]
        public void Parse_Throws_On_Invalid_String()
        {
            // act
            var action = () => VacancyReference.Parse("foo", CultureInfo.CurrentCulture);

            // assert
            action.Should().Throw<InvalidVacancyReferenceException>();
        }

        [Test]
        public void Can_TryParse_String()
        {
            // act
            bool result = VacancyReference.TryParse("12345", out var vacancyReference);

            // assert
            result.Should().BeTrue();
            vacancyReference.Should().Be(new VacancyReference("12345"));
        }

        [Test]
        public void Can_TryParse_Null()
        {
            // act
            bool result = VacancyReference.TryParse(null, out var vacancyReference);

            // assert
            result.Should().BeTrue();
            vacancyReference.Should().Be(VacancyReference.None);
        }

        [Test]
        public void Can_TryParse_Invalid_String()
        {
            // act
            bool result = VacancyReference.TryParse("foo", out var vacancyReference);

            // assert
            result.Should().BeFalse();
            vacancyReference.Should().Be(VacancyReference.None);
        }

        [Test]
        public void Equals_Compares_Valid_Reference_Correctly()
        {
            // arrange
            var vacancyReference = new VacancyReference(1234);

            // act
            bool result = vacancyReference.Equals(new VacancyReference("1234"));

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void Equals_Compares_Null_Reference_Correctly()
        {
            // arrange
            VacancyReference? value = null;
            var vacancyReference = new VacancyReference(1234);

            // act
            bool result = vacancyReference.Equals(value);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_Compares_None_To_Null_Reference_Correctly()
        {
            // arrange
            VacancyReference? value = null;

            // act
            bool result = VacancyReference.None.Equals(value);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void HashCode_Is_Generated_Correctly()
        {
            // act
            int result = new VacancyReference(1234).GetHashCode();

            // assert
            result.Should().Be(1234.GetHashCode());
        }
    }
}
