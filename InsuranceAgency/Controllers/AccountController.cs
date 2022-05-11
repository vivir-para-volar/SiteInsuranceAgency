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
        public ActionResult Register(Register model, string role)
        {
            if (ModelState.IsValid)
            {
                var userWithSameName = userManager.FindByName(model.UserName);
                if (userWithSameName != null)
                {
                    ModelState.AddModelError("UserName", "Данный логин уже используется");
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
                    userManager.AddToRole(user.Id, role);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при создании пользователя!");
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
                var userTelephone = userManager.GetPhoneNumber(User.Identity.GetUserId());
                ViewBag.EmployeeID = db.Employees.First(e => e.Telephone == userTelephone).ID;
            }
            else 
                ViewBag.EmployeeID = -1;

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeProfile(Profile model, int employeeID)
        {
            if (ModelState.IsValid)
            {
                MyIdentityUser user = userManager.FindById(User.Identity.GetUserId());
                user.UserName = model.UserName.Trim();
                user.FullName = model.FullName.Trim();
                user.BirthDate = model.BirthDate;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;

                IdentityResult result = userManager.Update(user);

                if (User.IsInRole("Operator") || User.IsInRole("Administrator"))
                {
                    AgencyDBContext db = new AgencyDBContext();

                    Employee employee = db.Employees.Find(employeeID);

                    int countTelephone = db.Employees.Where(e => e.Telephone == user.PhoneNumber && e.ID != employee.ID).Count();

                    if (countTelephone == 0)
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
                        ModelState.AddModelError("PhoneNumber", "Данный телефон уже используется");
                        return View(model);
                    }
                }

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