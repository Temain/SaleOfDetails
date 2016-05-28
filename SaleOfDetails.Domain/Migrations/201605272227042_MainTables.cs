namespace SaleOfDetails.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MainTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        PersonId = c.Int(nullable: false),
                        Phone = c.String(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ClientId)
                .ForeignKey("dbo.Person", t => t.PersonId)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.Sale",
                c => new
                    {
                        SaleId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        NumberOfProducts = c.Int(nullable: false),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ClientId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        SaleDate = c.DateTime(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.SaleId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.Employee", t => t.EmployeeId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .Index(t => t.ProductId)
                .Index(t => t.ClientId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductName = c.String(),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InStock = c.Int(nullable: false),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductId);
            
            AddColumn("dbo.Employee", "Phone", c => c.String());
            AddColumn("dbo.Employee", "CreatedAt", c => c.DateTime());
            AddColumn("dbo.Employee", "DeletedAt", c => c.DateTime());
            AddColumn("dbo.Person", "CreatedAt", c => c.DateTime());
            AddColumn("dbo.Person", "DeletedAt", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "PersonId", c => c.Int());
            AlterColumn("dbo.Person", "LastName", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Person", "FirstName", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.Person", "MiddleName", c => c.String(maxLength: 500));
            CreateIndex("dbo.AspNetUsers", "PersonId");
            AddForeignKey("dbo.AspNetUsers", "PersonId", "dbo.Person", "PersonId");
            DropColumn("dbo.Employee", "EmployeeDateEnd");
            DropColumn("dbo.Person", "Sex");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropColumn("dbo.AspNetUsers", "MiddleName");
            DropColumn("dbo.AspNetUsers", "Hometown");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Hometown", c => c.String());
            AddColumn("dbo.AspNetUsers", "MiddleName", c => c.String());
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String());
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String());
            AddColumn("dbo.Person", "Sex", c => c.Byte());
            AddColumn("dbo.Employee", "EmployeeDateEnd", c => c.DateTime());
            DropForeignKey("dbo.Sale", "ProductId", "dbo.Product");
            DropForeignKey("dbo.Sale", "EmployeeId", "dbo.Employee");
            DropForeignKey("dbo.Sale", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Client", "PersonId", "dbo.Person");
            DropForeignKey("dbo.AspNetUsers", "PersonId", "dbo.Person");
            DropIndex("dbo.Sale", new[] { "EmployeeId" });
            DropIndex("dbo.Sale", new[] { "ClientId" });
            DropIndex("dbo.Sale", new[] { "ProductId" });
            DropIndex("dbo.AspNetUsers", new[] { "PersonId" });
            DropIndex("dbo.Client", new[] { "PersonId" });
            AlterColumn("dbo.Person", "MiddleName", c => c.String(maxLength: 200));
            AlterColumn("dbo.Person", "FirstName", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Person", "LastName", c => c.String(nullable: false, maxLength: 200));
            DropColumn("dbo.AspNetUsers", "PersonId");
            DropColumn("dbo.Person", "DeletedAt");
            DropColumn("dbo.Person", "CreatedAt");
            DropColumn("dbo.Employee", "DeletedAt");
            DropColumn("dbo.Employee", "CreatedAt");
            DropColumn("dbo.Employee", "Phone");
            DropTable("dbo.Product");
            DropTable("dbo.Sale");
            DropTable("dbo.Client");
        }
    }
}
