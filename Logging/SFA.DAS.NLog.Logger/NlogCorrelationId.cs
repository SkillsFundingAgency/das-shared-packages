using System;

using NLog;

namespace SFA.DAS.NLog.Logger
{
    public class NlogCorrelationId
    {
        /// <summary>
        /// Setting correltation id with format Name-Date-UniqueId for JobCorrelationId key.
        /// <para />
        /// NlogCorrelationId.SetJobCorrelationId("My Web Job", true);
        /// <para />
        /// Example: MyWebJob-24-Nov-2017-750896dded2f430eaeaad350ec02bdf7
        /// </summary>
        /// <param name="name">Optional prefix. Whitespace will be removed</param>
        /// <param name="useDate">If date should be included. (24-Nov-2017) </param>
        public static void SetJobCorrelationId(string name = "", bool useDate = false)
        {
            var date = useDate ? $"-{DateTime.Now.ToString("dd-MMM-yyy")}" : string.Empty;
            var id = Guid.NewGuid().ToString("N");
            var jobCorrelationId = $"{name.Replace(" ", "")}{date}-{id}";
            MappedDiagnosticsLogicalContext.Set(Constants.JobCorrelationId, jobCorrelationId);
        }
    }
}
