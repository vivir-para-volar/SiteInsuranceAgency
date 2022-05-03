using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;

namespace InsuranceAgency.Controllers
{
    public class PhotosController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: Photos
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Index()
        {
            var photos = db.Photos.Include(p => p.Car);
            return View(photos.ToList());
        }

        // GET: Photos/Details/5
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Include(c => c.Car).First(c => c.ID == id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // GET: Photos/Create
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Create()
        {
            ViewBag.CarID = new SelectList(db.Car, "ID", "Model");
            return View();
        }

        // POST: Photos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Path,CarID")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Photos.Add(photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CarID = new SelectList(db.Car, "ID", "Model", photo.CarID);
            return View(photo);
        }

        // GET: Photos/Edit/5
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            ViewBag.CarID = new SelectList(db.Car, "ID", "Model", photo.CarID);
            return View(photo);
        }

        // POST: Photos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Path,CarID")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CarID = new SelectList(db.Car, "ID", "Model", photo.CarID);
            return View(photo);
        }

        // GET: Photos/Delete/5
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Include(c => c.Car).First(c => c.ID == id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = db.Photos.Find(id);
            db.Photos.Remove(photo);
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
