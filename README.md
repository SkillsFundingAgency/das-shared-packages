# Configuration
A shared library for storing configuration.

## Configuration - Azure table storage
Configuration is stored in a table called Configuration. The PartitionKey is the environment name, the RowKey is the service name and version number separated by an underscore. There is one extra column, Data, which is the Json representation of the configuration class for the service.
