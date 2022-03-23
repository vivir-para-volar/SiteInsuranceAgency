namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cars",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Model = c.String(),
                        VIN = c.String(),
                        RegistrationPlate = c.String(),
                        VehiclePassport = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                        CarID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Cars", t => t.CarID, cascadeDelete: true)
                .Index(t => t.CarID);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Birthday = c.DateTime(nullable: false),
                        Telephone = c.String(),
                        Passport = c.String(),
                        Login = c.String(),
                        Password = c.String(),
                        Admin = c.Boolean(nullable: false),
                        Works = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.InsuranceEvents",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        InsurancePayment = c.Int(nullable: false),
                        Description = c.String(),
                        PolicyID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PersonAllowedToDrives",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        DrivingLicence = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Policyholders",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Birthday = c.DateTime(nullable: false),
                        Telephone = c.String(),
                        Passport = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "CarID", "dbo.Cars");
            DropIndex("dbo.Photos", new[] { "CarID" });
            DropTable("dbo.Policyholders");
            DropTable("dbo.PersonAllowedToDrives");
            DropTable("dbo.InsuranceEvents");
            DropTable("dbo.Employees");
            DropTable("dbo.Photos");
            DropTable("dbo.Cars");
        }
    }
}
