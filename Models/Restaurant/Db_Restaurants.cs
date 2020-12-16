using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Dinning_Guide.Models.Restaurant
{
    public partial class Db_Restaurants : DbContext
    {
        public Db_Restaurants()
            : base("name=Db_Restaurants")
        {
        }

        public virtual DbSet<Restaurant> Restaurants { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>().ToTable("Restaurants");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
