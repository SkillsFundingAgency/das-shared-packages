# SFA.DAS.InputValidation

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status%2Fdas-shared-packages-SFA.DAS.InputValidation?repoName=SkillsFundingAgency%2Fdas-shared-packages&branchName=refs%2Fpull%2F817%2Fmerge)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=3757&repoName=SkillsFundingAgency%2Fdas-shared-packages&branchName=refs%2Fpull%2F817%2Fmerge)

[![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.InputValidation)](https://www.nuget.org/packages/SFA.DAS.InputValidation/)

Library for input validation against malicious characters that should be stopped


## Input Validation

Example usage for free text validation. Purpose of this is to stop users entering characters into input fields that could be used to cause damage to our downstream services

```csharp
    public class SomeTestClassValidator : AbstractValidator<SomeTestClass>
    {
        public SomeTestClassValidator()
        {
            RuleFor(x => x.MyTestString)
                .ValidFreeTextCharacters();
        }
    }

    public class SomeTestClass
    {
        public string? MyTestString { get; set; }
    }

```

