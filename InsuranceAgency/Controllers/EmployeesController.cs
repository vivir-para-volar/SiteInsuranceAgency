using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;
using InsuranceAgency.Models.Security;
using InsuranceAgency.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
                int countEmail = db.Employees.Where(e => e.Email == createEmployee.Email).Count();
                int countPassport = db.Employees.Where(e => e.Passport == createEmployee.Passport).Count();

                if (countTelephone == 0 && countPassport == 0 && countEmail == 0)
                {
                    var employee = new Employee();

                    employee.FullName = createEmployee.FullName;
                    employee.Birthday = createEmployee.Birthday;
                    employee.Telephone = createEmployee.Telephone;
                    employee.Email = createEmployee.Email;
                    employee.Passport = createEmployee.Passport;

                    var identityDB = new MyIdentityDbContext();
                    var userManager = new UserManager<MyIdentityUser>(new UserStore<MyIdentityUser>(identityDB));
                    var register = new Register();

                    var userWithSameName = userManager.FindByName(createEmployee.Login);
                    if (userWithSameName != null)
                    {
                        ModelState.AddModelError("Login", "Данный логин уже используется");
                        return View(createEmployee);
                    }
                    var userWithSameEmail = userManager.FindByEmail(createEmployee.Email);
                    if (userWithSameEmail != null)
                    {
                        ModelState.AddModelError("Email", "Данный Email уже используется");
                        return View(createEmployee);
                    }

                    MyIdentityUser user = new MyIdentityUser();

                    user.UserName = createEmployee.Login;
                    user.FullName = createEmployee.FullName;
                    user.BirthDate = createEmployee.Birthday;
                    user.PhoneNumber = createEmployee.Telephone;
                    user.Email = createEmployee.Email;

                    IdentityResult result = userManager.Create(user, createEmployee.Password);

                    if (result.Succeeded)
                    {
                        userManager.AddToRole(user.Id, "Operator");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ошибка при создании пользователя");
                        return View(createEmployee);
                    }

                    db.Employees.Add(employee);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    if (countTelephone > 0)
                        ModelState.AddModelError("Telephone", "Данный телефон уже используется");
                    if (countPassport > 0)
                        ModelState.AddModelError("Passport", "Данный пасспорт уже используется");
                    if (countEmail > 0)
                        ModelState.AddModelError("Email", "Данный Email уже используется");
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

            var identityDB = new MyIdentityDbContext();
            var userManager = new UserManager<MyIdentityUser>(new UserStore<MyIdentityUser>(identityDB));
            MyIdentityUser user = userManager.FindByEmail(employee.Email);
            ViewBag.UserID = user.Id;

            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FullName,Birthday,Telephone,Email,Passport")] Employee employee, string userID)
        {
            if (ModelState.IsValid)
            {
                employee.FullName = employee.FullName.Trim();

                int countTelephone = db.Employees.Where(e => e.Telephone == employee.Telephone && e.ID != employee.ID).Count();
                int countEmail = db.Employees.Where(e => e.Email == employee.Email && e.ID != employee.ID).Count();
                int countPassport = db.Employees.Where(e => e.Passport == employee.Passport && e.ID != employee.ID).Count();

                if (countTelephone == 0 && countPassport == 0 && countEmail == 0)
                {
                    var identityDB = new MyIdentityDbContext();
                    var userManager = new UserManager<MyIdentityUser>(new UserStore<MyIdentityUser>(identityDB));
                    MyIdentityUser user = userManager.FindById(userID);
                    user.FullName = employee.FullName;
                    user.BirthDate = employee.Birthday;
                    user.PhoneNumber = employee.Telephone;
                    user.Email = employee.Email;

                    IdentityResult result = userManager.Update(user);

                    if (result.Succeeded)
                    {
                        userManager.AddToRole(user.Id, "Operator");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ошибка при изменении пользователя");
                        return View(employee);
                    }

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
                    if (countEmail > 0)
                        ModelState.AddModelError("Email", "Данный Email уже используется");
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

            var identityDB = new MyIdentityDbContext();
            var userManager = new UserManager<MyIdentityUser>(new UserStore<MyIdentityUser>(identityDB));
            MyIdentityUser user = userManager.FindByEmail(employee.Email);
            IdentityResult result = userManager.Delete(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Ошибка при удаление пользователя");
                return View(employee);
            }

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
