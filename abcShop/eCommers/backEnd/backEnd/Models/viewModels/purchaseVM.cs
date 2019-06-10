using backEnd.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace backEnd.Models.viewModels
{
    public class purchaseVM
    {
        public int purchase_id { get; set; }
        [Required]
        public Nullable<int> supplier_id { get; set; }
        [Required]
        public Nullable<decimal> total_amount { get; set; }
        [Required]
        public Nullable<int> warehouse_id { get; set; }

        //this is product propretiys
        [Required]
        public int[] product_id { get; set; }

        //purchase detiels proprety
      
        public decimal[] bayPrice { get; set; }
        public decimal[] salePrice { get; set; }
        public int[] quantity { get; set; }
        public string sku { get; set; }

    }


    public class purchaseList
    {

        [Key]
        public int purchase_id { get; set; }
        
        public Nullable<decimal> total_amount { get; set; }
        public string WareHosueName { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }

        public string sName { get; set; }
        public string sPhone { get; set; }
        public string sAddress { get; set; }
        public string sContact_Person { get; set; }
        public string sMail { get; set; }

        public List<purchase_de> purchaseDetialsList { get; set; }

    }



    public partial class purchase_de
    {
        public int id { get; set; }
        public Nullable<int> purchase_id { get; set; }
        public string product_name { get; set; }
        public Nullable<decimal> purchase_price { get; set; }
        public Nullable<decimal> sale_price { get; set; }
        public Nullable<int> quantity { get; set; }
        public string sku { get; set; }
    }
}