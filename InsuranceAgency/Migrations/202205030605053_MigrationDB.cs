namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationDB : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Policyholders", "Telephone", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Policyholders", "Telephone", c => c.String(nullable: false, maxLength: 15));
        }
    }
}
