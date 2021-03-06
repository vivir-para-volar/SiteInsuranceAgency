using InsuranceAgency.Models.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace InsuranceAgency
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            MyIdentityDbContext db = new MyIdentityDbContext();
            RoleStore<MyIdentityRole> roleStore = new RoleStore<MyIdentityRole>(db);
            RoleManager<MyIdentityRole> roleManager = new RoleManager<MyIdentityRole>(roleStore);

            if (!roleManager.RoleExists("Administrator"))
            {
                MyIdentityRole newRole = new MyIdentityRole("Administrator", "????????????? ???????? ??????? ??????? ? ???????");
                roleManager.Create(newRole);
            }

            if (!roleManager.RoleExists("Operator"))
            {
                MyIdentityRole newRole = new MyIdentityRole("Operator", "????????? ????? ?????? ????????? ? ???????? ?????? ? ???????");
                roleManager.Create(newRole);
            }

            if (!roleManager.RoleExists("User"))
            {
                MyIdentityRole newRole = new MyIdentityRole("User", "???????????? ????? ?????? ???????? ???? ?????? ? ???????");
                roleManager.Create(newRole);
            }

            Jobs.MailSchedule.StartReportsSchedule();
            Jobs.MailSchedule.StartUsersSchedule();
        }
    }
}
