using backEnd.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backEnd.Models.viewModels
{
    public class slider
    {
        public int slider_image_id { get; set; }
        public Nullable<int> slider_id { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string link { get; set; }
        public string image { get; set; }
        public Nullable<int> sort_order { get; set; }

        public List<slider_image_tbl> sliderList { get; set; }
    }
}