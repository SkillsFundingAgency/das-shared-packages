

SFA.DAS.NLog.Targets.Redis
==========

This project is based on **NLog.Redis** but with the option of including all the properties of a LogEvent and the ability to add fields to include in the nlog configuration file.  

## Setup

    Install-Package SFA.DAS.NLog.Targets.Redis

### NLog Configuration

The `<target />` configuration section contains three required fields and two optional ones.

- **connectionStringName** (required): The ConnectionString name - Sets the Azure redis connection string
- **layout** (required): The layout that defines the format of the message field to be sent to the Redis target.
- **appName** (required): Name of the application. Added as a field to the log event. 
- **environmentKeyName** (optional): The settings key name from configuration - Sets the name of the configuration key that should be used to identify the Environment the application is running in.
- **includeAllProperties**:  true | false


#### Example Config File

    <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" throwExceptions="true">
      <extensions>

        <!-- Add reference to target assembly -->
        <add assembly="SFA.DAS.NLog.Targets.Redis" /> 

      </extensions>
      <targets>
           <target xsi:type="Redis" name="redis" appName="" environmentKeyName="EnvironmentName" connectionStringName="Redis" includeAllProperties="true"
                keySettingsKey="Logging.Redis.Key"
                layout="${message}" />

        <!-- Optional or multiple additional fields -->
        <field name="RequestId" layout="${aspnet-traceidentifier}"/>
      </targets>
      <rules>
        <logger name="*" minlevel="Info" writeTo="redis" />
      </rules>
    </nlog>
 
## Application Configuration (.Net Framework)

The keys specified in the **connectionStringName** (and optionally **environmentKeyName**) need to be available in AppSettings for the application.

<appSettings>
    <add key="Redis" value="my.dev.redis:6380,password=mysecurepassword,defaultDatabase=0,ssl=True"/>
    <add key="EnvironmentName" value="Test" />
</appSettings>


## Application Configuration (.Net Standard)

The keys specified in the **connectionStringName** (and optionally **environmentKeyName**) need to be available within the Configuration system provided by one the following providers:

- EnvironmentVariables
- appSettings.json
- appSettings.Development.json
