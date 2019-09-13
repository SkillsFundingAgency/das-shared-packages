using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Hmrc.Http
{
    public interface IHttpResponseLogger
    {
        Task LogResponseAsync(ILog logger, HttpResponseMessage response);
    }
}