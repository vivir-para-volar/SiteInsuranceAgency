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
    public class CreateCarController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: CreateCar/Create
        public ActionResult Create(int policyholderID = 0)
        {
            if (HttpContext.Request.UrlReferrer.LocalPath.ToLower().Contains(@"/createpolicy"))
                ViewBag.FromCreatePolicy = true;
            else ViewBag.FromCreatePolicy = false;

            ViewBag.PolicyholderID = policyholderID;
            return View();
        }

        // POST: CreateCar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Model,VIN,RegistrationPlate,VehiclePassport")] Car car, bool fromCreatePolicy, int policyholderID)
        {
            if (ModelState.IsValid)
            {
                car.Model = car.Model.Trim();

                int countVIN = db.Car.Where(c => c.VIN == car.VIN).Count();

                if (countVIN == 0)
                {
                    db.Car.Add(car);
                    db.SaveChanges();

                    string directory = Server.MapPath("~/Files/") + car.ID;
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    return RedirectToAction("AddPhoto", "CreateCar", new { carID = car.ID, fromCreatePolicy = fromCreatePolicy, policyholderID = policyholderID });
                }
                else
                {
                    if (countVIN > 0)
                        ModelState.AddModelError("VIN", "Данный VIN номер уже используется");
                }
            }

            return View(car);
        }

        // GET: CreateCar/AddPhoto/5
        public ActionResult AddPhoto(int? carID, bool fromCreatePolicy, int policyholderID)
        {
            if (carID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Car.Include(c => c.Photos)
                            .First(c => c.ID == carID);
            if (car == null)
            {
                return HttpNotFound();
            }

            ViewBag.FromCreatePolicy = fromCreatePolicy;
            ViewBag.PolicyholderID = policyholderID;

            List<byte[]> photos = new List<byte[]>();
            foreach (Photo photo in car.Photos)
            {
                photos.Add(System.IO.File.ReadAllBytes(Server.MapPath("~/Files/" + carID + "/" + photo.Path)));
            }
            ViewBag.Photos = photos;
            ViewBag.PhotosInfo = car.Photos;

            return View(car);
        }

        // POST: CreateCar/AddPhoto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPhoto(int carID, HttpPostedFileBase upload, bool fromCreatePolicy, int policyholderID)
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

                return RedirectToAction("AddPhoto", "CreateCar", new { carID = carID, fromCreatePolicy = fromCreatePolicy, policyholderID = policyholderID });
            }

            ModelState.AddModelError("", "Выберите файл");

            Car car = db.Car.Include(c => c.Photos)
                            .First(c => c.ID == carID);

            ViewBag.FromCreatePolicy = fromCreatePolicy;
            ViewBag.PolicyholderID = policyholderID;

            List<byte[]> photos = new List<byte[]>();
            foreach (Photo photo in car.Photos)
            {
                photos.Add(System.IO.File.ReadAllBytes(Server.MapPath("~/Files/" + carID + "/" + photo.Path)));
            }
            ViewBag.Photos = photos;
            ViewBag.PhotosInfo = car.Photos;

            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EndCreate(int carID, bool fromCreatePolicy, int policyholderID)
        {
            Car car = db.Car.Include(c => c.Photos)
                            .First(c => c.ID == carID);
            int count = car.Photos.Count();

            if (count != 0)
            {
                if (fromCreatePolicy)
                    return RedirectToAction("ChooseCar", "CreatePolicy", new { policyholderID = policyholderID });
                else
                    return RedirectToAction("Details", "Cars", new { id = carID });
            }

            ModelState.AddModelError("", "Добавьте хотя бы одну фотографию");

            ViewBag.FromCreatePolicy = fromCreatePolicy;
            ViewBag.PolicyholderID = policyholderID;

            List<byte[]> photos = new List<byte[]>();
            foreach (Photo photo in car.Photos)
            {
                photos.Add(System.IO.File.ReadAllBytes(Server.MapPath("~/Files/" + carID + "/" + photo.Path)));
            }
            ViewBag.Photos = photos;
            ViewBag.PhotosInfo = car.Photos;

            return View("AddPhoto", car);
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