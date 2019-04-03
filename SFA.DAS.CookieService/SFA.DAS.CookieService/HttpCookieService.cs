using System;
using System.Text;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;

namespace SFA.DAS.CookieService
{
    public class HttpCookieService<T> : ICookieService<T>
    {
        public void Create(HttpContextBase context, string name, T content, int expireDays)
        {
            var cookieContent = JsonConvert.SerializeObject(content);

            var encodedContent = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(cookieContent)));

            var userCookie = new HttpCookie(name, encodedContent)
            {
                Expires = DateTime.Now.AddDays(expireDays),
                Secure = true,
                HttpOnly = true,
            };

            context.Response.Cookies.Add(userCookie);
        }

        public void Update(HttpContextBase context, string name, T content)
        {
            var cookie = context.Request.Cookies[name];

            if (cookie != null)
            {
                var cookieContent = JsonConvert.SerializeObject(content);

                var encodedContent = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(cookieContent)));

                cookie.Value = encodedContent;

                context.Response.SetCookie(cookie);
            }
        }

        public void Delete(HttpContextBase context, string name)
        {
            if (context.Request.Cookies[name] != null)
            {
                var cookie = new HttpCookie(name)
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Value = null
                };
                context.Response.SetCookie(cookie);
            }
        }

        public T Get(HttpContextBase context, string name)
        {
            if (context.Request.Cookies[name] == null)
                return default(T);

            var base64EncodedBytes = Convert.FromBase64String(context.Request.Cookies[name].Value);
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(MachineKey.Unprotect(base64EncodedBytes)));
        }
    }
}
