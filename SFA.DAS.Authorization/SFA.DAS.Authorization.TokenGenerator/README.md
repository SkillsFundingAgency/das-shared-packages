# SFA.DAS.Authorization.TokenGenerator

## Context

The purpose of this project is to generate on demand a bearer token. It will create either a Employer or Provider token and can be used 
to call internalApi endpoints which require 'USER' bearer token authorization.

Its intended scope is to assist with testing and development.

The tokens generated are not intended for apps which use Identity managment authorization

## Setting up Configuration

* The repository includes example configurations in appsettings.json to help you get started. You can run the app with this configuration to generate a token and see it in action.
* In the configuration, you'll find three root properties:
  - UserBearerTokenSigningKey : This is the key the token will be signed with, this MUST match the key used by the api you are trying to call
  - ProviderUser : This object contains the claims that will be tokenized and signed for a Provider User
  - EmployerUser : This object contains the claims that will be tokenized and signed for a Employer User
* NOTE - While you can change the claim values for your user, do not change the claim keys. 
  e.g. if the claim is  - "sub": "abcd"
      you change 'abcd to 'efgh'
      but you can't change 'sub' to 'subject'

## Starting the Token Generator

* In the repository open the solution (SFA.DAS.Authorization.TokenGenerator.sln) in visual studio 
* Build the solution (it doesn't matter if its built in release mode or debug)
* Run the solution directly from Visual Studio or navigate to the build folder and run the executable from the command line 
  (e.g. SFA.DAS.Authorization.TokenGenerator.exe).

## Generating Tokens

* This process is fairly self explanitory and the console app will give you instructions
* First you will be prompted to select what type of user you are generating a token for (Provider or Employer)
* After selecting this it will display what claims are going to be tokenized
* It will then give you the option to generate the token, select a different user type or exit the app
* If you generate a token the output will be within the console, this can then be copied and pasted where needed.
* The tokens have a limited self life, so new tokens can be generated as needed


