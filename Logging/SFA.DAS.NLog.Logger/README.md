# SFA.DAS.NLog.Logging

Library to handle logging for SFA.DAS using NLog.

---
### `ILog` and `NLogLogger`
Defines NLog level-methods with optional `IDictionary<string, object> properties` and `ILogEntry logEntry`. The properties option maps the dictionary to NLog properties and the logEntry will take all the properties from an object implementing the ILogEntry and create the `<string,object>` dictionary.

---
### Logging ids between method calls and applications 
#### Create correlation id. 
To be able to follow request between method calls and application we can use the use correlation ids for each request and/or sessions. This is coupled with NLog and is using `MappedDiagnosticsLogicalContext` to store the ids. 
Using an Action filter can populate the `MappedDiagnosticsLogicalContext` each time someone call an MVC action and this ID will then be logged as a property in NLog.  

The `RequestIdActionFilter` wil create a new GUID for each request and the `SessionIdActionFilter` will try to read the sessionId from a cookie to populate the `MappedDiagnosticsLogicalContext`. If no cookie found the action filter will create a new cookie. 

We can also set the value in `MappedDiagnosticsLogicalContext` by using the keys:  
* `Constants.HeaderNameSessionCorrelationId`  
* `Constants.HeaderNameRequestCorrelationId` 
* `Constants.JobCorrelationId` 

#### Sending correlation ids to other applications
To send the id to another application via HTTP we can insert a delegate handler to the HttpClient. The `RequestIdMessageRequestHandler` and `SessionIdMessageRequestHandler` will add the ids to `HttpRequestMessage` header by grabbing the Session or Request id from NLog `MappedDiagnosticsLogicalContext`.

#### Receiving correlation ids from header
HttpActionFilter should be added to the APIs that want to read the header and populate `MappedDiagnosticsLogicalContext` with its value. 
RequestIdHttpActionFilter and SessionIdHttpActionFilter.

#### Setting correlation id for console applications.
The NlogCorrelationId class can be used to set a unique id and will be logged out for the key `DasJobCorrelationId`  

Use like: `NlogCorrelationId.SetJobCorrelationId("My Web Job", true);`  
Result: `MyWebJob-24-Nov-2017-750896dded2f430eaeaad350ec02bdf7`

---
