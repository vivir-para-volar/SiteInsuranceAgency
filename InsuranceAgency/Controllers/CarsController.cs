﻿using System.Data.Entity;
using System.Linq;
using System.Net;
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
            Car car = db.Car.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // GET: Cars/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Model,VIN,RegistrationPlate,VehiclePassport")] Car car)
        {
            if (ModelState.IsValid)
            {
                car.Model = car.Model.Trim();

                int countVIN = db.Car.Where(c => c.VIN == car.VIN).Count();

                if (countVIN == 0)
                {
                    db.Car.Add(car);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    if (countVIN > 0)
                        ModelState.AddModelError("VIN", "Данный VIN номер уже используется");
                }
            }

            return View(car);
        }

        // GET: Cars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Car.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
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
                    return RedirectToAction("Index");
                }
                else
                {
                    if (countVIN > 0)
                        ModelState.AddModelError("VIN", "Данный VIN номер уже используется");
                }
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Car
                        .Include(p => p.Policies)
                        .First(p => p.ID == id);
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
