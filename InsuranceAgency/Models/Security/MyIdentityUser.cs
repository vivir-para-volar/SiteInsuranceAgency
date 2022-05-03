using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InsuranceAgency.Models.Security
{
    public class MyIdentityUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
    }
}