using System.Web;
using System.Web.Mvc;

namespace Bertoni.TestFinale.Activity.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
