using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InsuranceAgency.Models;

namespace InsuranceAgency.Controllers
{
    [Authorize(Roles = "Administrator, Operator")]
    public class CarsController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: Cars
        public ActionResult Index()
        {
            return View(db.Car.ToList());
        }

        // GET: Cars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Car.Include(c => c.Photos)
                            .First(c => c.ID == id);
            if (car == null)
            {
                return HttpNotFound();
            }

            List<byte[]> photos = new List<byte[]>();
            foreach (Photo photo in car.Photos)
            {
                photos.Add(System.IO.File.ReadAllBytes(Server.MapPath("~/Files/" + id + "/" + photo.Path)));
            }
            ViewBag.Photos = photos;
            ViewBag.PhotosInfo = car.Photos;

            return View(car);
        }

        // GET: Cars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Car.Include(c => c.Photos)
                            .First(c => c.ID == id);
            if (car == null)
            {
                return HttpNotFound();
            }

            List<byte[]> photos = new List<byte[]>();
            foreach (Photo photo in car.Photos)
            {
                photos.Add(System.IO.File.ReadAllBytes(Server.MapPath("~/Files/" + id + "/" + photo.Path)));
            }
            ViewBag.Photos = photos;
            ViewBag.PhotosInfo = car.Photos;

            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Model,VIN,RegistrationPlate,VehiclePassport")] Car car)
        {
            if (ModelState.IsValid)
            {
                car.Model = car.Model.Trim();

                int countVIN = db.Car.Where(c => c.VIN == car.VIN && c.ID != car.ID).Count();

                if (countVIN == 0)
                {
                    db.Entry(car).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Cars", new { id = car.ID });
                }
                else
                {
                    if (countVIN > 0)
                        ModelState.AddModelError("VIN", "Данный VIN номер уже используется");
                }
            }
            return View(car);
        }

        // GET: Cars/AddPhoto/5
        public ActionResult AddPhoto(int? carID)
        {
            if (carID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Car.Find(carID);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/AddPhoto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPhoto(int carID, HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                // получаем имя файла
                string fileName = Path.GetFileName(upload.FileName);

                // сохраняем файл в папку Files в проекте
                upload.SaveAs(Server.MapPath("~/Files/" + carID + "/" + fileName));

                Photo photo = new Photo();
                photo.CarID = carID;
                photo.Path = fileName;
                photo.UploadDate = DateTime.Now;
                db.Photos.Add(photo);
                db.SaveChanges();

                return RedirectToAction("Edit", "Cars", new { id = carID });
            }

            ModelState.AddModelError("", "Выберите файл");
            Car car = db.Car.Find(carID);

            return View(car);
        }

        // GET: Cars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Car.Include(c => c.Policies)
                            .First(c => c.ID == id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = db.Car.Find(id);
            db.Car.Remove(car);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Cars/DeletePhoto/5
        [Authorize(Roles = "Administrator")]
        public ActionResult DeletePhoto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Include(p => p.Car)
                                   .First(p => p.ID == id);
            if (photo == null)
            {
                return HttpNotFound();
            }

            byte[] deletePhoto;
            deletePhoto = System.IO.File.ReadAllBytes(Server.MapPath("~/Files/" + photo.CarID + "/" + photo.Path));
            ViewBag.DeletePhoto = deletePhoto;

            return View(photo);
        }

        // POST: Cars/DeletePhoto/5
        [HttpPost, ActionName("DeletePhoto")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePhotoConfirmed(int id)
        {
            Photo photo = db.Photos.Find(id);
            db.Photos.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Edit", "Cars", new { id = photo.CarID });
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