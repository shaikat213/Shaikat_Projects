$(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});

var FundingsLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.FundConfirmationId = ko.observable();
    self.IPDCBankAccountId = ko.observable();
    //self.CreditDate = ko.observable('');
    //self.CreditDate.subscribe(function () {
    //    self.CreditDateText(moment(self.CreditDate()).format("DD/MM/YYYY"));
    //});
    //self.CreditDateText = ko.observable('');
    self.CreditDate = ko.observable(moment());
    self.CreditDateText = ko.observable();
    self.CreditDate.subscribe(function () {
        self.CreditDateText(moment(self.CreditDate()).format("DD/MM/YYYY"));
    });
    self.DepositDateTxt = ko.observable('');
    self.Amount = ko.observable().extend({ required: true });

    self.LoadFundings = function (data) {
        self.Id(data.Id);
        self.FundConfirmationId(data.FundConfirmationId);
        self.IPDCBankAccountId(data.IPDCBankAccountId);
        self.CreditDate(data.CreditDate);
        self.CreditDateText(data.CreditDateText);
        self.Amount(data.Amount);
    };
};
$(document).ready(function () {
    function FundConfirmationVm() {
        var self = this;
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.ApplicationNo = ko.observable();
        self.AccountTitle = ko.observable();
        self.CustomerType = ko.observable();
        self.CustomerTypeName = ko.observable();
        self.ProductName = ko.observable();
        self.ProposalId = ko.observable();
        self.TotalDepositAmount = ko.observable();

        self.Fundings = ko.observableArray([new FundingsLine()]);
        self.IPDCBankAccounts = ko.observableArray([]);
        self.AddFundingsLine = function () {
            self.Fundings.push(new FundingsLine());
        };
        self.RemovedFundings = ko.observableArray([]);
        self.RemovedFundingsLine = function (line) {
            if (line.Id() > 0)
                self.RemovedFundings.push(line.Id());
            self.Fundings.remove(line);
        };
        self.ValidDepositAmount = ko.computed(function () {
            var fundDepositsAmnt = 0;
            $.each(self.Fundings(), function (index, value) {
                if (value.Amount() > 0)
                    fundDepositsAmnt += parseFloat(value.Amount());
            });
            if (fundDepositsAmnt === self.TotalDepositAmount())
                return true;
            else
                return false;
        });

        self.FundReceived = ko.observable(false);

        self.SaveAsDraft=function() {
            self.FundReceived(false);
            self.SaveFundConfirmation();
        }

        self.SaveAndSubmit = function () {
            self.FundReceived(true);
            self.SaveFundConfirmation();
        }

        self.SaveFundConfirmation = function () {
            
            var fundings = ko.observableArray([]);
            $.each(self.Fundings(),
                    function (index, value) {
                        fundings.push({
                            Id: value.Id(),
                            FundConfirmationId: value.FundConfirmationId(),
                            IPDCBankAccountId: value.IPDCBankAccountId(),
                            CreditDate: value.CreditDate(),
                            CreditDateText: value.CreditDateText(),
                            Amount: value.Amount()
                        });
                    });

            var submitFundConfirm = {
                Id: self.Id(),
                ApplicationId: self.ApplicationId(),
                ApplicationNo: self.ApplicationNo(),
                AccountTitle: self.AccountTitle(),
                CustomerTypeName: self.CustomerTypeName(),
                ProductName: self.ProductName(),
                ProposalId: self.ProposalId(),
                FundReceived:self.FundReceived(),
                Fundings: fundings

            };
            $.ajax({
                url: '/IPDC/Operations/SaveFundConfirm',
                
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitFundConfirm),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    if(data.Id > 0)
                    {
                        self.Id(data.Id);
                        self.LoadOpFundConfirmData();
                    }
                    
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.LoadOpFundConfirmData = function () {
            if (self.ApplicationId() > 0) {
                $.getJSON("/IPDC/Operations/LoadFundConfirmationByAppId/?AppId=" + self.ApplicationId(),
                null,
                function (data) {
                    self.Fundings([]);
                    self.Id(data.Id);
                    self.ApplicationId(data.ApplicationId);
                    self.ApplicationNo(data.ApplicationNo);
                    self.AccountTitle(data.AccountTitle);
                    self.CustomerTypeName(data.CustomerTypeName);
                    self.ProposalId(data.ProposalId);
                    self.TotalDepositAmount(data.TotalDepositAmount);
                    self.ProductName(data.ProductName);
                    if (data.Fundings.length > 0) {
                        $.when(self.LoadIpdcBankAccount()).done(function () {
                            $.each(data.Fundings, function (index, value) {
                                var fundings = new FundingsLine();
                                if (typeof (value) != 'undefined') {
                                    fundings.LoadFundings(value);
                                    self.Fundings.push(fundings);
                                }
                            });
                        });
                    }
                    self.FundReceived(true);
                });
            }
        };

        self.LoadIpdcBankAccount = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetAllIpdcBankAccntiWithName',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.IPDCBankAccounts(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.Initialize=function() {
            self.LoadIpdcBankAccount();
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new FundConfirmationVm();
    aabmvm.Initialize();
    //var qValue = aabmvm.queryString("proposalId");
    //aabmvm.ProposalId(qValue);
    var appId = aabmvm.queryString("AppId");
    aabmvm.ApplicationId(appId);
    var selfId = aabmvm.queryString("Id");
    aabmvm.Id(selfId);
    aabmvm.LoadOpFundConfirmData();
    ko.applyBindings(aabmvm, document.getElementById("opFundConfirmation"));
});