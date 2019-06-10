
using backEnd.DataModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace backEnd.Models.viewModels
{
    public class supplier_vm
    {
        [Key]
        public int supplier_id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Contact_Person { get; set; }
        public string Mail { get; set; }
        public bool Status { get; set; }
        public string Create_by { get; set; }


        public List<Supplier_tbl> spList { get; set; }
    }
}