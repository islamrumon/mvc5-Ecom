
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace abcShop.Models
{
    public class checkOut
    {
        [Key]
        public int customer_id { get; set; }
        [Required]
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        [Required]
        public string telephone { get; set; }
        public decimal sAmount { get; set; }
        public Nullable<int> address_id { get; set; }
        public string ip { get; set; }
        public Nullable<int> newsletter { get; set; }
        public Nullable<bool> status { get; set; }
        public string token { get; set; }
        public string code { get; set; }
        public Nullable<int> approved { get; set; }
        public Nullable<System.DateTime> date_added { get; set; }
        public string aspnetuserid { get; set; }

        public double totaltaka { get; set; }

        //address table

        public string customerAddress { get; set; }
        
        public string shiping_Address { get; set; }
        
        public string shippingPhoneNumber { get; set; }

        //inser all cart id
        public string cartIDs { get; set; }

        //beling method
        public string belingMethod { get; set; }
        [Required]
        public int? shipping_id { get; set; }
        [Required]
        public int sid { get; set; }

        public string shipping_method { get; set; }
    }
}