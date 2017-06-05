$(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function DeveloperListVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.EntryId = ko.observable();

        self.Details = function (data) {
            var parameters = [{
                Name: 'Id',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 93,
                Menu: 'Developer Entry',
                Url: '/IPDC/Developer/DeveloperEntry',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new DeveloperListVm();
    var qValue = aabmvm.queryString("entryId");
    aabmvm.EntryId(qValue);
    var appId = aabmvm.queryString("AppId");
    aabmvm.Id(appId);
    ko.applyBindings(aabmvm, document.getElementById("developerList"));
});