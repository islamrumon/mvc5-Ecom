using backEnd.DataModel;
using backEnd.Models.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace backEnd.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ItemladgerController : Controller
    {
        private abcShopEntities db = new abcShopEntities();
        // GET: Itemladger
        public ActionResult Index()
        {
            //get all product
            var p = db.product_tbl.ToList();
            return View(p);
        }

        //show partial view
        public ActionResult showLadger(int id)
        {
            if (id != null)
            {
                var l = db.item_ladger.Where(a => a.proID == id).OrderByDescending(a=>a.ladger_id).ToList();
                ViewBag.proId = id;
                return PartialView(l);
            }
            else
            {

                return PartialView("No Ladger Create For This Product");
            }
        }


        //bydateshowLadger
        public JsonResult bydateshowLadger(itemLadger il)
        {
            string k = " ";
          
                var s = db.item_ladger.Where(a=>a.proID ==il.proId && a.date > il.start || a.date < il.end).ToList();

                foreach (var t in s) {
                    k += "<tr><td>"+t.date +"</td><td>"+t.wareHouse_ID +"</td><td>"+t.Invoies +"</td><td>"+t.pIn +"</td><td>"+t.pOut+"</td><td>"+t.pBlance+"</td></tr>";
                }

                return Json(k, JsonRequestBehavior.AllowGet);
        }

        //item assigen in ladger

        public void ladgerItem(int productID, int proQuntity, string status, int wareHouseID, string Inv)
        {

            //create ladger object
            item_ladger lt = new item_ladger();
            //get product 
            var pro = db.product_tbl.Find(productID);
            lt.proID = pro.product_id;
            //check status
            if (status == "in") {
                pro.quantity += proQuntity;
                lt.pIn = proQuntity;
                lt.pBlance = Convert.ToInt32(pro.quantity);
            }
            else if (status == "out") {
                pro.quantity -= proQuntity;
                lt.pOut = proQuntity;
                lt.pBlance = Convert.ToInt32(pro.quantity);
            }

            //default valu assign
            lt.date = DateTime.Now;
            lt.wareHouse_ID = wareHouseID;
            lt.Invoies = Inv;

            //save the data base
            db.item_ladger.Add(lt);
            db.SaveChanges();
        }
    }
}