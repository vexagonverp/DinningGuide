namespace Dinning_Guide.Models.Rate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Rate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IDReview { get; set; }

        public int IDRestaurant { get; set; }

        [Column("Rate")]
        public double Rate1 { get; set; }

        public string Review { get; set; }

        public virtual Rate Rates1 { get; set; }

        public virtual Rate Rate2 { get; set; }
    }
}
