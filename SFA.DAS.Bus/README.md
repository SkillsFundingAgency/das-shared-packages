# SFA  Digital Apprenticeship Service Bus Library

Provides a common interface to work with service bus systems. Includes implementations for:

* Azure Service Bus

## How to get it

The core interface and all implementations are published to nuget.org. Use the following to add:

**Client Interfaces**
```powershell
Install-Package SFA.DAS.Bus.Client
```

**Azure Service Bus Client**
```powershell
Install-Package SFA.DAS.Bus.Client.AzureServiceBus
```

## Publishing a message

### Create a IBusClient instance

**Azure Service Bus**
```csharp
IBusClient busClient = new AzureServiceBusClient("YOUR_CONNECTION_STRING");
```

### Publish a message
```csharp
var message = new SampleEvent();
await publisher.PublishAsync<SampleEvent>(message)
```