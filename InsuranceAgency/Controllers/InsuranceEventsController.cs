using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;
using InsuranceAgency.Models.ViewModels;

namespace InsuranceAgency.Controllers
{
    public class InsuranceEventsController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: InsuranceEvents
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Index()
        {
            return View(db.InsuranceEvents.Include(p => p.Policy).Include(p => p.Policy.Car).Include(p => p.Policy.Policyholder).ToList());
        }

        // GET: InsuranceEvents/Details/5
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsuranceEvent insuranceEvent = db.InsuranceEvents.Find(id);
            if (insuranceEvent == null)
            {
                return HttpNotFound();
            }
            Policy policy = db.Policies.Include(p => p.Car).Include(p => p.Employee).Include(p => p.Policyholder).First(p => p.ID == insuranceEvent.PolicyID);

            var allInfoAboutInsuranceEvent = new AllInfoAboutInsuranceEvent(policy, insuranceEvent);

            return View(allInfoAboutInsuranceEvent);
        }

        // GET: InsuranceEvents/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsuranceEvent insuranceEvent = db.InsuranceEvents.Find(id);
            if (insuranceEvent == null)
            {
                return HttpNotFound();
            }
            Policy policy = db.Policies.Include(p => p.Car).Include(p => p.Employee).Include(p => p.Policyholder).First(p => p.ID == insuranceEvent.PolicyID);

            var allInfoAboutInsuranceEvent = new AllInfoAboutInsuranceEvent(policy, insuranceEvent);

            return View(allInfoAboutInsuranceEvent);
        }

        // POST: InsuranceEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InsuranceEvent insuranceEvent = db.InsuranceEvents.Find(id);
            db.InsuranceEvents.Remove(insuranceEvent);
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
