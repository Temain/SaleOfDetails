namespace SaleOfDetails.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePhoneFromEmployee : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Employee", "EmployeeDateStart", c => c.DateTime());
            DropColumn("dbo.Employee", "Phone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employee", "Phone", c => c.String());
            AlterColumn("dbo.Employee", "EmployeeDateStart", c => c.DateTime(nullable: false));
        }
    }
}
