using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using InsuranceAgency.Models;
using InsuranceAgency.Models.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace WebCinema.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<MyIdentityUser> userManager;
        private RoleManager<MyIdentityRole> roleManager;

        public AccountController()
        {
            var db = new MyIdentityDbContext();

            var userStore = new UserStore<MyIdentityUser>(db);
            userManager = new UserManager<MyIdentityUser>(userStore);

            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6, //минимаольная длина пароля
                RequireNonLetterOrDigit = false, //требовать сиволы не явл. цифрами и буквами
                RequireDigit = false, //требовать цифры
                RequireLowercase = false, //требовать символы нижнего регистра
                RequireUppercase = false //требовать символы верхнего регистра
            };


            var roleStore = new RoleStore<MyIdentityRole>(db);
            roleManager = new RoleManager<MyIdentityRole>(roleStore);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register model)
        {
            if (ModelState.IsValid)
            {
                var userWithSameName = userManager.FindByName(model.UserName);
                if (userWithSameName != null)
                {
                    ModelState.AddModelError("UserName", "Данный логин уже используется");
                    return View(model);
                }
                var userWithSameEmail = userManager.FindByEmail(model.Email);
                if (userWithSameEmail != null)
                {
                    ModelState.AddModelError("Email", "Данный Email уже используется");
                    return View(model);
                }

                MyIdentityUser user = new MyIdentityUser();

                user.UserName = model.UserName;
                user.FullName = model.FullName;
                user.BirthDate = model.BirthDate;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;

                IdentityResult result = userManager.Create(user, model.Password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "User");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при создании пользователя");
                }
            }
            return View(model);
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                MyIdentityUser user = userManager.Find(model.UserName, model.Password);
                if (user != null)
                {
                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    ClaimsIdentity identity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationProperties props = new AuthenticationProperties();
                    props.IsPersistent = model.RememberMe;
                    authenticationManager.SignIn(props, identity);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин или пароль");
                }
            }
            return View(model);
        }

        [Authorize]
        public ActionResult UserProfile()
        {
            MyIdentityUser user = userManager.FindById(User.Identity.GetUserId());
            Profile model = new Profile();
            model.UserName = user.UserName;
            model.FullName = user.FullName;
            model.BirthDate = user.BirthDate;
            model.PhoneNumber = user.PhoneNumber;
            model.Email = user.Email;

            return View(model);
        }

        [Authorize]
        public ActionResult ChangeProfile()
        {
            MyIdentityUser user = userManager.FindById(User.Identity.GetUserId());
            Profile model = new Profile();
            model.UserName = user.UserName;
            model.FullName = user.FullName;
            model.BirthDate = user.BirthDate;
            model.PhoneNumber = user.PhoneNumber;
            model.Email = user.Email;

            if (User.IsInRole("Operator") || User.IsInRole("Administrator"))
            {
                AgencyDBContext db = new AgencyDBContext();
                var userEmail = userManager.GetEmail(User.Identity.GetUserId());
                ViewBag.UserID = db.Employees.First(e => e.Email == userEmail).ID;
            }
            else if (User.IsInRole("User"))
            {
                AgencyDBContext db = new AgencyDBContext();
                var userEmail = userManager.GetEmail(User.Identity.GetUserId());
                if (userEmail != null)
                {
                    Policyholder policyholder = db.Policyholders.FirstOrDefault(e => e.Email == userEmail);
                    if (policyholder != null) ViewBag.UserID = policyholder.ID;
                    else ViewBag.UserID = -1;
                }
                else ViewBag.UserID = -1;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeProfile(Profile model, int userID)
        {
            if (ModelState.IsValid)
            {
                MyIdentityUser user = userManager.FindById(User.Identity.GetUserId());
                user.UserName = model.UserName.Trim();
                user.FullName = model.FullName.Trim();
                user.BirthDate = model.BirthDate;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;

                List<MyIdentityUser> users = userManager.Users.ToList();
                int userWithSameName = users.Where(u => u.UserName == user.UserName && u.Id != user.Id).Count();
                int userWithSameEmail = users.Where(u => u.Email == user.Email && u.Id != user.Id).Count();

                if (userWithSameName != 0)
                {
                    ModelState.AddModelError("UserName", "Данный логин уже используется");
                    return View(model);
                }
                if (userWithSameEmail != 0)
                {
                    ModelState.AddModelError("Email", "Данный Email уже используется");
                    return View(model);
                }

                AgencyDBContext db = new AgencyDBContext();
                if (User.IsInRole("Operator") || User.IsInRole("Administrator"))
                {
                    Employee employee = db.Employees.Find(userID);

                    int countTelephone = db.Employees.Where(e => e.Telephone == user.PhoneNumber && e.ID != employee.ID).Count();
                    int countEmail = db.Employees.Where(e => e.Email == user.Email && e.ID != employee.ID).Count();

                    if (countTelephone == 0 && countEmail == 0)
                    {
                        employee.FullName = user.FullName;
                        employee.Birthday = user.BirthDate;
                        employee.Telephone = user.PhoneNumber;
                        employee.Email = user.Email;

                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (countTelephone > 0)
                            ModelState.AddModelError("PhoneNumber", "Данный телефон уже используется");
                        if (countEmail > 0)
                            ModelState.AddModelError("Email", "Данный Email уже используется");

                        return View(model);
                    }
                }
                else if (User.IsInRole("User"))
                {
                    if (userID != -1)
                    {
                        Policyholder policyholder = db.Policyholders.Find(userID);

                        int countTelephone = db.Policyholders.Where(p => p.Telephone == user.PhoneNumber && p.ID != policyholder.ID).Count();
                        int countEmail = db.Policyholders.Where(p => p.Email == user.Email && p.ID != policyholder.ID).Count();

                        if (countTelephone == 0 && countEmail == 0)
                        {
                            policyholder.FullName = user.FullName;
                            policyholder.Birthday = user.BirthDate;
                            policyholder.Telephone = user.PhoneNumber;
                            policyholder.Email = user.Email;

                            db.Entry(policyholder).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            if (countTelephone > 0)
                                ModelState.AddModelError("PhoneNumber", "Данный телефон уже используется");
                            if (countEmail > 0)
                                ModelState.AddModelError("Email", "Данный Email уже используется");

                            return View(model);
                        }
                    }
                }

                IdentityResult result = userManager.Update(user);

                if (result.Succeeded)
                {
                    return View("UserProfile", model);
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при сохранении профиля");
                }
            }
            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                string userID = User.Identity.GetUserId();
                IdentityResult result = userManager.ChangePassword(userID, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut();
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", " Ошибка при смене пароля");
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}