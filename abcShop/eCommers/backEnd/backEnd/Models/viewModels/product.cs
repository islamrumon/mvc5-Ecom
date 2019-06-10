using backEnd.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace backEnd.Models.viewModels
{
    public class product
    {
        public int product_id { get; set; }
        public string p_name { get; set; }
        public Nullable<double> quantity { get; set; }
        public Nullable<int> stock_status_id { get; set; }
        public string main_image { get; set; }
        public Nullable<int> manufacturer_id { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<int> points { get; set; }
        public Nullable<System.DateTime> date_available { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> viewed { get; set; }
        public Nullable<System.DateTime> date_added { get; set; }
        public Nullable<System.DateTime> date_modified { get; set; }
        public HttpPostedFileBase mainImage { get; set; }
        public HttpPostedFileBase[] file { get; set; }
        [AllowHtml]
        public string discription { get; set; }
        public string tag { get; set; }
        public string meta_title { get; set; }
        public string meta_description { get; set; }
        public string meta_keyword { get; set; }
        public Nullable<int> Entry_by { get; set; }
        public Nullable<int> sort_order { get; set; }
        public int sl { get; set; }
        public int[] SelectedValues { get; set; }
        public int?[] CategoryId { get; set; }
        public int id { get; set; }
        public Nullable<int>[] varient_id { get; set; }
        
        public Nullable<int> value { get; set; }
        public Nullable<int> variant_unit { get; set; }
        public int product_image_id { get; set; }
        public string videoLink { get; set; }
        public string image { get; set; }
        
        public int r_id { get; set; }
        
        public int?[] related_id { get; set; }


        //product variant value
        public Nullable<int> productVariant_Id { get; set; } 

        public int?[] pVprice { get; set; }
        public Nullable<int>[] variant_unit_id { get; set; }
        public string[] unit_name { get; set; }
        
        public string variant_name { get; set; }
        public Nullable<int>[] quentity { get; set; }


        ////this 
        public v_VM[][][][] variant { get; set; }


        //there are discount proprety
        public int product_discount_id { get; set; }
       
        public Nullable<int> customer_group_id { get; set; }
        public Nullable<int> discount_quantity { get; set; }
        public Nullable<int> priority { get; set; }
        public Nullable<decimal> discount_price { get; set; }
        public Nullable<System.DateTime> discount_date_start { get; set; }
        public Nullable<System.DateTime> discount_date_end { get; set; }
    }
}