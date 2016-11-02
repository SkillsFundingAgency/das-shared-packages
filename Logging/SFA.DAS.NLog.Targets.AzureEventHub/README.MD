
## NLog Target for Azure Event Hub - DAS.SFA.NLog.Targets.AzureEventHub

A custom [NLog](https://github.com/nlog/nlog/wiki) target that pushes log events to [Azure Event Hub](https://azure.microsoft.com/en-gb/services/event-hubs/).

The Event Hub target works best with the [BufferingWrapper](https://github.com/nlog/nlog/wiki/BufferingWrapper-target) or [AsyncWrapper](https://github.com/nlog/nlog/wiki/AsyncWrapper-target) targets applied.

## Configuration Syntax

Add the following to your NLog.config file to reference the EventHub target assembly:

```xml
<extensions>
    <add assembly="SFA.DAS.NLog.Targets.AzureEventHub"/>
</extensions>
```

The target has the following configuration format:

```xml
<targets>
  <target xsi:type="AzureEventHub"
          name="String"
          appName="String"
          layout="Layout"
          eventHubConnectionStringKey="String"
          eventHubNameKey="String">
  </target>
</targets>
```
## Parameters
* _name_ - Name of the target. [String](String)
* _appName_ - Name of the application. Added as property to the log event. [String](String)
* _layout_ - (Optional) Layout that should be used to calculate the value for the **message** field. [Layout](https://github.com/nlog/nlog/wiki/Layouts)
* _eventHubConnectionString_ - Azure Event Hub connection string. [String](String)
* _eventHubName_ - Event Hub Name. [String](String)
* _eventHubConnectionStringKey_ The app setting key that should be used to for the Connection string [String](String)
 _eventHubNameKey_ - The app setting key that should be used for the Event Hub Name. [String](String)

### Example
```xml
<appSettings>
	<add key="EventHubName" value="Event hub name"/>
	<add key="EventHubConnectionString" value="Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=<sas name>;SharedAccessKey=<sas key>;TransportType=Amqp"/>
</appSettings>

<targets>
  <target xsi:type="AzureEventHub"
          name="eventHubLogger"
          appName="MyTestApplication"
          layout="${message}"
          eventHubConnectionStringKey="EventHubConnectionString"
          eventHubNameKey="EventHubName">
  </target>
</targets>
```