using AutoFixture;
using SFA.DAS.Common.Domain.Models;

namespace SFA.DAS.Testing.AutoFixture
{
    public class ValidVacancyReferenceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new VacancyReference("VAC1234567890"));
        }
    }
}