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
    
    public partial class newsletter_tbl
    {
        public int newsletter_id { get; set; }
        public string email { get; set; }
        public Nullable<System.DateTime> date_add { get; set; }
        public Nullable<int> status { get; set; }
    }
}
