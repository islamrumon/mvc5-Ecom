using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using abcShop.DataModel;
using Microsoft.AspNet.Identity;

namespace abcShop.Controllers
{
    public class HomeController : Controller
    {
        private abcShopEntities db = new abcShopEntities();
        public string tokenID;
        private string userID;

        public ActionResult Index()
        {

            //Session["cart"] = "Carts";
            var site = db.site_setting_tbl.Find(1);
            //Session["siteName"] = site.siteName;
            tokenID = Session.SessionID.ToString();
            Session["siteLogo"] = ConfigurationManager.AppSettings["backEndUrl"].ToString() + site.logo;
            ViewBag.showMenu = menu();
            ViewBag.cartPanal = cartPanl(tokenID);
            ViewBag.simpalCart = simpalCart(tokenID);
            ViewBag.banner = banner();
            ViewBag.manufacture = manufacture();
            ViewBag.footer = footerSocial();
            return View();
        }


        //this is show all menu print in font
        public  string menu()
        {
            //if (Session["data"] == null)
            //{
            //    //genaret a token 
            //    string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            //    //create a session
            //    Session["data"] = token;

            //}



            string menu = "";

            var Super = db.category_tbl.Where(a => a.status == true && a.parent_id == 0).ToList();


            foreach (var super in Super)
            {


              
                           

                var Sub = db.category_tbl.Where(a => a.status == true && a.parent_id == super.category_id).ToList();
                if (Sub.Count > 0)
                {
                    menu += "<li class='menu-item-has-children'>\n" +
                           "<a href='/categorys/products/" + super.category_id + "'>" + super.cat_name + "</a>\n";
                    menu += "<ul class='sub-menu'>\n";
                    foreach (var sub in Sub)
                    {


                        var secoundSub = db.category_tbl.Where(a => a.status == true && a.parent_id == sub.category_id).ToList();
                        if (secoundSub.Count > 0)
                        {
                            menu += "<li class='menu-item-has-children'>\n" +
                             "<a href = '/categorys/products/" + sub.category_id + "' >" + sub.cat_name + "</a><ul class='sub-menu'>\n";


                            foreach (var ssub in secoundSub)
                            {
                                menu += "<li><a href='/categorys/products/" + ssub.category_id + "' >" + ssub.cat_name + "</a></li>\n";

                            }
                            menu += "</ul></li>\n";
                        }
                        else
                        {
                            menu += "<li>" +
                              "<a href ='/categorys/products/" + sub.category_id + "' >" + sub.cat_name + "</a></li>\n";
                        }
                        //menu += "<li>" +
                        //      "<a href = '/products/'" + sub.category_id + "' >" + sub.cat_name + "</a></li>\n";


                    }
                    menu += "</ul>";
                }
                else {
                    menu += "<li class='menu-item'>\n" +
                         "<a href='/categorys/products/" + super.category_id + "'>" + super.cat_name + "</a>\n";
                }
                

                menu += "\n</li>\n";
            }
            return menu;
        }

        //this is for cartpanl
        public string cartPanl(string tokenID)
        {
            //get user  id
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    userID = User.Identity.GetUserId();
                }
            }
            catch (Exception ex)
            { }

            int i = 0;
            decimal totalPrice = 0;
            //Session["cart"] = "Carts";

            //tokenID = Session.SessionID.ToString();
            var cart = db.userCart_tbl.Where(a =>a.sessionID == tokenID).ToList();
            foreach (var p in cart)
            {
                totalPrice += Convert.ToDecimal(p.totalPrice);
                i++;
            }



            string html = 
            "<a class='mini-cart-link' href='/Cart'>" +
       "<span class='mini-cart-icon title18 color'><i class='fa fa-shopping-basket'></i></span>" +
       "<span class='mini-cart-number' id='quntity'>" + i + " Item - <span class='color' id='prices'>৳ " + totalPrice + "</span></span>" +
   "</a>";

            return html;
        }

        //this is simpalCart Function
        public string simpalCart(string tokenID)
        {
            string html = " ";
            decimal price = 0;
            var link = ConfigurationManager.AppSettings["productimgPath"].ToString();
            // get session ID

            //Session["cart"] = "Carts";
            //string tokenID = Session.SessionID.ToString();
            //get Authencated user id
            string userID = "";
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    userID = User.Identity.GetUserId();
                }
            }
            catch(Exception ex)
            { }
            //get user list
            var cart = db.userCart_tbl.Where(a => a.userID == userID || a.sessionID == tokenID).ToList();


            foreach (var t in cart)
            {
                var p = db.product_tbl.Find(t.product_ID);
                html += " <div class='product-mini-cart table'>" +
                                            "<div class='product-thumb'>" +
                                            "<a href ='/product/detail/" + t.product_ID + "' class='product-thumb-link'><img src='" + link + p.main_image + "'></a>" +
                                            "<p>Quantity: "+t.Quantity+"</p>"+
                                        "</div>" +
                                        "<div class='product-info'>" +
                                            "<h4 class='product-title' style='font-size: 14px'><a href = '/product/detail/" + t.product_ID + "' >" + t.productName + "</a></h4></hr>" +
                                            "<div class='product-price'>" +
                                                "<ins><span>৳ " + t.totalPrice + "</span></ins>" +

                                            "</div>" +

                                        "</div>" +
                                    "</div>";
                price = +Convert.ToDecimal(t.totalPrice);
            }



            return html;
        }

        //banner
        public string banner()
        {
            string sp ="";

            var s = db.slider_image_tbl.OrderBy(a => a.sort_order).ToList();

            foreach(var t in s)
            {
                var i = ConfigurationManager.AppSettings["sliderPath"].ToString() + t.image;
                sp += "<div class='item-slider item-slider1'>" +
                     "<div class='banner-thumb'><a href ='" + t.link + "' ><img src='" + i + "' class='sizes'/></a></div>" +
                     "<div class='banner-info animated' data-animated='bounceIn'>" +
                 "<div class='text-center white text-uppercase'>" +
                    "<h2 class='title120 font-bold animated' data-animated='flash'>" + t.title + "</h2>" +
                    //<h2 class="title120 font-bold animated" data-animated="flash">45<span class="title90">% Off</span></h2>
                    "<h4 class='title18'>" + t.subtitle + "</h4>" +
                    "<a href ='" + t.link + "' class='btn-arrow white'>Shop now</a>" +
                "</div></div></div>";
            }

            return sp;
        }
      
        //show manufacture
        public string manufacture()
        {
            string s = "";

            var n = db.manufacturer_tbl.OrderBy(a => a.manufacturer_id).ToList();

            foreach (var e in n)
            {
                var i = ConfigurationManager.AppSettings["brandimgPath"].ToString() + e.logo;

                s += "<div class='item-brand'>" +
                       //< a href = "#" class="pulse-grow"><img src='" + i + "' alt='' /></a>
                       "<a href='/product/Brand/"+e.manufacturer_id+"' class='pulse-grow'><img src='" + i + "' alt='' /></a>" +

                 "</div>";
            }

            return s;
        }

        public ActionResult about() {
            tokenID = Session.SessionID.ToString();
            ViewBag.showMenu = menu();
            ViewBag.cartPanal = cartPanl(tokenID);
            ViewBag.simpalCart = simpalCart(tokenID);
            ViewBag.footer = footerSocial();
            var about = db.aboutTbls.Find(1);
            return View(about);
        }

        [HttpGet]
        public ActionResult contact() {
            tokenID = Session.SessionID.ToString();
            ViewBag.showMenu = menu();
            ViewBag.cartPanal = cartPanl(tokenID);
            ViewBag.simpalCart = simpalCart(tokenID);
            ViewBag.footer = footerSocial();

            //this contect 
            var about = db.aboutTbls.Find(1);
            ViewBag.map = about.mapLink;
            ViewBag.phoneNumber1 = about.phoneNumber1;
            ViewBag.phonenumber2 = about.phoneNumber2;
            ViewBag.email = about.emial;

            return View();
        }

        [HttpPost]
        public ActionResult contact(contact_tbl ct)
        {
            tokenID = Session.SessionID.ToString();
            ViewBag.showMenu = menu();
            ViewBag.footer = footerSocial();
            ViewBag.cartPanal = cartPanl(tokenID);
            ViewBag.simpalCart = simpalCart(tokenID);
            var about = db.aboutTbls.Find(1);
            ViewBag.map = about.mapLink;
            ViewBag.phoneNumber1 = about.phoneNumber1;
            ViewBag.phonenumber2 = about.phoneNumber2;
            ViewBag.email = about.emial;
            //this contect 
            if (ModelState.IsValid) {
                ct.date_add = DateTime.Now;
                db.contact_tbl.Add(ct);
                db.SaveChanges();
            }
            return View(ct);
        }


        public ActionResult privacypolicy() {
            tokenID = Session.SessionID.ToString();
            ViewBag.showMenu = menu();
            ViewBag.cartPanal = cartPanl(tokenID);
            ViewBag.simpalCart = simpalCart(tokenID);
            return View();
        }

        public ActionResult termconditions() {
            tokenID = Session.SessionID.ToString();
            ViewBag.showMenu = menu();
            ViewBag.cartPanal = cartPanl(tokenID);
            ViewBag.simpalCart = simpalCart(tokenID);
            return View();
        }

        public string footerSocial() {

            var s = db.social_sites.Find(1);

            string html = "<li><div class='social-network'>"+
                                        "<a href ='"+s.fb+"' class='float-shadow'><img src = '/forntEnd/images/icon/icon-fb.png'/></a>"+
                                        "<a href = '"+s.tw+"' class='float-shadow'><img src = '/forntEnd/images/icon/icon-tw.png'/></a>"+
                                        "<a href = '"+s.ig+"' class='float-shadow'><img src = '/forntEnd/images/icon/icon-ig.png'/></a>"+
                                        "</div></li>";

            return html;
            
        }
    }
}
