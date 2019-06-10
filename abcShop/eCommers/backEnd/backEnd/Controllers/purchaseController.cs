using abcShop_auth.Areas.access.Controllers;
using backEnd.DataModel;
using backEnd.Models.viewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace backEnd.Controllers
{
    [Authorize]
    public class purchaseController : Controller
    {

        string message = "";
        bool status = false;
        private abcShopEntities db = new abcShopEntities();

        public ActionResult Index()
        {
            var p = db.purchases.OrderByDescending(a => a.purchase_id).ToList();
            List<purchaseList> pu = new List<purchaseList>();
            foreach (var t in p) {
                var s = db.Supplier_tbl.Find(t.supplier_id);
                var w = db.Warehouse_tbl.Find(t.warehouse_id);

                pu.Add(new purchaseList
                {
                purchase_id = t.purchase_id,
                sName = s.Name,
                WareHosueName = w.w_name,
                create_date = t.create_date,
                total_amount = t.total_amount,


            });   
            }
            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            return View(pu);
        }
        [HttpGet]
        public ActionResult create()
        {
            purchaseVM purchasevm = new purchaseVM();

            //supplier
            var suplier = db.Supplier_tbl.ToList();
            ViewBag.Suplier = new SelectList(suplier, "supplier_id", "Name");


            //Warehouse
            var warehouse = db.Warehouse_tbl.ToList();
            ViewBag.Warehouse = new SelectList(warehouse, "warehouse_id", "w_name");

            //list of purchase
            var product = db.product_tbl.Where(p => p.status == 1).ToList();
            ViewBag.ProductList = new MultiSelectList(product, "product_id", "p_name");

            
            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];

            return View();
        }


       

        //pass the table row
        [HttpPost]
        public JsonResult productRows(int? pID)
        {
            string td = "";
            //get product
            var t = db.product_tbl.Find(pID);

            td += "<tr id='tr_"+t.product_id+"'>"+
                  "<td> "+t.p_name+ " <input type='hidden' value='" + t.product_id+ "' name='product_id'/> </td>" +
                  "<td><input class='form-control' value='' name='bayPrice'/> </td>" +
                  "<td><input class='form-control' value='" + t.quantity + "' name='quantity'/> </td>" +
                  
                  "<td><input class='form-control' value='" + t.price + "' name='salePrice'/></td>" +
                  "<td><button onclick='removePro(" +t.product_id+")' type='button' class='close'>" +
                  "<span aria-hidden='true'> &times;</span>"+
                  "</button><button class='btn btn-primary' type='button' onclick='showVarint("+t.product_id+")'>Edit</button></td></tr>";

            return Json(td, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult create(purchaseVM pvm)
        {

            decimal total_amount = 0;
           
            if (pvm !=null && pvm.product_id.Count() != 0)
            {
                purchase p = new purchase();
                p.supplier_id = pvm.supplier_id;
                p.warehouse_id = pvm.warehouse_id;
                p.total_amount = pvm.total_amount;
                p.create_date = DateTime.Now;
                db.purchases.Add(p);
                db.SaveChanges();

                //get the purchas id
                var id = db.purchases.Find(p.purchase_id);

                //create purchase invoice
                id.pInv = "#PINV-" + id.purchase_id + id.supplier_id;
                //save all data in purchasse detiels page
                purchase_details purchaseDetails = new purchase_details();
                for(int i=0; i < pvm.product_id.Count(); i++)
                {
                    purchaseDetails.product_id = pvm.product_id[i];
                    purchaseDetails.quantity = pvm.quantity[i];
                    purchaseDetails.purchase_price = pvm.bayPrice[i];
                    purchaseDetails.sale_price = pvm.salePrice[i];
                    purchaseDetails.purchase_id = id.purchase_id;
                    purchaseDetails.sku = "SKU=" + purchaseDetails.product_id + "-" + purchaseDetails.purchase_id;
                    db.purchase_details.Add(purchaseDetails);
                    db.SaveChanges();

                    //samition the total price
                    total_amount += Convert.ToDecimal(purchaseDetails.purchase_price);
                    //update every product colams
                    var pro = db.product_tbl.Find(purchaseDetails.product_id);

                    //first create itemladger 
                    ItemladgerController ic = new ItemladgerController();
                    ic.ladgerItem(Convert.ToInt32(pro.product_id), Convert.ToInt32(purchaseDetails.quantity), "in", Convert.ToInt32(p.warehouse_id), Convert.ToString(id.pInv));

                    pro.quantity += purchaseDetails.quantity;
                    pro.price = purchaseDetails.sale_price;

                    db.Entry(pro).State = EntityState.Modified;
                    db.SaveChanges();

                }


                //save the total price in 
                id.total_amount = total_amount;
                db.Entry(id).State = EntityState.Modified;
                db.SaveChanges();
                
                
                message = " Purchase Created Is Successfully";
                status = true;

                TempData["Message"] = message;
                TempData["Status"] = status;

                return RedirectToAction("purchaseInvoice", new { id=id.purchase_id});

            }
            else
            {
                message = "There Are Some Problems";
            }

            TempData["Message"] = message;
            TempData["Status"] = status;

            return RedirectToAction("create");
        }

        //this is for invoice

          [HttpGet]
          public ActionResult purchaseInvoice(int? id)
          {
            if (id != null)
            {
                purchaseList p = new purchaseList();

                var t = db.purchases.Find(id);

                p.create_date = t.create_date;
                p.purchase_id = t.purchase_id;

                //get suplier name
                var s = db.Supplier_tbl.Find(t.supplier_id);
                p.sName = s.Name;
                p.sPhone = s.Phone;
                p.sMail = s.Mail;
                p.sContact_Person = s.Contact_Person;
  
                p.total_amount = t.total_amount;

                // get all purchase product
                var pd = db.purchase_details.Where(a => a.purchase_id == p.purchase_id).ToList();

                List<purchase_de> pde = new List<purchase_de>();
                foreach (var r in pd) {
                    var pi = db.product_tbl.Find(r.product_id);
                    pde.Add(new purchase_de
                    {
                        id = r.id,
                        purchase_id = r.purchase_id,
                        purchase_price = r.purchase_price,
                        quantity = r.quantity,
                        sale_price = r.sale_price,
                        sku = r.sku,

                        //get product name;
                        product_name = pi.p_name,
                    });
                  
                }
                p.purchaseDetialsList = pde;
                ViewBag.Message = TempData["Message"];
                ViewBag.Status = TempData["Status"];
                return View(p);

            }
            else
            {
                message = "Try Agine there are some Problems";
                return RedirectToAction("Create");
            }
        }
        //add product in purchase
        [HttpGet]
        public ActionResult addProduct(int? purchaseID)
        {
            return View();
        }


        //purcase details 
        [HttpGet]
        public ActionResult details(int? purchaseID)
        {
            return View();
        }

        [HttpGet]
        public ActionResult varintSetup(int? id)
        {
            product pro = new product();
            //create manageProduct object
            if (id != null)
            {
                manageProductsController mp = new manageProductsController();

                ViewBag.product_id = id;
                ViewBag.variantList = mp.variantlistforEdit(Convert.ToInt32(id));
                ViewBag.vValue = mp.vValue(Convert.ToInt32(id));
                
                return View(pro);

            }
            else
            {
                return HttpNotFound();
            }
           


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult varintSetup(product pro) {

            product_variant_value pvv = new product_variant_value();
            //product variant
            product_variant_tbl pv = new product_variant_tbl();
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
                message = "Product Varint Proprety Is updated";
                status = true;

                
                
            }
            else
            {
                message = "There Are Some Probelms";
            }

            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
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

    }
}