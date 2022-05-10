namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Email", c => c.String(nullable: false));
            DropColumn("dbo.Employees", "Login");
            DropColumn("dbo.Employees", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Password", c => c.String(nullable: false, maxLength: 40));
            AddColumn("dbo.Employees", "Login", c => c.String(nullable: false, maxLength: 40));
            DropColumn("dbo.Employees", "Email");
        }
    }
}
