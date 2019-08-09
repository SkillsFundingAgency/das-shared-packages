# Encoding Dev Console

## Getting started

1. ensure you have the [encoding configuration](https://github.com/SkillsFundingAgency/das-employer-config) loaded into your local azure storage.
1. clone the repo locally
1. build with visual studio or `dotnet build`

## Example usage

1. help: `dotnet SFA.DAS.Encoding.DevConsole.dll -h`
1. encode `3` as a PublicAccountLegalEntityId: `dotnet SFA.DAS.Encoding.DevConsole.dll -v=3 -t=PublicAccountLegalEntityId -a=encode`
1. decode `dotnet SFA.DAS.Encoding.DevConsole.dll -v NXV3XN -t PublicAccountLegalEntityId`
