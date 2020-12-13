namespace Dinning_Guide.Models.Restaurant
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Db_Restaurants : DbContext
    {
        public Db_Restaurants()
            : base("name=Restaurants")
        {
        }

        public virtual DbSet<Restaurant> Restaurants { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
