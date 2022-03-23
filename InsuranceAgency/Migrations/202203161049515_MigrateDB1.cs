namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Policies",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        InsuranceType = c.String(),
                        InsurancePremium = c.Int(nullable: false),
                        InsuranceAmount = c.Int(nullable: false),
                        DateOfConclusion = c.DateTime(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false),
                        PolicyholderID = c.Int(nullable: false),
                        CarID = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Cars", t => t.CarID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.Policyholders", t => t.PolicyholderID, cascadeDelete: true)
                .Index(t => t.PolicyholderID)
                .Index(t => t.CarID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.PersonAllowedToDrivePolicies",
                c => new
                    {
                        PersonAllowedToDrive_ID = c.Int(nullable: false),
                        Policy_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PersonAllowedToDrive_ID, t.Policy_ID })
                .ForeignKey("dbo.PersonAllowedToDrives", t => t.PersonAllowedToDrive_ID, cascadeDelete: true)
                .ForeignKey("dbo.Policies", t => t.Policy_ID, cascadeDelete: true)
                .Index(t => t.PersonAllowedToDrive_ID)
                .Index(t => t.Policy_ID);
            
            CreateIndex("dbo.InsuranceEvents", "PolicyID");
            AddForeignKey("dbo.InsuranceEvents", "PolicyID", "dbo.Policies", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Policies", "PolicyholderID", "dbo.Policyholders");
            DropForeignKey("dbo.PersonAllowedToDrivePolicies", "Policy_ID", "dbo.Policies");
            DropForeignKey("dbo.PersonAllowedToDrivePolicies", "PersonAllowedToDrive_ID", "dbo.PersonAllowedToDrives");
            DropForeignKey("dbo.InsuranceEvents", "PolicyID", "dbo.Policies");
            DropForeignKey("dbo.Policies", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Policies", "CarID", "dbo.Cars");
            DropIndex("dbo.PersonAllowedToDrivePolicies", new[] { "Policy_ID" });
            DropIndex("dbo.PersonAllowedToDrivePolicies", new[] { "PersonAllowedToDrive_ID" });
            DropIndex("dbo.InsuranceEvents", new[] { "PolicyID" });
            DropIndex("dbo.Policies", new[] { "EmployeeID" });
            DropIndex("dbo.Policies", new[] { "CarID" });
            DropIndex("dbo.Policies", new[] { "PolicyholderID" });
            DropTable("dbo.PersonAllowedToDrivePolicies");
            DropTable("dbo.Policies");
        }
    }
}
