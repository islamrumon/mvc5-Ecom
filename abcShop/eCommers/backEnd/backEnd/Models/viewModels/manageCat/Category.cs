using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace backEnd.Models.viewModels.manageCat
{
    public class Category
    {
        
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Logo { get; set; }
        public HttpPostedFileBase logo { get; set; }
        public string Banner { get; set; }
        public HttpPostedFileBase banner { get; set; }
        public bool? top_cat { get; set; }
        public int? sort_order { get; set; }
        public bool? status { get; set; }
        public DateTime date_added { get; set; }
        public DateTime date_modified { get; set; }
        public string Entry_by { get; set; }
        public int? ParentCategoryId { get; set; }
        public string parentName { get;set;}
    }

    public class create_category_vm
    {

        [Required]
        [Display(Name = "Category Name")]
        public string cat_name { get; set; }
        [Display(Name = "Parent Category")]
        public int Parent_id { get; set; }
        [Display(Name = "Show In Top")]
        public bool top_cat { get; set; }
        public string videoLink { get; set; }
        public HttpPostedFileBase banner { get; set; }
        public HttpPostedFileBase logo { get; set; }

    }

    public class catTreeview
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }


        public catTreeview ParentCategory { get; set; }
        public List<catTreeview> Children { get; set; }
    }


    public class edit_category_vm
    {
       
        public int serial { get; set; }
        public string CategoryName { get; set; }

        public string LogoV { get; set; }

        [Display(Name = "Logo")]
        [DataType(DataType.Upload)]
        [NotMapped]
        public HttpPostedFileBase logo { get; set; }

        public string BannerV { get; set; }

        [Display(Name = "Logo")]
        [DataType(DataType.Upload)]
        [NotMapped]
        public HttpPostedFileBase banner { get; set; }
        public bool top_cat { get; set; }
        public int sort_order { get; set; }
        public bool status { get; set; }
        public string videoLink { get; set; }
        public DateTime date_modified { get; set; }
        public string Entry_by { get; set; }
     
        public int parentID { get; set; }

    }
}