

SFA.DAS.NLog.Targets.Redis
==========

This project is based on **NLog.Redis** but with the option of including all the properties of a LogEvent.  

## Installation

    Install-Package SFA.DAS.NLog.Targets.Redis

## Usage

The `<target />` configuration section contains six required fields and one optional field.

- connectionStringKey (required): Key for appSettings - Sets the Azure redis connection string
- keySettingsKey (required): Key for appSettings - Sets the key that should be used to identify the Redis list or the pub/sub channel to which the log messages are being delivered.
- layout (required): The layout that defines the format of the message to be sent to the Redis target.
- appName (required): Name of the application. Added as property to the log event. 
- environment (optional): Name of the environment.
- includeAllProperties:  true | false


## Config File

    <appSettings>
        <add key="Logging.Redis.ConnectionString" value="my.dev.redis:6380,password=mysecurepassword,defaultDatabase=0,ssl=True"/>
        <add key="Logging.Redis.Key" value="logsKey" />
        <add key="EnvironmentName" value="Test" />
    </appSettings>


    <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" throwExceptions="true">
      <extensions>
        <add assembly="SFA.DAS.NLog.Targets.Redis" />
      </extensions>
      <targets>
           <target xsi:type="Redis" name="redis" appName="" environmentKey="EnvironmentName" connectionStringKey="Logging.Redis.ConnectionString" includeAllProperties="true"
                keySettingsKey="Logging.Redis.Key"
                layout="${message}" />
      </targets>
      <rules>
        <logger name="*" minlevel="Info" writeTo="redis" />
      </rules>
    </nlog>
 
