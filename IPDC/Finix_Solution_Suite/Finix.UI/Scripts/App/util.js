/// <reference path="../toastr.js" />
/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
var Messager = {
    ShowMessage: function (resText) {
        var resObj = eval('(' + resText + ')');
        if (resObj.Result == 'OK') {
            toastr.success(resObj.Message);
        }
        if (resObj.Result == "ERROR") {
            toastr.error(resObj.Message);
        }
    }
};
