
using backEnd.DataModel;
using backEnd.Models.viewModels;
using backEnd.Models.viewModels.manageCat;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace backEnd.Controllers
{
    [Authorize]
    public class orderController : Controller
    {
        string message;
        bool status = false;
        private abcShopEntities db = new abcShopEntities();
        // GET: order
        public ActionResult index()
        {
            
            var orders = db.order_tbl.OrderByDescending(a=>a.order_id).ToList();
            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];
            return View(orders);
        }

        //View Order Detiels
        
        public ActionResult View(int? id)
        {
            if (id !=null)
            {
                //initila order vm  model 
                order_vm ovm = new order_vm();
                //get all order detials
                var order = db.order_tbl.Find(id);

                if(order != null)
                {
                    //add data in order_vm
                    ovm.customer_id = order.customer_id;
                    ovm.payment_address_1 = order.payment_address_1;
                    ovm.payment_address_2 = order.payment_address_2;
                    ovm.firstname = order.firstname;
                    ovm.lastname = order.lastname;
                    ovm.payment_method = order.payment_method;
                    ovm.payment_telephone = order.payment_telephone;
                    ovm.telephone = order.telephone;
                    ovm.date_added = order.date_added;
                    ovm.shipping_method = order.shipping_method;
                    //this is total price
                    ovm.total = order.total;
                    ovm.invoice_no = order.invoice_no;
                    ovm.invoice_prefix = order.invoice_prefix;
                    //add shipping detials
                    ovm.shipping_firstname = order.shipping_firstname;
                    ovm.shipping_lastname = order.shipping_lastname;
                    ovm.shipping_telephone = order.shipping_telephone;
                    ovm.order_id = order.order_id;
                    ovm.shipping_address_1 = order.shipping_address_1;
                    ovm.shipping_address_2 = order.shipping_address_2;

                    //add adition data
                    ovm.comment = order.comment;
                    ovm.status_name = order.order_status;
                    ovm.sAmount =Convert.ToDecimal(order.amont);
                    //add prtoduct list
                    var ovs = new List<oPtbl>();
                    var oproduct = db.order_product_tbl.Where(a => a.order_id == id).ToList();
                    //add product in list
                    foreach (var o in oproduct)
                    {
                        var ov = new oPtbl();
                        ov.order_product_id = o.order_product_id;
                        ov.product_id = o.product_id;
                        ov.price = o.price;
                        ov.total = o.total;
                        ov.discount =Convert.ToDecimal(o.dicount);

                        //check pvv null
                        if (o.varints != null && o.varints != "null")
                        {
                            //decode json
                            List<int> v = JsonConvert.DeserializeObject<List<int>>(o.varints);

                            //initail list
                            List<pvriant> pv = new List<pvriant>();

                            foreach (var i in v)
                            {
                                //get varint valu
                                var value = db.product_variant_value.Find(i);
                                pv.Add(new pvriant
                                {
                                    price = Convert.ToDecimal(value.price),
                                    variantName = value.variant_name,
                                    varintUnit = value.variant_unit_name,
                                    pvvID = value.pvvID,
                                });

                                //if (ovm.status_name == "Shipped")
                                //{
                                //    //upddate the quantity
                                //    value.quentity -= o.quantity;
                                //    db.Entry(value).State = EntityState.Modified;
                                //    db.SaveChanges();
                                //}
                               

                            }
                            //add in list
                            ov.varints = pv;
                        }
                        else
                        {
                            ov.varints = null;
                        }
                        ov.name = o.name;
                        ov.quantity = o.quantity;
                        //if (ovm.status_name == "Shipped ")
                        //{
                        //    //update quantity form product table
                        //    var p = db.product_tbl.Find(ov.product_id);
                        //    p.quantity -= o.quantity;
                        //    db.Entry(p).State = EntityState.Modified;
                        //    db.SaveChanges();
                        //}

                        ov.reward = o.reward;
                        ovs.Add(ov);
                    }
                    ovm.orderProductList = ovs;

                    //show all order history
                    ViewBag.ToolList = new SelectList(db.order_history_tbl.ToList(), "order_status_id", "name");
                    ViewBag.WareHouse = new SelectList(db.Warehouse_tbl.ToList(), "warehouse_id", "w_name");
                    return View(ovm);
                }


                return RedirectToAction("Index");

            }
            else
            {
                return RedirectToAction("Index");
            }
           
        }

        //order status save
        [HttpPost]
        public ActionResult orderStatus(order_vm ovm)
        {
            if (ovm.status_name != null && ovm.order_id != 0)
            {
                var order = db.order_tbl.Find(ovm.order_id);
                order.order_status = ovm.status_name;
                order.WareHouseID = ovm.warehouse_id;
                order.comment = ovm.comment;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                //save order history
                order_history_tbl oh = new order_history_tbl();
                oh.notify = ovm.notify;
                oh.order_id = ovm.order_id;
                oh.order_status = ovm.status_name;
                oh.date_added = DateTime.Now;
                oh.comment = ovm.comment;
                oh.user_id = User.Identity.GetUserId();
                db.order_history_tbl.Add(oh);
                db.SaveChanges();

                if (ovm.notify == true)
                {
                    //get heare customer id
                    //send email in customer
                }

                var os = db.order_status_tbl.Find(71);
                if (oh.order_status == os.name)
                {
                    //change the status name;
                    var ord = db.order_tbl.Find(ovm.order_id);
                    ord.order_status = ord.order_status + ".";
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();


                }
                else
                {
                }

                

                

                message = "Order History created";
                status = true;
            }
            else
            {
                message = "There Are some Problems history is no created";
            }

            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("View", new { id = ovm.order_id });
        }


        //delete order
        public ActionResult Delete(int? id)
        {
            if(id != null)
            {
                //get all order row detiels
                var or = db.order_tbl.Find(id);
                db.order_tbl.Remove(or);
                db.SaveChanges();
                //get order product id
                var op = db.order_product_tbl.Where(a => a.order_id == id).ToList();
                foreach(var a in op)
                {
                    //get order_product tbl
                    var o = db.order_product_tbl.Find(a.order_id);
                    if (o.varints != null || o.varints != "null")
                    {
                        List<int> s = JsonConvert.DeserializeObject<List<int>>(o.varints);
                        foreach (var i in s)
                        {
                            //update varinat unit

                            var pvv = db.product_variant_value.Find(i);
                            pvv.quentity += o.quantity;
                            db.Entry(pvv).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //update product quantity
                    var p = db.product_tbl.Find(o.product_id);
                    p.quantity += o.quantity;
                    db.Entry(p).State = EntityState
                        .Modified;
                    db.SaveChanges();

                    db.order_product_tbl.Remove(a);
                    db.SaveChanges();
                }

              

                //delete order history
                var oh = db.order_history_tbl.Where(a => a.order_id == id).ToList();
                foreach(var t in oh)
                {
                    db.order_history_tbl.Remove(t);
                    db.SaveChanges();
                }
                

                message = "Order Is  Deleted";
                status = true;

            }
            else
            {
                message = "There have some problems try Again";
            }

            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("index");
        }


        //Edit order
        [HttpGet]
        public ActionResult orderEdit(int? id)
        {
            //first order table update 
            if (id != null)
            {
                //initila order vm  model 
                order_vm ovm = new order_vm();
                //get all order detials
                var order = db.order_tbl.Find(id);

                if (order != null)
                {
                    //add data in order_vm
                    ovm.customer_id = order.customer_id;
                    ovm.payment_address_1 = order.payment_address_1;
                    ovm.payment_address_2 = order.payment_address_2;
                    ovm.firstname = order.firstname;
                    ovm.lastname = order.lastname;
                    ovm.payment_method = order.payment_method;
                    ovm.payment_telephone = order.payment_telephone;
                    ovm.telephone = order.telephone;
                    ovm.date_added = order.date_added;
                    ovm.order_status = order.order_status;
                    //this is total price
                    ovm.total = order.total;
                    ovm.invoice_prefix = order.invoice_prefix;
                    ovm.invoice_no = order.invoice_no;
                    //add shipping detials
                    ovm.shipping_firstname = order.shipping_firstname;
                    ovm.shipping_lastname = order.shipping_lastname;
                    ovm.shipping_telephone = order.shipping_telephone;
                    ovm.order_id = order.order_id;
                    ovm.shipping_address_1 = order.shipping_address_1;
                    ovm.shipping_address_2 = order.shipping_address_2;
                    ovm.email = order.email;
                    ovm.comment = order.comment;
                    //add adition data
                   
                    ovm.status_name = order.order_status;

                    //add prtoduct list
                    var ovs = new List<oPtbl>();
                    var oproduct = db.order_product_tbl.Where(a => a.order_id == id).ToList();
                    //add product in list
                    foreach (var o in oproduct)
                    {
                        var ov = new oPtbl();
                        ov.product_id = o.product_id;
                        ov.price = o.price;
                        ov.total = o.total;
                        ov.order_product_id = o.order_product_id;
                        //check pvv null
                        if (o.varints != null && o.varints != "null")
                        {

                            
                            //decode json
                            List<int> v = JsonConvert.DeserializeObject<List<int>>(o.varints);

                            //initail list
                            List<pvriant> pv = new List<pvriant>();

                            foreach (var i in v)
                            {
                                //get varint valu
                                var value = db.product_variant_value.Find(i);
                                pv.Add(new pvriant
                                {
                                    price = Convert.ToDecimal(value.price),
                                    variantName = value.variant_name,
                                    varintUnit = value.variant_unit_name,
                                    pvvID = value.pvvID,
                                });

                            }
                            //add in list
                            ov.varints = pv;
                        }
                        else
                        {
                            ov.varints = null;
                        }
                        ov.name = o.name;
                        ov.quantity = o.quantity;
                        ov.reward = o.reward;
                        ovs.Add(ov);
                    }
                    ovm.orderProductList = ovs;

                    //show all order history
                    
                    return View(ovm);
                }

                ViewBag.Message = TempData["Message"];
                ViewBag.Status = TempData["Status"];


                return RedirectToAction("index");

            }
            else
            {
                return RedirectToAction("index");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult orderEdit(order_vm ovm)
        {
            //order table instans
            order_tbl otb = new order_tbl();
            otb.order_id = ovm.order_id;
            otb.payment_address_1 = ovm.payment_address_1;
            otb.lastname = ovm.lastname;
            otb.firstname = ovm.firstname;
            otb.total = ovm.total;
            otb.customer_id = ovm.customer_id;
            otb.order_status = ovm.order_status;
            otb.comment = ovm.comment;
            otb.date_added = ovm.date_added;
            otb.invoice_no = ovm.invoice_no;
            otb.invoice_prefix = ovm.invoice_prefix;
            otb.email = ovm.email;
            otb.payment_telephone = ovm.payment_telephone;
            otb.payment_method = ovm.payment_method;
            otb.shipping_address_1 = ovm.shipping_address_1;
            otb.shipping_telephone = ovm.shipping_telephone;
            otb.date_modified = DateTime.Now;
            db.Entry(otb).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("View",new { id = ovm.order_id});
        }


        //edit product or order
        public ActionResult editProduct(int? id)
        {
            var product = new productsv();
            if (id != null)
            {
                //get orderproduct product id
                var op = db.order_product_tbl.Find(id);
                var p = db.product_tbl.Find(op.product_id);
                product.order_product_id = op.order_product_id;
                product.order_id = op.order_id;
                product.discription = p.discription;
                product.main_image = p.main_image;
                product.meta_description = p.meta_description;
                product.meta_keyword = p.meta_keyword;
                product.meta_title = p.meta_title;
                product.price = p.price;
                product.product_id = p.product_id;
                product.quantity = p.quantity;
               
                product.p_name = p.p_name;
                
                ViewBag.variant = variant(p.product_id);
                return View(product);
            }
            else
            {
                return HttpNotFound();
            }
        }

        //productUpdate
       [HttpPost]
        public ActionResult productUpdate(string pvv, int? product_id, int? Quantity, int? opID, int? oID)
        {
            decimal vpricr = 0;
            //inatilize order table object 
            order_product_tbl op = new order_product_tbl();
            if (product_id != null && opID != null)
            {
                //get product detiels
                var p = db.product_tbl.Find(product_id);

                op.order_product_id = Convert.ToInt32(opID);
                op.product_id = p.product_id;
                op.name = p.p_name;
               if (pvv != null && pvv != "null"  )
                {
                   //int t= Convert.ToInt32(pvv);
                    //listing json tp
                    List<int> ss = JsonConvert.DeserializeObject<List<int>>(pvv);
                    
                    //setep varint price
                    foreach (var v in ss)
                    {
                        var pv = db.product_variant_value.Find(v);
                        vpricr += Convert.ToDecimal(pv.price);
                    }

                }
                op.order_id = oID;
                op.price = p.price + vpricr;
                op.quantity = Quantity;
                op.total = op.price * op.quantity;
                if (pvv != null )
                {
                    op.varints = pvv;
                }
                else
                {
                    //op.model = null;
                }
                db.Entry(op).State = EntityState.Modified;
                db.SaveChanges();
                decimal tprice = 0;
                //now update all amonu total price
                var i = db.order_product_tbl.Where(a => a.order_id == oID).ToList();
                //samition the price
                foreach(var l in i)
                {
                    tprice += Convert.ToDecimal(l.total);
                }
                var o = db.order_tbl.Find(oID);
                o.total = tprice;
                db.Entry(o).State = EntityState.Modified;
                db.SaveChanges();

                var s = db.order_product_tbl.Find(op.order_product_id);
                message = "Product Updated Successfully";
                status = true;
                return RedirectToAction("orderEdit", new { id=s.order_id});
            }
            else
            {
                message = "There are some Problem try agien";
            }
            TempData["Status"] = status;
            TempData["Message"] = message;
            return RedirectToAction("orderEdit", new {id = opID });
        }
        //this is varint 
        public string variant(int Pid)
        {
            string html = "<table>";

            //get variant type
            var r = db.product_variant_tbl.Where(a => a.product_id == Pid).ToList();

            //loop for print design
            foreach (var i in r)
            {
                var v = db.variants.Find(i.Variant_id);

                //this is checkbox section
                if (v.type == "CheckBox")
                {
                    html += "<tr><td>" + v.varient_name + ": \t \t</td>" +
                        "<td>";

                    //var print variant value
                    var vv = db.product_variant_value.Where(a => a.product_id == i.product_id && a.variant_id == i.Variant_id).ToList();
                    //print hole varint with value
                    foreach (var q in vv)
                    {
                        html +=
                            "<label  for='" + q.variant_unit_name + "'>" +
                                     "<input  onclick='changePrice(" + q.price + "," + q.variant_id + ")' class='form-check-input' value='" + q.pvvID + "' name='pvvID" + q.variant_id + "' type='checkbox'>" + q.variant_unit_name + "\t <span class='color'>" + q.price + " ৳ </span>" +
                                          "</label>";
                    }
                    html += "</td></tr>";
                }

                //this is Radio section
                else if (v.type == "Radio")
                {
                    html += "<tr><td>" + v.varient_name + ": \t \t</td>" +
                        "<td>";

                    //var print variant value
                    var vv = db.product_variant_value.Where(a => a.product_id == i.product_id && a.variant_id == i.Variant_id).ToList();
                    foreach (var s in vv)
                    {
                        html +=
                           "<label for='" + s.variant_unit_name + "'>" +
                             "<input onclick='changePrice(" + s.price + "," + s.variant_id + ")' type='radio' value='" + s.pvvID + "'  name='pvvID" + s.variant_id + "'>" + s.variant_unit_name + "\t <span class='color'>" + s.price + " ৳</span>" +
                           "</label>";
                    }
                    html += "</td></tr>";

                }

                //this is Select section
                else if (v.type == "Select")
                {
                    html += "<tr><td>" + v.varient_name + ": \t \t </td>" +
                        "<td><div class='custom-selectoption'>" +
                              "<select class='form-control' id='changePrice1' name='pvvID" + v.varient_id + "'>" +
                              "<option>--</option>";
                    var vv = db.product_variant_value.Where(a => a.product_id == i.product_id && a.variant_id == i.Variant_id).ToList();

                    foreach (var s in vv)
                    {
                        html += "<option id='filed'  data-price='" + s.price + "'  data-id='" + s.variant_id + "' value='" + s.pvvID + "'>" + s.variant_unit_name + "\t <span class='color'>" + s.price + " ৳ </span>" + "</option>";
                    }
                    html += "</select>" +
                             "</div></td></tr>";

                }

            }


            html += "</table>";
            return html;
        }


        //delete product form order product tbl
        public ActionResult deleteProduct(int? id)
        {
            int? ortder_id = 0;
            //get orderid form order product table
            if (id != null)
            {
                var o = db.order_product_tbl.Find(id);
                ortder_id = o.order_id;
                db.order_product_tbl.Remove(o);
                db.SaveChanges();

                //update the order table 
                decimal tprice = 0;
                //now update all amonu total price
                var i = db.order_product_tbl.Where(a => a.order_id == ortder_id).ToList();
                //samition the price
                foreach (var l in i)
                {
                    tprice += Convert.ToDecimal(l.total);
                }
                var os = db.order_tbl.Find(ortder_id);
                os.total = tprice;
                db.Entry(os).State = EntityState.Modified;
                db.SaveChanges();

                message = "Order Is Updated";
                status = true;

            }
            else
            {
                message = " there have problems try agien";
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("View", new { id = ortder_id });
            

         }

        //create invoice
        [HttpPost]
        public JsonResult createInvoice(int? id)
        {
            if (id != null)
            {
                //get invoice id  
                var sa= db.site_setting_tbl.Find(1);
                int sireal = Convert.ToInt32(sa.Invoic);
                //increament id
                sireal++;
                //get order tbl data
                var os = db.order_tbl.Find(id);
                
                os.invoice_no = sireal;
                os.invoice_prefix = "#INV";
                db.Entry(os).State = EntityState.Modified;
                db.SaveChanges();

                //update setting tbl invoice row
                sa.Invoic = sireal;
                db.Entry(sa).State = EntityState.Modified;
                db.SaveChanges();


                //quntity the cart product 
                var or = db.order_product_tbl.Where(a => a.order_id == os.order_id).ToList();

                foreach (var u in or)
                {
                    //get order_product tbl
                    var o = db.order_product_tbl.Find(u.order_product_id);
                    if (o.varints != null || o.varints != "null")
                    {
                        List<int> s = JsonConvert.DeserializeObject<List<int>>(o.varints);
                        if (s != null)
                        {
                            foreach (var i in s)
                            {
                                //update varinat iunt
                                var pvv = db.product_variant_value.Find(i);





                                pvv.quentity -= u.quantity;
                                db.Entry(pvv).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                    }

                    //update product quantity
                    var p = db.product_tbl.Find(u.product_id);

                    //first create itemladger 
                    ItemladgerController ic = new ItemladgerController();
                    ic.ladgerItem(Convert.ToInt32(p.product_id), Convert.ToInt32(u.quantity), "out", Convert.ToInt32(os.WareHouseID), Convert.ToString(os.invoice_prefix + os.invoice_no));


                    p.quantity -= u.quantity;
                    db.Entry(p).State = EntityState
                        .Modified;
                    db.SaveChanges();

                    //update dicount table quantity
                    // get discount 
                    var date = DateTime.Today;
                    var d = db.product_discount_tbl.OrderBy(a => a.priority).Where(a => a.date_start <= date && a.date_end >= date && a.quantity != 0 && a.product_id == u.product_id).FirstOrDefault();
                    if (d != null)
                    {
                        d.quantity -= u.quantity;
                        db.Entry(d).State = EntityState.Modified;
                    }

                }


             

            }
            return Json(false, JsonRequestBehavior.AllowGet);
            

        }
    }
}