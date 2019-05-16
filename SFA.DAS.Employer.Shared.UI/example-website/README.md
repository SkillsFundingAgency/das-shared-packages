# Example Website

This project is an example of how to use the shared menu package. 


## Setup

1. Update the *appSetting.json* file. Add setting for the **oidc** section with some valid employer idams values.
2. Ensure the Kestral port is the same as that configured for the relying party in step 1.
3. Run Azure Storage Emulator
4. Either create the config row with RowKey *SFA.DAS.Employer.Shared.UI_1.0* manually based on contents from the configuration repository OR use the *das-config-updater* tool to generate all application configurations.
5. Run the project. 
