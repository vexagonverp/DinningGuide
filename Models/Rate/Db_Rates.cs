using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Dinning_Guide.Models.Rate
{
    public partial class Db_Rates : DbContext
    {
        public Db_Rates()
            : base("name=Db_Rates")
        {
        }

        public virtual DbSet<Rate> Rates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rate>().ToTable("Rates");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
