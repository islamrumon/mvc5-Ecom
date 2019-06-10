using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using backEnd.DataModel;
using backEnd.Models.viewModels;
using backEnd.Models.viewModels.manageCat;
using ImageResizer;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebGrease.Css.Extensions;

namespace abcShop_auth.Areas.access.Controllers
{
    [Authorize]
    public class manageProductsController : Controller
    {
        private abcShopEntities db = new abcShopEntities();
        string path = "";
        string fileName = "";
        string extension = "";
        string savePath = "";
        string message = "";
        bool status = false;
        string op = "";
        string cp = "";
        string imgTr = "";

        private List<int?> TakenIds = new List<int?>(); //this list for geting id whene requrcive function is execute
        List<Category> _categories = new List<Category>();

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.variantlist = variantlist();
            var categories = db.category_tbl.Where(p => p.status == true && p.parent_id != 0).ToList();
            var product = db.product_tbl.Where(p => p.status == 1).ToList();
            ViewBag.ProductList = new MultiSelectList(product, "product_id", "p_name");
            ViewBag.CategoryList = new MultiSelectList(categories, "category_id", "cat_name");
            var brant = db.manufacturer_tbl.ToList();
            ViewBag.branList = new SelectList(brant, "manufacturer_id", "brand_name");
            return View();
        }


        public string variantlist()
        {
            string r = "";
            var s = db.variants.ToList();
            foreach (var vL in s)
            {
                r += "<button type='button' class='btn btn-dark btn-block' onclick='changeView(this, " + vL.varient_id + ", \"" + vL.varient_name + "\")' >" + vL.varient_name + "</button>";
            }
            return r;
        }

        #region
        ////this is requarcive function for creating category
        //[NonAction]
        //private string BuildCategoriesMap(List<Category> categories)
        //{

        //    var map = "";
        //    if (categories.Count > 0)
        //    {
        //        map += "";
        //        foreach (Category cat in categories)
        //        {
        //            if ((!cat.CategoryId.HasValue) || (cat.CategoryId.HasValue && (!TakenIds.Contains(cat.CategoryId))))

        //            {


        //                map += "<option " + cat.CategoryId + ">" + cat.CategoryName;
        //                List<Category> subCats = _categories.Where(c => c.ParentCategoryId == cat.CategoryId).ToList();
        //                map += BuildCategoriesMap(subCats);
        //                map += "</option>";
        //            }
        //            TakenIds.Add(cat.CategoryId);
        //        }
        //        //map += "</tr>";
        //    }

        //    return map;
        //}
        ////end requrciv function 
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(product proVMs)
        {

            //product 
            product_tbl proTB = new product_tbl();
            //imgae
            product_image_tbl imgTB = new product_image_tbl();
            //related
            product_related_tbl relTB = new product_related_tbl();
            //category
            product_to_category_tbl catTB = new product_to_category_tbl();
            //product variant_value
            product_variant_value pvv = new product_variant_value();
            //product variant
            product_variant_tbl pv = new product_variant_tbl();

            if (ModelState.IsValid)
            {
                //test = test.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                proTB.p_name = proVMs.p_name;
                proTB.price = proVMs.price;
                proTB.quantity = proVMs.quantity;
                proTB.status = 1;
                proTB.Entry_by = User.Identity.GetUserId();
                proTB.date_added = DateTime.Now;
                proTB.discription = proVMs.discription;
                proTB.VideoLink = proVMs.videoLink;
                proTB.manufacturer_id = proVMs.manufacturer_id;
                db.product_tbl.Add(proTB);
                db.SaveChanges();

                var p_Id = db.product_tbl.Find(proTB.product_id);

                //product variant
                if (proVMs.varient_id != null)
                {
                    int j = 0;
                    foreach (var p in proVMs.varient_id)
                    {
                        pv.product_id = p_Id.product_id;
                        pv.Variant_id = p.Value;
                        db.product_variant_tbl.Add(pv);
                        db.SaveChanges();
                        var pv_id = db.product_variant_tbl.Find(pv.id);

                        var vs = db.variants.Find(p.Value);
                        if (proVMs.variant_unit_id != null)
                        {

                            int i = 0;
                            foreach (var psv in proVMs.variant_unit_id)
                            {
                                var v = db.variant_unit.Find(psv.Value);

                                if (v.variant_id == p.Value)
                                {


                                    pvv.product_id = p_Id.product_id;
                                    pvv.pv_Id = pv_id.id;
                                    pvv.variant_unit_id = Convert.ToInt32(proVMs.variant_unit_id[j].Value);
                                    pvv.price = Convert.ToInt32(proVMs.pVprice[j].Value);
                                    pvv.quentity = Convert.ToInt32(proVMs.quentity[j].Value);
                                    pvv.variant_unit_name = v.unit_name;
                                    pvv.variant_id = pv_id.Variant_id;
                                    pvv.variant_name = vs.varient_name;
                                    db.product_variant_value.Add(pvv);
                                    j++;
                                    db.SaveChanges();
                                }




                                i++;
                            }


                        }
                    }
                }



                try

                {
                    fileName = Path.GetFileNameWithoutExtension(proVMs.mainImage.FileName);
                    extension = Path.GetExtension(proVMs.mainImage.FileName);
                    fileName = "P" + p_Id.product_id + Guid.NewGuid() + extension;
                    fileName = fileName.Replace(" ", "-");
                    path = fileName;





                    string paths = System.IO.Path.Combine(Server.MapPath("~/AppFiles/Product/"), path);
                    proVMs.mainImage.SaveAs(paths);
                    ResizeSettings resizeSetting = new ResizeSettings
                    {
                        Width = 213,
                        Height = 213,

                    };
                    ImageBuilder.Current.Build(paths, paths, resizeSetting);
                    proTB.main_image = path;
                    db.SaveChanges();


                   
                }
                catch (Exception e)
                {

                }



                //save category table 
                if (proVMs.CategoryId != null)
                {
                    foreach (var cat in proVMs.CategoryId)
                    {
                        catTB.product_id = p_Id.product_id;
                        catTB.category_id = cat.Value;
                        db.product_to_category_tbl.Add(catTB);
                        db.SaveChanges();
                    }


                }

                if (proVMs.related_id != null)
                {
                    //save related table 
                    foreach (var rel in proVMs.related_id)
                    {
                        relTB.product_ID = p_Id.product_id;
                        relTB.releted_product_ID = rel.Value;
                        db.product_related_tbl.Add(relTB);
                        db.SaveChanges();
                    }


                }


                if (proVMs.file != null)
                {
                    foreach (var img in proVMs.file)
                    {
                        try
                        {
                            fileName = Path.GetFileNameWithoutExtension(img.FileName);
                            extension = Path.GetExtension(img.FileName);
                            fileName = "P" + p_Id.product_id + Guid.NewGuid() + extension;
                            fileName = fileName.Replace(" ", "-");
                            path = fileName;

                            //savePath = Server.MapPath("~/AppFiles/Product/");
                            //img.SaveAs(Path.Combine(savePath, fileName));

                            string paths = System.IO.Path.Combine(Server.MapPath("~/AppFiles/Product/"), path);
                            img.SaveAs(paths);
                            ResizeSettings resizeSetting = new ResizeSettings
                            {
                                Width = 213,
                                Height = 213,

                            };
                            ImageBuilder.Current.Build(paths, paths, resizeSetting);
                            

                            imgTB.product_id = p_Id.product_id;
                            imgTB.image = path;
                            db.product_image_tbl.Add(imgTB);
                            db.SaveChanges();


                        }
                        catch (Exception e)
                        {

                        }


                    }
                }



                message = "Product Created Success Fully";
                status = true;
            }
            else
            {
                message = "There Are Some Problem, Please try agine";
            }

            ViewBag.Message = message;
            ViewBag.Status = status;
            ViewBag.variantlist = variantlist();
            var categories = db.category_tbl.Where(p => p.status == true).ToList();
            var product = db.product_tbl.Where(p => p.status == 1).ToList();
            ViewBag.ProductList = new MultiSelectList(product, "product_id", "p_name");
            ViewBag.CategoryList = new MultiSelectList(categories, "category_id", "cat_name");
            var brant = db.manufacturer_tbl.ToList();
            ViewBag.branList = new SelectList(brant, "manufacturer_id", "brand_name");
            return View();

        }


        public ActionResult Index()
        {

            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];
            var p = db.product_tbl.OrderByDescending(a => a.product_id).ToList();
            return View(p);
        }

        // GET: access/manageProducts/Delete/5

        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                message = "This is  Bad Requst";
            }
            product_tbl pro = db.product_tbl.Find(id);
            if (pro == null)
            {
                message = "Product Is Not Found";
            }
            else
            {
                //delete main image
                path = Server.MapPath("~/AppFiles/Product/" + pro.main_image);
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


                db.product_tbl.Remove(pro);
                db.SaveChanges();

                //delete variant
                db.product_variant_tbl.RemoveRange(db.product_variant_tbl.Where(p => p.product_id == id));
                db.SaveChanges();

                //delete variant product variant value
                db.product_variant_value.RemoveRange(db.product_variant_value.Where(v => v.product_id == id));
                db.SaveChanges();

                //delete related product
                db.product_related_tbl.RemoveRange(db.product_related_tbl.Where(b => b.product_ID == id));
                db.SaveChanges();

                //delete product category
                db.product_to_category_tbl.RemoveRange(db.product_to_category_tbl.Where(a => a.product_id == id));
                db.SaveChanges();

                //delete image gallery

                var imgs = db.product_image_tbl.Where(i => i.product_id == id).ToList();
                foreach (var img in imgs)
                {
                    path = Server.MapPath("~/AppFiles/Product/" + img.image);
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
                    db.product_image_tbl.Remove(img);
                    db.SaveChanges();
                }

                message = "Product Content is Deleted";
                status = true;

            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("Index");
        }

        //variant list for edit
        public string variantlistforEdit(int id)
        {
            string r = "";

            var s = db.variants.ToList();
            var pvv = db.product_variant_value.Where(a => a.product_id == id).ToList();
            foreach (var vL in s)
            {
                r += "<button type='button' class='btn btn-block " + (pvv.Any(a => a.variant_id == vL.varient_id) ? "btn-success" : "btn-dark") + "' onclick='changeView(this, " + vL.varient_id + ", \"" + vL.varient_name + "\")' >" + vL.varient_name + "</button>";
            }
            return r;
        }
        //Edit product
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var proVM = new product();
            if (id == null)
            {
                message = "This is Bad Requst";
            }
            else
            {
                product_tbl proV = db.product_tbl.Find(id);
                if (proV == null)
                {
                    message = "Product Is Not Found";
                }
                else
                {
                    proVM.product_id = proV.product_id;
                    proVM.price = proV.price;
                    proVM.p_name = proV.p_name;
                    proVM.quantity = proV.quantity;
                    proVM.main_image = proV.main_image;
                    proVM.discription = proV.discription;
                    proVM.videoLink = proVM.videoLink;


                    ViewBag.variantList = variantlistforEdit(proV.product_id);
                    ViewBag.vValue = vValue(Convert.ToInt32(id));
                    ViewBag.img = showGallery(Convert.ToInt32(id));
                    ViewBag.cat = editCat(Convert.ToInt32(id));
                    ViewBag.pro = editRelated(Convert.ToInt32(id));
                    ViewBag.brand = editBrand(Convert.ToInt32(id));

                    ViewBag.Message = TempData["Message"];
                    ViewBag.Status = TempData["Status"];
                    return View(proVM);
                }


            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("Index");
        }

        //show variant value 
        public string vValue(int id)
        {
            string tr = "";
            var s = db.product_variant_value.Where(a => a.product_id == id);

            var va = db.product_variant_tbl.Where(a => a.product_id == id);

            foreach (var v in va)
            {
                var vri = db.variants.Find(v.Variant_id);
                tr += "<div class='section-content m-0'>" +
            "" +

              //"<button class='content-collapse' data-toggle='tooltip' data-placement='top' data-original-title='Collapse'><i class='fa fa-angle-down'></i></button>" +
              // "<button class='content-close' data-toggle='tooltip' data-placement='top' data-original-title='Close'><i class='fa fa-close'></i></button>" +
              "<div class='content-details show'><div id='appendVarientUnit" + vri.varient_id + "'><h4 class='content-title'>" + vri.varient_name + "</h4><table id='veTable" + vri.varient_id + "' class='table' ><input type='hidden' name='varient_id' value='" + vri.varient_id + "' />";

                foreach (var value in s)
                {

                    if (vri.varient_id == value.variant_id)
                    {
                        tr += "<tr id='ar" + value.variant_unit_id + "'>" +
                               "<th><select name='variant_unit_id' class='form-control'>";

                        var ss = db.variant_unit.Where(a => a.variant_id == value.variant_id).ToList();

                        foreach (var st in ss)
                        {
                            tr += "<option " + (st.variant_unit_id == value.variant_unit_id ? "selected" : "") + " value='" + st.variant_unit_id + "'>" + st.unit_name + "</option>";
                        }
                        //<input class='form-control' type='text' readonly name='unit_name' value='" + value.variant_unit_name + "' />

                        tr += "</select></th>" +
                                   "<td><input class='form-control' type='text' placeholder='Quentity' value='" + value.quentity + "' name='quentity' /></td>" +
                                   "<td><input class='form-control' type='text' placeholder='+/- Price' value='" + value.price + "' name='pVprice' /></td>" +
                                   "<td><button class='btn btn-danger removeVfrom' type='button'><i class='fa fa-trash'></i></button> </td>" +
                                           "</tr>";
                    }

                }
                tr += "</table><button class=\"btn btn-success\" type=\"button\" onclick=\"addRow('#veTable" + v.Variant_id + "')\"  ><i class=\"fa fa-edit\"></i></button></div></div>";

            }


            return tr;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(product pro)

        {
            //related
            product_related_tbl relTB = new product_related_tbl();
            //category
            product_to_category_tbl catTB = new product_to_category_tbl();
            //product table
            product_tbl proTB = new product_tbl();
            //product image
            product_image_tbl imgTB = new product_image_tbl();
            //product variant_value
            product_variant_value pvv = new product_variant_value();
            //product variant
            product_variant_tbl pv = new product_variant_tbl();

            if (ModelState.IsValid)
            {
                if (pro.mainImage != null)
                {
                    path = Server.MapPath("~/AppFiles/Product/" + pro.main_image);
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


                    fileName = Path.GetFileNameWithoutExtension(pro.mainImage.FileName);
                    extension = Path.GetExtension(pro.mainImage.FileName);
                    fileName = "P" + pro.product_id + Guid.NewGuid() + extension;
                    fileName = fileName.Replace(" ", "-");
                    path = fileName;

                    
                    string paths = System.IO.Path.Combine(Server.MapPath("~/AppFiles/Product/"), path);
                    pro.mainImage.SaveAs(paths);
                    ResizeSettings resizeSetting = new ResizeSettings
                    {
                        Width = 213,
                        Height = 213,
                        
                    };
                    ImageBuilder.Current.Build(paths, paths, resizeSetting);
                    proTB.main_image = path;
                }
                else
                {
                    proTB.main_image = pro.main_image;
                }
                proTB.product_id = pro.product_id;
                proTB.p_name = pro.p_name;
                proTB.price = pro.price;
                proTB.quantity = pro.quantity;
                proTB.status = 1;
                proTB.Entry_by = User.Identity.GetUserId();
                proTB.date_added = DateTime.Now;
                proTB.discription = pro.discription;
                proTB.VideoLink = pro.videoLink;
                proTB.manufacturer_id = pro.manufacturer_id;
                db.Entry(proTB).State = EntityState.Modified;
                db.SaveChanges();



                //product variant
                if (pro.varient_id != null)
                {

                    //delete all product variant related data
                    var v1 = db.product_variant_tbl.Where(a => a.product_id == pro.product_id).ToList();
                    foreach (var s in v1)
                    {
                        db.product_variant_tbl.Remove(s);
                        db.SaveChanges();
                    }
                    //delete product farinat value
                    var pv2 = db.product_variant_value.Where(a => a.product_id == pro.product_id).ToList();
                    foreach (var t in pv2)
                    {
                        db.product_variant_value.Remove(t);
                        db.SaveChanges();
                    }
                    //add new data
                    int j = 0;
                    foreach (var p in pro.varient_id)
                    {
                        pv.product_id = pro.product_id;
                        pv.Variant_id = p.Value;
                        db.product_variant_tbl.Add(pv);
                        db.SaveChanges();
                        var pv_id = db.product_variant_tbl.Find(pv.id);

                        var vs = db.variants.Find(p.Value);
                        if (pro.variant_unit_id != null)
                        {


                            // addd edit variant value
                            int i = 0;
                            foreach (var psv in pro.variant_unit_id)
                            {
                                var v = db.variant_unit.Find(psv.Value);

                                if (v.variant_id == p.Value)
                                {


                                    pvv.product_id = pro.product_id;
                                    pvv.pv_Id = pv_id.id;
                                    pvv.variant_unit_id = Convert.ToInt32(pro.variant_unit_id[j].Value);
                                    pvv.price = Convert.ToInt32(pro.pVprice[j].Value);
                                    pvv.quentity = Convert.ToInt32(pro.quentity[j].Value);
                                    pvv.variant_unit_name = v.unit_name;
                                    pvv.variant_id = pv_id.Variant_id;
                                    pvv.variant_name = vs.varient_name;
                                    db.product_variant_value.Add(pvv);
                                    j++;
                                    db.SaveChanges();
                                }




                                i++;
                            }


                        }
                    }
                }
                else
                {
                    //delete all product variant related data
                    var v1 = db.product_variant_tbl.Where(a => a.product_id == pro.product_id).ToList();
                    foreach (var s in v1)
                    {
                        db.product_variant_tbl.Remove(s);
                        db.SaveChanges();
                    }
                    //delete product farinat value
                    var pv2 = db.product_variant_value.Where(a => a.product_id == pro.product_id).ToList();
                    foreach (var t in pv2)
                    {
                        db.product_variant_value.Remove(t);
                        db.SaveChanges();
                    }
                }





                //save category table 
                if (pro.CategoryId != null)
                {
                    var pCat = db.product_to_category_tbl.Where(a => a.product_id == pro.product_id).ToList();
                    foreach (var cat in pCat)
                    {
                        db.product_to_category_tbl.Remove(cat);
                        db.SaveChanges();
                    }

                    foreach (var cat in pro.CategoryId)
                    {
                        catTB.product_id = pro.product_id;
                        catTB.category_id = cat.Value;
                        db.product_to_category_tbl.Add(catTB);
                        db.SaveChanges();
                    }
                }

                if (pro.related_id != null)
                {
                    var rel = db.product_related_tbl.Where(a => a.product_ID == pro.product_id).ToList();
                    foreach (var r in rel)
                    {
                        db.product_related_tbl.Remove(r);
                        db.SaveChanges();
                    }

                    //save related table 
                    foreach (var re in pro.related_id)
                    {
                        relTB.product_ID = pro.product_id;
                        relTB.releted_product_ID = re.Value;
                        db.product_related_tbl.Add(relTB);
                        db.SaveChanges();
                    }
                }
                if (pro.file != null)
                {
                    foreach (var img in pro.file)
                    {
                        try
                        {
                            fileName = Path.GetFileNameWithoutExtension(img.FileName);
                            extension = Path.GetExtension(img.FileName);
                            fileName = "P" + pro.id + Guid.NewGuid() + extension;
                            fileName = fileName.Replace(" ", "-");
                            path = fileName;

                            string paths = System.IO.Path.Combine(Server.MapPath("~/AppFiles/Product/"), path);
                            img.SaveAs(paths);
                            ResizeSettings resizeSetting = new ResizeSettings
                            {
                                Width = 213,
                                Height = 213,

                            };
                            ImageBuilder.Current.Build(paths, paths, resizeSetting);

                            imgTB.product_id = pro.id;
                            imgTB.image = path;
                            db.product_image_tbl.Add(imgTB);
                            db.SaveChanges();
                           
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                message = "Product is Updated";
                status = true;
                TempData["Message"] = message;
                TempData["Status"] = status;
                return RedirectToAction("Edit", new
                {
                    pro.id,
                });
            }
            message = "There are some problems";
            TempData["Message"] = message;

            return RedirectToAction("Edit", new
            {
                id = pro.id,
            });
        }


        public string showGallery(int id)
        {
            var img = db.product_image_tbl.Where(i => i.product_id == id).ToList();
            foreach (var i in img)
            {
                //var path = HttpContext.Server.MapPath("\\AppFiles\\Product\\" + i.image);

                imgTr += "<div class='col-sm-4' id='tr_" + i.product_image_id + "'>" +
                    "<div class='card'>" +
                    "<img class='card-img-top' src ='../../../AppFiles/Product/" + i.image + "'>" +
                    "<div class='card-body'><p class='card-footer'>" +
                    "<a  onclick='deleteGallery(" + i.product_image_id + ")' class='btn btn-outline-danger'>" +
                    "<i class='pe-7s-trash'></i>" +
                    "</a>" +
                    "</p>" +
                    "</div>" +
                    "</div>" +
                    "</div>";



            }
            return imgTr;
        }
        //edit product Gallery
        [HttpGet]
        public ActionResult editGallery(int? id)
        {
            if (id == null)
            {
                message = "This is Bad Requst";
                TempData["Message"] = message;
                return RedirectToAction("Edit");
            }
            else
            {
                //product_image_tbl gallery = db.product_image_tbl.Find(id);
                var gallery = db.product_image_tbl.Where(i => i.product_id == id).ToList();
                if (gallery == null)
                {
                    message = "Gallery Is Not Found";
                    TempData["Message"] = message;
                    return RedirectToAction("Edit");
                }
                else
                {
                    ViewBag.id = id;
                    return PartialView(gallery);
                }
            }
        }

        [HttpPost]
        //delete galery image
        public JsonResult deleteGallery(int[] id)
        {

            if (id == null)
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
            else
            {

                foreach (var img in id)
                {
                    var data = db.product_image_tbl.Find(img);
                    path = Server.MapPath("~/AppFiles/Product/" + data.image);
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
                    db.product_image_tbl.Remove(data);
                    db.SaveChanges();
                }
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        //addGalleryImage
        [HttpGet]
        public ActionResult addGalleryImage(int? product_id)
        {
            product pro = new product();
            pro.product_id = Convert.ToInt32(product_id);
            return PartialView(pro);
        }

        //addGalleryImage HttpPost
        [HttpPost]
        public ActionResult addGalleryImage(product pro)
        {
            product_image_tbl imgTB = new product_image_tbl();
            if (pro.file != null)
            {
                foreach (var img in pro.file)
                {
                    try
                    {
                        fileName = Path.GetFileNameWithoutExtension(img.FileName);
                        extension = Path.GetExtension(img.FileName);
                        fileName = "P" + pro.id + Guid.NewGuid() + extension;
                        fileName = fileName.Replace(" ", "-");
                        path = fileName;

                        string paths = System.IO.Path.Combine(Server.MapPath("~/AppFiles/Product/"), path);
                        img.SaveAs(paths);
                        ResizeSettings resizeSetting = new ResizeSettings
                        {
                            Width = 213,
                            Height = 213,

                        };
                        ImageBuilder.Current.Build(paths, paths, resizeSetting);

                        imgTB.product_id = pro.id;
                        imgTB.image = path;
                        db.product_image_tbl.Add(imgTB);
                        db.SaveChanges();
                       
                    }
                    catch (Exception e)
                    {

                    }


                }
                message = "Product Gallery Is Updated";
                status = true;
            }
            else
            {
                message = "Product Gallery Is Not Selected So Gallery Is Not Updated";
            }

            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("Edit", new { @id = pro.id });
        }

        /// editRelate function
        [NonAction]
        public string editRelated(int id)
        {

            var rel = db.product_related_tbl.Where(a => a.product_ID == id).Select(i => i.releted_product_ID).ToList();
            var allPro = db.product_tbl.ToList();



            foreach (var a in allPro)
            {
                if (rel.Contains(a.product_id))
                {
                    op += "<option selected value=" + a.product_id + ">" + a.p_name + "</option>";
                }
                else
                {
                    op += "<option  value=" + a.product_id + ">" + a.p_name + "</option>";

                }
            }

            return op;
        }


        ///editCat
        [NonAction]
        public string editCat(int id)
        {
            var pCat = db.product_to_category_tbl.Where(a => a.product_id == id).Select(i => i.category_id).ToList();
            var allCat = db.category_tbl.ToList();

            foreach (var c in allCat)
            {
                if (pCat.Contains(c.category_id))
                {
                    cp += "<option selected value=" + c.category_id + ">" + c.cat_name + "</option>";
                }
                else
                {
                    cp += "<option  value=" + c.category_id + ">" + c.cat_name + "</option>";

                }
            }

            return cp;
        }


        ///editCat
        [NonAction]
        public string editBrand(int id)
        {
            string b = "";
            var manu = db.manufacturer_tbl.ToList();
            var pm = db.product_tbl.FirstOrDefault(a => a.product_id == id);
            foreach (var c in manu)
            {
                if (pm.manufacturer_id == c.manufacturer_id)
                {
                    b += "<option selected value=" + c.manufacturer_id + ">" + c.brand_name + "</option>";
                }
                else
                {
                    b += "<option  value=" + c.manufacturer_id + ">" + c.brand_name + "</option>";

                }
            }

            return b;
        }



        #region //work with variant
        [HttpPost]
        public JsonResult getUnit(int? id)
        {
            if (id != null)
            {
                var vu = db.variant_unit.Where(a => a.variant_id == id).ToList();
                return Json(new { list = vu }, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }



        #endregion


        public JsonResult multiDelete(productID proo)
        {
            if (proo != null)
            {

                foreach (var id in proo.proIds)
                {

                    if (id == null)
                    {
                        message = "This is  Bad Requst";
                    }
                    product_tbl pro = db.product_tbl.Find(id);
                    if (pro == null)
                    {
                        message = "Product Is Not Found";
                    }
                    else
                    {
                        //delete main image
                        path = Server.MapPath("~/AppFiles/Product/" + pro.main_image);
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


                        db.product_tbl.Remove(pro);
                        db.SaveChanges();

                        //delete variant
                        db.product_variant_tbl.RemoveRange(db.product_variant_tbl.Where(p => p.product_id == id));
                        db.SaveChanges();

                        //delete variant product variant value
                        db.product_variant_value.RemoveRange(db.product_variant_value.Where(v => v.product_id == id));
                        db.SaveChanges();

                        //delete related product
                        db.product_related_tbl.RemoveRange(db.product_related_tbl.Where(b => b.product_ID == id));
                        db.SaveChanges();

                        //delete product category
                        db.product_to_category_tbl.RemoveRange(db.product_to_category_tbl.Where(a => a.product_id == id));
                        db.SaveChanges();

                        //delete image gallery

                        var imgs = db.product_image_tbl.Where(i => i.product_id == id).ToList();
                        foreach (var img in imgs)
                        {
                            path = Server.MapPath("~/AppFiles/Product/" + img.image);
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
                            db.product_image_tbl.Remove(img);
                            db.SaveChanges();
                        }
                    }
                }
            }

            return Json(JsonRequestBehavior.AllowGet);

        }



        public class productID
        {
            public int[] proIds { get; set; }
        }


     
    }
}

