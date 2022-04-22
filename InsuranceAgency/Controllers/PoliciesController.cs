using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;

namespace InsuranceAgency.Controllers
{
    public class PoliciesController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: Policies
        public ActionResult Index()
        {
            var policies = db.Policies.Include(p => p.Car).Include(p => p.Employee).Include(p => p.Policyholder);
            return View(policies.ToList());
        }

        // GET: Policies/Details/5
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
            return View(policy);
        }

        // GET: Policies/Create
        public ActionResult Create()
        {
            ViewBag.CarID = new SelectList(db.Car, "ID", "Model");
            ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "FullName");
            ViewBag.PolicyholderID = new SelectList(db.Policyholders, "ID", "FullName");
            return View();
        }

        // POST: Policies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,InsuranceType,InsurancePremium,InsuranceAmount,DateOfConclusion,ExpirationDate,PolicyholderID,CarID,EmployeeID")] Policy policy)
        {
            if (ModelState.IsValid)
            {
                db.Policies.Add(policy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CarID = new SelectList(db.Car, "ID", "Model");
            ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "FullName");
            ViewBag.PolicyholderID = new SelectList(db.Policyholders, "ID", "FullName");
            return View(policy);
        }

        // GET: Policies/Edit/5
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
