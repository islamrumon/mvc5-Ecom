//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace abcShop.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class address_tbl
    {
        public int address_id { get; set; }
        public Nullable<int> customer_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string shepingAddress { get; set; }
        public string address_2 { get; set; }
        public string district { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        public string thana { get; set; }
        public string SheppingPhoneNumber { get; set; }
    }
}
