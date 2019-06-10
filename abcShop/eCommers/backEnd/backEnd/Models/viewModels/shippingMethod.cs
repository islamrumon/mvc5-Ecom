using backEnd.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace backEnd.Models.viewModels
{
    public class shippingMethod
    {
        [Key]
        public int shipping_id { get; set; }
        public string title { get; set; }
        public Nullable<decimal> amount { get; set; }
        public Nullable<int> status { get; set; }

        ///this is list
        ///
        public List<payment_method_tbl> MethodList { get; set; }
    }

}