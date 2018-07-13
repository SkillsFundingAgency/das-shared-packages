using System.Data.Common;

namespace SFA.DAS.NServiceBus.MsSqlServer
{
    public static class DbCommandExtensions
    {
        public static void AddParameter(this DbCommand command, string parameterName, object value)
        {
            var param = command.CreateParameter();

            param.ParameterName = parameterName;
            param.Value = value;

            command.Parameters.Add(param);
        }
    }
}