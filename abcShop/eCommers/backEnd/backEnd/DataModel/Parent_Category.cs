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
    
    public partial class Parent_Category
    {
        public int category_id { get; set; }
        public string cat_name { get; set; }
        public string logo { get; set; }
        public string banner { get; set; }
        public Nullable<int> parent_id { get; set; }
        public Nullable<bool> top_cat { get; set; }
        public Nullable<int> sort_order { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<System.DateTime> date_added { get; set; }
        public Nullable<System.DateTime> date_modified { get; set; }
        public string Entry_by { get; set; }
        public string Expr1 { get; set; }
    }
}
