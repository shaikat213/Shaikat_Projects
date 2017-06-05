///// <reference path="../knockout-3.4.1.js" />


//function SomeViewModel() {
//    var self = this;

//    this.ArrayOfModels = ko.mapping.fromJS([]);

//    self.GetModelsByAjax = function () {
//        var productId = $('#ddlProduct').val();
//        $.ajax({
//            type: 'POST',
//            url: '/admin/management/',
//            data: { productId: productId },
//            success: function (data) {
//                self.mapData(data);
//            },
//            dataType: 'json'
//        });
//    };

//    self.mapData = function (models) {
//        ko.mapping.fromJS(models, self.ArrayOfModels);
//    };
//}

//ko.applyBindings(new SomeViewModel());

function getIntToYear(val) {
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var m = val % 12;
    var y = Math.floor(val / 12);
    return months[m] + ', ' + y;

}
$(document).ready(function () {
    $(function () {
        $("#slider-range").slider({
            range: true,
            min: 24192,
            max: 24299,
            values: [24192, 24223],
            slide: function (event, ui) {
                $("#amount").val(getIntToYear(ui.values[0]) + " - " + getIntToYear(ui.values[1]));
            }
        });
        $("#amount").val(getIntToYear($("#slider-range").slider("values", 0)) +
          " - " + getIntToYear($("#slider-range").slider("values", 1)));
    });

    $(function () {
        $("#slider-range2").slider({
            range: true,
            min: 24192,
            max: 24299,
            values: [24205, 24243],
            slide: function (event, ui) {
                $("#amount2").val(getIntToYear(ui.values[0]) + " - " + getIntToYear(ui.values[1]));
            }
        });
        $("#amount2").val(getIntToYear($("#slider-range2").slider("values", 0)) +
          " - " + getIntToYear($("#slider-range2").slider("values", 1)));
    });
});