using System.Web.Mvc;

namespace InsuranceAgency.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.MainPhoto = System.IO.File.ReadAllBytes(Server.MapPath("~/fonts/main.webp"));
            return View();
        }
    }
}