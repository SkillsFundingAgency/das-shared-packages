using System.Collections.Generic;

using FluentAssertions;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.Commitments.Api.Client.UnitTests
{

    [TestFixture]
    public class HelperTest
    {
        private QueryStringHelper _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new QueryStringHelper();
        }

        [Test]
        public void TestDefaultQuery()
        {
            var query = new ApprenticeshipSearchQuery();
            var queryString = _sut.GetQueryString(query);

            queryString.Should().Be("?PageNumber=1&PageSize=25");
        }

        [Test]
        public void TestEmptyQueryNull()
        {
            var queryString = _sut.GetQueryString(null);

            queryString.Should().Be("");
        }

        [Test]
        public void TestApprenticeshipStatus()
        {
            var query = new ApprenticeshipSearchQuery
                            {
                                ApprenticeshipStatuses = new List<ApprenticeshipStatus> { ApprenticeshipStatus.Finished, ApprenticeshipStatus.Paused }
                            };

            var queryString = _sut.GetQueryString(query);

            queryString.Should().Be("?ApprenticeshipStatuses=Finished&ApprenticeshipStatuses=Paused&PageNumber=1&PageSize=25");
        }

        [Test]
        public void TestRecordStatuses()
        {
            var query = new ApprenticeshipSearchQuery
            {
                RecordStatuses = new List<RecordStatus> { RecordStatus.ChangeRequested, RecordStatus.ChangesPending }
            };

            var queryString = _sut.GetQueryString(query);

            queryString.Should().Be("?RecordStatuses=ChangeRequested&RecordStatuses=ChangesPending&PageNumber=1&PageSize=25");
        }

        [Test]
        public void ShouldHandleAllEmptyLists()
        {
            var query = new ApprenticeshipSearchQuery
            {
                ApprenticeshipStatuses = new List<ApprenticeshipStatus>(),
                RecordStatuses = new List<RecordStatus>(),
                EmployerOrganisationIds = new List<string>(),
                TrainingCourses = new List<string>(),
                TrainingProviderIds = null
            };

            var queryString = _sut.GetQueryString(query);

            queryString.Should().Be("?PageNumber=1&PageSize=25");
        }
    }
}