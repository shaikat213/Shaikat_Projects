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

    function NWVerificationHistoryVm() {
        var self = this;
        self.Id = ko.observable();
        self.AppId = ko.observable();
        self.CIFId = ko.observable();
        self.CifNo = ko.observable();
        self.CIFName = ko.observable();
        self.NetWorthId = ko.observable();
        self.NetWorthVerification = ko.observableArray([]);

        self.LoadNWVerificatinHistory = function () {
            self.NetWorthVerification([]);
            var url = '/IPDC/Verification/LoadNWVerificationlHistory?AppId=' + self.AppId() + '&NetWorthId=' + self.NetWorthId() + '&CIFPId=' + self.CIFId();
            return $.ajax({
                type: "GET",
                url: url,
                contentType: "application/json",
                success: function (data) {
                    self.NetWorthVerification(data.NetWorthVerification);
                    self.CifNo(data.CifNo);
                    self.CIFName(data.CIFName);
                },
                error: function () {
                    alert(error.status + "<--and-->" + error.statusText);
                }
            });
        };

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
        self.Details = function (data) {
            debugger;
            console.log(data);
            if (data.Id > 0) {
                var parameters = [
                    {
                        Name: 'Id',
                        Value: data.Id
                    }
                ];
                var menuInfo = {
                    Id: 90,
                    Menu: 'Networth Verification Details',
                    Url: '/IPDC/Verification/NetWorthVerification',
                    Parameters: parameters
                }
                window.parent.AddTabFromExternal(menuInfo);
            }
        }
    }

    appvm = new NWVerificationHistoryVm();
    var qValue = appvm.queryString('AppId');
    appvm.AppId(qValue);
    var networthId = appvm.queryString('NetWorthId');
    appvm.NetWorthId(networthId);
    var cifId = appvm.queryString('CIFPId');
    appvm.CIFId(cifId);
    appvm.LoadNWVerificatinHistory();
    ko.applyBindings(appvm, $('#nwverificationhistory')[0]);

});