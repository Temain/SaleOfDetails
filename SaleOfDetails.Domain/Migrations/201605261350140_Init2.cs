namespace SaleOfDetails.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Employee", "EmployeeGuid");
            DropColumn("dbo.Employee", "EmployeeCode");
            DropColumn("dbo.Employee", "IsDeleted");
            DropColumn("dbo.Person", "PersonGuid");
            DropColumn("dbo.Person", "PersonMasterGuid");
            DropColumn("dbo.Person", "PersonCode");
            DropColumn("dbo.Person", "PassportNumber");
            DropColumn("dbo.Person", "PassportSeries");
            DropColumn("dbo.Person", "IsMarkedAsDuplicated");
            DropColumn("dbo.Person", "IsDeleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Person", "IsDeleted", c => c.Boolean());
            AddColumn("dbo.Person", "IsMarkedAsDuplicated", c => c.Boolean());
            AddColumn("dbo.Person", "PassportSeries", c => c.String());
            AddColumn("dbo.Person", "PassportNumber", c => c.String());
            AddColumn("dbo.Person", "PersonCode", c => c.String(maxLength: 20));
            AddColumn("dbo.Person", "PersonMasterGuid", c => c.Guid());
            AddColumn("dbo.Person", "PersonGuid", c => c.Guid());
            AddColumn("dbo.Employee", "IsDeleted", c => c.Boolean());
            AddColumn("dbo.Employee", "EmployeeCode", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.Employee", "EmployeeGuid", c => c.Guid());
        }
    }
}
