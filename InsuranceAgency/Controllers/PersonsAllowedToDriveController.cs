using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;

namespace InsuranceAgency.Controllers
{
    [Authorize(Roles = "Administrator, Operator")]
    public class PersonsAllowedToDriveController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: PersonsAllowedToDrive
        public ActionResult Index()
        {
            return View(db.PersonAllowedToDrives.ToList());
        }

        // GET: PersonsAllowedToDrive/Details/5
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
        public ActionResult Create(int policyID = 0)
        {
            if (HttpContext.Request.UrlReferrer.LocalPath.ToLower().Contains(@"/createpolicy"))
                ViewBag.FromCreatePolicy = true;
            else ViewBag.FromCreatePolicy = false;

            ViewBag.PolicyID = policyID;
            return View();
        }

        // POST: PersonsAllowedToDrive/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FullName,DrivingLicence")] PersonAllowedToDrive personAllowedToDrive, bool fromCreatePolicy, int policyID)
        {
            if (ModelState.IsValid)
            {
                personAllowedToDrive.FullName = personAllowedToDrive.FullName.Trim();

                int countDrivingLicence = db.PersonAllowedToDrives.Where(p => p.DrivingLicence == personAllowedToDrive.DrivingLicence).Count();

                if (countDrivingLicence == 0)
                {
                    db.PersonAllowedToDrives.Add(personAllowedToDrive);
                    db.SaveChanges();

                    if (fromCreatePolicy)
                        return RedirectToAction("ChoosePersonsAllowedToDrive", "CreatePolicy", new { policyID = policyID });
                    else
                        return RedirectToAction("Details", "PersonsAllowedToDrive", new { id = personAllowedToDrive.ID });
                }
                else
                {
                    if (countDrivingLicence > 0)
                        ModelState.AddModelError("DrivingLicence", "Данное водительское удостоверение уже используется");
                }
            }

            return View(personAllowedToDrive);
        }

        // GET: PersonsAllowedToDrive/Edit/5
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
                personAllowedToDrive.FullName = personAllowedToDrive.FullName.Trim();

                int countDrivingLicence = db.PersonAllowedToDrives.Where(p => p.DrivingLicence == personAllowedToDrive.DrivingLicence && p.ID != personAllowedToDrive.ID).Count();

                if (countDrivingLicence == 0)
                {
                    db.Entry(personAllowedToDrive).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "PersonsAllowedToDrive", new { id = personAllowedToDrive.ID });
                }
                else
                {
                    if (countDrivingLicence > 0)
                        ModelState.AddModelError("Telephone", "Данный телефон уже используется");
                }
            }
            return View(personAllowedToDrive);
        }

        // GET: PersonsAllowedToDrive/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonAllowedToDrive personAllowedToDrive = db.PersonAllowedToDrives
                                                            .Include(p => p.Policies)
                                                            .First(p => p.ID == id);
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
