using FluentAssertions;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.UnitTests.Models
{
    public class GovUkCredentialSubjectTests
    {
        [TestCase("2000-01-01", "2020-01-01")]
        [TestCase(null, "2020-01-01")]
        [TestCase("2000-01-01", null)]
        [TestCase(null, null)]
        public void When_Only_One_Name_And_Full_Validity_Period_Then_Single_HistoricalName_Returned(string? validFromRaw, string? validUntilRaw)
        {
            var subject = new GovUkCredentialSubject
            {
                Names = new List<GovUkName>
                {
                    new GovUkName
                    {
                        ValidFromRaw = validFromRaw,
                        ValidUntilRaw = validUntilRaw,
                        NameParts = new List<GovUkNamePart>
                        {
                            new GovUkNamePart
                            {
                                Type = "GivenName",
                                Value = "Alice"
                            },
                            new GovUkNamePart
                            {
                                Type = "FamilyName",
                                Value = "Smith"
                            }
                        }
                    }
                }
            };

            var result = subject.GetHistoricalNames();

            result.Should().HaveCount(1);
            result[0].GivenNames.Should().Be("Alice");
            result[0].FamilyNames.Should().Be("Smith");
            result[0].ValidFrom.Should().Be(validFromRaw != null ? DateTime.Parse(validFromRaw) : DateTime.MinValue);
            result[0].ValidUntil.Should().Be(validUntilRaw != null ? DateTime.Parse(validUntilRaw) : DateTime.MaxValue);
        }

        [Test]
        public void Then_If_NameParts_Change_Over_Time_HistoricalNames_Are_Split()
        {
            var subject = new GovUkCredentialSubject
            {
                Names = new List<GovUkName>
                {
                    new GovUkName
                    {
                        NameParts = new List<GovUkNamePart>
                        {
                            new GovUkNamePart
                            {
                                Type = "GivenName",
                                Value = "Alice",
                                ValidFromRaw = null,
                                ValidUntilRaw = "2010-01-01"
                            },
                            new GovUkNamePart
                            {
                                Type = "GivenName",
                                Value = "Alicia",
                                ValidFromRaw = "2010-01-01",
                                ValidUntilRaw = null
                            },
                            new GovUkNamePart
                            {
                                Type = "FamilyName",
                                Value = "Smith"
                            }
                        }
                    }
                }
            };

            var result = subject.GetHistoricalNames();

            result.Should().HaveCount(2);
            result[0].GivenNames.Should().Be("Alice");
            result[0].FamilyNames.Should().Be("Smith");
            result[0].ValidFrom.Should().Be(DateTime.MinValue);
            result[0].ValidUntil.Should().Be(DateTime.Parse("2010-01-01"));

            result[1].GivenNames.Should().Be("Alicia");
            result[1].FamilyNames.Should().Be("Smith");
            result[1].ValidFrom.Should().Be(DateTime.Parse("2010-01-01"));
            result[1].ValidUntil.Should().Be(DateTime.MaxValue); 
        }

        [Test]
        public void Then_If_NameParts_Are_Identical_And_Adjacent_They_Are_Collapsed()
        {
            var subject = new GovUkCredentialSubject
            {
                Names = new List<GovUkName>
                {
                    new GovUkName
                    {
                        NameParts = new List<GovUkNamePart>
                        {
                            new GovUkNamePart
                            {
                                Type = "GivenName",
                                Value = "Alice",
                                ValidFromRaw = null,
                                ValidUntilRaw = "2010-01-01"
                            },
                            new GovUkNamePart
                            {
                                Type = "GivenName",
                                Value = "Alice",
                                ValidFromRaw = "2010-01-01",
                                ValidUntilRaw = null
                            },
                            new GovUkNamePart
                            {
                                Type = "FamilyName",
                                Value = "Smith"
                            }
                        }
                    }
                }
            };

            var result = subject.GetHistoricalNames();

            result.Should().HaveCount(1);
            result[0].GivenNames.Should().Be("Alice");
            result[0].FamilyNames.Should().Be("Smith");
            result[0].ValidFrom.Should().Be(DateTime.MinValue);
            result[0].ValidUntil.Should().Be(DateTime.MaxValue);
        }

        [Test]
        public void Then_If_No_NameParts_Returns_Empty_List()
        {
            var subject = new GovUkCredentialSubject
            {
                Names = new List<GovUkName>()
            };

            var result = subject.GetHistoricalNames();

            result.Should().BeEmpty();
        }

        [Test]
        public void Then_If_NameParts_Empty_But_Names_Present_Returns_Empty()
        {
            var subject = new GovUkCredentialSubject
            {
                Names = new List<GovUkName>
                {
                    new GovUkName
                    {
                        NameParts = new List<GovUkNamePart>()
                    }
                }
            };

            var result = subject.GetHistoricalNames();

            result.Should().BeEmpty();
        }
    }
}
