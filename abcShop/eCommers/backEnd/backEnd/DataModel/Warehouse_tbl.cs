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
    
    public partial class Warehouse_tbl
    {
        public int warehouse_id { get; set; }
        public string w_name { get; set; }
        public string w_location { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}
