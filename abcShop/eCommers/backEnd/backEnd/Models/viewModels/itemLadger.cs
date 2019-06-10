using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backEnd.Models.viewModels
{
    public class itemLadger
    {
        public int proId { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
}