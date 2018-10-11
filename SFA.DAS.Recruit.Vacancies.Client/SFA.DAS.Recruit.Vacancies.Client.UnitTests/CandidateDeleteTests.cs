using System;
using NUnit.Framework;

namespace SFA.DAS.Recruit.Vacancies.Client.UnitTests
{
    [TestFixture]
    public class CandidateDeleteTests
    {
        [Ignore("A helper test to add a message to the queue when debugging")]
        [Test]
        public void DeleteCandidate()
        {
            var sut = new Client(null, null, null, "UseDevelopmentStorage=true");

            sut.DeleteCandidate(Guid.NewGuid());
        }
    }
}
