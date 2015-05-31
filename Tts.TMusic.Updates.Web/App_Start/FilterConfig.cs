using System.Web;
using System.Web.Mvc;

namespace Tts.TMusic.Updates.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
