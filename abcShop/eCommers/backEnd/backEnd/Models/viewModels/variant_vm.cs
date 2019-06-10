using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace backEnd.Models.viewModels
{
    public enum Type
    {
        Select,
        Radio,
        CheckBox,
    }
    public class variant_vm
    {
        [Key]
        public int varient_id { get; set; }
        public string variant_name { get; set; }
        public string type { get; set; }
        
        public string[] unit_name { get; set; }
    }
}