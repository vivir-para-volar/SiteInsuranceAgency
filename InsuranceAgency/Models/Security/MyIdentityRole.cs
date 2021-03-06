using Microsoft.AspNet.Identity.EntityFramework;

namespace InsuranceAgency.Models.Security
{
    public class MyIdentityRole : IdentityRole
    {
        public MyIdentityRole()
        {

        }

        public MyIdentityRole(string roleName, string description) : base(roleName)
        {
            this.Description = description;
        }

        public string Description { get; set; }
    }

}