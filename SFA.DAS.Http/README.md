# SFA.DAS.Http

![badge](https://sfa-gov-uk.visualstudio.com/DefaultCollection/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/405/badge)

[![NuGet Badge](https://buildstats.info/nuget/SFA.DAS.Http)](https://www.nuget.org/packages/SFA.DAS.Http/)

Library containing Http related classes such as base classes for connecting to Api's.


## Http

### Example Usage
Create a custom Api Client class using the RestHttpClient class and HttpClientBuilder.
```csharp
public class Example
    {
        public async Task CustomApiClientExample()
        {
            HttpClient httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(new MyClientConfiguration()))
                .Build();

            var restApiClient = new RestHttpClient(httpClient);

            var result = await restApiClient.Get("http://some-host/some-resource/");
        }

        private class MyClientConfiguration : IJwtClientConfiguration
        { 
            public string ClientToken { get => "SomeTokenValue"; }
        }        
    }
```

## HttpClientBuilder

This class builds a HttpClient with specified configuration:

### `WithDefaultHeaders()`
Adds default headers such as Accept header.

### `WithHandler(DelegatingHandler)`
Adds a handler that inherits from _System.Net.Http._`DelegatingHandler`

### `WithBearerTokenHeader(IGenerateBearerToken)`
Adds a Bearer header with the value provided by the implementation of `IGenerateBearerToken` interface that is passed in. Currently two generators are provided:
* `JwtBearerTokenGenerator`
* `AzureADBearerTokenGenerator`
