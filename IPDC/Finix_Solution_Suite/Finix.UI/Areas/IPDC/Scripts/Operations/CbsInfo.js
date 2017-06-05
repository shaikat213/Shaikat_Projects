
$(document).ready(function () {
    function cifs() {
        var self = this;

        self.Id = ko.observable();
        self.CIFNo = ko.observable();
        self.CIFName = ko.observable();
        self.CBSCIFNo = ko.observable();


    }
    function CbsInfoVm() {
        var self = this;
        self.applicationCIFList = ko.observableArray([]);
        self.ApplicationId = ko.observable();
        
        self.CBSAccountNo = ko.observable();
        self.CBSBranchId = ko.observable().extend({
            pattern: {
                message: 'Branch ID must be Numeric Value',
                params: /^([0-9\-])+$/
            },
            minLength: { params: 4, message: "Branch ID must be 4 digit long" },
            maxLength: { params: 4, message: "Branch ID must be 4 digit long" }
        });
        self.InstrumentNo = ko.observable();
        self.InstrumentDate = ko.observable();
        self.InstrumentDateText = ko.observable();
        self.AccountOpenDate = ko.observable();
        self.ProductType = ko.observable();
        self.AccountOpenDateTxt = ko.observable();
        self.MaturityDate = ko.observable();
        self.MaturityDateTxt = ko.observable();
        self.InstrumentDispatchStatus = ko.observable();
        self.InstrumentDispatchStatusList = ko.observableArray([]);
        self.MaturityAmount = ko.observable();
        self.MaturityAmountFormatted = ko.pureComputed({
            read: function () {
                if (self.MaturityAmount() > 0)
                    return self.MaturityAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.MaturityAmount(isNaN(value) ? 0 : value);
            },
            owner: self
        });

        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {

            if (self.errors().length === 0)
                return true;
            else {
                return false;
            }

        });

        self.CifList = function () {
            $.getJSON("/IPDC/Operations/GetCBSData?AppId=" + self.ApplicationId(), null, function (data) {
                self.applicationCIFList([]);
                $.each(data.CIFs, function(index, value) {
                    var cif = new cifs();
                    cif.CIFNo(value.CIFNo);
                    cif.Id(value.Id);
                    cif.CIFName(value.CIFName);
                    cif.CBSCIFNo(value.CBSCIFNo);
                    self.applicationCIFList.push(cif);

                });
                self.CBSAccountNo(data.CBSInfo.CBSAccountNo);
                self.CBSBranchId(data.CBSInfo.CBSBranchId);
                $.when(self.GetInstrumentStateList()).done(function () {
                    self.InstrumentDispatchStatus(data.CBSInfo.InstrumentDispatchStatus);
                    self.InstrumentNo(data.CBSInfo.InstrumentNo);
                });
                self.InstrumentDate(moment(data.CBSInfo.InstrumentDate));
                self.AccountOpenDate(moment(data.CBSInfo.AccountOpenDate));
                self.MaturityDate(moment(data.CBSInfo.MaturityDate));
                self.ProductType(data.CBSInfo.ProductType);
                self.MaturityAmount(data.CBSInfo.MaturityAmount);
            });
        };

        self.GetInstrumentStateList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/GetInstrumentStateList',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.InstrumentDispatchStatusList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SaveCbsInfo = function () {
            self.InstrumentDateText(moment(self.InstrumentDate()).format('DD/MM/YYYY'));
            self.AccountOpenDateTxt(moment(self.AccountOpenDate()).format('DD/MM/YYYY'));
            self.MaturityDateTxt(moment(self.MaturityDate()).format('DD/MM/YYYY'));
            var submitCbsInfo = {
                CIFs: self.applicationCIFList(),
                ApplicationId: self.ApplicationId(),
                CBSAccountNo: self.CBSAccountNo(),
                InstrumentNo: self.InstrumentNo(),
                InstrumentDate: self.InstrumentDate(),
                InstrumentDateText: self.InstrumentDateText(),
                AccountOpenDate: self.AccountOpenDate(),
                AccountOpenDateTxt: self.AccountOpenDateTxt(),
                MaturityDate: self.MaturityDate(),
                MaturityDateTxt: self.MaturityDateTxt(),
                InstrumentDispatchStatus:self.InstrumentDispatchStatus(),
                MaturityAmount: self.MaturityAmount(),
                CBSBranchId: self.CBSBranchId()
            };
            $.ajax({
                url: '/IPDC/Operations/SaveCBSinfo',
                //cache: false,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitCbsInfo),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    //self.Reset();
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var cbsInfoVm = new CbsInfoVm();
    //var qValue = cbsInfoVm.queryString("entryId");
    //cbsInfoVm.EntryId(qValue);
    var appId = cbsInfoVm.queryString("AppId");
    cbsInfoVm.ApplicationId(appId);
    cbsInfoVm.GetInstrumentStateList();
    cbsInfoVm.CifList();
    ko.applyBindings(cbsInfoVm, document.getElementById("cbsInfo"));
});