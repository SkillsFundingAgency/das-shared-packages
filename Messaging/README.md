# SFA  Digital Apprenticeship Service Messaging Library

Provides a common interface to work with messaging sub-systems. Includes implementations for:

* File System (For local testing only)
* Azure Table Storage Queues
* Azure Service Bus

## How to get it

The core interface and all implementations are published to nuget.org. Use the following to add:

**Interfaces and File System**
```powershell
Install-Package SFA.DAS.Messaging
```

**Azure Table Storage Queues**
```powershell
Install-Package SFA.DAS.Messaging.AzureStorageQueue
```

**Azure Service Bus**
```powershell
Install-Package SFA.DAS.Messaging.AzureServiceBus
```

## Publishing a message

### Create a IMessagePublisher instance

**File System**
```csharp
IPollingMessageReceiver publisher = new FileSystemMessageService("YOUR_WORKING_DIR");
```

**Azure Table Storage Queue**
```csharp
IPollingMessageReceiver publisher = new AzureStorageQueueService("YOUR_CONNECTION_STRING", "YOUR_QUEUE_NAME");
```

**Azure Service Bus**
For messages with a single consumer
```csharp
IPollingMessageReceiver publisher = new AzureServiceBusMessageService("YOUR_CONNECTION_STRING", "YOUR_QUEUE_NAME");
```
Or for messages with multiple consumers
```csharp
ISubscribedMessagePublisher publisher = new AzureServiceBusSubscribedMessageService("YOUR_CONNECTION_STRING");
```

### Publish a message
```csharp
var message = new SampleEvent();
await publisher.PublishAsync(message)
```

## Polling to receive a message

### Create a IPollingMessageReceiver instance

**File System**
```csharp
IPollingMessageReceiver receiver = new FileSystemMessageService("YOUR_WORKING_DIR");
```

**Azure Table Storage Queue**
```csharp
IPollingMessageReceiver receiver = new AzureStorageQueueService("YOUR_CONNECTION_STRING", "YOUR_QUEUE_NAME");
```

**Azure Service Bus**
```csharp
IPollingMessageReceiver receiver = new AzureServiceBusMessageService("YOUR_CONNECTION_STRING", "YOUR_QUEUE_NAME");
```

### Poll in a loop to check for messages
```csharp
while (!token.IsCancellationRequested)
{
    // Get next message on the queue
    var message = await receiver.ReceiveAsAsync<SampleEvent>();
    if (message != null)
    {
        try
        {
            // Process message
            Process(message.Content);

            // Mark message as complete and remove from queue
            await message.CompleteAsync();
        }
        catch
        {
            // Abort message processing (will remove lock on message where applicable)
            await message.AbortAsync();
        }
    }
    else
    {
        // No message to process, wait for a bit before checking again
        await Task.Delay(1000, token);
    }
}
```
