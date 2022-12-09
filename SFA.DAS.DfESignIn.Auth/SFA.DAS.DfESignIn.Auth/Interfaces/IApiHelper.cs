using System;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IApiHelper
    {
        Task<T> Get<T>(string endpoint);
    }
}
