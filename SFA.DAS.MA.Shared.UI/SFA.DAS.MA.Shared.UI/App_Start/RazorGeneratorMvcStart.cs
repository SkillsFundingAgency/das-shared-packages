
#if NET462
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(SFA.DAS.MA.Shared.UI.App_Start.RazorGeneratorMvcStart), "Start")]
namespace SFA.DAS.MA.Shared.UI.App_Start
{
    public static class RazorGeneratorMvcStart
    {
        public static void Start()
        {

            var engine = new RazorGenerator.Mvc.PrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly)
            {
                UsePhysicalViewsIfNewer = System.Web.HttpContext.Current.Request.IsLocal
            };

            System.Web.Mvc.ViewEngines.Engines.Insert(0, engine);

            // StartPage lookups are done by WebPages. 
            System.Web.WebPages.VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}
#endif