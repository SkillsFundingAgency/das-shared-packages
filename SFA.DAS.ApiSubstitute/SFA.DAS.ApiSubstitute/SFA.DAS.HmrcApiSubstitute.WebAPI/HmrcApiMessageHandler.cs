using System;
using SFA.DAS.ApiSubstitute.Utilities;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using HMRC.ESFA.Levy.Api.Types;
using System.Net;

namespace SFA.DAS.HmrcApiSubstitute.WebAPI
{
    public class HmrcApiMessageHandler : ApiMessageHandlers
    {
        public string DefaultGetEmploymentStatusEndPoint { get; private set; }

        private IObjectCreator _objectCreator;
        
        public const string EmpRef = "111/ABC00001";
        public const string Nino = "AB956884A";
        public static DateTime FromDate = new DateTime(2016,10,10);
        public static DateTime ToDate = DateTime.Now;

        public HmrcApiMessageHandler(string baseAddress) : base(baseAddress)
        {
            _objectCreator = new ObjectCreator();
            ConfigureDefaultResponse();
        }

        private void ConfigureDefaultResponse()
        {
            ConfigureGetEmploymentStatus();
        }

        public void OverrideGetSubmissionEvents<T>(T response, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupPatch(DefaultGetEmploymentStatusEndPoint, httpStatusCode, response);
        }

        public void SetupGetEmploymentStatus<T>(T response, string empRef, string nino, DateTime? fromDate = null, DateTime? toDate = null, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            SetupPatch(GetEmploymentStatus(empRef, nino, fromDate, toDate), httpStatusCode, response);
        }

        private void ConfigureGetEmploymentStatus()
        {
            var response = _objectCreator.Create<EmploymentStatus>(x => { x.Employed = true; x.Empref = EmpRef; x.Nino = Nino; x.FromDate = FromDate; x.ToDate = ToDate; });

            DefaultGetEmploymentStatusEndPoint = GetEmploymentStatus(EmpRef, Nino, FromDate, ToDate);

            SetupGet(DefaultGetEmploymentStatusEndPoint, response);
        }

        private string GetEmploymentStatus(string empRef, string nino, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var route = $"apprenticeship-levy/epaye/{(empRef)}/employed/{nino}";

            string endpoint = route;

            if (fromDate.HasValue)
            {
                endpoint = route + $"?fromDate={fromDate?.ToString("yyyy-MM-dd")}";
            }

            if (toDate.HasValue)
            {
                endpoint = route + $"?toDate={toDate?.ToString("yyyy-MM-dd")}";
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                endpoint = route + $"?fromDate={fromDate?.ToString("yyyy-MM-dd")}&toDate={toDate?.ToString("yyyy-MM-dd")}";
            }

            return endpoint;
        }
    }
}