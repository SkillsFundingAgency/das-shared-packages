using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Recruit.Vacancies.Client.Entities;

namespace SFA.DAS.Recruit.Vacancies.Client.UnitTests
{
    [TestFixture]
    public class ApplicationSubmitTests
    {

        [Ignore("A helper test to add a message to the queue when debugging")] 
        [Test]
        public void SubmitApplication()
        {
            var vacancyApplication = GetApplication();

            var sut = new Client(null, null, null, "UseDevelopmentStorage=true");

            sut.SubmitApplication(vacancyApplication);
        }

        private Application GetApplication()
        {
            return new Application
            {
                AddressLine1 = "address line 1",
                AddressLine2 = "address line 2",
                AddressLine3 = "address line 3",
                AddressLine4 = "address line 4",
                ApplicationDate = DateTime.Now,
                BirthDate = DateTime.Parse("2009-10-29"),
                CandidateId = Guid.NewGuid(),
                DisabilityStatus = "Yes",
                EducationFromYear = 2010,
                EducationInstitution = "education institution",
                EducationToYear = 2011,
                Email = "email",
                FirstName = "first name",
                HobbiesAndInterests = "hobbies",
                Improvements = "improvements",
                LastName = "last name",
                Phone = "phone",
                Postcode = "postcode",
                Qualifications = new List<ApplicationQualification>
                {
                    new ApplicationQualification
                    {
                        Grade = "grade 1",
                        IsPredicted = true,
                        QualificationType = "qualification type 1",
                        Subject = "subject 1",
                        Year = 2011
                    },
                    new ApplicationQualification
                    {
                        Grade = "grade 2",
                        IsPredicted = false,
                        QualificationType = "qualification type 2",
                        Subject = "subject 2",
                        Year = 2012
                    }
                },
                Skills = new List<string> { "skill 1", "skill 2" },
                Strengths = "strengths",
                Support = "support",
                AdditionalQuestion1 = "Additional Question 1",
                AdditionalQuestion2 = "Additional Question 2",
                TrainingCourses = new List<ApplicationTrainingCourse>
                {
                    new ApplicationTrainingCourse
                    {
                        FromDate = DateTime.Parse("2000-01-01"),
                        Provider = "provider 1",
                        Title = "title 1",
                        ToDate = DateTime.Parse("2000-01-02")
                    },
                    new ApplicationTrainingCourse
                    {
                        FromDate = DateTime.Parse("2000-02-01"),
                        Provider = "provider 2",
                        Title = "title 2",
                        ToDate = DateTime.Parse("2000-02-02")
                    }
                },
                VacancyReference = 1000000016,
                WorkExperiences = new List<ApplicationWorkExperience>
                {
                    new ApplicationWorkExperience
                    {
                        ToDate = DateTime.Parse("2001-01-01"),
                        FromDate = DateTime.Parse("2001-01-02"),
                        Description = "description 1",
                        Employer = "employer 1",
                        JobTitle = "job title 1"
                    },
                    new ApplicationWorkExperience
                    {
                        ToDate = DateTime.Parse("2001-02-01"),
                        FromDate = DateTime.Parse("2001-02-02"),
                        Description = "description 2",
                        Employer = "employer 2",
                        JobTitle = "job title 2"
                    }
                }
            };
        }
    }
}
