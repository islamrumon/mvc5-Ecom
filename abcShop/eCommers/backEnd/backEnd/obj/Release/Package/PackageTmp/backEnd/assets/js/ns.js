// add row
function changeView(thisTag, id, name) {

    $(thisTag).toggleClass('btn-success');

    if ($("#appendArea").find("#appendVarientUnit" + id).length > 0) {
        $("#appendVarientUnit" + id).remove();
    } else {

        $.ajax({
            url: '/purchase/getUnit/' + id,
            type: 'post',
            dataType: 'json',
            success: function (result) {
                var rt = '<div id="appendVarientUnit' + id + '"><h4 class="content-title">' + name + '</h4><table id="veTable' + id + '" class="table" ><tbody><input type="hidden" name="varient_id" value="' + id + '" /><tr id="ar' + result.variant_unit_id + '"><td><select name="variant_unit_id" class="form-control">';
                $.each(result.list, function (index, value) {
                    rt += '<option value="' + value.variant_unit_id + '">' + value.unit_name + '</option>';

                });
                rt += "</select></td><td><input class=\"form-control\" type=\"text\" placeholder=\"Quentity\" name=\"quentity\"/></td>" +
                    "<td><input class=\"form-control\" type=\"text\" placeholder=\"+/-Price\" name=\"pVprice\" /></td>" +
                    "<td><button class=\"btn btn-danger removeVfrom\" type=\"button\" ><i class=\"fa fa-trash\"></i></button></td>" +
                    "</tr><tbody></table><button class=\"btn btn-success\" type=\"button\" onclick=\"addRow('#veTable" + id + "')\"  ><i class=\"fa fa-edit\"></i></button></div>";

                $("#appendArea").append(rt);
            },
            error: function () {
                $.notify("BOOM!", "error");
            }

        });
    }
}

$("#appendArea").on('click', '.removeVfrom', function (e) {
    alert(e);
    e.preventDefault();
    $(this).parent().parent().remove();

});

function addRow(e) {

    $(e + ' tbody').each(function () {
        var addRow = $(this).find("tr:first").html();
        $(this).append('<tr>' + addRow + '</tr>');
    });
    return this;
}