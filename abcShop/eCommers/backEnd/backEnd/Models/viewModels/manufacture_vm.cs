using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace backEnd.Models.viewModels
{
    public class manufacture_vm
    {
        [Key]
        public int manufacturer_id { get; set; }
        [Required]
        public string brand_name { get; set; }
        public string logo { get; set; }
        public Nullable<int> sort_order { get; set; }

        public HttpPostedFileBase file { get; set; }
    }
}