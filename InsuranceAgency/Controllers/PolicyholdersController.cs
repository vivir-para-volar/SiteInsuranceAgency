using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InsuranceAgency.Models;
using InsuranceAgency.Models.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
            if (HttpContext.Request.UrlReferrer.LocalPath.ToLower().Contains(@"/createpolicy"))
                ViewBag.FromCreatePolicy = true;
            else ViewBag.FromCreatePolicy = false;
            return View();
        }

        // POST: Policyholders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FullName,Birthday,Telephone,Email,Passport")] Policyholder policyholder, bool fromCreatePolicy)
        {
            if (ModelState.IsValid)
            {
                policyholder.FullName = policyholder.FullName.Trim();

                int countTelephone = db.Policyholders.Where(p => p.Telephone == policyholder.Telephone).Count();
                int countPassport = db.Policyholders.Where(p => p.Passport == policyholder.Passport).Count();

                int countEmail = 0;
                bool flag = true;
                if (policyholder.Email != null)
                    countEmail = db.Policyholders.Where(p => p.Email == policyholder.Email).Count();
                else flag = false;

                if (countTelephone == 0 && countPassport == 0 && (!flag || (flag && countEmail == 0)))
                {
                    db.Policyholders.Add(policyholder);
                    db.SaveChanges();

                    if (fromCreatePolicy)
                        return RedirectToAction("Index", "CreatePolicy");
                    else
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

            var identityDB = new MyIdentityDbContext();
            var userManager = new UserManager<MyIdentityUser>(new UserStore<MyIdentityUser>(identityDB));
            if (policyholder.Email != null)
            {
                MyIdentityUser user = userManager.FindByEmail(policyholder.Email);
                if (user != null) ViewBag.UserID = user.Id;
                else ViewBag.UserID = "-1";
            }
            else ViewBag.UserID = "-1";

            return View(policyholder);
        }

        // POST: Policyholders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FullName,Birthday,Email,Telephone,Passport")] Policyholder policyholder, string userID)
        {
            if (ModelState.IsValid)
            {
                policyholder.FullName = policyholder.FullName.Trim();

                int countTelephone = db.Policyholders.Where(p => p.Telephone == policyholder.Telephone && p.ID != policyholder.ID).Count();
                int countPassport = db.Policyholders.Where(p => p.Passport == policyholder.Passport && p.ID != policyholder.ID).Count();

                int countEmail = 0;
                bool flag = true;
                if (policyholder.Email != null)
                    countEmail = db.Policyholders.Where(p => p.Email == policyholder.Email && p.ID != policyholder.ID).Count();
                else flag = false;

                if (countTelephone == 0 && countPassport == 0 && (!flag || (flag && countEmail == 0)))
                {
                    if (userID != "-1")
                    {
                        var identityDB = new MyIdentityDbContext();
                        var userManager = new UserManager<MyIdentityUser>(new UserStore<MyIdentityUser>(identityDB));
                        MyIdentityUser user = userManager.FindById(userID);
                        user.FullName = policyholder.FullName;
                        user.BirthDate = policyholder.Birthday;
                        user.PhoneNumber = policyholder.Telephone;
                        user.Email = policyholder.Email;

                        if (user.Email != null)
                        {
                            List<MyIdentityUser> users = userManager.Users.ToList();
                            int userWithSameEmail = users.Where(u => u.Email == user.Email && u.Id != user.Id).Count();
                            if (userWithSameEmail != 0)
                            {
                                ModelState.AddModelError("Email", "Данный Email уже используется");
                                return View(policyholder);
                            }
                        }

                        IdentityResult result = userManager.Update(user);

                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", "Ошибка при изменении пользователя");
                            return View(policyholder);
                        }
                    }
                    else
                    {
                        //если у сотрудника или админа есть такая почта
                        if (policyholder.Email != null)
                        {
                            var identityDB = new MyIdentityDbContext();
                            var userManager = new UserManager<MyIdentityUser>(new UserStore<MyIdentityUser>(identityDB));
                            var roleManager = new RoleManager<MyIdentityRole>(new RoleStore<MyIdentityRole>(identityDB));

                            List<MyIdentityUser> users = userManager.Users.ToList();
                            int userWithSameEmail = users.Where(u => u.Email == policyholder.Email && roleManager.FindById(u.Roles.ToArray()[0].RoleId).Name != "User").Count();
                            if (userWithSameEmail != 0)
                            {
                                ModelState.AddModelError("Email", "Данный Email уже используется");
                                return View(policyholder);
                            }
                        }
                    }

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
                    if (countEmail > 0)
                        ModelState.AddModelError("Email", "Данный Email уже используется");
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
