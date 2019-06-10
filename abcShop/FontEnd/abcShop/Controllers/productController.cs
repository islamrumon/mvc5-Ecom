
using System.Collections.Generic;
using System.Linq;

using System.Web.Mvc;
using abcShop.Models;
using abcShop.DataModel;

using System.Configuration;
using System;

namespace abcShop.Controllers
{
    public class productController : Controller
    {
        categorysController cc = new categorysController();
        private abcShopEntities db = new abcShopEntities();
        // GET: product
        public ActionResult detail(int? id)
        {
            var cates = new List<category_tbl>();
            var Product = new product();
            if (id != null)
            {
                var pro = new List<product_tbl>();
                var p = db.product_tbl.Find(id);
                var g = db.product_image_tbl.Where(a => a.product_id == p.product_id).ToList();

                Product.discription = p.discription;
                Product.main_image = p.main_image;
                Product.meta_description = p.meta_description;
                Product.meta_keyword = p.meta_keyword;
                Product.meta_title = p.meta_title;
                Product.price = p.price;
                Product.product_id = p.product_id;
                Product.quantity = p.quantity;
                Product.videoLink = p.VideoLink;

                //get discount price
                var date = DateTime.Today;
                var d = db.product_discount_tbl.OrderBy(a => a.priority).Where(a => a.date_start <= date && a.date_end >= date && a.quantity != 0 && a.product_id == id).FirstOrDefault();
                if (d != null)
                {
                    Product.discountPrice  =Convert.ToDecimal(Product.price - d.price);
                    
                }

                Product.p_name = p.p_name;
                ViewBag.imgPath = ConfigurationManager.AppSettings["productimgPath"].ToString();
                ViewBag.Variant = cc.variant(p.product_id);
                //add gallery img
                Product.gallery = g;
             
                var r = db.product_related_tbl.Where(a => a.product_ID == p.product_id).ToList();
                foreach(var rel in r)
                {
                    var pr = db.product_tbl.OrderByDescending(a=>a.product_id).Where(a => a.product_id == rel.releted_product_ID).ToList();
                    foreach (var p1 in pr)
                    {
                        var ps = new product_tbl();
                        ps.product_id = p1.product_id;
                        ps.p_name = p1.p_name;
                        ps.price = p1.price;
                        ps.points = p1.points;
                        ps.quantity = p1.quantity;
                        ps.main_image = p1.main_image;
                        pro.Add(ps);
                    }
                    
                }
                //this section ic common
                var home = new HomeController();
                string sessionID = Session.SessionID.ToString();
                ViewBag.showMenu = home.menu();
                ViewBag.footer = home.footerSocial();
                ViewBag.cartPanal = home.cartPanl(sessionID);
                ViewBag.simpalCart = home.simpalCart(sessionID);
                Product.relatedProduct = pro;
                ViewBag.bycategory = bycategory(id);
                return View(Product);
            }
            else
            {

                return HttpNotFound();
            }
        }

        //search option
        [HttpGet]
        public ActionResult search(string s)
        {
            if (s != null)
            {
                var home = new HomeController();
                ViewBag.proImg = ConfigurationManager.AppSettings["productimgPath"].ToString();
                string sessionID = Session.SessionID.ToString();
                ViewBag.showMenu = home.menu();
                ViewBag.footer = home.footerSocial();
                ViewBag.cartPanal = home.cartPanl(sessionID);
                ViewBag.simpalCart = home.simpalCart(sessionID);
                ViewBag.key = s;
                //get value
                var p = db.product_tbl.Where(a => a.p_name.Contains(s) || a.discription.Contains(s)).ToList();
                return View(p);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
           
        }

        //all product
        [HttpPost]
        public JsonResult All()
        {
            var p = db.product_tbl.ToList();
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        //there are all product show with discount
        public string allProduct(int? id)
        {

            var pcTbl = db.product_to_category_tbl.Where(a => a.category_id == id).ToList();


            var s = ConfigurationManager.AppSettings["productimgPath"].ToString();
            string html = "";
            foreach (var p in pcTbl)
            {
                var Pro = db.product_tbl.OrderByDescending(a => a.product_id).Where(a => a.product_id == p.product_id).ToList();

                foreach (var t in Pro)
                {
                    html += "<form id = 'cartForm'><div class='col-md-4 col-sm-6 col-xs-6'>" +
                             "<div class='item-product item-product-grid text-center'>" +
                             "<div class='product-thumb'>" +
                             "<input type='hidden' name='product_id' value='" + t.product_id + "' /><input type='hidden' name='quentity' value='1' />" +
                             "<a href='/product/detail/" + t.product_id + "' class='product-thumb-link rotate-thumb'>" +
                             "<img src='" + s + t.main_image + "' >" +
                             "<img src='" + s + t.main_image + "'>" +
                             "</a>" +
                             //"<a style='cursor:pointer' onclick='quckView(" + value.product_id + ")' class='quickview-link'><i class='fa fa-search' aria-hidden='true'></i></a>" +
                             "<a href='/categorys/quickview/" + t.product_id + "' class='quickview-link fancybox fancybox.iframe'><i class='fa fa-search' aria-hidden='true'></i></a>" +
                             "</div>" +
                             "<div class='product-info'>" +
                             "<h3 class='product-title'><a href='/product/detail/" + t.product_id + "'>" + t.p_name + "</a></h3>" +
                             "<div class='product-price'>";
                    //check the discount
                    var date = DateTime.Today;
                    var d = db.product_discount_tbl.OrderBy(a => a.priority).Where(a => a.date_start <= date && a.date_end >= date && a.quantity != 0  && a.product_id == t.product_id).FirstOrDefault();
                    if (d != null)
                    {
                        var pr = t.price - d.price;
                        html += "<del class='silver'>৳ <span>" + t.price + "</span></del>" +
                              "<ins class='color'>৳ <span>" + pr + "</span></ins>";
                    }
                    else
                    {
                        html += "<ins class='color'>৳ <span>" + t.price + "</span></ins>";
                    }

                    html += "</div>" +
                     "<div class='product-rate'>" +
                     "<div class='product-rating' style='width:100%'></div>" +
                     "</div>" +
                     "<div class='product-extra-link'>" +

                     "<a href='javascript:void(0)' class='addcart-link'  onclick='addCarts(" + t.product_id + ")'>Add to cart</a>" +
                     "</div>" +
                     "</div>" +
                     "</div>" +
                     "</div></form>";
                }
            }
            return html;

        }

        //show related product by category
        public string bycategory(int? id)
        {
            var l = ConfigurationManager.AppSettings["productimgPath"].ToString();
            string sn = "";
            var c = db.product_to_category_tbl.Where(a => a.product_id == id).ToList();
            foreach (var ci in c)
            {
                int i = 0;

                sn += "<div class='item'>";
                var cp = db.product_to_category_tbl.Where(a=>a.category_id == ci.category_id).GroupBy(a=>a.product_id).Select(a=> new { product_id=a.Key }).ToList();
                foreach(var p in cp)
                {
                    i++;
                    var pro = db.product_tbl.Find(p.product_id);

                    if (pro != null)
                    {
                        sn += "<div class='item-wg-product table'>" +
                                               "<div class='product-thumb'>" +
                                               "<a href ='/product/detail/" + pro.product_id + "' class='product-thumb-link zoom-thumb'>" +
                                                   "<img src='"+l+""+pro.main_image+"'>" +
                                               "</a>" +
                                               "<a href='/categorys/quickview/" + pro.product_id + "' class='quickview-link fancybox fancybox.iframe'><i class='fa fa-search' aria-hidden='true'></i></a>" +
                                           "</div>" +
                                           "<div class='product-info'>" +
                                               "<h3 class='product-title'><a href ='/product/detail/" + pro.product_id + "' >" + pro.p_name + "</a></h3>" +
                                               "<div class='product-price'>" +

                                                   "<ins class='color'>৳ <span>" + pro.price + "</span></ins>" +
                                               "</div>" +

                                           "</div>" +
                                       "</div>";


                        if (i == 10)
                        {
                            sn += " </div>";
                            break;
                        }
                    }
                   
                }


            }

            return sn;
        }


        //brand
        [HttpGet]
        public ActionResult Brand(int? id) {

            var home = new HomeController();
            ViewBag.proImg = ConfigurationManager.AppSettings["productimgPath"].ToString();
            string sessionID = Session.SessionID.ToString();
            ViewBag.showMenu = home.menu();
            ViewBag.footer = home.footerSocial();
            ViewBag.cartPanal = home.cartPanl(sessionID);
            ViewBag.simpalCart = home.simpalCart(sessionID);

            var product = db.product_tbl.Where(a => a.manufacturer_id == id).ToList();
            return View(product);
        }
    }
}