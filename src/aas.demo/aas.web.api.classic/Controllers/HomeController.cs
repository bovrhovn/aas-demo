using System.Web.Mvc;

namespace aas.web.api.classic.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() => View();
    }
}