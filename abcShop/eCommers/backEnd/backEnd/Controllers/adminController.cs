
using backEnd.DataModel;

using backEnd.Models.viewModels.manageCat;

using Microsoft.AspNet.Identity;

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace abcShop_auth.Areas.access.Controllers
{
   
    public class adminController : Controller
    {

        private abcShopEntities db = new abcShopEntities();
        string message;
        bool status = false;
        private List<int?> TakenIds = new List<int?>(); //this list for geting id whene requrcive function is execute
        List<Category> _categories = new List<Category>();


        [Authorize]
        // GET: admin
        public ActionResult panal()
        {
           

            var log = db.site_setting_tbl.Find(1);
            Session["logo"] = log.logo;
            //profile pic
            var user = db.AspNetUsers.Find(User.Identity.GetUserId());
            
            Session["pic"] = user.image;
            return View();
        }

        [Authorize(Roles = "ShowCategory")]
        [HttpGet]
        //this action for Manage And Show Category
        public ActionResult manageCat()
        {

            ViewBag.Tree = GetAllCategoriesForTree();
            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];
            return View();
        }

 

        [NonAction]
        //this function for gatting all categories with parent or sub
        public string GetAllCategoriesForTree()
        {
            // var dt = db.category_tbl.ToList();
            var dt = db.Parent_Category.ToList();
            if (dt != null)
            {
                //assin data in database to viewmodal categories
                foreach (var item in dt)
                {
                    //add data in catagories modal
                    _categories.Add(
                        new Category
                        {
                            CategoryId = item.category_id,
                            CategoryName = item.cat_name,
                            //ParentCategoryId = item.parent_id != 0 ? item.parent_id : 0,
                            ParentCategoryId = item.parent_id,
                            top_cat = item.top_cat,
                            sort_order = item.sort_order,
                            status = item.status,
                            parentName = item.Expr1,
                        });
                }
            }
            return catTree(_categories,0);
        }

        [NonAction]
        public string catTree(List<Category> flatObjects, int? parentId, int prevLevel = -1, int currLevel = 0, string root_td = "")
        {


            foreach (var data in flatObjects)
            {
                //int index = flatObjects.IndexOf(data);


                if (parentId == data.ParentCategoryId)
                {

                    if (currLevel > prevLevel)
                    {
                        root_td += "<tr>";

                    }
                    if (currLevel == prevLevel)
                    {

                        root_td += "</tr>";
                    }



                    root_td +=

                         "<td> #</td>"
                        + "<td>" + data.CategoryName + "</td>"
                        + "<td>" + data.parentName + "</td>"
                        + "<td>" + data.status + "</td>"

                        + "<td>" + data.sort_order + "</td>";
                    if (User.IsInRole("manageCategory"))
                    {
                        root_td += "<td> <a  onclick=\"editCat(" + data.CategoryId + ")\" type=\"button\" class=\"btn radius btn-sm btn-primary float-right\"><i class=\"fa fa-edit\"></i></a>" +
                        "<a href=\"/admin/deleteCat/" + data.CategoryId + "\", OnClick = \"return confirm('Are you sure you to delete this Record?'); \" type=\"button\" class=\"btn radius btn-sm btn-danger\"><i class=\"pe-7s-trash\"></i></a></td></tr> ";
                    }
                       
                    
                

                       

                    if (currLevel > prevLevel)
                    {
                        prevLevel = currLevel;
                    }

                    currLevel++;
                    root_td += catTree(flatObjects, data.CategoryId, prevLevel, currLevel);
                    currLevel--;

                }


            }
            if (currLevel == prevLevel)
            {
                root_td += "</tr>";
            }

            return root_td;
        }


        //this is requarcive function for creating category
        //[NonAction]
        //private string BuildCategoriesMap(List<Category> categories)
        //{
        //    var map = "";
        //    if (categories.Count > 0)
        //    {
        //        map += "<tr>";
        //        foreach (Category cat in categories)
        //        {
        //            if ((!cat.CategoryId.HasValue) || (cat.CategoryId.HasValue && (!TakenIds.Contains(cat.CategoryId))))

        //            {
        //                map += "<td>" + cat.CategoryName;
        //                List<Category> subCats = _categories.Where(c => c.ParentCategoryId == cat.CategoryId).ToList();
        //                map += BuildCategoriesMap(subCats);
        //                map += "</td>";
        //            }
        //            TakenIds.Add(cat.CategoryId);
        //        }
        //        map += "</tr>";
        //    }
        //    return map;
        //}
        //end requrciv function 

        //Category Create Method
        [Authorize(Roles = "manageCategory")]
        [HttpGet]
        //this is for Add category
        public ActionResult createCat()
        {


            ViewBag.parentCat = new SelectList(db.category_tbl.ToList(), "category_id", "cat_name");
            return PartialView();
        }

        
        [Authorize(Roles = "manageCategory")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult createCat(create_category_vm cat, HttpPostedFileBase logo, HttpPostedFileBase banner)
        {
            category_tbl cat_tbl = new category_tbl();
        
            string path;
            cat_tbl.cat_name = cat.cat_name;
            cat_tbl.Entry_by = User.Identity.GetUserId();
            cat_tbl.date_added = DateTime.Now;
            cat_tbl.parent_id = cat.Parent_id;
            cat_tbl.sort_order = 0;
            cat_tbl.status = true;
            cat_tbl.VideoLink = cat.videoLink;
            cat_tbl.top_cat = cat.top_cat;
            db.category_tbl.Add(cat_tbl);
            db.SaveChanges();


            var cat_Id = db.category_tbl.Find(cat_tbl.category_id);

            if (cat.cat_name != null)
            {

                if (cat.logo != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(logo.FileName);
                    string extension = Path.GetExtension(logo.FileName);
                    fileName = "L" + cat_Id.category_id + extension;
                    fileName = fileName.Replace(" ", "-");

                    path = fileName;

                    string savePath = Server.MapPath("~/AppFiles/Cat/");
                    logo.SaveAs(Path.Combine(savePath, fileName));


                    cat_tbl.logo = path;
                }
                else
                {
                    cat_tbl.logo = "download.png";
                }


                if (cat.banner != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(banner.FileName);
                    string extension = Path.GetExtension(banner.FileName);
                    fileName = "B" + cat_Id.category_id + extension; ;
                    fileName = fileName.Replace(" ", "-");

                    path = fileName;

                    string savePath = Server.MapPath("~/AppFiles/Cat/");
                    banner.SaveAs(Path.Combine(savePath, fileName));
                    cat_tbl.banner = path;
                }
                else
                {
                    cat_tbl.banner = "download.png";
                }

                db.SaveChanges();
                message = "Save Success Fully";
                status = true;
            }
            else
            {
                message = "Your Entry Is Wrong";
            }

            TempData["Status"] = status;
            TempData["Message"] = message;
            return RedirectToAction("manageCat");
        }
   

        //Category Edit Methodes
        [Authorize(Roles = "manageCategory")]
        #region
        [HttpGet]
        public ActionResult editCat(int? id)
        {
            edit_category_vm catVM = new edit_category_vm();
            if (id == null)
            {
                message = "Your Action is Wrong";
            }
            else
            {
                category_tbl cat_tbl = db.category_tbl.Find(id);
                if (cat_tbl == null)
                {
                    message = "No Data Found";
                }
                else
                {
                    catVM.serial = cat_tbl.category_id;
                    catVM.CategoryName = cat_tbl.cat_name;
                    catVM.LogoV = cat_tbl.logo;
                    catVM.BannerV = cat_tbl.banner;
                    catVM.parentID = cat_tbl.parent_id;
                    catVM.top_cat = cat_tbl.top_cat;
                    catVM.sort_order = cat_tbl.sort_order;
                    catVM.status = cat_tbl.status;
                    catVM.videoLink = cat_tbl.VideoLink;
                    ViewBag.parentCat = new SelectList(db.category_tbl.ToList(), "category_id", "cat_name");
                    return PartialView(catVM);
                }
            }

            TempData["Message"] = message;
            return RedirectToAction("manageCat");

        }



        [Authorize(Roles = "manageCategory")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editCat(edit_category_vm cat, HttpPostedFileBase logo, HttpPostedFileBase banner)
        {


            string path = "";
            category_tbl cat_tbl = new category_tbl();

            if (cat.CategoryName !=null)
            {
                cat_tbl = db.category_tbl.Find(cat.serial);

                if (cat.logo != null && cat_tbl.logo != cat.logo.FileName)
                {
                    if (cat_tbl.logo != null)
                    {
                        string fullPath = Request.MapPath("~/AppFiles/Cat/" + cat_tbl.logo);
                        if (System.IO.File.Exists(fullPath))
                        {
                            try
                            {
                                System.IO.File.Delete(fullPath);

                            }
                            catch (Exception e)
                            {
                                //Debug.WriteLine(e.Message);
                            }
                        }
                    }
                    //delete logo form system fileFolder

                    //save the new logo
                    string fileName = Path.GetFileNameWithoutExtension(logo.FileName);
                    string extension = Path.GetExtension(logo.FileName);
                    fileName = "L" + cat.serial + extension;
                    fileName = fileName.Replace(" ", "-");
                    path = fileName;
                    string savePath = Server.MapPath("~/AppFiles/Cat/");
                    logo.SaveAs(Path.Combine(savePath, fileName));
                    cat_tbl.logo = path;

                }// manage cat logo


                if (cat.banner != null && cat_tbl.banner != cat.banner.FileName)
                {
                    if (cat_tbl.banner != null)
                    {
                        string fullPath = Request.MapPath("~/AppFiles/Cat/" + cat_tbl.banner);
                        if (System.IO.File.Exists(fullPath))
                        {
                            try
                            {
                                System.IO.File.Delete(fullPath);

                            }
                            catch (Exception e)
                            {
                                //Debug.WriteLine(e.Message);
                            }
                        }
                    }//delete the banner form filefolder

                    string fileName = Path.GetFileNameWithoutExtension(banner.FileName);
                    string extension = Path.GetExtension(banner.FileName);
                    fileName = "B" + cat.serial + extension;
                    fileName = fileName.Replace(" ", "-");
                    path = fileName;
                    string savePath = Server.MapPath("~/AppFiles/Cat/");
                    banner.SaveAs(Path.Combine(savePath, fileName));
                    cat_tbl.banner = path;
                }

                cat_tbl.category_id = cat.serial;
                cat_tbl.cat_name = cat.CategoryName;
                cat_tbl.date_modified = DateTime.Now;
                cat_tbl.Entry_by = User.Identity.GetUserId();
                cat_tbl.parent_id = cat.parentID;
                cat_tbl.sort_order = cat.sort_order;
                cat_tbl.status = cat.status;
                cat_tbl.top_cat = cat.top_cat;
                cat_tbl.VideoLink = cat.videoLink;

                db.Entry(cat_tbl).State = EntityState.Modified;
                db.SaveChanges();

                message = "Category Updated Successfully";
                status = true;
            }
            else
            {
                message = "Thare have some problems.try agine";
            }




            TempData["Status"] = status;
            TempData["Message"] = message;
            return RedirectToAction("manageCat");
        }
        #endregion

        [Authorize(Roles = "manageCategory")]
        //Category Delete Method
        #region 
        public ActionResult deleteCat(int? id)
        {
            if (id == null)
            {
                message = "Wrong Action Atempt";
            }
            else
            {
                var deletecat = db.category_tbl.Find(id);
                if (deletecat == null)
                {
                    message = "Data Field is empty";
                }
                else
                {

                    //Validation logo
                    if (deletecat.logo != null)
                    {
                        //delete the logo
                        string fullPath = Request.MapPath("~/AppFiles/Cat/" + deletecat.logo);
                        if (System.IO.File.Exists(fullPath))
                        {
                            try
                            {
                                System.IO.File.Delete(fullPath);

                            }
                            catch (Exception e)
                            {
                                //Debug.WriteLine(e.Message);
                            }
                        }
                    }


                    //validation Banaer
                    if (deletecat.banner != null)
                    {
                        string fullPath = Request.MapPath("~/AppFiles/Cat/" + deletecat.banner);
                        if (System.IO.File.Exists(fullPath))
                        {
                            try
                            {
                                System.IO.File.Delete(fullPath);
                            }
                            catch (Exception e)
                            {
                                //Debug.WriteLine(e.Message);
                            }
                        }
                    }


                    db.category_tbl.Remove(deletecat);
                    db.SaveChanges();
                    message = "delete successfully";
                    status = true;
                }
            }
            TempData["Status"] = status;
            TempData["Message"] = message;
            return RedirectToAction("manageCat");

        }
        #endregion

        //this is faul Code
        #region

        //[NonAction]
        ////this function for gatting all categories with parent or sub
        //public string GetAllCategoriesForTree()
        //{
        //    // assin a model in list
        //    List<Category> categories = new List<Category>();

        //    //get data form database
        //    //var dt = db.category_tbl.Where(x=>x.parent_id == x.category_tbl);

        //    var dt = db.Parent_Category.ToList();

        //    if (dt != null)
        //    {

        //        //assin data in database to viewmodal categories
        //        foreach (var item in dt)
        //        {

        //            //add data in catagories modal
        //            categories.Add(
        //                new Category
        //                {
        //                    CategoryId = item.category_id,
        //                    CategoryName = item.cat_name,
        //                    //ParentCategoryId = item.parent_id != 0 ? item.parent_id : 0,
        //                    ParentCategoryId = item.parent_id,
        //                    top_cat = item.top_cat,
        //                    sort_order = item.sort_order,
        //                    status = item.status,
        //                    parentName = item.Expr1,
        //                });


        //        }



        //    }
        //    return BuildCategoriesMap(categories);
        //    #region 
        //    //            pass value FillRecursive functionget value form FillRecursive function, and save the data in catTreeview list
        //    //             List<catTreeview> headerTree = FillRecursive(categories, 0);

        //    //            return catTree(categories, 0);
        //    //            #region BindingHeaderMenus

        //    //            string root_td = string.Empty;
        //    //            string cat1_names = string.Empty;
        //    //            string cat2_names = string.Empty;


        //    //            //< tr >
        //    //            //        < td > Tiger Nixon </ td >

        //    //            //           < td > System Architect </ td >

        //    //            //              < td > Edinburgh </ td >

        //    //            //              < td > 61 </ td >

        //    //            //              < td >


        //    //            //                  < button type = "button" class="btn radius btn-sm btn-primary float-right"><i class="fa fa-edit"></i></button>
        //    //            //            <button type = "button" class="btn radius btn-sm btn-danger"><i class="pe-7s-trash"></i></button>
        //    //            //        </td>

        //    //            //    </tr>

        //    //            foreach (var item in headerTree)
        //    //            {
        //    //                root_td += "<tr>"
        //    //                    +"<td>" + item.CategoryName + "</td>"
        //    //                    +"<td>"+item.CategoryId+"</td> </tr>";

        //    //                cat1_names = "";
        //    //                foreach (var down1 in item.Children)
        //    //                {
        //    //                    cat2_names = "<tr>"
        //    //                    + "<td> -" + down1.CategoryName + "</td>"
        //    //                    + "<td>" + down1.CategoryId + "</td> </tr>";
        //    //                    foreach (var down2 in down1.Children)
        //    //                    {
        //    //                        cat2_names += "<tr>"
        //    //                    + "<td> --" + down2.CategoryName + "</td>"
        //    //                    + "<td>" + down2.CategoryId + "</td> </tr>";
        //    //                    }
        //    //    cat1_names += cat2_names;

        //    //                }
        //    //root_td += cat1_names;
        //    //            }
        //    //            return  root_td;

        //    //            foreach (var item in headerTree)
        //    //            {
        //    //                root_td += "<td>"+item.CategoryName+"</td>";

        //    //                cat1_names = "";
        //    //                foreach (var down1 in item.Children)
        //    //                {
        //    //                    cat2_names = "<td> -"+down1.CategoryName+"</td>";
        //    //                    foreach (var down2 in down1.Children)
        //    //                    {
        //    //                        cat2_names += "<td> --" + down2.CategoryName + "</td>";
        //    //                    }
        //    //                    cat1_names += cat2_names;

        //    //                }
        //    //                root_td +=  cat1_names ;
        //    //            }
        //    //            return "<tr>"+  root_td + "</tr>";

        //    //            foreach (var item in headerTree)
        //    //            {
        //    //                root_li += "<li class=\"dropdown mega-menu-fullwidth\">"
        //    //                           + "<a href=\"/Product/ListProduct?cat=" + item.CategoryId + "\" class=\"dropdown-toggle\" data-hover=\"dropdown\" data-toggle=\"dropdown\">" + item.CategoryName + "</a>";

        //    //                down1_names = "";
        //    //                foreach (var down1 in item.Children)
        //    //                {
        //    //                    down2_names = "";
        //    //                    foreach (var down2 in down1.Children)
        //    //                    {
        //    //                        down2_names += "<li><a href=\"/Product/ListProduct?cat=" + down2.CategoryId + "\">" + down2.CategoryName + "</a></li>";
        //    //                    }
        //    //                    down1_names += "<div class=\"col-md-2 col-sm-6\">"
        //    //                                    + "<h3 class=\"mega-menu-heading\"><a href=\"/Product/ListProduct?cat=" + down1.CategoryId + "\">" + down1.CategoryName + "</a></h3>"
        //    //                                    + "<ul class=\"list-unstyled style-list\">"
        //    //                                    + down2_names
        //    //                                    + "</ul>"
        //    //                                  + "</div>";
        //    //                }
        //    //                root_li += "<ul class=\"dropdown-menu\">"
        //    //                            + "<li>"
        //    //                                + "<div class=\"mega-menu-content\">"
        //    //                                    + "<div class=\"container\">"
        //    //                                        + "<div class=\"row\">"
        //    //                                            + down1_names
        //    //                                        + "</div>"
        //    //                                    + "</div>"
        //    //                                + "<div>"
        //    //                            + "</li>"
        //    //                            + "</ul>"
        //    //                     + "</li>";
        //    //            }
        //    //            #endregion

        //    //            return "<ul class=\"nav navbar-nav\">" + root_li + "</ul>";
        //    //            }
        //    //            return "Record Not Found!!";
        //    #endregion
        //}



        #region
        //      function createTreeView($array, $currentParent, $currLevel = 0, $prevLevel = -1)
        //      {

        //          foreach ($array as $category) {

        //              if ($currentParent == $category['parent']) {
        //                  if ($currLevel > $prevLevel) echo " <ol class='tree'> ";

        //                  if ($currLevel == $prevLevel) echo " </li> ";

        //                  echo '<li> <label for="subfolder2">'.$category['name'];

        //                  if ($currLevel > $prevLevel) { $prevLevel = $currLevel; }

        //$currLevel++;

        //                  createTreeView($array, $category['id'], $currLevel, $prevLevel);

        //$currLevel--;
        //              }

        //          }

        //          if ($currLevel == $prevLevel) echo " </li>  </ol> ";

        //      }





        #endregion
        #endregion

        #region //this is slider
        [HttpGet]
        public ActionResult createSlider()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult createSlider(slider_image_tbl slider)
        {
            string path;
            if (ModelState.IsValid && slider.banner != null)
            {
               var s= db.slider_image_tbl.Add(slider);
                db.SaveChanges();


                var id = db.slider_image_tbl.Find(s.slider_image_id);

                //save the logo

                if (slider.banner != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(slider.banner.FileName);
                    string extension = Path.GetExtension(slider.banner.FileName);
                    fileName = "Slider" + id.slider_image_id + extension;
                    fileName = fileName.Replace(" ", "-");

                    path = fileName;

                    string savePath = Server.MapPath("~/AppFiles/");
                    slider.banner.SaveAs(Path.Combine(savePath, fileName));
                    slider.image = path;
                    db.SaveChanges();
                }

                message = "Slider  Create Successfully";
                status = true;
        
  
                 TempData["Message"] = message;
                 TempData["Status"] = status;
                return View();
            }
            else
            {
                TempData["Message"] = "There Are some Problems";
                TempData["Status"] = false;
                return RedirectToAction("createSlider");
            }


        }

        //this is list
        public ActionResult listSlider()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            var t = db.slider_image_tbl.OrderByDescending(a => a.sort_order).ToList();
            return View(t);
        }

        //delete Slider
        public ActionResult deleteSlider(int id)
        {
            if (id !=null)
            {
                var r = db.slider_image_tbl.Find(id);
                db.slider_image_tbl.Remove(r);
                db.SaveChanges();
                message = "Slider Is deleted";
                status = true;
                
            }
            else
            {
                message = "There are SomeProblems";
            }

            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("listSlider");
        }

        //edit slider
        [HttpGet]
        public ActionResult editSlider(int? id)
        {
            if (id !=null)
            {
                    var t = db.slider_image_tbl.Find(id);
                    if (t != null)
                    {
                        return View(t);
                    }
            }
            TempData["Message"] = "There Are Some Probelms";
            return RedirectToAction("listSlider");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editSlider(slider_image_tbl slider)
        {
            string path;
            if (ModelState.IsValid)
            {
                if (slider.banner != null)
                {
                    path = Server.MapPath("~/AppFiles/" + slider.image);
                    if (System.IO.File.Exists(path))
                    {
                        try
                        {
                            System.IO.File.Delete(path);

                        }
                        catch (Exception e)
                        {
                            //Debug.WriteLine(e.Message);
                        }
                    }

                    string fileName = Path.GetFileNameWithoutExtension(slider.banner.FileName);
                    string extension = Path.GetExtension(slider.banner.FileName);
                    fileName = "Slider" + slider.slider_image_id + extension;
                    fileName = fileName.Replace(" ", "-");

                    path = fileName;

                    string savePath = Server.MapPath("~/AppFiles/");
                    slider.banner.SaveAs(Path.Combine(savePath, fileName));
                    slider.image = path;



                }
                

                db.Entry(slider).State = EntityState.Modified;
                db.SaveChanges();
                message = "ManuFacturer Updated Successfully";
                status = true;
                
            }
            else
            {
                message = "There ARe Some Probelms";
                
                
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("listSlider");
        }

        #endregion

        #region
        public ActionResult messages() {

            var message = db.contact_tbl.OrderByDescending(a => a.contact_id).ToList();
            return View(message);
        }
        #endregion

    }
}