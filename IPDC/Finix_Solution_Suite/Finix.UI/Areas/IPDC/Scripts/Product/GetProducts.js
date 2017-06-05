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
        self.Details = function (data) {
            var parameters = [{
                Name: 'Id',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 93,
                Menu: 'Product Entry',
                Url: '/IPDC/Product/ProductEntry',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.Message = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.Id
            }];
            var menuInfo = {
                //Id: 93,
                //Menu: 'Application',
                Url: '/IPDC/Messaging/NewMessage',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
    }

    var vm = new SearchApplication();
    //vm.Search();
    ko.applyBindings(vm, document.getElementById("SearchApplicationVm"));
});