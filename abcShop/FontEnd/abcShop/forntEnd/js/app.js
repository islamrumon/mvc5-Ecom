






///this script for show product modal
function quckView(id) {

    var url = '/categorys/quickview/' + id;
    $("#loadUrl").load(url, function () {
        $("#showModal").modal('show');
        $('body').append("<script type='text/javascript' src='/forntEnd/js/theme.js'><\/script>");
        $('body').append("<script type='text/javascript' src='/forntEnd/js/app.js'><\/script>");
        
        
    });

}






//this is for app js vretaeing function for handel client site
//var myid = [];
//var price;
//function changePrice(pr, id) {
//    add(id, pr);
//};

////get select value

//$("#changePrice1").change( function () {

//    var id = $("#changePrice1 :selected").data('id');
//    var pr = $("#changePrice1 :selected").data('price');
//    add(id, pr);
//});


//function add(id, price) {
    
//    var result = 0;
//    var flag = false;
//    $.each(myid, function (index, value) {
//        if (value.id == id) {
//            value.price = price;
//            flag = true;
//        }
//    });
//    // add data in array
//    if (!flag) {
//        myid.push({ id: id, price: price });
//    }

//    $.each(myid, function (index, value) {
//        result = result + value.price;
//        //console.log(result);
//    });
//    //console.log(myid);
//    var mainPrice = parseFloat($("#mainPrice").text());
//    $("#mainPrice").hide();
//    var s = mainPrice + result;
//    //add new mprice
//    $("#cprice").text(s);
//}
//this is end function
var myid = [];
var price;
var quintity = 1;

function changePrice(pr, id) {
    add(id, pr, quintity);
    //qty();
};
//get select value

$("#changePrice1").change(function () { 
    var id = $("#changePrice1 :selected").data('id');
    var pr = $("#changePrice1 :selected").data('price');
    add(id, pr, quintity);
    
});

function add(id, price, quintity) {
    var flag = false;
    $.each(myid, function (index, value) {
        if (value.id === id) {
            value.price = price;
            flag = true;
        }
    });
    // add data in array
    if (!flag) {
        myid.push({ id: id, price: price });
    }

    pPrice();
}

function pPrice() {

    var result = 0;
    if (myid !== null) {
        $.each(myid, function (index, value) {
            result += value.price * quintity;
            //console.log(result);
        });
    //console.log(myid);
    }
   
    var mainPrice = parseFloat($("#mainPrice").text());
    $("#mainPrice").hide();
    var s = (mainPrice * quintity) + result;
    //add new mprice
    $("#cprice").text(s);
}



//function qty() {
//    debugger
//    //var q = parseInt(document.forms["cartForm"]["quentity"].value);
//    var q = parseInt(document.getElementById("quentity").value)
//    quintity = q;
//    pPrice();
//    console.log(quintity);
//}

//change price par quintity 

//delete cart product
function deleteCartProduct(id) {

    $.ajax({
        url: '/cart/deleteCartProduct/' + id,
        type: 'post',
        success: function (result) {
            $.notify("Product Is Remove form Cart", "info");
            $("#de_" + id).remove();
            getSession();
        }
    });
}

//Get Cart form data
var pvv = [];

function addCart() {
  debugger

    var d = $("#cartForm").serialize();
     
    var f = $("#cartForm");
    
    var b = f.find("input[name*='pvvID'], select[name*='pvvID']").serializeArray();
    var pvvID = [];
    $.each(b, function (index, v) {
        pvvID.push(v.value);
        //pvvID[] = [v.value];
    })
    traditional = true,
        f.serialize();
    var quentity = parseInt(document.getElementById("quentity").value)
    var id = parseInt(document.getElementById("product_id").value)
    
    $.ajax({
        url: '/Cart/pvvCart',
        data: { pvv: pvvID, product_id: id, Quantity: quentity },
        type: 'post',
        success: function () {
           
            //pvvID = [];
        },
         error: function () {
             //$.notify("BOOM!", "error");
             $.notify("Add To Cart", "success");
             getSession();
        }
    });
    pvvID.length = 0;
}

function addCarts(id) {
    debugger
    var pvvIDs = [];
    var quentity = 1;

    $.ajax({
        url: '/Cart/pvvCart',
        data: { pvv: pvvIDs, product_id: id, Quantity: quentity },
        type: 'post',
        success: function (result) {
           
            if (result !== null) { 
                window.location.href = result;
            }
        },
        error: function () {
            $.notify("Add To Cart", "success");
            getSession();
        }
        
    });
}


//this fucntion for show cart data

function getSession() {
    $.ajax({
        url: '/Cart/getSession',
        type: 'get',
        success: function (tokenID) {
            //alert(tokenID);
           cartsView(tokenID);
            
        }
    })
}

function cartsView(tokenID) {
  
    $.ajax({
        url: '/Cart/cartsView',
        type: 'get',
        data: { tokenID: tokenID},
        success: function (ts) {
            $('.list-mini-cart-item').empty();
            $('.list-mini-cart-item').append(ts);
                //$.each(ts, function (index, t) {
                //    cm += '<div class="product-mini-cart table">'
                //    '<div class="product-thumb">' +
                //        '<a href="/product/detail/' + t.product_ID + '" class="product-thumb-link"><img src="' + link + t.main_image + '"></a>' +
                //        '<p>Quantity: ' + t.Quantity + '</p>' +
                //        '</div>' +
                //        '<div class="product-info">' +
                //        '<h4 class="product-title" style="font-size: 14px"><a href="/product/detail/' + t.product_ID + '" >' + t.productName + '</a></h4></hr>' +
                //        '<div class="product-price">' +
                //        '<ins><span>৳ ' + t.totalPrice + '</span></ins>' +

                //        '</div>' +

                //        '</div></div>';
                //});
           
            
            //console.log(ts);
        }

    });

    //next cart icon

    
    $.ajax({
        url: '/Cart/cartS',
        type: 'get',
        data: { tokenID: tokenID},
        success: function (result) {
            $('#cat').empty();
            $('#cat').append(result);
            //console.log(result);
        }
    });
}

//for sumpalCart() 
//function sumpalCart() {
    
//    $('#simpalCart').html('');
//    var htmls = "<div class='list-mini-cart-item'  style='max-height: 300px; overflow: auto'>";
//    var price =0;
//    $.ajax({
//        url: '/cart/sumpalCart',
//        type: 'post',
//        success: function (result) {

//            $.each(result, function (index, value) {
//                htmls += "<div class='product-mini-cart table'>" +
//                    "<div class='product-thumb'>" +
//                    "<a href='/product/detail/" + value.product_id + "' class='product-thumb-link'><img  src='" + value.img + "' style='width: 100px; height: 100px;'></a>" +
//                    //<a href="Home/quick_view" class="quickview-link fancybox fancybox.iframe"><i class="fa fa-search" aria-hidden="true"></i></a>
//                    "</div>" +
//                    "<div class='product-info'>" +
//                    "<h3 class='product-title'><a href='javascript:void(0)'>" + value.p_name + "</a></h3>" +
//                    "<div class='product-price'>" +
//                    "<ins><span>৳  " + value.VorMainPrice + "</span></ins>" +

//                    "</div>" +
//                    "<div class='product-rate'>" +
//                    "<div class='product-rating' style='width:100%'></div>" +
//                    "</div>" +
//                    "</div>" +
//                    "</div >";
//                price = price + value.VorMainPrice;
//            });
            
//            htmls += "</div><div class='mini-cart-total  clearfix'>" +
//                "<strong class='pull-left title18'> TOTAL</strong>" +
//                "<span class='pull-right color title18'>৳ " + price + "</span>" +
//                "</div>";

//            $("#simpalCart").html(htmls)
//        }
        

//    });
//}


//delete form cart table



//dynamicaly call a function

//change 

function price(taka) {
    debugger
    if (taka !== 0) {
        var s = $(".totalChangabl").get.value();
        $(".totalChangabl").hide();
        s += taka;
        $(".tt").val(s);
    }
}

//this is varinta by product

//search product
//function search() {
//    debugger
//    var pageURL = $(location).attr("href");
//    var url = "http://localhost:8787/product/search";
//    if (url === pageURL) {
//        alert(ok);
//    } else {
//        window.location.href = url;
//    }
//    alert(pageURL);
//}


//this is for product show by varinat

