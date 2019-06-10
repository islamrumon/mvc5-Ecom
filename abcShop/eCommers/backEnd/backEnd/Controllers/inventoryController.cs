

using backEnd.DataModel;
using backEnd.Models.viewModels;

using Microsoft.AspNet.Identity;
using System;

using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace abcShop_auth.Areas.access.Controllers
{
    [Authorize]
    public class inventoryController : Controller
    {
        string message = "";
        bool status = false;
        int i=1;

        public abcShopEntities db = new abcShopEntities();

        // GET: access/inventory

        #region //this is for supplier setup
        [HttpGet]
        public ActionResult supplier()
        {
            supplier_vm sv = new supplier_vm();
            sv.spList = db.Supplier_tbl.OrderByDescending(a=>a.supplier_id).ToList();
            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            return View(sv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult supplier(supplier_vm sp)
        {
            Supplier_tbl st = new Supplier_tbl();
            if (ModelState.IsValid)
            {
                st.Name = sp.Name;
                st.Phone = sp.Phone;
                st.Mail = sp.Mail;
                st.Status = true;
                st.Address = sp.Address;
                st.Contact_Person = sp.Contact_Person;
                st.Create_by = User.Identity.GetUserId();
                db.Supplier_tbl.Add(st);
                db.SaveChanges();

                message = "Supplier Created Successfully";
                status = true;
            }
            else
            {
                message = "There have Some provlem, Try Agien";
            }
            TempData["Message"]= message;
            TempData["Status"]= status;
            return RedirectToAction("supplier");
        }

        //edit section
        [HttpGet]
        public ActionResult editSupplier(int? id)
        {
            if(id != null)
            {
                var s = db.Supplier_tbl.Find(id);
                return PartialView(s);
            }
            message = "There have Some provlem, Try Agien";
            TempData["Message"] = message;
            return RedirectToAction("supplier");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editSupplier(Supplier_tbl st)
        {
            if (ModelState.IsValid)
            {
                st.Create_by = User.Identity.GetUserId();
                db.Entry(st).State = EntityState.Modified;
                db.SaveChanges();

                message = "Supplier Updated Successfully";
                status = true;
            }
            else
            {
                message = "There Are some Problems.Try Agien";
            }
            TempData["Message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("supplier");
        }

        /// <summary>
        /// for deletetd
        /// </summary>
        /// <returns></returns>
        public JsonResult deleteSupplier(int? id)
        {
            var s =db.Supplier_tbl.Find(id);
            db.Supplier_tbl.Remove(s);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region //this is for customer setup

        public ActionResult customer()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.Status = TempData["Status"];
            var s = db.customer_tbl.OrderByDescending(a=>a.customer_id).ToList();
            return View(s);
        }

        [HttpGet]
        public ActionResult addCustomer()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addCustomer(customer_tbl ct)
        {
            AspNetUser aspNetUser = new AspNetUser();
            if (ModelState.IsValid)
            {
                ct.status = true;

               // ct.password = Convert.ToString("1234");
                ct.date_added = DateTime.Now;


                //this work not complate connect to AspNetUser tabel and pass data on this table

                ////add data in AspNetUser table
                //aspNetUser.Email = ct.email;
                //aspNetUser.UserName = ct.firstname;
                //db.AspNetUsers.Add(aspNetUser);
                //db.SaveChanges();

                //var v = db.AspNetUsers.Find(aspNetUser.Id);
                //ct.aspnetuserid = v.Id;
                db.customer_tbl.Add(ct);
                db.SaveChanges();

                message = "Customer Created Succesfully";
                status = true;
            



        }
            else {
                message = "There asre some problems,Try Agien";
            }

            TempData["message"] = message;
            TempData["Status"] = status;
            return RedirectToAction("customer");
        }
        #endregion

    }
}