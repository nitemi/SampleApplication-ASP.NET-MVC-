using MyFirstAspnetProject.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MyFirstAspnetProject.Data
{
    public class MyDbContext: DbContext
    {

        public MyDbContext()  : base ("MyContext") //<=connection string
        { 

        }
        public DbSet<Products> Products { get; set; }

        public DbSet <Category> Categories { get; set; }

        public DbSet<ProductImage> ProductImages { get; set; }

        //by default Ef tends to pluralise name, so we override it
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}