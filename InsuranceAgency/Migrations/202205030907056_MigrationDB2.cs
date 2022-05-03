namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationDB2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "Login", c => c.String(nullable: false, maxLength: 40));
            AlterColumn("dbo.Employees", "Password", c => c.String(nullable: false, maxLength: 40));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "Password", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.Employees", "Login", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
