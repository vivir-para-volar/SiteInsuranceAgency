using Microsoft.AspNet.Identity.EntityFramework;

namespace InsuranceAgency.Models.Security
{
    public class MyIdentityDbContext : IdentityDbContext<MyIdentityUser>
    {
        public MyIdentityDbContext() : base("AgencyDBContext")
        {

        }
    }
}