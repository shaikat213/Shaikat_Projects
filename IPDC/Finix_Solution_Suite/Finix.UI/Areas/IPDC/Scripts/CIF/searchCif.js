$(function () {
    //$('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#NextFollowUpEdit').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#NextFollowUpDetails').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
});
$(document).ready(function () {

    function SearchApplication() {
        var self = this;
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.Search = function () {
        //    var data = self.test() ? self.test() : "";
         
        //    $.ajax({
        //        type: "GET",
        //        url: '/IPDC/CIF/GetCifBySearch?test=' + data,
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (data) {
                    
        //            self.LoadData(data);
        //        },
        //        error: function (error) {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        }
        self.Details = function(data) {
            var parameters = [{
                Name: 'cifpid',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 93,
                Menu: 'CIF Personal',
                Url: '/IPDC/CIF/CIF_Personal',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
    }

    var vm = new SearchApplication();
    //vm.Search();
    ko.applyBindings(vm, document.getElementById("SearchApplicationVm"));
});