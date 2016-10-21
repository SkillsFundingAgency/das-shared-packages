

SFA.DAS.NLog.Targets.Redis
==========

This project is based on **NLog.Redis** but with the option of including all the properties of a LogEvent.  
Out but will be a json string.

## Installation

    Install-Package SFA.DAS.NLog.Targets.Redis


## Usage

The `<target />` configuration section contains five required fields and one optional field.

- host (required): The host name or IP Address of the Redis server.
- port (required): The port the Redis server is listening on.
- key (required): The key that should be used to identify the Redis list or the pub/sub channel to which the log messages are being delivered.
- password: password for redis server
- dataType (required): The Redis data type that should be used to store the log messages. This can be either `list` or `channel`.
- layout (required): The layout that defines the format of the message to be sent to the Redis target.
- db (optional): The Redis database id to store the log messages in, if the Redis database type `list` is chosen.
- appName (required): Name of the application. Added as property to the log event. 
- includeAllProperties:  true | false


## Config File

    <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" throwExceptions="true">
      <extensions>
        <add assembly="SFA.DAS.NLog.Targets.Redis" />
      </extensions>
      <targets>
        <target xsi:type="Redis" name="redis" host="127.0.0.1" port="3679" db="0" 
                key="logKey" dataType="list" 
                layout="${message}" includeAllProperties="true" password="abba42" />
      </targets>
      <rules>
        <logger name="*" minlevel="Info" writeTo="redis" />
      </rules>
    </nlog>
 
