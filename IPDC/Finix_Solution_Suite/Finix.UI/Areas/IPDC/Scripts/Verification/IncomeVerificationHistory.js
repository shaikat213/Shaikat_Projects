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

    function IncomeVerificationHistoryVm() {
        var self = this;
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.CifId = ko.observable();
        self.Name = ko.observable();
        self.CIFNo = ko.observable();
        self.ApplicationNo = ko.observable();
        self.VerificationDate = ko.observable();
        self.VerificationDateText = ko.observable();
        self.IncomeList = ko.observableArray([]);

        self.LoadData = function () {
            //
            if (self.CifId() > 0) {
                $.getJSON("/IPDC/CIF/GetCIF_Info/?cifId=" + self.CifId(),
                    null,
                    function (data) {
                        console.log(data);
                        self.Name(data.Name);
                        self.CIFNo(data.CIFNo);

                    });

            }
        }


        self.LoadIncomeHistory = function () {
            //console.log(self.AppId());
            if (self.CifId() > 0) {
                $.getJSON("/IPDC/Verification/LoadIncomeHistory/?AppId=" + self.ApplicationId() + '&CifId=' + self.CifId() + '&Id=' + self.Id(),
                    null,
                    function (data) {
                        self.IncomeList(data);
                    });
            }
        }
        self.Initialize = function () {

            if (self.CifId() > 0 || self.Id()) {
                self.LoadIncomeHistory();
                self.LoadData();
            }
        }

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
                    Menu: 'Income Verification Details',
                    Url: '/IPDC/Verification/IncomeVerification',
                    Parameters: parameters
                }
                window.parent.AddTabFromExternal(menuInfo);
            }
        }
    }


    appvm = new IncomeVerificationHistoryVm();
    var qValue = appvm.queryString('AppId');
    appvm.ApplicationId(qValue);
    var cifId = appvm.queryString('CIFPId');
    appvm.CifId(cifId);
    var cpvId = appvm.queryString('Id');
    appvm.Id(cpvId);
    appvm.Initialize();
    ko.applyBindings(appvm, $('#incomeverificationhistory')[0]);

});