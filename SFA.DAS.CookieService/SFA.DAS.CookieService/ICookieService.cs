using System.Web;

namespace SFA.DAS.CookieService
{
    public interface ICookieService<T>
    {
        void Create(HttpContextBase context, string name, T content, int expireDays);
        void Update(HttpContextBase context, string name, T content);
        void Delete(HttpContextBase context, string name);
        T Get(HttpContextBase context, string name);
    }
}