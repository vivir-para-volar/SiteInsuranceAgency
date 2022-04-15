using InsuranceAgency.Models;
using System.Linq;
using System.Web.Mvc;

namespace InsuranceAgency.Controllers
{
    public class CreatePolicyController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: CreatePolicy
        public ActionResult Index()
        {
            return View(db.Policyholders.ToList());
        }

        // POST: CreatePolicy/ChooseCar
        [HttpPost]
        public ActionResult Index(int policyholderID)
        {
            Policyholder policyholder = db.Policyholders.Find(policyholderID);
            if (policyholder == null)
                return HttpNotFound();

            ViewBag.Policyholder = policyholder;
            return View("ChooseCar", db.Car.ToList());
        }

        // POST: CreatePolicy/ChooseCar
        [HttpPost]
        public ActionResult ChooseCar(int policyholderID, int carID)
        {
            Car car = db.Car.Find(carID);
            if (car == null)
                return HttpNotFound();

            ViewBag.Policyholder = db.Policyholders.Find(policyholderID);
            ViewBag.Car = car;
            return View("ChooseEmployee", db.Employees.ToList());
        }

        // POST: CreatePolicy/ChooseEmployee
        [HttpPost]
        public ActionResult ChooseEmployee(int policyholderID, int carID, int employeeID)
        {
            Employee employee = db.Employees.Find(employeeID);
            if (employee == null)
                return HttpNotFound();

            ViewBag.Policyholder = db.Policyholders.Find(policyholderID);
            ViewBag.Car = db.Car.Find(carID);
            ViewBag.Employee = employee;
            return View("ChooseInfo");
        }

        // POST: CreatePolicy/ChooseInfo
        [HttpPost]
        public ActionResult ChooseInfo([Bind(Include = "ID,InsuranceType,InsurancePremium,InsuranceAmount,DateOfConclusion,ExpirationDate,PolicyholderID,CarID,EmployeeID")] Policy policy)
        {
            if (ModelState.IsValid)
            {
                db.Policies.Add(policy);
                db.SaveChanges();
                return RedirectToAction("Index", "Policies");
            }
            ViewBag.Policyholder = db.Policyholders.Find(policy.PolicyholderID);
            ViewBag.Car = db.Car.Find(policy.CarID);
            ViewBag.Employee = db.Employees.Find(policy.EmployeeID);
            return View(policy);
        }
    }
}