using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;
using InsuranceAgency.Models.Security;
using InsuranceAgency.Models.ViewModels;
using WebCinema.Controllers;

namespace InsuranceAgency.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EmployeesController : Controller
    {
        private AgencyDBContext db = new AgencyDBContext();

        // GET: Employees
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: Employees/Details/5
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FullName,Birthday,Telephone,Email,Passport,Login,Password")] CreateEmployee createEmployee)
        {
            if (ModelState.IsValid)
            {
                createEmployee.FullName = createEmployee.FullName.Trim();

                int countTelephone = db.Employees.Where(e => e.Telephone == createEmployee.Telephone).Count();
                int countPassport = db.Employees.Where(e => e.Passport == createEmployee.Passport).Count();

                if (countTelephone == 0 && countPassport == 0)
                {
                    var employee = new Employee();

                    employee.FullName = createEmployee.FullName;
                    employee.Birthday = createEmployee.Birthday;
                    employee.Telephone = createEmployee.Telephone;
                    employee.Email = createEmployee.Email;
                    employee.Passport = createEmployee.Passport;

                    db.Employees.Add(employee);
                    db.SaveChanges();


                    var ac = new AccountController();
                    var register = new Register();

                    register.UserName = createEmployee.Login;
                    register.FullName = createEmployee.FullName;
                    register.BirthDate = createEmployee.Birthday;
                    register.PhoneNumber = createEmployee.Telephone;
                    register.Email = createEmployee.Email;
                    register.Password = createEmployee.Password;

                    ac.Register(register, "Operator");

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

            return View(createEmployee);
        }

        // GET: Employees/Edit/5
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
        public ActionResult Edit([Bind(Include = "ID,FullName,Birthday,Telephone,Email,Passport")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.FullName = employee.FullName.Trim();

                int countTelephone = db.Employees.Where(e => e.Telephone == employee.Telephone && e.ID != employee.ID).Count();
                int countPassport = db.Employees.Where(e => e.Passport == employee.Passport && e.ID != employee.ID).Count();

                if (countTelephone == 0 && countPassport == 0)
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
                }
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees
                                    .Include(p => p.Policies)
                                    .First(p => p.ID == id);
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
