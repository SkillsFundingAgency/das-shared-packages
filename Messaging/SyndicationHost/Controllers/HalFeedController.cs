using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Messaging.Syndication.Hal;
using SFA.DAS.Messaging.Syndication.SqlServer;
using SyndicationHost.Hal;

namespace SyndicationHost.Controllers
{
    public class HalFeedController : Controller
    {
        // GET: HalFeed
        public async Task<ActionResult> Index(int page = 1)
        {
            var messageService = new HalJsonMessageService<Message>(
                new SqlServerMessageRepository("server=.;database=scratchpad;trusted_connection=true;", "usp_StoreMessage", "usp_GetPageOfMessages"),
                new HalResourceAttributeExtrator(),
                new PageLinkBuilder());
            var halPage = await messageService.GetPageAsync(page, 10);
            var contentType = halPage.Headers["Content-Type"].DefaultIfEmpty("application/hal+json").FirstOrDefault();

            return Content(halPage.Content, contentType);
        }
    }
}