namespace MyFirstAspnetProject.Migrations
{
     using System;
        using System.Data.Entity.Migrations;

        public partial class Added_ProductImage : DbMigration
        {
            public override void Up()
            {
                CreateTable(
                    "dbo.ProductImage",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ImagePath = c.String(nullable: false, maxLength: 255),
                    })
                    .PrimaryKey(t => t.Id);

            }

            public override void Down()
            {
                DropTable("dbo.ProductImage");
            }
        }
    }

    //public partial class Added_ProductImage : DbMigration
    //{
    //    public override void Up()
    //    {
    //    }

    //    public override void Down()
    //    {
    //    }
    //}