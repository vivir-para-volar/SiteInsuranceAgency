namespace InsuranceAgency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cars", "Model", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Cars", "VIN", c => c.String(nullable: false, maxLength: 17));
            AlterColumn("dbo.Cars", "RegistrationPlate", c => c.String(nullable: false, maxLength: 25));
            AlterColumn("dbo.Cars", "VehiclePassport", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Photos", "Path", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Policies", "InsuranceType", c => c.String(nullable: false, maxLength: 5));
            AlterColumn("dbo.Employees", "FullName", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.Employees", "Telephone", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("dbo.Employees", "Passport", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Employees", "Login", c => c.String(maxLength: 50));
            AlterColumn("dbo.Employees", "Password", c => c.String(maxLength: 32));
            AlterColumn("dbo.InsuranceEvents", "Description", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.PersonAllowedToDrives", "FullName", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.PersonAllowedToDrives", "DrivingLicence", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Policyholders", "FullName", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.Policyholders", "Telephone", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("dbo.Policyholders", "Passport", c => c.String(nullable: false, maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Policyholders", "Passport", c => c.String());
            AlterColumn("dbo.Policyholders", "Telephone", c => c.String());
            AlterColumn("dbo.Policyholders", "FullName", c => c.String());
            AlterColumn("dbo.PersonAllowedToDrives", "DrivingLicence", c => c.String());
            AlterColumn("dbo.PersonAllowedToDrives", "FullName", c => c.String());
            AlterColumn("dbo.InsuranceEvents", "Description", c => c.String());
            AlterColumn("dbo.Employees", "Password", c => c.String());
            AlterColumn("dbo.Employees", "Login", c => c.String());
            AlterColumn("dbo.Employees", "Passport", c => c.String());
            AlterColumn("dbo.Employees", "Telephone", c => c.String());
            AlterColumn("dbo.Employees", "FullName", c => c.String());
            AlterColumn("dbo.Policies", "InsuranceType", c => c.String());
            AlterColumn("dbo.Photos", "Path", c => c.String());
            AlterColumn("dbo.Cars", "VehiclePassport", c => c.String());
            AlterColumn("dbo.Cars", "RegistrationPlate", c => c.String());
            AlterColumn("dbo.Cars", "VIN", c => c.String());
            AlterColumn("dbo.Cars", "Model", c => c.String());
        }
    }
}
