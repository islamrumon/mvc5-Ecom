
//function createCat() for show Partial View For creating data
$(document).ready(function () {

    $("#createCat").click(function () {
                
        $(".modal-title").text('Create Category');
        var url = '/admin/createCat';

        $("#loadUrl").load(url, function () {
            $("#showModal").modal('show');
        });

    });

    
            
});

//add gallleryImage
function addGalleryImage(id) {
 
    $(".modal-title").text('Add Gallery Image');
    var url = '/manageProducts/addGalleryImage/'+id;

    $("#loadUrl").load(url, function () {
        $("#showModal").modal('show');
    });
}
//End The Showing Partial view in modal


//append the image in gallery
$('#appendIMG').click(function () {

    var count = ($("#image input").length) + 1;

    var appendImage = '<div class="col-md-4" >' +
        '<input id="file' + count + '" name="file" type="file" class="form-control" onchange="return fileValidation(' + count + ')" /></div>' +
        '<div class="col-md-4">' +
        '<div  id="imagePreview' + count + '"></div>' +
        
        '</div>' +
        '<div class="col-md-4">' +
        '<div id="error' + count + '"></div></div>';
    $('#image').append(appendImage);

});

var cr= 0 ;
//append the image in gallery in edit page
function appendImg() {
     
    var count = cr++;
     
    var appendImage = '<div id="areas' + count +'" class="col-md-4 custom-file">' + 
        '<div  id="imagePreview' + count + '">' +
        '<label for="file' + count + '" class="custom-file-upload">' +
        '<span class="galleryImg"><i class="fa fa-cloud-upload"></i>Upload Gallery Image</span>'+
        '<div id="error' + count + '"></div></label>' +
        '</div > ' +
        '<input id="file'+ count + '" name="file" class="inputfiled" type="file" onchange="return fileValidation(' + count + ')" />'+
         '<div>' +
        '<a href="javascript:void(0)" class="btn btn-danger" onclick="removimg(' + count + ')"><i class="pe-7s-trash"></i></a>' +
        '</div><div>';
    $('#image').append(appendImage); 

};


function removimg(num) {

    $('#areas' + num).remove();
}

//append inputfield
function appininputForm() {

    var cou = ($("#appinForm input").length) + 1;

    var appendForm = 
        '<div class="input-group mb-3" id="area' + cou +'">'+
        '<input class="form-control" placeholder="Type Varient Value" type="text" required name="unit_name" id="unit_name' + cou +'" />'+
            
            '<div class="input-group-append">'+
               '<button class="btn btn-danger" onclick="removiInputForm(' + cou +')"><i class="pe-7s-trash"></i></button>'+
        '</div></div>';



    $("#appinForm").append(appendForm);
}

//remove removiInputForm fild
function removiInputForm(num) {
    $('#area' + num).remove();
}

//create product
$("#createbtnformData").click(function () {
    $("#createFormData").submit();
});

//function editCat() for show update category
var editCat = function (id) {
    $(".modal-title").text('Edit Category');
    var url = '/admin/editCat/' + id;
    $("#loadUrl").load(url, function () {
        $("#showModal").modal('show');
    });
}

//function galleryTab(id) {
//    $.ajax({
//        url: '/manageProducts/editGallery/' + id, 
//        type: 'get',
//        dataType: 'html',
//        success: function (data) {
//            $('#gallery').html(data);
//        },
//        error: function () {
//            alert("Thare are some error");
//        }
//    });
//}

//var arr = [];
////function deleteGallery for deleting gallery
//function deleteGallery(id) {
//    debugger
//    arr.push(id);
//    $("#tr_" + id).remove();
//}

////submit edite product
//$("#btnformData").click(function () {
//    $.ajax({
//        url: '/manageProducts/deleteGallery/',
//        type: 'post',
//        data: { id: arr },
//        success: function (data) {
//            $.notify("Gallery Images is Deleted Successfully", "success");
//        },
//        error: function () {
//            $.notify("BOOM!", "error");
//        }
//    })
//    $("#formData").submit();
//});


//this is image validation
function fileValidation(count) {
    var fileInput = document.getElementById('file' + count);
    var filePath = fileInput.value;
    var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.gif)$/i;
    if (!allowedExtensions.exec(filePath)) {
       // $('#error').show();
        var error = 'Please upload file having extensions .jpeg/.jpg/.png/.gif only.';
        document.getElementById('error'+ count).innerHTML = '<span class="text-danger">' + error +'</span>'
        //alert();
        fileInput.value = '';
        return false;
    } else {
        //Image preview
        if (fileInput.files && fileInput.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#error' + count).hide();
                $('#editHidden' + count).hide();
                document.getElementById('imagePreview' + count).innerHTML = '<img width="200" height="200"  src="' + e.target.result + '"/>';
            };
            reader.readAsDataURL(fileInput.files[0]);
        }
    }
}

//this is image validation
function fileValidationBanners() {
    debugger
    var fileInput = document.getElementById('banner');
    var filePath = fileInput.value;
    var allowedExtensions = /(\.jpg|\.jpeg|\.png|\.gif)$/i;
    if (!allowedExtensions.exec(filePath)) {
        // $('#error').show();
        var error = 'Please upload file having extensions .jpeg/.jpg/.png/.gif only.';
        document.getElementById('errorBanner').innerHTML = '<span class="text-danger">' + error + '</span>'
        //alert();
        fileInput.value = '';
        return false;
    } else {
        //Image preview
        if (fileInput.files && fileInput.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#errorBanner').hide();
                $('#editHiddenBanner').hide();
                document.getElementById('imagePreviewBanner').innerHTML = '<img width="140" height="100" class="img-thumbnail" src="' + e.target.result + '"/>';
            };
            reader.readAsDataURL(fileInput.files[0]);
        }
    }
}


//delete deleteVariant
//$("deleteVariant").click(function (id)){}
function deleteVariant(id) {
    var x = confirm("Are you sure you want to delete?");
    if (x) {
        $.ajax({
            type: 'post',
            url: '/settings/deleteVarirant',
            data: { id: id },
            success: function () {
                $.notify("Variant Deleted", "success");
                $("#tr_" + id).remove();
            },
            error: function () {
                $.notify("BOOM!", "error");
            }
        });
        return true;
    }
    else {
        return false;
    }
    
}



//this function for edit order
var ids =[]

$("#changePrice1").change(function () {

    var id = $("#changePrice1 :selected").data('id');
    var pr = $("#changePrice1 :selected").data('price');
    addVarint(pr, id);

});

function changePrice(price, id) {

    addVarint(price,id);
}

function addVarint(price, id) {

    var flax = true
    $.each(ids, function (index, value) {
        if (value.id === id) {
            value.price = price;
            flax = false;
        }
    });

    //add data in array
    if (flax) {
        ids.push({ id: id, price: price });
    }

    pPrice();
}
var quintity = 1;
//print all data
function pPrice() {
    debugger
    var result = 0;
    if (ids !== null) {
        $.each(ids, function (index, value) {
            result += value.price;
        });
    }

    var mainPrice = parseFloat($("#mainPrice").text());
    $("#mainPrice").hide();
    var s = (mainPrice * quintity) + result;
    $("#cprice").text(s);
}


function qty() {
   
    //var q = parseInt(document.forms["cartForm"]["quentity"].value);
    var q = parseInt(document.getElementById("quentity").value)
    quintity = q;
    pPrice();
    console.log(quintity);
}




//Get Cart form data
var pvv = [];

function addCart() {
    debugger
    var f = $("#cartForm");
    var b = f.find("input[name*='pvvID'], select[name*='pvvID']").serializeArray();
    var pvvID = [];
    $.each(b, function (index, v) {
        pvvID.push(v.value);
        //pvvID[] = [v.value];
    });
   
        
    var quentity = parseInt(document.getElementById("quentity").value)
    var id = parseInt(document.getElementById("product_id").value)
    var order_product_id = parseInt(document.getElementById("order_product_id").value)
    var order_id = parseInt(document.getElementById("order_id").value)
    if (pvvID.length === 0) {
        pvvID = null
    }
    var myJsonString = JSON.stringify(pvvID);
    
    $.ajax({
        
        url: '/order/productUpdate',
        data: { pvv: myJsonString, product_id: id, Quantity: quentity, opID: order_product_id, oID: order_id},
        type: 'post',

        success: function (result) {
            $.notify("Product Updated Successfully", "success");
            window.location.reload();
            //pvvID = [];
        },
        error: function () {
            $.notify("BOOM!", "error");
        }
    });
    pvvID.length = 0;
}

//this is use purcesh select option
