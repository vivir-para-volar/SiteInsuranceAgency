using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;

namespace InsuranceAgency.Controllers
{
    [Authorize(Roles = "Administrator, Operator")]
    public class PolicyholdersController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: Policyholders
        public ActionResult Index()
        {
            return View(db.Policyholders.ToList());
        }

        // GET: Policyholders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policyholder policyholder = db.Policyholders.Find(id);
            if (policyholder == null)
            {
                return HttpNotFound();
            }
            return View(policyholder);
        }

        // GET: Policyholders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Policyholders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FullName,Birthday,Telephone,Passport")] Policyholder policyholder)
        {
            if (ModelState.IsValid)
            {
                policyholder.FullName = policyholder.FullName.Trim();

                int countTelephone = db.Policyholders.Where(p => p.Telephone == policyholder.Telephone).Count();
                int countPassport = db.Policyholders.Where(p => p.Passport == policyholder.Passport).Count();

                if (countTelephone == 0 && countPassport == 0)
                {
                    db.Policyholders.Add(policyholder);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    if (countTelephone > 0)
                        ModelState.AddModelError("Telephone", "Данный телефон уже используется");
                    if (countPassport > 0)
                        ModelState.AddModelError("Passport", "Данный пасспорт уже используется");
                }
            }

            return View(policyholder);
        }

        // GET: Policyholders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policyholder policyholder = db.Policyholders.Find(id);
            if (policyholder == null)
            {
                return HttpNotFound();
            }
            return View(policyholder);
        }

        // POST: Policyholders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FullName,Birthday,Telephone,Passport")] Policyholder policyholder)
        {
            if (ModelState.IsValid)
            {
                policyholder.FullName = policyholder.FullName.Trim();

                int countTelephone = db.Policyholders.Where(p => p.Telephone == policyholder.Telephone && p.ID != policyholder.ID).Count();
                int countPassport = db.Policyholders.Where(p => p.Passport == policyholder.Passport && p.ID != policyholder.ID).Count();

                if (countTelephone == 0 && countPassport == 0)
                {
                    db.Entry(policyholder).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    if (countTelephone > 0)
                        ModelState.AddModelError("Telephone", "Данный телефон уже используется");
                    if (countPassport > 0)
                        ModelState.AddModelError("Passport", "Данный пасспорт уже используется");
                }
            }
            return View(policyholder);
        }

        // GET: Policyholders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Policyholder policyholder = db.Policyholders
                                            .Include(p => p.Policies)
                                            .First(p => p.ID == id);
            if (policyholder == null)
            {
                return HttpNotFound();
            }
            return View(policyholder);
        }

        // POST: Policyholders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Policyholder policyholder = db.Policyholders.Find(id);
            db.Policyholders.Remove(policyholder);
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
