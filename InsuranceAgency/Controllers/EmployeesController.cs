﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;
using InsuranceAgency.Models.Security;
using WebCinema.Controllers;

namespace InsuranceAgency.Controllers
{
    public class EmployeesController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: Employees
        [Authorize (Roles = "Administrator")]
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: Employees/Details/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FullName,Birthday,Telephone,Passport,Login,Password,Admin,Works")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.FullName = employee.FullName.Trim();

                int countTelephone = db.Employees.Where(e => e.Telephone == employee.Telephone && e.ID != employee.ID).Count();
                int countPassport = db.Employees.Where(e => e.Passport == employee.Passport && e.ID != employee.ID).Count();
                int countLogin = db.Employees.Where(e => e.Login == employee.Login && e.ID != employee.ID).Count();

                if (countTelephone == 0 && countPassport == 0 && countLogin == 0)
                {
                    db.Employees.Add(employee);
                    db.SaveChanges();

                    var ac = new AccountController();
                    Register register = new Register();

                    register.UserName = employee.Login;
                    register.FullName = employee.FullName;
                    register.BirthDate = employee.Birthday;
                    register.PhoneNumber = employee.Telephone;
                    register.Password = employee.Password;

                    ac.RegisterOperator(register);

                    return RedirectToAction("Index");
                }
                else
                {
                    if (countTelephone > 0)
                        ModelState.AddModelError("Telephone", "Данный телефон уже используется");
                    if (countPassport > 0)
                        ModelState.AddModelError("Passport", "Данный пасспорт уже используется");
                    if (countLogin > 0)
                        ModelState.AddModelError("Login", "Данный логин уже используется");
                }
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FullName,Birthday,Telephone,Passport,Login,Password,Admin,Works")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.FullName = employee.FullName.Trim();

                int countTelephone = db.Employees.Where(e => e.Telephone == employee.Telephone && e.ID != employee.ID).Count();
                int countPassport = db.Employees.Where(e => e.Passport == employee.Passport && e.ID != employee.ID).Count();
                int countLogin = db.Employees.Where(e => e.Login == employee.Login && e.ID != employee.ID).Count();

                if (countTelephone == 0 && countPassport == 0 && countLogin == 0)
                {
                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    if (countTelephone > 0)
                        ModelState.AddModelError("Telephone", "Данный телефон уже используется");
                    if (countPassport > 0)
                        ModelState.AddModelError("Passport", "Данный пасспорт уже используется");
                    if (countLogin > 0)
                        ModelState.AddModelError("Login", "Данный логин уже используется");
                }
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
