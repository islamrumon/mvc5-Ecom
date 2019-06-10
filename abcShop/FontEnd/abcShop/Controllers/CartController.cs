using abcShop.DataModel;
using abcShop.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace abcShop.Controllers
{


    public class CartController : Controller
    {
        HomeController home = new HomeController();
        string message;
        private abcShopEntities db = new abcShopEntities();

      

        private ApplicationDbContext adb = new ApplicationDbContext();

        #region //this is cart section
        // GET: Cart
        public ActionResult Index()
        {   //get session ID
            string sessionID = Session.SessionID.ToString();
            //get Authencated user id
            string userID = "";
            if (User.Identity.IsAuthenticated)
            {
                userID = User.Identity.GetUserId();
            }
            //get user list
            var cart = db.userCart_tbl.Where(a=>a.sessionID == sessionID || a.userID == userID).ToList();

            //initail cart model
            List<cart> cr = new List<cart>();
            cart cartModel = new cart();
            //loop them
            foreach (var c in cart)
            {
                cartModel.cartID = c.CartID;
                cartModel.product_id = Convert.ToInt32(c.product_ID);
                cartModel.p_name = c.productName;
                cartModel.totalPrice = Convert.ToDouble(c.totalPrice);
                cartModel.VorMainPrice = Convert.ToDouble(c.mainPrice + c.varintPrice);
                cartModel.quntity = Convert.ToInt32(c.Quantity);
                cartModel.discountPrices = Convert.ToDecimal(c.discountPrice) * cartModel.quntity;
                //get product img
                var p = db.product_tbl.Find(Convert.ToInt32(c.product_ID));
                cartModel.img = p.main_image;

                if (c.pvvalueID != "null")
                {
                    //convert josn type variant in string
                    List<int> v = JsonConvert.DeserializeObject<List<int>>(c.pvvalueID);

                    //initial pvriant list
                    List<pvriant> li = new List<pvriant>();

                    //foreach and asin
                    foreach (var pv in v)
                    {
                        var l = db.product_variant_value.Find(pv);
                        li.Add(new pvriant
                        {
                            pvvID = l.pvvID,
                            variantName = l.variant_name,
                            varintUnit = l.variant_unit_name,
                        });
                    }

                    //add to list in list proprety
                    cartModel.varints = li;


                }
                else
                {
                    cartModel.varints = null;

                }
                //cr.Add(cartModel);

                cr.Add(new cart {
                    varints = cartModel.varints,
                    quntity = cartModel.quntity,
                    VorMainPrice = cartModel.VorMainPrice,
                    totalPrice = cartModel.totalPrice,
                    discountPrices = cartModel.discountPrices,
                    p_name = cartModel.p_name,
                    product_id =cartModel.product_id,
                    cartID =cartModel.cartID,
                    img = cartModel.img
                });
            }

            string tokenID = Session.SessionID.ToString();


            ViewBag.showMenu = home.menu();
            ViewBag.cartPanal = home.cartPanl(tokenID);
            ViewBag.simpalCart = home.simpalCart(tokenID);

            ViewBag.proImg = ConfigurationManager.AppSettings["productimgPath"].ToString();
            return View(cr);
        }

      
        [HttpPost]
        public ActionResult pvvCart(int[] pvv, int? product_id, int? Quantity)
        {
            double totalPrice =0 ;
            
            double Price = 0;
            userCart_tbl cart = new userCart_tbl();
            if (product_id != null)
            {

                //check
                //check this product have cart or not

                //get product detials
                var product = db.product_tbl.Find(product_id);
                if (product != null)
                {

                    //get varient price and sum this
                    if (pvv != null)
                    {
                        foreach (var p in pvv)
                        {
                            var v = db.product_variant_value.Find(p);
                            if (v.product_id == product.product_id)
                            {

                                Price += Convert.ToDouble(v.price);

                            }
                            else
                            {
                                return Json(JsonRequestBehavior.DenyGet);
                            }
                        }

                        totalPrice = Convert.ToDouble(product.price) + Price;
                        //if quantity 
                        if (Quantity != null)
                        {
                            totalPrice = totalPrice * Convert.ToDouble(Quantity);
                        }


                        var jsontypevarintID = JsonConvert.SerializeObject(pvv);

                        cart.pvvalueID = jsontypevarintID;
                    }
                    else
                    {
                        //check this product have variant
                        //if have product then redirect to product detils pages
                        var vr = db.product_variant_value.Where(a => a.product_id == product_id).ToList();
                        if (vr.Count > 0)
                        {
                            return Json(Url.Action("detail", "product", new { id = product_id }));

                            //RedirectToAction("detail", "detail", new { id = product_id });
                        }

                        cart.pvvalueID = "null";
                        totalPrice = Convert.ToDouble(product.price);
                        //if quantity 
                        if (Quantity != null)
                        {
                            totalPrice = totalPrice * Convert.ToDouble(Quantity);
                        }

                    }
                   

                    //add in cart model
                    cart.product_ID = product.product_id;
                    cart.productName = product.p_name;
                    // get discount 
                    var date = DateTime.Today;
                    var d = db.product_discount_tbl.OrderBy(a => a.priority).Where(a => a.date_start <= date && a.date_end >= date && a.quantity != 0 && a.product_id == cart.product_ID).FirstOrDefault();
                    if (d != null)
                    {
                        cart.discountPrice = Convert.ToDecimal(d.price);
                        
                    }
                   
                    cart.Quantity = Quantity;
                    cart.mainPrice = product.price;

                    cart.totalPrice = Convert.ToDecimal(totalPrice) - Convert.ToDecimal(cart.discountPrice * Quantity);
                    cart.varintPrice = Convert.ToDecimal(Price);
                    string sessionID = Session.SessionID.ToString();
                    cart.sessionID = sessionID;

                    if (User.Identity.IsAuthenticated)
                    {
                        cart.userID = User.Identity.GetUserId();
                    }

                    //check that the product have in cart table
                    var c = db.userCart_tbl.SingleOrDefault(a => a.product_ID == cart.product_ID && a.sessionID == sessionID && a.pvvalueID == cart.pvvalueID);
                    if (c != null)
                    {
                        cart.CartID = c.CartID;
                        cart.Quantity = c.Quantity + cart.Quantity;
                        cart.totalPrice = c.totalPrice + Convert.ToDecimal(totalPrice);
                        db.UpdateCart(cart.Quantity,cart.totalPrice,cart.CartID,cart.sessionID);
                        
                    }
                    else
                    {
                        db.userCart_tbl.Add(cart);
                        db.SaveChanges();
                    }
                  

                    var home = new HomeController();
                    ViewBag.cartPanal = home.cartPanl(sessionID);
                    ViewBag.simpalCart = home.simpalCart(sessionID);

                   
                    return Json(null,JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
               
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
           
        }

      
        ////for sortCart
        //[HttpPost]
        //public JsonResult sumpalCart()
        //{

        //    ViewBag.proImg = ConfigurationManager.AppSettings["productimgPath"].ToString();
        //    //get session ID
        //    string sessionID = Session.SessionID.ToString();
        //    //get Authencated user id
        //    string userID = "";
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        userID = User.Identity.GetUserId();
        //    }
        //    //get user list
        //    var cart = db.userCart_tbl.Where(a => a.sessionID == sessionID || a.userID == userID).ToList();

        //    //initail cart model
        //    List<cart> cr = new List<cart>();
        //    cart cartModel = new cart();
        //    //loop them
        //    foreach (var c in cart)
        //    {
        //        cartModel.cartID = c.CartID;
        //        cartModel.product_id = Convert.ToInt32(c.product_ID);
        //        cartModel.p_name = c.productName;
        //        cartModel.totalPrice = Convert.ToDouble(c.totalPrice);
        //        cartModel.VorMainPrice = Convert.ToDouble(c.mainPrice + c.varintPrice);
        //        cartModel.quntity = Convert.ToInt32(c.Quantity);

        //        //get product img
        //        var p = db.product_tbl.Find(Convert.ToInt32(c.product_ID));
        //        cartModel.img = ViewBag.proImg + p.main_image;

        //        if (c.pvvalueID != "null")
        //        {
        //            //convert josn type variant in string
        //            List<int> v = JsonConvert.DeserializeObject<List<int>>(c.pvvalueID);

        //            //initial pvriant list
        //            List<pvriant> li = new List<pvriant>();

        //            //foreach and asin
        //            foreach (var pv in v)
        //            {
        //                var l = db.product_variant_value.Find(pv);
        //                li.Add(new pvriant
        //                {
        //                    pvvID = l.pvvID,
        //                    variantName = l.variant_name,
        //                    varintUnit = l.variant_unit_name,
        //                });
        //            }

        //            //add to list in list proprety
        //            cartModel.varints = li;


        //        }
        //        //cr.Add(cartModel);

        //        cr.Add(new cart
        //        {
        //            varints = cartModel.varints,
        //            quntity = cartModel.quntity,
        //            VorMainPrice = cartModel.VorMainPrice,
        //            totalPrice = cartModel.totalPrice,
        //            p_name = cartModel.p_name,
        //            product_id = cartModel.product_id,
        //            cartID = cartModel.cartID,
        //            img = cartModel.img
        //        });
        //    }


        //    HomeController home = new HomeController();
        //    ViewBag.showMenu = home.menu();


        //    return Json(cr,JsonRequestBehavior.AllowGet);
        //}


        //update cart

       [HttpPost]
       public ActionResult update(cart cart)
        {
            int i=0;
            foreach(var c in cart.cartIDArray)
            {
                
                //get the singla cart 
                var certs = db.userCart_tbl.Find(c);
                var q = cart.quntityArray[i];
                //moduify the quantity colums
                certs.Quantity = q;
                certs.totalPrice = (certs.varintPrice + certs.mainPrice) * q;

                if (certs.discountPrice != null)
                {
                    var cs = certs.discountPrice * q;

                    certs.totalPrice = certs.totalPrice - cs;
                }
                
                
                if (certs.totalPrice == 0)
                {
                    //delete whices quantity is 0
                    db.userCart_tbl.Remove(certs);
                    db.SaveChanges();
                }
                else
                {

                    db.Entry(certs).State = EntityState.Modified;
                    db.SaveChanges();
                }
                i++;

            }
            var home = new HomeController();
            string sessionID = Session.SessionID.ToString();
            ViewBag.cartPanal = home.cartPanl(sessionID);
            ViewBag.simpalCart = home.simpalCart(sessionID);
            return RedirectToAction("Index");
        }


        //delete form cart
        [HttpPost]
        public ActionResult deleteCartProduct(int? id)
        {
            if(id != null)
            {
                var c = db.userCart_tbl.Find(id);
                db.userCart_tbl.Remove(c);
                db.SaveChanges();
                string sessionID = Session.SessionID.ToString();

                var home = new HomeController();
                ViewBag.cartPanal = home.cartPanl(sessionID);
                ViewBag.simpalCart = home.simpalCart(sessionID);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.DenyGet);
        }
        #endregion


        #region //this is Checkout

        [Authorize]
        [HttpGet]
        public ActionResult Checkout()
        {
            checkOut cout = new checkOut();
            double totalPrice = 0;

            var UserID = User.Identity.GetUserId();
            string tokenID = Session.SessionID.ToString();
            //update userCart table user id coluam
            var updatesCart = db.userCart_tbl.Where(a => a.userID == UserID || a.sessionID == tokenID).ToList();
            foreach (var c in updatesCart)
            {
                c.userID = User.Identity.GetUserId();
                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();

                totalPrice = totalPrice +Convert.ToDouble( c.totalPrice);
            }
            cout.totaltaka = totalPrice;

            ViewBag.totalprice = totalPrice;
            ViewBag.showMenu =home.menu();
            ViewBag.footer = home.footerSocial();
            ViewBag.cartPanal = home.cartPanl(tokenID);
            ViewBag.simpalCart = home.simpalCart(tokenID);
            
            ViewBag.paymentMethod = paymentMethod(); ;
            ViewBag.shippingMethod = shippingMethod();
            //show checkout view  user detials
            var user =  adb.Users.Find(UserID);
            cout.email = user.Email;
            cout.aspnetuserid = user.Id;


            //get customer detital
            var  customer = db.customer_tbl.SingleOrDefault(a=>a.aspnetuserid == user.Id);

            if (customer != null)
            {
                cout.firstname = customer.firstname;
                cout.lastname = customer.lastname;
                cout.telephone = customer.telephone;
                cout.customer_id = customer.customer_id;

                //get address detitlas
                var address = db.address_tbl.SingleOrDefault(a => a.customer_id == customer.customer_id);
                cout.address_id = address.address_id;
                cout.shiping_Address = address.shepingAddress;
                cout.shippingPhoneNumber = address.SheppingPhoneNumber;
                cout.customerAddress = address.address_2;

            }


            ViewBag.Message = TempData["message"];
            return View(cout);
        }


        //save the costomer detiels 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(checkOut couts)
        {

            int cID;
            //customer object 
            customer_tbl ctbl = new customer_tbl();

            //address object
            address_tbl atbl = new address_tbl();
            if (ModelState.IsValid)
            {
                //get user email form 
                var user = adb.Users.Find(couts.aspnetuserid);

                //save customer
                
                ctbl.lastname = couts.lastname;
                ctbl.firstname = couts.firstname;
                ctbl.email = couts.email;
                ctbl.newsletter = true;
                ctbl.telephone = couts.telephone;
                ctbl.aspnetuserid = couts.aspnetuserid;
                //save addrerss
                atbl.address_2 = couts.customerAddress;
                atbl.firstname = couts.firstname;
                atbl.lastname = couts.lastname;
                atbl.shepingAddress = couts.shiping_Address;
                atbl.SheppingPhoneNumber = couts.shippingPhoneNumber;
                


                if ((couts.customer_id == null || couts.customer_id == 0) && (couts.address_id == null || couts.address_id == 0))
                {
                    //for new customer
                    ctbl.date_added = DateTime.Now;
                    

                    //save in custeomer database
                    db.customer_tbl.Add(ctbl);
                    db.SaveChanges();

                    //get user id
                    var c = db.customer_tbl.Find(ctbl.customer_id);

                    //save address in database

                    cID = c.customer_id;
                    atbl.customer_id = cID;
                    db.address_tbl.Add(atbl);
                    db.SaveChanges();
                }
                else
                {
                    ctbl.customer_id = couts.customer_id;
                    db.Entry(ctbl).State = EntityState.Modified;
                    db.SaveChanges();
                    cID = ctbl.customer_id;
                    atbl.customer_id = cID;
                    atbl.address_id = Convert.ToInt32(couts.address_id);
                    db.Entry(atbl).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //user identiiy field
                user.PhoneNumber = couts.telephone;
                user.UserName = couts.firstname;
                adb.Entry(user).State = EntityState.Modified;
                adb.SaveChanges();

                //get the payment method
                if (couts.shipping_id != null)
                {
                    var s = db.payment_method_tbl.Find(couts.shipping_id);
                    couts.belingMethod = s.title;
                    couts.sAmount = Convert.ToDecimal(s.amount);
                    couts.totaltaka +=Convert.ToDouble(s.amount);
                }

                //get the shipping method
                if (couts.sid != null) {
                    var s = db.shipping_method.Find(couts.sid);
                    couts.shipping_method = s.Name;
                }
                

                //now start play with order

                //create order_tbl object
                order_tbl otbl = new order_tbl();

                //save data in order_tbl 
                otbl.customer_id = cID;
                otbl.date_added = DateTime.Now;
                otbl.email = couts.email;
                otbl.firstname = couts.firstname;
                otbl.lastname = couts.lastname;
                otbl.payment_address_1 = couts.customerAddress;
                otbl.payment_method = couts.belingMethod ;
                otbl.payment_telephone = couts.telephone;
                otbl.shipping_address_1 = couts.shiping_Address;
                otbl.shipping_telephone = couts.shippingPhoneNumber;
                otbl.total = Convert.ToDecimal(couts.totaltaka);
                otbl.amont = couts.sAmount;
                otbl.shipping_method = couts.shipping_method;
                db.order_tbl.Add(otbl);
                db.SaveChanges();

                //get otbl id
                
                //get order table id
                var order = db.order_tbl.Find(otbl.order_id);
                //next save prodect data in product order table
                order_product_tbl optbl = new order_product_tbl();


                var ucart = db.userCart_tbl.Where(a=>a.userID == user.Id).ToList();

                foreach(var u in ucart)
                {
                    optbl.order_id = order.order_id;
                    optbl.product_id = u.product_ID;
                    optbl.quantity = u.Quantity;
                    optbl.dicount = u.discountPrice;
                    optbl.price = u.varintPrice + u.mainPrice;
                    optbl.total = u.totalPrice;
                    optbl.name = u.productName;
                    optbl.varints = u.pvvalueID;
                    
                    //save single product 
                    db.order_product_tbl.Add(optbl);
                    db.SaveChanges();


                    ////get order_product tbl
                    //var o = db.order_product_tbl.Find(optbl.order_product_id);
                    //if (o.varints != null || o.varints != "null")
                    //{
                    //    List<int> s = JsonConvert.DeserializeObject<List<int>>(o.varints);
                    //    if (s != null)
                    //    {
                    //        foreach (var i in s)
                    //        {
                    //            //update varinat unit

                    //            var pvv = db.product_variant_value.Find(i);
                    //            pvv.quentity -= u.Quantity;
                    //            db.Entry(pvv).State = EntityState.Modified;
                    //            db.SaveChanges();
                    //        }
                    //    }
                      
                    //}

                    ////update product quantity
                    //var p = db.product_tbl.Find(u.product_ID);
                    //p.quantity -= u.Quantity;
                    //db.Entry(p).State = EntityState
                    //    .Modified;
                    //db.SaveChanges();
                }


                //delete cart table data

                foreach (var d in ucart) {
                    db.userCart_tbl.Remove(d);
                    db.SaveChanges();
                }
                //add successfully message
                message = "Your Order Is Successfully Complete. We Contect YOu Very Soon Thank you";

                //update quantity 
            }
            else
            {
                message = "Insert All Data";
            }

            TempData["message"] = message;
            Session["cart"] = null;
            return RedirectToAction("Checkout");
        }

        #endregion


        //show all payment method
        public string paymentMethod()
        {
            string hm = "";
                var sm = db.payment_method_tbl.Where(a => a.status == 1).ToList();
            foreach(var s in sm)
            {
                 hm += " <li>"+
                        "<input type='radio' class='shipping_method' name='shipping_id' value='" + s.shipping_id+ "' id='payment_method_" + s.shipping_id+"_free_shipping'>"+
                        "<label for='payment_method_"+s.shipping_id+"_free_shipping'>"+s.title+"("+s.amount+")</label></li>";
            }

            return hm;
        }


        //show all shipping method
        public string shippingMethod()
        {
            string hm = "";
            var sm = db.shipping_method.ToList();
            foreach (var s in sm)
            {
                hm += " <li>" +
                       "<input type='radio' class='shipping_method' name='sid' value='" + s.id + "' id='shipping_method_" + s.id + "_free_shipping'>" +
                       "<label for='shipping_method_" + s.id + "_free_shipping'>" + s.Name + "</label></li>";
            }

            return hm;
        }

        //show total price 

        public JsonResult cartS(string tokenID)
        {
           

            int i = 0;
            decimal totalPrice = 0;
            //Session["cart"] = "Carts";

            //tokenID = Session.SessionID.ToString();
            var cart = db.userCart_tbl.Where(a => a.sessionID == tokenID).ToList();
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

            
            return Json(html, JsonRequestBehavior.AllowGet);

        }

        //this is appind car icon
        [HttpGet]
        public JsonResult cartsView(string tokenID)
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
            catch (Exception ex)
            { }
            //get user list
            var cart = db.userCart_tbl.Where(a => a.userID == userID || a.sessionID == tokenID).ToList().OrderByDescending(a=>a.CartID);


            foreach (var t in cart)
            {
                var p = db.product_tbl.Find(t.product_ID);
                html += " <div class='product-mini-cart table'>" +
                                            "<div class='product-thumb'>" +
                                            "<a href ='/product/detail/" + t.product_ID + "' class='product-thumb-link'><img src='" + link + p.main_image + "'></a>" +
                                            "<p>Quantity: " + t.Quantity + "</p>" +
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


            return Json(html, JsonRequestBehavior.AllowGet);
        }

        //get session id
        public JsonResult getSession()
        {
            string tokenID = Session.SessionID.ToString();
            return Json(tokenID, JsonRequestBehavior.AllowGet);
        }

    }
}