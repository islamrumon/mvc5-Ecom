using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace abcShop.Models
{
    public class cart
    {
        public int cartID { get; set; }
        public int product_id { get; set; }
        public decimal discountPrices { get; set; }
        public string p_name { get; set; }
        public double totalPrice { get; set; }
        public int quntity { get; set; }
        public string img { get; set; }
        public List<pvriant> varints { get; set; }
        public double VorMainPrice { get; set; }

        public int[] quntityArray { get; set; }
        public int[] cartIDArray { get; set; }
        public int[] product_idArray { get; set; }
    }

    public class pvriant
    {
        public string varintUnit { get; set; }
        public string variantName { get; set; }
        public int pvvID { get; set; }
    }
}