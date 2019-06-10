using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using backEnd.DataModel;

namespace backEnd.Models.viewModels
{
    public class wareHouse_VM
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        public string location { get; set; }
        public Nullable<int> createdby { get; set; }
        public Nullable<System.DateTime> createdat { get; set; }
        public Nullable<System.DateTime> updatedat { get; set; }

        public List<Warehouse_tbl> wareHosueList { get; set; }
    }
}