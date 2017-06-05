$(function () {
    $('#VarificationDateId').datetimepicker({ format: 'DD/MM/YYYY' });

});
$(document).ready(function () {

    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });
    function CIBVerificationHistoryVm() {
        var self = this;
        self.Id = ko.observable();
        self.AppId = ko.observable();
        self.CIFId = ko.observable();
        self.CifNo = ko.observable();
        self.CIFName = ko.observable();
        self.CibType = ko.observable();
        self.CIBPersonal = ko.observableArray([]);
        self.CIBOrganizatioanl = ko.observableArray([]);

        self.LoadCibPersonalHistory = function () {
            self.CIBPersonal([]);
            self.CIBOrganizatioanl([]);
            var url = '/IPDC/Verification/LoadCibPersonalHistory?AppId=' + self.AppId() + '&CIFPId=' + self.CIFId() + '&Id=' + self.Id() + '&CibType=' + self.CibType();
                return $.ajax({
                    type: "GET",
                    url: url,
                    contentType: "application/json",
                    success: function (data) {
                        if (typeof (data.CIBPersonal) != "undefined"  && data.CIBPersonal.length > 0) {
                            self.PersonalVisible(true);
                            self.OrganizationalVisible(false);
                        }
                        if (data.CIBOrganizational.length > 0) {
                            self.PersonalVisible(false);
                            self.OrganizationalVisible(true);
                        }
                        self.CIBPersonal(data.CIBPersonal);
                        self.CIBOrganizatioanl(data.CIBOrganizational);
                        self.CifNo(data.CifNo);
                        self.CIFName(data.CIFName);
                    },
                    error: function () {
                        alert(error.status + "<--and-->" + error.statusText);
                    }
                });
            
        };

        self.PersonalVisible = ko.observable(false);
        self.OrganizationalVisible = ko.observable(false);

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
        self.Details = function (data) {
            if (data.Id > 0) {
                var parameters = [
                    {
                        Name: 'Id',
                        Value: data.Id
                    }
                ];
                var menuInfo = {
                    Id: 90,
                    Menu: 'CIB Details',
                    Url: '/IPDC/Verification/CIB',
                    Parameters: parameters
                }
                window.parent.AddTabFromExternal(menuInfo);
            }
        }
    }

    appvm = new CIBVerificationHistoryVm();
    
    var qValue = appvm.queryString('AppId');
    appvm.AppId(qValue);
    var cifId = appvm.queryString('CIFPId');
    appvm.CIFId(cifId);
    appvm.CibType(appvm.queryString('CibType'));
    appvm.LoadCibPersonalHistory();
    ko.applyBindings(appvm, $('#cibverificationhistory')[0]);

});