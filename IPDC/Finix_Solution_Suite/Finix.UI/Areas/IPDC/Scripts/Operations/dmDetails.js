
$(document).ready(function () {
    $(function () {

    });
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });
    function dmDetails(data) {
        var self = this;
        self.Id = ko.observable();
        self.DisbursementMemoId = ko.observable();
        self.DisbursementMode = ko.observable();
        self.ChequeDeliveryOption = ko.observable();
        self.DisbursementAmount = ko.observable();
        self.IPDCBankAccountId = ko.observable();
        self.BankName = ko.observable();
        self.BranchName = ko.observable();
        self.RoutingNo = ko.observable();
        self.AccountName = ko.observable();
        self.AccountNo = ko.observable();
        self.ChequeNo = ko.observable();
        self.ChequeIssuedTo = ko.observable();
        self.ChequeDate = ko.observable();
        self.ClientAccountNo = ko.observable();
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.DisbursementMemoId(data ? data.DisbursementMemoId : 0);
            self.DisbursementMode(data ? data.DisbursementMode : 0);
            self.ChequeDeliveryOption(data ? data.ChequeDeliveryOption : 0);
            self.DisbursementAmount(data ? data.DisbursementAmount : 0);
            self.IPDCBankAccountId(data ? data.IPDCBankAccountId : 0);
            self.BankName(data ? data.BankName : "");
            self.BranchName(data ? data.BranchName : "");
            self.RoutingNo(data ? data.RoutingNo : "");
            self.AccountName(data ? data.AccountName : "");
            self.AccountNo(data ? data.AccountNo : "");
            self.ChequeNo(data ? data.ChequeNo : "");
            self.ChequeIssuedTo(data ? data.ChequeIssuedTo : "");
            self.ChequeDate(data && data.ChequeDate ? moment(data.ChequeDate) : moment());
            self.ClientAccountNo(data ? data.ClientAccountNo : "");
        }
    }
    function DisbursmentMemoDetailVm() {
        var self = this;
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.ProposalId = ko.observable();
        self.DmDetailList = ko.observableArray([]);
        self.DisbursementModes = ko.observableArray([]);
        self.ChequeDeliveryOptions = ko.observableArray([]);
        self.DisbursementMemoId = ko.observableArray([]);
        self.IPDCBankAccounts = ko.observableArray([]);
        self.ApplicationNo = ko.observable();
        self.AccountTitle = ko.observable();
        self.ProductName = ko.observable();
        self.TotalLoanAmount = ko.observable();
        self.CurrentDisbursementAmount = ko.observable();
        self.TotalDisbursedAmount = ko.observable();
        self.DisbursedDate = ko.observable();
        self.AmountCheck = ko.computed( function() {
            var currentDisbursementAmount = self.CurrentDisbursementAmount();
            $.each(self.DmDetailList(), function(index, value) {
                if (value.DisbursementAmount()) {
                    currentDisbursementAmount -= value.DisbursementAmount();
                }
            });
            if (currentDisbursementAmount === 0)
                return true;
            else
                return false;
        });
        self.AddDMDetails = function () {
            self.DmDetailList.push(new dmDetails());
        }
        self.RemovedDMDetails = ko.observableArray([]);
        self.RemoveDMDetails = function (line) {
            if (line.Id() > 0)
                self.RemovedDMDetails.push(line.Id());
            self.DmDetailList.remove(line);
        }
        self.GetDisbursementModes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/GetDisbursementModes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.DisbursementModes(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetChequeDeliveryOptions = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/GetChequeDeliveryOptions',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ChequeDeliveryOptions(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetIPDCBankAccounts = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/GetIPDCBankAccounts',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.IPDCBankAccounts(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        //self.SavePrint = function () {
        //    self.Id(' ');
        //    $.when(self.SavePO()).done(function () {
        //        self.setUrl();
        //    });
        //}
        //self.setUrl = function () {
        //    if ( typeof(self.Id()) != 'undefined') {
        //        window.open('/IPDC/Operations/PurchaseOrderReport?reportTypeId=PDF&poId=' + self.Id(), '_blank');
        //    }
        //};

        self.Submit = function () {
            var detailInfo = ko.observableArray([]);
            $.each(self.DmDetailList(),
              function (index, value) {
                  detailInfo.push({
                      Id: value.Id,
                      DisbursementMemoId: self.DisbursementMemoId(),
                      DisbursementMode: value.DisbursementMode,
                      ChequeDeliveryOption: value.ChequeDeliveryOption,
                      DisbursementAmount: value.DisbursementAmount,
                      IPDCBankAccountId: value.IPDCBankAccountId,
                      BankName: value.BankName,
                      BranchName: value.BranchName,
                      RoutingNo: value.RoutingNo,
                      AccountName: value.AccountName,
                      AccountNo: value.AccountNo,
                      ChequeNo: value.ChequeNo,
                      ChequeIssuedTo: value.ChequeIssuedTo,
                      ChequeDate: value.ChequeDate,
                      ChequeDateTxt: moment(value.ChequeDate()).format("DD/MM/YYYY"),
                      ClientAccountNo: value.ClientAccountNo,
                      RemovedDMDetails: self.RemovedDMDetails
                  });
              });
            var submitData = {
                Id: self.DisbursementMemoId(),
                DisbursementDetails: detailInfo,
                DisbursedDate: self.DisbursedDate(),
                DisbursedDateTxt: moment(self.DisbursedDate()).format("DD/MM/YYYY")
            }

            return $.ajax({
                type: "POST",
                url: '/IPDC/Operations/SaveDisbursmentMemoDetails',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    self.Id(data.Id);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadDMDetails = function () {
            if (self.DisbursementMemoId() > 0) {
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/Operations/LoadDMDetails/?dmId=' + self.DisbursementMemoId(),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (data) {
                        self.ApplicationNo(data.ApplicationNo);
                        self.AccountTitle(data.AccountTitle);
                        self.ProductName(data.ProductName);
                        self.TotalLoanAmount(data.TotalLoanAmount);
                        self.CurrentDisbursementAmount(data.CurrentDisbursementAmount);
                        self.TotalDisbursedAmount(data.TotalDisbursedAmount);
                        $.each(data.DisbursementDetails, function (index, value) {
                            var dtl = new dmDetails();
                            $.when(self.GetDisbursementModes()).done(function () {
                                $.when(self.GetChequeDeliveryOptions()).done(function () {
                                    $.when(self.GetIPDCBankAccounts()).done(function () {
                                        if (typeof (value) != 'undefined') {
                                            dtl.LoadData(value);
                                            self.DmDetailList.push(dtl);
                                        }
                                    });
                                });
                            });
                        });
                    }
                });
            }
        };
        self.Initialize = function () {
            self.GetDisbursementModes();
            self.GetChequeDeliveryOptions();
            self.GetIPDCBankAccounts();
            //self.GetGroupCountry();
            if (self.DisbursementMemoId() > 0) {
                self.LoadDMDetails();
            }

        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var oflvm = new DisbursmentMemoDetailVm();
    var appId = oflvm.queryString('ApplicationId');
    oflvm.ApplicationId(appId);
    var propId = oflvm.queryString('ProposalId');
    oflvm.ProposalId(propId);
    var id = oflvm.queryString('Id');
    oflvm.Id(id); //DisbursementMemoId
    var id = oflvm.queryString('MemoId');
    oflvm.DisbursementMemoId(id); //DisbursementMemoId
    oflvm.Initialize();
    ko.applyBindings(oflvm, $('#DisbursmentMemoDetailVW')[0]);

});