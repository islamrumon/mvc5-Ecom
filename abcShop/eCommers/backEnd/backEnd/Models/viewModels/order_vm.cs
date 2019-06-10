using backEnd.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace backEnd.Models.viewModels
{
    public class order_vm
    {

        //this is all order table proprety 
        [Key]
        public int order_id { get; set; }
        public Nullable<int> invoice_no { get; set; }
        public string invoice_prefix { get; set; }
        public Nullable<int> customer_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string payment_firstname { get; set; }
        public string payment_lastname { get; set; }
        public string payment_telephone { get; set; }
        public string payment_address_1 { get; set; }
        public string payment_address_2 { get; set; }
        public string payment_district { get; set; }
        public string payment_thana { get; set; }
        public string payment_postcode { get; set; }
        public string payment_country { get; set; }
        public string payment_method { get; set; }
        public string shipping_firstname { get; set; }
        public string shipping_lastname { get; set; }
        public string shipping_telephone { get; set; }
        public string shipping_address_1 { get; set; }
        public string shipping_address_2 { get; set; }
        public string shipping_district { get; set; }
        public string shipping_thana { get; set; }
        public string shipping_postcode { get; set; }
        public string shipping_country { get; set; }
        public string shipping_method { get; set; }
        public string comment { get; set; }
        public Nullable<decimal> total { get; set; }
        public string tracking { get; set; }
        public Nullable<int> order_status_id { get; set; }
        public string status_name { get; set; }
        public string ip { get; set; }
        public string user_agent { get; set; }
        public Nullable<System.DateTime> date_added { get; set; }
        public Nullable<System.DateTime> date_modified { get; set; }

        public int warehouse_id { get; set; }

        //end order table proprety

        //this is order product table
        public int order_history_id { get; set; }
              public string order_status { get; set; }
        public bool notify { get; set; }
       

        //order product list
        public List<oPtbl> orderProductList { get; set; }

        //varint list
        public decimal sAmount { get; set; }

    }

    public class oPtbl {
        public int order_product_id { get; set; }
        public Nullable<int> order_id { get; set; }
        public Nullable<int> product_id { get; set; }
        public string name { get; set; }
        public string model { get; set; }
        public Nullable<int> quantity { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<int> reward { get; set; }

        public decimal discount { get; set; }
        public List<pvriant> varints { get; set; }
    }

    public class pvriant
    {
        public string varintUnit { get; set; }
        public string variantName { get; set; }
        public int pvvID { get; set; }
        public decimal price { get; set; }
    }
}