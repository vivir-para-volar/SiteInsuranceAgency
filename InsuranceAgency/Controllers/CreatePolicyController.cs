using InsuranceAgency.Models.ViewModels;
using InsuranceAgency.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using InsuranceAgency.Models.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace InsuranceAgency.Controllers
{
    [Authorize(Roles = "Administrator, Operator")]
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public ActionResult ChooseCar(int policyholderID, int carID)
        {
            Car car = db.Car.Find(carID);
            if (car == null)
                return HttpNotFound();

            var identitydb = new MyIdentityDbContext();
            var userManager = new UserManager<MyIdentityUser>(new UserStore<MyIdentityUser>(identitydb));
            var userTelephone = userManager.GetPhoneNumber(User.Identity.GetUserId());
            Employee employee = db.Employees.First(e => e.Telephone == userTelephone);

            ViewBag.Policyholder = db.Policyholders.Find(policyholderID);
            ViewBag.Car = car;
            ViewBag.Employee = employee;

            ViewBag.InsuranceType = new SelectList(new List<string> { "ОСАГО", "КАСКО" });
            ViewBag.Period = new SelectList(new List<string> { "6 месяцев", "12 месяцев" });
            return View("ChooseInfo");
        }

        //// POST: CreatePolicy/ChooseEmployee
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ChooseEmployee(int policyholderID, int carID, int employeeID)
        //{
        //    Employee employee = db.Employees.Find(employeeID);
        //    if (employee == null)
        //        return HttpNotFound();

        //    ViewBag.Policyholder = db.Policyholders.Find(policyholderID);
        //    ViewBag.Car = db.Car.Find(carID);
        //    ViewBag.Employee = employee;

        //    ViewBag.InsuranceType = new SelectList(new List<string> { "ОСАГО", "КАСКО" });
        //    ViewBag.Period = new SelectList(new List<string> { "6 месяцев", "12 месяцев" });
        //    return View("ChooseInfo");
        //}

        // POST: CreatePolicy/ChooseInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseInfo([Bind(Include = "ID,InsuranceType,InsurancePremium,InsuranceAmount,DateOfConclusion,ExpirationDate,PolicyholderID,CarID,EmployeeID")] Policy policy, string period)
        {
            if (ModelState.IsValid)
            {
                switch (period)
                {
                    case "6 месяцев":
                        policy.ExpirationDate = policy.DateOfConclusion.AddMonths(6);
                        break;
                    case "12 месяцев":
                        policy.ExpirationDate = policy.DateOfConclusion.AddMonths(12);
                        break;
                }
                Policy newPolicy = db.Policies.Add(policy);
                db.SaveChanges();

                ViewBag.PolicyID = newPolicy.ID;
                return View("ChoosePersonsAllowedToDrive", db.PersonAllowedToDrives.ToList());
            }

            ViewBag.Policyholder = db.Policyholders.Find(policy.PolicyholderID);
            ViewBag.Car = db.Car.Find(policy.CarID);
            ViewBag.Employee = db.Employees.Find(policy.EmployeeID);

            ViewBag.InsuranceType = new SelectList(new List<string> { "ОСАГО", "КАСКО" });
            ViewBag.Period = new SelectList(new List<string> { "6 месяцев", "12 месяцев" });
            return View(policy);
        }

        // POST: CreatePolicy/ChoosePersonsAllowedToDrive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChoosePersonsAllowedToDrive(int policyID, PersonsAllowedToDriveCheck[] personsAllowedToDriveChecked)
        {
            Policy policy = db.Policies.Find(policyID);
            if (policy == null)
                return HttpNotFound();

            var listPersonsAllowedToDrive = new List<PersonAllowedToDrive>();
            foreach (var item in personsAllowedToDriveChecked)
            {
                if(item.Check == true)
                {
                    PersonAllowedToDrive person = db.PersonAllowedToDrives.Find(item.PersonAllowedToDriveID);
                    if (person == null)
                        return HttpNotFound();

                    person.Policies.Add(policy);
                    listPersonsAllowedToDrive.Add(person);
                }
            }

            if(listPersonsAllowedToDrive.Count == 0)
            {
                ViewBag.PolicyID = policyID;
                ModelState.AddModelError("Error", "Выберите хотя бы одно лицо, допущенное к управлению");
                return View(db.PersonAllowedToDrives.ToList());
            }

            db.SaveChanges();

            return RedirectToAction("Index", "Policies");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}