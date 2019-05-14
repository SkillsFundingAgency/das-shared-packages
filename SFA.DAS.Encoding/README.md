# SFA.DAS.Encoding

## Local development
- Clone the repo `das-shared-packages`
- Navigate to the `src/SFA.DAS.Encoding` folder
- Open the .sln file in your editor of choice (e.g. Visual Studio 2017)
- Build (<kbd>F6</kbd>), run all tests (<kbd>Ctrl</kbd>+<kbd>U</kbd>, <kbd>Ctrl</kbd>+<kbd>L</kbd> if using Resharper).

## IoC
In your IoC it is recommended to register the `EncodingConfig` by reading and parsing the config json:
```c#
var encodingConfigJson = configuration.GetSection(nameof(EncodingConfig)).Value;
var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
services.AddSingleton(encodingConfig);
```

Also, you should register `IEncodingService` as a singleton, e.g. using .NET Core:

`services.AddSingleton<IEncodingService, EncodingService>();`

## Configuration
Please read the configuration for this package, located in the [`das-employer-config` repository](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-reservations/SFA.DAS.Encoding.json).
This configuration is already available for DAS Azure based deployments so does not need to be deployed again.
In order to utilise the configuration you will need to include `SFA.DAS.Encoding` in the list of config names for your ARM template.
