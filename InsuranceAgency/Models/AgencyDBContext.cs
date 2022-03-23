using System.Data.Entity;

namespace InsuranceAgency.Models
{
    public class AgencyDBContext : DbContext
    {
        public DbSet<Car> Car { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<InsuranceEvent> InsuranceEvents { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Policyholder> Policyholders { get; set; }
        public DbSet<PersonAllowedToDrive> PersonAllowedToDrives { get; set; }
    }
}