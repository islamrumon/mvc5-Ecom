
using backEnd.DataModel;
using backEnd.Models.viewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace abcShop_auth.Areas.access.Controllers
{
    [Authorize]
    public class settingsController : Controller
    {

        int i = 0;
        string message = "";
        bool status = false;

        public abcShopEntities db = new abcShopEntities();
        // GET: access/settings

        public ActionResult payment()
        {
            return View();
        }

        #region //this is site general setup
        [HttpGet]
        public ActionResult general()
        {

            var g = db.site_setting_tbl.Find(1);
            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            ViewBag.url = Request.Url.GetLeftPart(UriPartial.Authority);
            return View(g);
        }

        [HttpPost]
        public ActionResult general(site_setting_tbl gt, HttpPostedFileBase file)
        {
            string surl = Request.Url.GetLeftPart(UriPartial.Authority);
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string path = Server.MapPath(gt.logo);
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

                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    fileName = "L" + gt.siteName + extension;
                    fileName = fileName.Replace(" ", "-");

                    path = "/AppFiles/" + fileName;

                    string savePath = Server.MapPath("~/AppFiles/");
                    file.SaveAs(Path.Combine(savePath, fileName));
                    gt.logo = path;



                }

                db.Entry(gt).State = EntityState.Modified;
                db.SaveChanges();
                message = "General Setup Updated Successfully";
                status = true;

            }
            else
            {
                message = "There are some Problems. Try Again";
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("general");

        }


        #endregion

      
        #region //this is order status setup
        [HttpGet]
        public ActionResult orderStatus()
        {

            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            ViewBag.osList = statusList();
            return View();
        }

        //list function
        public string statusList()
        {
            string tr = "";
            var o = db.order_status_tbl.OrderByDescending(a => a.order_status_id).ToList();

            foreach (var s in o)
            {
                i++;
                tr += "<tr id='tr_" + s.order_status_id + "'>" +
                   "<td>" + i + "</td><td>" + s.name + "</td><td>" +
                  "<a href='javescript:void()'  type ='button' class='btn radius btn-sm btn-primary '  onclick='editOrderS(" + s.order_status_id+ ")'><i class='fa fa-edit'></i></a> | " +
                 "<a href ='javescript:void()' type='button' class='btn radius btn-sm btn-danger'  onclick='deleteOrderS(" + s.order_status_id+ ")'><i class='pe-7s-trash'></i></a></td></tr>";
            }
            return tr;
        }

        [HttpPost]
        public JsonResult orderStatus(order_status_tbl os)
        {
            
                db.order_status_tbl.Add(os);
                db.SaveChanges();
                var s = db.order_status_tbl.Find(os.order_status_id);
                return Json(s, JsonRequestBehavior.AllowGet);
         
        }

        [HttpPost]
        public JsonResult deleteOrderS(int? id)
        {
            if (id !=null)
            {
                var v = db.order_status_tbl.Find(id);
                db.order_status_tbl.Remove(v);
                db.SaveChanges();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult editOrderS(int? id)
        {
            if (id != null)
            {
                var i = db.order_status_tbl.Find(id);
                return Json(i,JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult editOrderS(order_status_tbl os)
        {
            if (os != null)
            {
                db.Entry(os).State = EntityState.Modified;
                db.SaveChanges();
                return Json(os,JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
            
        }
        #endregion

        #region //This is All Variant Setup
        //this is for variant controller
        [HttpGet]
        public ActionResult variants()
        {

            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            ViewBag.variantList = showvarintList();
            return View();
        }
         
        [HttpPost]
        public ActionResult variants(variant_vm VM)
        {
            variant v = new variant();
            variant_unit vu = new variant_unit();
            if (ModelState.IsValid)
            {

                v.varient_name = VM.variant_name;
                v.type =Convert.ToString(VM.type);
                db.variants.Add(v);
                db.SaveChanges();

                var id = db.variants.Find(v.varient_id);

                if (VM.unit_name != null)
                {
                   foreach(var s in VM.unit_name)
                    {
                        vu.variant_id = Convert.ToInt32(id.varient_id);
                        vu.unit_name = s;
                        
                        db.variant_unit.Add(vu);
                        db.SaveChanges();
                    }
                }
                message = "Variant setup successfully";
                status = true;

            }
            else
            {
                message = "There are some problems";
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("variants");
        }
        //save variant name
        
        public string showvarintList()
        {
            string tr = "";
            var v = db.variants.OrderByDescending(a => a.varient_id).ToList();
            foreach (var t in v)
            {
                i++;
                tr += "<tr id='tr_" + t.varient_id + "'>" +
                    "<td>" + i + "</td><td>" + t.varient_name + "</td><td>"+vUnitList(t.varient_id)+ "</td><td>" +
                   "<a href='javescript:void()'  type ='button' class='btn radius btn-sm btn-primary '  onclick='editVariant(" + t.varient_id + ")'><i class='fa fa-edit'></i></a> | " +
                  "<a href ='javescript:void()' type='button' class='btn radius btn-sm btn-danger'  onclick='deleteVariant(" + t.varient_id + ")'><i class='pe-7s-trash'></i></a></td></tr>";
            }

            return tr;
        }

        public string vUnitList(int id)
        {
            string s = " ";
            var g = db.variant_unit.Where(a=>a.variant_id == id).ToList();
            foreach (var t in g)
            {
                s += "<span class='badge badge-pill badge-success'>" + t.unit_name+"</span>";
            }
            return s;
        }
            
        //delete varaint
        [HttpPost]
        public JsonResult deleteVarirant(int? id)
        {
            
            if (id == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var vr = db.variants.Find(id);
                db.variants.Remove(vr);
                db.SaveChanges();

                var vu = db.variant_unit.Where(i => i.variant_id == id).ToList();
                foreach(var s in vu)
                {
                    var f = db.variant_unit.Find(s.variant_unit_id);
                    db.variant_unit.Remove(f);
                    db.SaveChanges();
                }
                return Json(true , JsonRequestBehavior.AllowGet);
            }
        }

        
       [HttpGet]
       public ActionResult variantEdit(int? id)
        {
            
            variant_vm vm = new variant_vm();
            if (id != null)
            {
               variant v = db.variants.Find(id);
                if ( v != null)
                {
                    vm.varient_id = v.varient_id;
                    vm.variant_name = v.varient_name;
                    vm.type = v.type;
                    ViewBag.vunitList = vunitList(Convert.ToInt32(v.varient_id));
                    return PartialView(vm);
                }
                else
                {
                    message = "Varant is not Found";
                }
            }
            else
            {
                message = "There are some problem";
            }

            TempData["Message"] = message;
            return RedirectToAction("variants");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult variantEdit(variant_vm vm)
        {
            variant v = new variant();
            variant_unit vu = new variant_unit();
            if (ModelState.IsValid)
            {
                v.varient_id = vm.varient_id;
                v.varient_name = vm.variant_name;
                v.type =Convert.ToString( vm.type);
                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();


                //delete the variant unit
                var de = db.variant_unit.Where(i=>i.variant_id == vm.varient_id).ToList();
                if (de != null)
                {
                    foreach (var s in de)
                    {
                        var f = db.variant_unit.Find(s.variant_unit_id);
                        db.variant_unit.Remove(f);
                        db.SaveChanges();
                    }
                }
                if(vm.unit_name != null)
                {
                    foreach (var s in vm.unit_name)
                    {
                        vu.variant_id = Convert.ToInt32(vm.varient_id);
                        vu.unit_name = s;
                        db.variant_unit.Add(vu);
                        db.SaveChanges();
                    }
                }

                message = "Variant updated Successfully";
                status = true;
                
            }
            else
            {
                message = "There Have Some Problems";
                
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("variants");

        }

        
        public string vunitList(int id)
        {

            string h = "";
            var u = db.variant_unit.Where(a => a.variant_id == id).ToList();
            foreach (var i in u)
            {
           

                h += "<div class='input-group mb-3' id='area" + i.variant_unit_id + "'>" +
                "<input class='form-control' placeholder='Type Varient Value' type='text[' name='unit_name' id='unit_name" + i.variant_unit_id + "' value=" + i.unit_name + "  />" +
                "<div class='input-group-append'>" +
                "<a href='javascript:void(0)' class='btn btn-danger' onclick='deleteInputForm1(" + i.variant_unit_id + ")'><i class='pe-7s-trash'></i></a>" +
                "</div></div>";
            }

            
            return h;


        }
        #endregion


        #region//This is All ManuFacture setup

        [HttpGet]
        public ActionResult manufacture()
        {

            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];
            ViewBag.brandList = mfList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult manufacture(manufacture_vm mf)
        {
            string path;
            if (ModelState.IsValid)
            {
                manufacturer_tbl mt = new manufacturer_tbl();

                mt.brand_name = mf.brand_name;
                mt.sort_order = mf.sort_order;
                db.manufacturer_tbl.Add(mt);
                db.SaveChanges();

                var id = db.manufacturer_tbl.Find(mt.manufacturer_id);

                //save the logo

                if (mf.file != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(mf.file.FileName);
                    string extension = Path.GetExtension(mf.file.FileName);
                    fileName = "L" + id.manufacturer_id + extension;
                    fileName = fileName.Replace(" ", "-");

                    path = fileName;

                    string savePath = Server.MapPath("~/AppFiles/manufacturer/");
                    mf.file.SaveAs(Path.Combine(savePath, fileName));
                    mt.logo = path;
                    db.SaveChanges();
                }

                message = "Menufacture  Create Successfully";
                status = true;
            }
            else
            {
                message = "There have Some Problems";
            }

            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("manufacture");
        }


        //show tabli list
        public string mfList()
        {
            string surl = Request.Url.GetLeftPart(UriPartial.Authority);
            string mf = "";
            var m = db.manufacturer_tbl.OrderByDescending(a=>a.manufacturer_id).ToList();
            foreach (var t in m)
            {
                i++;
                mf+= "<tr id='tr_" + t.manufacturer_id + "'>" +
                                      "<td>" + i + "</td><td>" + t.brand_name + "</td> <td><img width='140' height='100' class='img-thumbnail' src='"+ surl + "/AppFiles/manufacturer/" + t.logo + "'></td><td>" +

                                          "<a href='javescript:void()'  type ='button' class='btn radius btn-sm btn-primary '  onclick='editmanufacturer(" + t.manufacturer_id + ")'><i class='fa fa-edit'></i></a> | " +
                                         "<a href ='javescript:void()' type='button' class='btn radius btn-sm btn-danger'  onclick='deletemanufacturer(" + t.manufacturer_id + ")'><i class='pe-7s-trash'></i></a></td></tr>";
            }

            return mf;
        }

        //this is delete action
        public JsonResult deletemanufacturer(int? id)
        {
            string path = "";
            var m = db.manufacturer_tbl.Find(id);
            path = Server.MapPath("~/AppFiles/manufacturer/" + m.logo);
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
            db.manufacturer_tbl.Remove(m);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        //this is for edit action
        [HttpGet]
        public ActionResult editmanufacturer(int? id)
        {
            string surl = Request.Url.GetLeftPart(UriPartial.Authority);
            manufacture_vm mv = new manufacture_vm();
            if (id != null)
            {
                var m = db.manufacturer_tbl.Find(id);
                mv.brand_name = m.brand_name;
                mv.logo = m.logo;
                mv.manufacturer_id = m.manufacturer_id;
                mv.sort_order = m.sort_order;
                ViewBag.url = surl;
                return PartialView(mv);
            }
            message = "There have Some Problem. Try agin";
            TempData["Message"] = message;
            return RedirectToAction("manufacture");
            //return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editmanufacturer(manufacture_vm mv)
        {
            string path = "";
            manufacturer_tbl mt = new manufacturer_tbl();
            if (ModelState.IsValid)
            {
                mt.brand_name = mv.brand_name;
                mt.manufacturer_id = mv.manufacturer_id;
                mt.sort_order = mv.sort_order;
                if (mv.file != null)
                {
                    path = Server.MapPath("~/AppFiles/manufacturer/" + mv.logo);
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
                   
                        string fileName = Path.GetFileNameWithoutExtension(mv.file.FileName);
                        string extension = Path.GetExtension(mv.file.FileName);
                        fileName = "L" + mv.manufacturer_id + extension;
                        fileName = fileName.Replace(" ", "-");

                        path = fileName;

                        string savePath = Server.MapPath("~/AppFiles/manufacturer/");
                        mv.file.SaveAs(Path.Combine(savePath, fileName));
                        mt.logo = path;
                        

                    
                }
                else
                {
                    mt.logo = mv.logo;
                }

                db.Entry(mt).State = EntityState.Modified;
                db.SaveChanges();
                message = "ManuFacturer Updated Successfully";
                status = true;
            }
            else
            {
                message = "There Are Some Problems, Try Again";
            }

            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("manufacture");
        }
        #endregion


        #region
        [HttpGet]
        public ActionResult wareHouse()
        {

            wareHouse_VM wvm = new wareHouse_VM();
            wvm.wareHosueList = db.Warehouse_tbl.OrderByDescending(a => a.warehouse_id).ToList();
            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            return View(wvm);
        }

        [HttpPost]
        public ActionResult wareHouse(wareHouse_VM wvm)
        {
            var wh = new Warehouse_tbl();
            if (ModelState.IsValid)
            {
                wh.created_at = DateTime.Now;
                wh.created_by = User.Identity.GetUserId();
                wh.w_name = wvm.name;
                wh.w_location = wvm.location;
                db.Warehouse_tbl.Add(wh);
                db.SaveChanges();

                status = true;
                message = "Ware House Created Successfully";
            }
            else
            {
                message = "Ther ARe somw Problem";
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("wareHouse");
        }

        //edit section
        [HttpGet]
        public ActionResult editwareHouse(int? id)
        {
            if (id != null)
            {
                var s = db.Warehouse_tbl.Find(id);
                return PartialView(s);
            }
            message = "There have Some provlem, Try Agien";
            TempData["Message"] = message;
            return RedirectToAction("wareHouse");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editwareHouse(Warehouse_tbl wh)
        {
            if (ModelState.IsValid)
            {
                wh.created_by = User.Identity.GetUserId();
                wh.updated_at = DateTime.Now;
                db.Entry(wh).State = EntityState.Modified;
                db.SaveChanges();

                message = "WareHouse Updated Successfully";
                status = true;
            }
            else
            {
                message = "There Are some Problems.Try Agien";
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("wareHouse");
        }

        /// <summary>
        /// for deletetd
        /// </summary>
        /// <returns></returns>
        public JsonResult deletewareHouse(int? id)
        {
            var s = db.Warehouse_tbl.Find(id);
            db.Warehouse_tbl.Remove(s);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region //this is shipping method

        [HttpGet]
        public ActionResult shippingMethod()
        {
            shippingMethod sm = new shippingMethod();

            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            sm.MethodList = db.payment_method_tbl.ToList();
            return View(sm);
            
        }

        //save new data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult shippingMethod(shippingMethod sm)
        {
            if (ModelState.IsValid)
            {
                payment_method_tbl s = new payment_method_tbl();
                s.amount = sm.amount;
                s.status = sm.status;
                s.title = sm.title;
                db.payment_method_tbl.Add(s);
                db.SaveChanges();

                message = "New Created Successfully";
                status = true;
            }
            else
            {
                message = "Ther Have Some problems";
            }

            TempData["Message"] = message;
            TempData["Stauts"] = status;
            return RedirectToAction("shippingMethod");
        }


        //edit Shipping Methiod
        [HttpGet]
        public ActionResult editSmethod(int? id)
        {
            if (id != null)
            {
                var s = db.payment_method_tbl.Find(id);
                return View(s);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editSmethod(payment_method_tbl sp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sp).State = EntityState.Modified;
                db.SaveChanges();
                message = "Updated Successfully";
                status = true;
            }
            else {
                message = "There have some problems";
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("shippingMethod");
        }


        #endregion


        #region //this is about us

        [HttpGet]
        public ActionResult aboutList() {

            var about = db.aboutTbls.Find(1);
            return View(about);
        }

        [HttpPost]

        public ActionResult aboutList(aboutTbl at) {

            if (ModelState.IsValid) {

                db.Entry(at).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(at);
        }
        #endregion

        #region //this is social site

        [HttpGet]
        public ActionResult social() {
            var socials = db.social_sites.Find(1);
            return View(socials);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult social(social_site ss) {
            if (ModelState.IsValid) {
                db.Entry(ss).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(ss);
        }

        #endregion
    }
}