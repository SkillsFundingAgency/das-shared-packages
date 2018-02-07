using System;
using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using HMRC.ESFA.Levy.Api.Types;

namespace SFA.DAS.HmrcApiSubstitute.WebAPI
{
    public class HmrcApiMessageHandler : ApiMessageHandlers
    {
        private IObjectCreator _objectCreator;

        public string GetEmploymentStatus = $"apprenticeship-levy/epaye/{(EmpRef)}/employed/{Nino}?fromDate={FromDate.ToString("yyyy-MM-dd")}&toDate={ToDate.ToString("yyyy-MM-dd")}";

        public const string EmpRef = "111/ABC00001";
        public const string Nino = "AB956884A";
        public static DateTime FromDate = new DateTime(2016,10,10);
        public static DateTime ToDate = DateTime.Now;

        public HmrcApiMessageHandler(string baseAddress) : base(baseAddress)
        {
            _objectCreator = new ObjectCreator();
            ConfigureDefaultResponse();
        }
        public void ConfigureDefaultResponse()
        {
            ConfigureGetEmploymentStatus();
        }

        public void OverrideGetSubmissionEvents<T>(T response)
        {
            SetupPut(GetEmploymentStatus);
            SetupGet(GetEmploymentStatus, response);
        }

        public void ConfigureGetEmploymentStatus()
        {
            var response = _objectCreator.Create<EmploymentStatus>(x => { x.Employed = true; x.Empref = EmpRef; x.Nino = Nino; x.FromDate = FromDate; x.ToDate = ToDate; });
            SetupGet(GetEmploymentStatus, response);
        }
        
    }
}