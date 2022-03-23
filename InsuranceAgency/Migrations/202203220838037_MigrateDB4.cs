namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InsuranceEvents", "PolicyID", "dbo.Policies");
            DropIndex("dbo.InsuranceEvents", new[] { "PolicyID" });
            DropColumn("dbo.InsuranceEvents", "PolicyID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InsuranceEvents", "PolicyID", c => c.Int(nullable: false));
            CreateIndex("dbo.InsuranceEvents", "PolicyID");
            AddForeignKey("dbo.InsuranceEvents", "PolicyID", "dbo.Policies", "ID", cascadeDelete: true);
        }
    }
}
