using System;
using NUnit.Framework;

namespace SFA.DAS.Recruit.Vacancies.Client.UnitTests
{
    [TestFixture]
    public class ApplicationWithdrawTests
    {
        [Ignore("A helper test to add a message to the queue when debugging")]
        [Test]
        public void WithdrawApplication()
        {
            var sut = new Client(null, null, null, "UseDevelopmentStorage=true");

            sut.WithdrawApplication(1234567890, Guid.NewGuid());
        }
    }
}
