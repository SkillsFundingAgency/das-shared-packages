

SFA.DAS.NLog.Targets.Redis
==========

This project is based on **NLog.Redis** but with the option of including all the properties of a LogEvent.  
Out but will be a json string.

## Installation

    Install-Package SFA.DAS.NLog.Targets.Redis

## Usage

The `<target />` configuration section contains five required fields and one optional field.

- connectionStringKey (required): Key for appSettings - Setts the Azure redis connection string
- DbSettingsKey (required): Key for appSettings - Setts the port the Redis server is listening on.
- KeySettingsKey (required): Key for appSettings - Setts the key that should be used to identify the Redis list or the pub/sub channel to which the log messages are being delivered.
- layout (required): The layout that defines the format of the message to be sent to the Redis target.
- appName (required): Name of the application. Added as property to the log event. 
- dataType (required): The Redis data type that should be used to store the log messages. This can be either `list` or `channel`.
- includeAllProperties:  true | false


## Config File

    <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" throwExceptions="true">
      <extensions>
        <add assembly="SFA.DAS.NLog.Targets.Redis" />
      </extensions>
      <targets>
           <target appName="" xsi:type="Redis" name="redis" connectionStringKey="Logging.Redis.ConnectionString" includeAllProperties="true"
                dataType="list" DbSettingsKey="Logging.Redis.Db" KeySettingsKey="Logging.Redis.Key"
                layout="${message}" />
      </targets>
      <rules>
        <logger name="*" minlevel="Info" writeTo="redis" />
      </rules>
    </nlog>
 
