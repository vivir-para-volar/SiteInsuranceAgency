using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;
using InsuranceAgency.Models.Security;
using InsuranceAgency.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InsuranceAgency.Controllers
{
    public class PoliciesController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: Policies
        [Authorize]
        public ActionResult Index()
        {
            List<Policy> policies = new List<Policy>();
            if (User.IsInRole("User"))
            {
                var identitydb = new MyIdentityDbContext();
                var userManager = new UserManager<MyIdentityUser>(new UserStore<MyIdentityUser>(identitydb));
                var userEmail = userManager.GetEmail(User.Identity.GetUserId());

                if (userEmail != null)
                {
                    Policyholder policyholder = db.Policyholders.FirstOrDefault(e => e.Email == userEmail);
                    if (policyholder != null)
                    {
                        int id = policyholder.ID;
                        policies = db.Policies.Include(p => p.Car).Include(p => p.Employee).Include(p => p.Policyholder).Where(p => p.PolicyholderID == id).ToList();
                    }
                }
            }
            else
            {
                policies = db.Policies.Include(p => p.Car).Include(p => p.Employee).Include(p => p.Policyholder).ToList();
            }
            return View(policies);
        }

        // GET: Policies/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policy policy = db.Policies.Include(p => p.Car)
                                       .Include(p => p.Employee)
                                       .Include(p => p.Policyholder)
                                       .First(p => p.ID == id);
            if (policy == null)
            {
                return HttpNotFound();
            }
            List<PersonAllowedToDrive> listPersonsAllowedToDrive = db.PersonAllowedToDrives.Include(p => p.Policies).ToArray().Where(p => p.Policies.Contains(policy)).ToList();
            List<InsuranceEvent> listInsuranceEvents = db.InsuranceEvents.Where(i => i.PolicyID == id).ToList();

            var allInfoAboutPolicy = new AllInfoAboutPolicy(policy, listPersonsAllowedToDrive, listInsuranceEvents);

            return View(allInfoAboutPolicy);
        }

        // GET: Policies/Edit/5
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policy policy = db.Policies.Include(p => p.Car)
                                       .Include(p => p.Employee)
                                       .Include(p => p.Policyholder)
                                       .First(p => p.ID == id);
            if (policy == null)
            {
                return HttpNotFound();
            }

            ViewBag.LastExpirationDate = policy.ExpirationDate;
            return View(policy);
        }

        // POST: Policies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Edit([Bind(Include = "ID,InsuranceType,InsurancePremium,InsuranceAmount,DateOfConclusion,ExpirationDate,PolicyholderID,CarID,EmployeeID")] Policy policy, DateTime lastExpirationDate)
        {
            if (ModelState.IsValid)
            {
                bool flag = true;

                if (policy.InsurancePremium <= 0)
                {
                    ModelState.AddModelError("InsurancePremium", "Страховая премия не может быть меньше или равна 0");
                    flag = false;
                }
                if (policy.InsuranceAmount <= 0)
                {
                    ModelState.AddModelError("InsuranceAmount", "Страховая сумма не может быть меньше или равна 0");
                    flag = false;
                }
                if (policy.InsurancePremium >= policy.InsuranceAmount)
                {
                    ModelState.AddModelError("InsurancePremium", "Страховая премия не может быть больше или равна Страховой сумме");
                    flag = false;
                }
                if(policy.ExpirationDate < policy.DateOfConclusion)
                {
                    ModelState.AddModelError("ExpirationDate", "Дата окончания действия не может быть меньше Даты заключения");
                    flag = false;
                }
                if (policy.ExpirationDate > lastExpirationDate)
                {
                    ModelState.AddModelError("ExpirationDate", "Дата окончания действия нельзя увеличить");
                    flag = false;
                }

                if (flag)
                {
                    db.Entry(policy).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Policies", new { id = policy.ID });
                }
            }

            ViewBag.LastExpirationDate = lastExpirationDate;
            Policy policyError = db.Policies.Include(p => p.Car)
                                            .Include(p => p.Employee)
                                            .Include(p => p.Policyholder)
                                            .First(p => p.ID == policy.ID);
            return View(policyError);
        }

        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult EditPersonsAllowedToDrive(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policy policy = db.Policies.Include(p => p.PersonsAllowedToDrive)
                                       .First(p => p.ID == id);
            if (policy == null)
            {
                return HttpNotFound();
            }
            ViewBag.PolicyID = id;
            ViewBag.PersonsAllowedToDrive = policy.PersonsAllowedToDrive;

            return View(db.PersonAllowedToDrives.ToList());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Operator")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonsAllowedToDrive(int policyID, PersonsAllowedToDriveCheck[] personsAllowedToDriveChecked)
        {
            Policy policy = db.Policies.Include(p => p.PersonsAllowedToDrive)
                                       .First(p => p.ID == policyID);
            if (policy == null)
                return HttpNotFound();

            var listPersonsAllowedToDrive = new List<PersonAllowedToDrive>();
            foreach (var item in personsAllowedToDriveChecked)
            {
                if (item.Check == true)
                {
                    PersonAllowedToDrive person = db.PersonAllowedToDrives.Find(item.PersonAllowedToDriveID);
                    if (person == null)
                        return HttpNotFound();

                    person.Policies.Remove(policy);
                    listPersonsAllowedToDrive.Add(person);
                }
            }

            if (listPersonsAllowedToDrive.Count == 0)
            {
                ViewBag.PolicyID = policyID;
                ViewBag.PersonsAllowedToDrive = policy.PersonsAllowedToDrive;
                ModelState.AddModelError("Error", "Выберите хотя бы одно лицо, допущенное к управлению");
                return View(db.PersonAllowedToDrives.ToList());
            }
            else
            {
                policy.PersonsAllowedToDrive.Clear();
                foreach (var person in listPersonsAllowedToDrive)
                {
                    person.Policies.Add(policy);
                    policy.PersonsAllowedToDrive.Add(person);
                }
            }

            db.SaveChanges();

            return RedirectToAction("Details", "Policies", new { id = policy.ID });
        }

        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult CreateInsuranceEvent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Policy policy = db.Policies.Find(id);

            if (policy == null)
                return HttpNotFound();

            ViewBag.PolicyID = policy.ID;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Operator")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInsuranceEvent([Bind(Include = "ID,Date,InsurancePayment,Description,PolicyID")] InsuranceEvent insuranceEvent)
        {
            if (ModelState.IsValid)
            {
                bool flag = true;

                Policy policy = db.Policies.Find(insuranceEvent.PolicyID);

                if (insuranceEvent.InsurancePayment <= 0)
                {
                    ModelState.AddModelError("InsurancePayment", "Страховая выплата не может быть меньше или равна 0");
                    flag = false;
                }
                if (insuranceEvent.InsurancePayment > policy.InsuranceAmount)
                {
                    ModelState.AddModelError("InsurancePayment", "Страховая выплата не может быть больше Страховой суммы");
                    flag = false;
                }
                if (insuranceEvent.Date < policy.DateOfConclusion)
                {
                    ModelState.AddModelError("Date", "Дата не может быть меньше Даты заключения полиса");
                    flag = false;
                }
                if (insuranceEvent.Date > policy.ExpirationDate)
                {
                    ModelState.AddModelError("Date", "Дата не может быть больше Даты окончания действия полиса");
                    flag = false;
                }

                if (flag)
                {
                    db.InsuranceEvents.Add(insuranceEvent);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Policies", new { id = insuranceEvent.PolicyID });
                }
            }

            ViewBag.PolicyID = insuranceEvent.PolicyID;
            return View(insuranceEvent);
        }

        // GET: Policies/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policy policy = db.Policies.Include(p => p.Car)
                                       .Include(p => p.Employee)
                                       .Include(p => p.Policyholder)
                                       .First(p => p.ID == id);
            if (policy == null)
            {
                return HttpNotFound();
            }
            return View(policy);
        }

        // POST: Policies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            Policy policy = db.Policies.Find(id);
            db.Policies.Remove(policy);
            db.SaveChanges();
            return RedirectToAction("Index");
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
