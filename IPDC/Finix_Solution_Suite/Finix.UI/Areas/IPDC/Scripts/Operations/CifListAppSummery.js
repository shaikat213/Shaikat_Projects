
$(document).ready(function () {
    function CifListAppSummery() {
        var self = this;
        self.Id = ko.observable();
        self.applicationCIFList = ko.observableArray([]);
        self.EntryId = ko.observable();
        self.EntryAppId = ko.observable();
        self.ApplicationId = ko.observable();

        self.CifList = function () {
            $.getJSON("/IPDC/Operations/GetAllCifByAppId?AppId=" + self.ApplicationId(), null, function (data) {
                self.applicationCIFList(data);
            });
        };

        self.SavePrint = function (line) {
            self.EntryId(line.Id);
            self.EntryAppId(line.ApplicationId);
            self.setUrl();

        }
        self.setUrl = function () {
            if (typeof (self.EntryId()) != 'undefined') {
                window.open('/IPDC/Operations/AppSummeryReportDeposit?reportTypeId=PDF&AppId=' + self.EntryAppId(), '_blank');
            }
        };

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new CifListAppSummery();
    var appId = aabmvm.queryString("AppId");
    aabmvm.ApplicationId(appId);
    aabmvm.CifList();
    ko.applyBindings(aabmvm, document.getElementById("ciflistAppSummery"));
});