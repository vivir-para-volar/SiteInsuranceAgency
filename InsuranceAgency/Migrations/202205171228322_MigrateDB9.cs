namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB9 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Policyholders", "Email", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Policyholders", "Email", c => c.String(nullable: false));
        }
    }
}
