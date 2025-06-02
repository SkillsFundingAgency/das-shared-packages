using System;
using AutoFixture;
using SFA.DAS.Common.Domain.Models;

namespace SFA.DAS.Testing.AutoFixture
{
    public class ValidVacancyReferenceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var random = new Random();
            string number = random.Next(1000000000, int.MaxValue).ToString()[..10];
            fixture.Register(() => new VacancyReference($"VAC{number}"));
        }
    }
}