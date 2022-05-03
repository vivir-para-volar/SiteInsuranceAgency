using System.Web.Mvc;

namespace InsuranceAgency.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}