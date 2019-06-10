using backEnd.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace backEnd.Models.viewModels.manageCat
{
    public class productsv
    {
        [Key]
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
        public string discription { get; set; }
        public string tag { get; set; }
        public string meta_title { get; set; }
        public string meta_description { get; set; }
        public string meta_keyword { get; set; }
        public string Entry_by { get; set; }
        public Nullable<int> sort_order { get; set; }

        //varenat valus
        public int?[] pvvID { get; set; }
        public Nullable<int> pv_Id { get; set; }

        public Nullable<int> v_price { get; set; }
        public Nullable<int> variant_unit_id { get; set; }
        public string variant_unit_name { get; set; }
        public Nullable<int> variant_id { get; set; }
        public string variant_name { get; set; }
        public Nullable<int> quentity { get; set; }

        //print all glarey
        public List<product_image_tbl> gallery { get; set; }

        //this list for print all related product
        public List<product_tbl> relatedProduct { get; set; }

        //order_product_id
        public int order_product_id { get; set; }
        public int? order_id { get; set; }
        //this list for print all new Arrive
        public List<product_tbl> newProduct { get; set; }
        //print categorys list
        public List<category_tbl> proCategory { get; set; }
    }
}