using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;

namespace InsuranceAgency.Controllers
{
    public class PersonsAllowedToDriveController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: PersonsAllowedToDrive
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Index()
        {
            return View(db.PersonAllowedToDrives.ToList());
        }

        // GET: PersonsAllowedToDrive/Details/5
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonAllowedToDrive personAllowedToDrive = db.PersonAllowedToDrives.Find(id);
            if (personAllowedToDrive == null)
            {
                return HttpNotFound();
            }
            return View(personAllowedToDrive);
        }

        // GET: PersonsAllowedToDrive/Create
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PersonsAllowedToDrive/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FullName,DrivingLicence")] PersonAllowedToDrive personAllowedToDrive)
        {
            if (ModelState.IsValid)
            {
                db.PersonAllowedToDrives.Add(personAllowedToDrive);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(personAllowedToDrive);
        }

        // GET: PersonsAllowedToDrive/Edit/5
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonAllowedToDrive personAllowedToDrive = db.PersonAllowedToDrives.Find(id);
            if (personAllowedToDrive == null)
            {
                return HttpNotFound();
            }
            return View(personAllowedToDrive);
        }

        // POST: PersonsAllowedToDrive/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FullName,DrivingLicence")] PersonAllowedToDrive personAllowedToDrive)
        {
            if (ModelState.IsValid)
            {
                db.Entry(personAllowedToDrive).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(personAllowedToDrive);
        }

        // GET: PersonsAllowedToDrive/Delete/5
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonAllowedToDrive personAllowedToDrive = db.PersonAllowedToDrives.Find(id);
            if (personAllowedToDrive == null)
            {
                return HttpNotFound();
            }
            return View(personAllowedToDrive);
        }

        // POST: PersonsAllowedToDrive/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PersonAllowedToDrive personAllowedToDrive = db.PersonAllowedToDrives.Find(id);
            db.PersonAllowedToDrives.Remove(personAllowedToDrive);
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
