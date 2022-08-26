using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.Recruit.Vacancies.Client.UnitTests.ClientTests
{
    public class GetApprenticeshipProgrammesTests
    {
        [Test, Ignore("needs db")]
        public async Task Then_Gets_ApprenticeshipProgrammes_From_Recruit_Mongo()
        {
            var client = new Client("", "", "", "");
            var result = await client.GetApprenticeshipProgrammes();

            Assert.That(result.Count > 0);
        }
    }
}
