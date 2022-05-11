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
                var userTelephone = userManager.GetPhoneNumber(User.Identity.GetUserId());

                try
                {
                    int id = db.Policyholders.First(p => p.Telephone == userTelephone).ID;
                    policies = db.Policies.Include(p => p.Car).Include(p => p.Employee).Include(p => p.Policyholder).Where(p => p.PolicyholderID == id).ToList();
                }
                catch { }
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
            Policy policy = db.Policies
                                .Include(p => p.Car)
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
            Policy policy = db.Policies
                                .Include(p => p.Car)
                                .Include(p => p.Employee)
                                .Include(p => p.Policyholder)
                                .First(p => p.ID == id);
            if (policy == null)
            {
                return HttpNotFound();
            }
            return View(policy);
        }

        // POST: Policies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Edit([Bind(Include = "ID,InsuranceType,InsurancePremium,InsuranceAmount,DateOfConclusion,ExpirationDate,PolicyholderID,CarID,EmployeeID")] Policy policy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(policy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Policy policyError = db.Policies
                                .Include(p => p.Car)
                                .Include(p => p.Employee)
                                .Include(p => p.Policyholder)
                                .First(p => p.ID == policy.ID);
            return View(policyError);
        }

        // GET: Policies/Delete/5
        [Authorize(Roles = "Administrator, Operator")]
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
        [Authorize(Roles = "Administrator, Operator")]
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
