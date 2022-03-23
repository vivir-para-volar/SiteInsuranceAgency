namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employees", "Login", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Employees", "Password", c => c.String(nullable: false, maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Employees", "Password", c => c.String(maxLength: 32));
            AlterColumn("dbo.Employees", "Login", c => c.String(maxLength: 50));
        }
    }
}
