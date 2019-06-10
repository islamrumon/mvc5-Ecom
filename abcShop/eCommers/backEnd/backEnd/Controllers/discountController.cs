using backEnd.DataModel;
using backEnd.Models.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace backEnd.Controllers
{
    public class discountController : Controller
    {

        string message;
        bool status = false;
        private abcShopEntities db = new abcShopEntities();

        // GET: discount
        public ActionResult Index()
        {
            var p = db.product_tbl.OrderByDescending(a => a.product_id).ToList();
            ViewBag.Status = TempData["Status"];
            ViewBag.Message = TempData["Message"];
            return View(p);
        }

       [HttpGet]
       public ActionResult addDiscount(int? id)
        {
            if (id != null)
            {
                var p = db.product_tbl.Find(id);

                ViewBag.product_id = p.product_id;
                ViewBag.p_name = p.p_name;
                ViewBag.quantity = p.quantity;
                ViewBag.price = p.price;

                //if have product discount
                ViewBag.tblDiscount = tblDiscount(p.product_id);
                ViewBag.Status = TempData["Status"];
                ViewBag.Message = TempData["Message"];
                return View();
             
            }
            else
            {
                message = "There Are Some Problems";
                TempData["Status"] = status;
                TempData["Message"] = message;
                return RedirectToAction("Index");

            }

           
        }

        public string tblDiscount(int id)
        {
            string html = "";

            var d = db.product_discount_tbl.Where(a => a.product_id == id).ToList();
            if (d != null)
            {
                

                foreach(var t in d)
                {
                    html += "<tr>" +
                        "<td>" + t.quantity + "</td>" +
                        "<td>" + t.price + "</td>" +
                        "<td>" + t.priority + "</td>" +
                        "<td>" + t.date_start + "</td>" +
                        "<td>" + t.date_end + "</td>" +
                        "<td><button class='btn btn-info' onclick='editdiscover("+t.product_discount_id+")'>Edit</button></td>" +
                        "</tr>";
                }
                
            }
            return html;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addDiscount(product_discount_tbl pro)
        {
            if (ModelState.IsValid)
            {
                db.product_discount_tbl.Add(pro);
                db.SaveChanges();
                message = "Discount is Created";
                status = true;
            }
            else
            {
                message = "There Are Some Probems";
            }
            TempData["Status"] = status;
            TempData["Message"] = message;
            return RedirectToAction("addDiscount",new {id = pro.product_id});
        }


        [HttpGet]
        public PartialViewResult editdiscover(int id)
        {
         
                var d = db.product_discount_tbl.Find(id);
                
                    return PartialView(d);
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editdiscover(product_discount_tbl pro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pro).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                message = "Discount Is updated";
                status = true;
            }
            else
            {
                message = " there are some problems";
            }

            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("addDiscount", new { id = pro.product_id });


        }
    }
}