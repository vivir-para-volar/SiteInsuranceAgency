namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB6 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Employees", "Admin");
            DropColumn("dbo.Employees", "Works");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Works", c => c.Boolean(nullable: false));
            AddColumn("dbo.Employees", "Admin", c => c.Boolean(nullable: false));
        }
    }
}
