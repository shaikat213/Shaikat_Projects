
$(document).ready(function () {
    function CIFAccounOpenList() {
        var self = this;
        self.Id = ko.observable();
        self.applicationCIFList = ko.observableArray([]);
        self.applicationCIFOrgList = ko.observableArray([]);
        self.EntryId = ko.observable();
        self.ApplicationId = ko.observable();

        self.CifList = function () {
            $.getJSON("/IPDC/Operations/GetAllCifByAppId?AppId=" + self.ApplicationId(), null, function (data) {
                self.applicationCIFList(data);
            });
        };

        self.CifOrgList = function () {
            $.getJSON("/IPDC/Operations/GetAllCifOrgByAppId?AppId=" + self.ApplicationId(), null, function (data) {
                self.applicationCIFOrgList(data);
            });
        };

        self.SavePrint = function (line) {
            self.EntryId(line.Id);
            self.setUrl();

        }

        self.SaveOrgPrint = function (line) {
            self.EntryId(line.Id);
            self.setOrgUrl();

        }

        self.setUrl = function () {
            if (typeof (self.EntryId()) != 'undefined') {
                window.open('/IPDC/CIF/CifSummeryReport?reportTypeId=PDF&cifId=' + self.EntryId(), '_blank');
            }
        };

        self.setOrgUrl = function () {
            if (typeof (self.EntryId()) != 'undefined') {
                window.open('/IPDC/CIF/OrgCifSummeryReport?reportTypeId=PDF&cifId=' + self.EntryId(), '_blank');
            }
        };

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new CIFAccounOpenList();
    var appId = aabmvm.queryString("AppId");
    aabmvm.ApplicationId(appId);
    aabmvm.CifList();
    aabmvm.CifOrgList();
    ko.applyBindings(aabmvm, document.getElementById("opCifListApplication"));
});