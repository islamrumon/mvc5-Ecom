using abcShop.Models;
using abcShop.DataModel;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System;

namespace abcShop.Controllers
{
    public class categorysController : Controller
    {
        private abcShopEntities db = new abcShopEntities();
        // GET: Category by products
        public ActionResult products(int? id)
        {
            var products = new product();
            var cates = new List<category_tbl>();
            //if(!string.IsNullOrEmpty(Session.SessionID))

            List<product_tbl> p = new List<product_tbl>();

            if (id ==null)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                var cs = db.category_tbl.Find(id);
                ViewBag.baner = cs.banner;
                ViewBag.video = cs.VideoLink;
                var getProduct = db.product_to_category_tbl.Where(a => a.category_id == id).ToList();
                foreach(var t in getProduct) {
                    p.Add(db.product_tbl.Find(t.product_id));
                }
                
                // bag data 
                ViewBag.varient_search = varient_search(id);

                //get min price
                ViewBag.minP = p.Select(asd=>asd.price).Min(); 
                //get max price
                ViewBag.maxP = p.Select(a => a.price).Max();
                var cat = db.category_tbl.Where(a => a.parent_id == id).ToList();
                if (cat.Count>0)
                {
                    foreach(var c in cat)
                    {
                        var category = new category_tbl();
                        category.category_id = c.category_id;
                        category.parent_id = c.parent_id;
                        category.logo = c.logo;
                        category.VideoLink = c.VideoLink;
                        category.sort_order = c.sort_order;
                        category.cat_name = c.cat_name;
                        category.banner = c.banner;
                        cates.Add(category);

                    }
                    products.proCategory = cates;
                }

                if (getProduct.Count > 0)
                {
                    commonWork(id);
                    return View(products);
                }
                else
                {
                    return HttpNotFound();
                }
            }


        }

        //thare are varint section
        public string varient_search(int? id)
        {

            string sm = "";

            int sx = 0;
            int su = 0;

            //var query = from pc in db.product_to_category_tbl
            //            join
            //            pv in db.product_variant_value on pc.product_id equals pv.product_id
            //            where (pc.category_id == id)
            //            select new { pv.variant_unit_name, pv.variant_name, pv.variant_id, pv.variant_unit_id };
            var ss = db.prcVeriantSearch(id).ToList();
            foreach(var s in ss)
            {
                if (sx != s.variant_id)
                {
                    if (su == 1)
                    {
                        sm += "</ul>";
                    }

                    sm += "<h2 class='title18 title-widget font-bold'>" + s.variant_name + "</h2>"+
                        "<ul class='list-group'  style='background-color: #F5F5F5; float: left; height: 100px; overflow-y: scroll; overflow-x: hidden; width:100%;'>";
                    sx = Convert.ToInt32(s.variant_id);
                
                    sm += "<li class='list-group-item'><div  class='checkbox'>" +
                                "<label>" +
                                "<input onclick='varint(" + s.variant_unit_id + ")' type='checkbox'>" + s.variant_unit_name + "  " +
                                "</label><span class='badge pull-right'></span></div></li>";

                    su = 1;
                }
                else
                {
                    sm += "<li class='list-group-item'><div  class='checkbox'>" +
                                "<label>" +
                                "<input onclick='varint(" + s.variant_unit_id + ")' type='checkbox'>" + s.variant_unit_name + "  " +
                                "</label><span class='badge pull-right'></span></div></li>";
                }
                
            }
            return sm;

            
        }
        //product by category
        [HttpPost]
        public JsonResult byCategory(int? id)
        {
            if (id != null)
            {

                //var pro = new List<product_tbl>();
                //var pcTbl = db.product_to_category_tbl.Where(a => a.category_id == id).ToList();

                //foreach (var p in pcTbl)
                //{
                //    var Pro = db.product_tbl.OrderByDescending(a => a.product_id).Where(a => a.product_id == p.product_id).ToList();

                //    foreach (var p1 in Pro)
                //    {
                //        var ps = new product_tbl();
                //        ps.product_id = p1.product_id;
                //        ps.p_name = p1.p_name;
                //        ps.price = p1.price;
                //        ps.points = p1.points;
                //        ps.VideoLink = p1.VideoLink;
                //        ps.quantity = p1.quantity;
                //        ps.main_image = p1.main_image;
                //        pro.Add(ps);  

                //    }
                //}

                productController pc = new productController();
                string s = pc.allProduct(id);
                return Json(s, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);



        }


        //product by filterProduct
        [HttpPost]
        public JsonResult filterProduct(int max, int min, string pvv, int cid)
        {
            if (pvv != "")
            {
                List<int> s = JsonConvert.DeserializeObject<List<int>>(pvv);

                //list 
                var product = new List<product_tbl>();
                foreach (var v in s)
                {
                    //get varint type data

                    var pro = db.product_variant_value.Where(a => a.variant_unit_id == v).GroupBy(a => a.product_id).Select(g => new { product_id = g.Key }).ToList();
                    //var p = db.product_variant_value.Where(a => a.variant_unit_id == v).ToList();
                    foreach (var o in pro)
                    {
                        //var k = IGrouping<int>g => new { p_name = g.Key }
                        //get product
                        var p1 = db.product_tbl.Find(o.product_id);
                        var ps = new product_tbl();
                        ps.product_id = p1.product_id;
                        ps.p_name = p1.p_name;
                        ps.price = p1.price;
                        ps.points = p1.points;
                        ps.quantity = p1.quantity;
                        ps.main_image = p1.main_image;
                        product.Add(ps);
                    }
                }

                var p = product.Where(a => a.price >= min && a.price <= max).ToList();

                return Json(new { list = p }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var p = db.prcFilter(max,min,cid).ToList();

                return Json(new { list = p }, JsonRequestBehavior.AllowGet);
            }
        }
        //product by byVariant
        [HttpPost]
        public JsonResult byVariant(string vr)
        {
            if (vr != null || vr != "null")
            {
                List<int> s = JsonConvert.DeserializeObject<List<int>>(vr);

                //list 
                var product = new List<product_tbl>();
                foreach (var v in s)
                {
                    //get varint type data

                    var p = db.product_variant_value.Where(a => a.variant_unit_id == v).GroupBy(a=>a.product_id).Select(g => new { product_id =g.Key }).ToList();
                    //var p = db.product_variant_value.Where(a => a.variant_unit_id == v).ToList();
                    foreach (var o in p)
                    {
                        //var k = IGrouping<int>g => new { p_name = g.Key }
                        //get product
                        var p1 = db.product_tbl.Find(o.product_id);
                        var ps = new product_tbl();
                        ps.product_id = p1.product_id;
                        ps.p_name = p1.p_name;
                        ps.price = p1.price;
                        ps.points = p1.points;
                        ps.quantity = p1.quantity;
                        ps.main_image = p1.main_image;
                        product.Add(ps);
                    }
                }

                return Json(new { list = product }, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }


        //singleproduct in pop up
        public ActionResult quickview(int? id)
        {
            var product = new product();
            if (id != null)
            {
                var p = db.product_tbl.Find(id);
                var g = db.product_image_tbl.Where(a => a.product_id == p.product_id).ToList();
                string tokenID = Session.SessionID.ToString();

                product.discription = p.discription;
                product.main_image = p.main_image;
                product.meta_description = p.meta_description;
                product.meta_keyword = p.meta_keyword;
                product.meta_title = p.meta_title;
                product.price = p.price;
                product.videoLink = p.VideoLink;
                product.product_id = p.product_id;
                product.quantity = p.quantity;
                product.gallery = g;
                product.p_name = p.p_name;
                ViewBag.imgPath = ConfigurationManager.AppSettings["productimgPath"].ToString();
                ViewBag.variant = variant(p.product_id);
                var home = new HomeController();
                ViewBag.cartPanal = home.cartPanl(tokenID);
                ViewBag.simpalCart = home.simpalCart(tokenID);
                return View(product);
            }
            else
            {
                return HttpNotFound();
            }
        }

        //this is variant manag
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
                                     "<input  onclick='changePrice(" + q.price + "," + q.variant_id + ")' class='form-check-input' value='" + q.pvvID + "' name='pvvID"+ q.variant_id +"' type='checkbox'>" + q.variant_unit_name + "\t <span class='color'>" + q.price + " ৳ </span>" +
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
                              "<select class='form-control' id='changePrice1' name='pvvID" + v.varient_id + "'>" ;
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


        //this finction for only this controller
        public void commonWork(int? id)
        {
            string tokenID = Session.SessionID.ToString();
            var cat = db.category_tbl.Find(id);
            //this section ic common
            var home = new HomeController();
            ViewBag.cartPanal = home.cartPanl(tokenID);
            ViewBag.simpalCart = home.simpalCart(tokenID);
            ViewBag.showMenu = home.menu();
           
            ViewBag.catID = cat.category_id;
            ViewBag.proImg = ConfigurationManager.AppSettings["productimgPath"].ToString();
            ViewBag.catName = cat.cat_name;
            ViewBag.baner = ConfigurationManager.AppSettings["catimgPath"].ToString()+ cat.banner;
            
        }

        
    }


    
}