using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dinning_Guide.Models.Review
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}