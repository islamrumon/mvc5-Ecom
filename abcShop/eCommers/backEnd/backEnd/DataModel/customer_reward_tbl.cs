//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace backEnd.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class customer_reward_tbl
    {
        public int customer_reward_id { get; set; }
        public Nullable<int> customer_id { get; set; }
        public Nullable<int> order_id { get; set; }
        public string description { get; set; }
        public Nullable<int> points { get; set; }
        public Nullable<System.DateTime> date_added { get; set; }
    }
}
