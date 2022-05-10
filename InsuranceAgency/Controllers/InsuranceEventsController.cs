using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;

namespace InsuranceAgency.Controllers
{
    [Authorize(Roles = "Administrator, Operator")]
    public class InsuranceEventsController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: InsuranceEvents
        public ActionResult Index()
        {
            return View(db.InsuranceEvents.ToList());
        }

        // GET: InsuranceEvents/Details/5
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
            return View(insuranceEvent);
        }

        // GET: InsuranceEvents/Create
        public ActionResult Create()
        {
            ViewBag.PolicyID = new SelectList(db.Policies, "ID", "ID");
            return View();
        }

        // POST: InsuranceEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Date,InsurancePayment,Description,PolicyID")] InsuranceEvent insuranceEvent)
        {
            if (ModelState.IsValid)
            {
                db.InsuranceEvents.Add(insuranceEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PolicyID = new SelectList(db.Policies, "ID", "ID", insuranceEvent.PolicyID);
            return View(insuranceEvent);
        }

        // GET: InsuranceEvents/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.PolicyID = new SelectList(db.Policies, "ID", "ID", insuranceEvent.PolicyID);
            return View(insuranceEvent);
        }

        // POST: InsuranceEvents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Date,InsurancePayment,Description,PolicyID")] InsuranceEvent insuranceEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuranceEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PolicyID = new SelectList(db.Policies, "ID", "ID", insuranceEvent.PolicyID);
            return View(insuranceEvent);
        }

        // GET: InsuranceEvents/Delete/5
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
            return View(insuranceEvent);
        }

        // POST: InsuranceEvents/Delete/5
        [HttpPost, ActionName("Delete")]
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
