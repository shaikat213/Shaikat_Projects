

//$(document).ready(function () {

var SavingsInBankLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.BankName = ko.observable();
    self.BankDepositType = ko.observable();
    self.CurrentBalance = ko.observable();
    self.LoadData = function (data) {
        self.Id(data ? data.Id : 0);
        self.BankName(data ? data.BankName : "");
        self.BankDepositType(data ? data.BankDepositType : 0);
        self.CurrentBalance(data ? data.CurrentBalance : 0);
    }

};

var InvestmentLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.InvestmentType = ko.observable();
    self.Amount = ko.observable();
    self.LoadData = function (data) {
        self.Id(data ? data.Id : 0);
        self.InvestmentType(data ? data.InvestmentType : 0);
        self.Amount(data ? data.Amount : 0);
    }
};

var PropertiesLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.Description = ko.observable();
    self.MarketValue = ko.observable();
    self.Encumbered = ko.observable(false);
    self.LoadData = function (data) {
        self.Id(data ? data.Id : 0);
        self.Description(data ? data.Description : "");
        self.MarketValue(data ? data.MarketValue : 0);
        self.Encumbered(data ? data.Encumbered : false);
    }
};

var BusinessSharesLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.BusinessShareType = ko.observable();
    self.ValueOfOwnership = ko.observable();
    self.LoadData = function (data) {
        self.Id(data ? data.Id : 0);
        self.BusinessShareType(data ? data.BusinessShareType : 0);
        self.ValueOfOwnership(data ? data.ValueOfOwnership : 0);
    }
};


var LiabilitiesLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.LoanType = ko.observable();
    self.LoanAmountOrLimit = ko.observable();
    self.InstallmentAmount = ko.observable();
    self.Term = ko.observable();
    self.RemainingTerm = ko.observable();
    self.OutstandingAmount = ko.observable();
    self.BankOrFIName = ko.observable();
    self.LiabilityType = ko.observable();
    self.LoadData = function (data) {
        self.Id(data ? data.Id : 0);
        self.LoanType(data ? data.LoanType : 0);
        self.LoanAmountOrLimit(data ? data.LoanAmountOrLimit : 0);
        self.InstallmentAmount(data ? data.InstallmentAmount : 0);
        self.Term(data ? data.Term : 0);
        self.RemainingTerm(data ? data.RemainingTerm : 0);
        self.OutstandingAmount(data ? data.OutstandingAmount : 0);
        self.BankOrFIName(data ? data.BankOrFIName : "");
        self.LiabilityType(data ? data.LiabilityType : 0);
    }
};



var CIFNetWorthViewModel = function () {
    var self = this;
    self.queryString = function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }
    self.Id = ko.observable();
    self.CIF_PersonalId = ko.observable();
    self.CashAtHand = ko.observable();
    self.OtherSavings = ko.observable();

    self.SavingsInBank = ko.observableArray([]); //new SavingsInBankLine()
    self.BankDepositTypes = ko.observableArray([]);
    self.AddBankSavings = function () {
        self.SavingsInBank.push(new SavingsInBankLine());
    }
    self.RemovedSavings = ko.observableArray([]);
    self.RemoveBankSavings = function (line) {
        if (line.Id() > 0)
            self.RemovedSavings.push(line.Id());
        self.SavingsInBank.remove(line);
    }

    self.Investments = ko.observableArray([]);//new InvestmentLine()
    self.InvestmentTypes = ko.observableArray([]);
    self.AddInvestments = function () {
        self.Investments.push(new InvestmentLine());
    }
    self.RemovedInvestments = ko.observableArray([]);
    self.RemoveInvestments = function (line) {
        if (line.Id() > 0)
            self.RemovedInvestments.push(line.Id());
        self.Investments.remove(line);
    }

    self.Properties = ko.observableArray([]); //new PropertiesLine()
    self.AddProperties = function () {
        self.Properties.push(new PropertiesLine());
    }
    self.RemovedProperties = ko.observableArray([]);
    self.RemoveProperties = function (line) {
        if (line.Id() > 0)
            self.RemovedProperties.push(line.Id());
        self.Properties.remove(line);
    }

    self.BusinessShares = ko.observableArray([]); //new BusinessSharesLine()
    self.BusinessShareTypes = ko.observableArray([]);
    self.AddBusinessShares = function () {
        self.BusinessShares.push(new BusinessSharesLine());
    }
    self.RemovedShareinBusines = ko.observableArray([]);
    self.RemoveShareinBusines = function (line) {
        if (line.Id() > 0)
            self.RemovedShareinBusines.push(line.Id());
        self.BusinessShares.remove(line);
    }

    //Liabilities
    self.Liabilities = ko.observableArray([]); //new LiabilitiesLine()
    self.LoanTypes = ko.observableArray([]);
    self.LiabilityTypes = ko.observableArray();
    self.AddLiabilities = function () {
        self.Liabilities.push(new LiabilitiesLine());
    }
    self.RemovedLiabilities = ko.observableArray([]);
    self.RemoveLiabilities = function (line) {
        if (line.Id() > 0)
            self.RemovedLiabilities.push(line.Id());
        self.Liabilities.remove(line);
    }


    //Other Liabilities
    self.OfficeLoan = ko.observable();
    self.UnpaidTaxes = ko.observable();
    self.OtherLiabilities = ko.observable();

    //Total Liabilities

    self.TotalLiabilities = ko.pureComputed(function () {
        var officeLoan = 0;
        var unpaidTax = 0;
        var otherLiability = 0;
        var outstandingAmount = 0;

        if (self.OfficeLoan() > 0)
            officeLoan = parseFloat(self.OfficeLoan());
        if (self.UnpaidTaxes() > 0)
            unpaidTax = parseFloat(self.UnpaidTaxes());
        if (self.OtherLiabilities() > 0)
            otherLiability = parseFloat(self.OtherLiabilities());


        $.each(self.Liabilities(),
            function (index, value) {

                if (value.OutstandingAmount() > 0)
                    outstandingAmount += parseFloat(value.OutstandingAmount());
            });

        return (officeLoan + unpaidTax + otherLiability + outstandingAmount);
    });

    self.NetWorth = ko.pureComputed(function () {

        return ((parseFloat(self.TotalAsset())) - (parseFloat(self.TotalLiabilities())));

    });

    self.TotalAsset = ko.pureComputed(function () {
        var cashAtHand = 0;
        var otherSavings = 0;

        if (self.CashAtHand() > 0)
            cashAtHand = parseFloat(self.CashAtHand());
        var bankingSavings = 0;
        $.each(self.SavingsInBank(),
            function (index, value) {
                if (value.CurrentBalance() > 0)
                    bankingSavings += parseFloat(value.CurrentBalance());
            });
        var propertySaving = 0;
        $.each(self.Properties(),
            function (index, value) {
                if (value.MarketValue() > 0)
                    propertySaving += parseFloat(value.MarketValue());
            });
        var investmentSaving = 0;
        $.each(self.Investments(),
            function (index, value) {
                if (value.Amount() > 0)
                    investmentSaving += parseFloat(value.Amount());
            });

        var BusinessShares = 0;
        $.each(self.BusinessShares(),
            function (index, value) {
                if (value.ValueOfOwnership() > 0)
                    BusinessShares += parseFloat(value.ValueOfOwnership());
            });

        if (self.OtherSavings() > 0)
            otherSavings = parseFloat(self.OtherSavings());

        return (cashAtHand + bankingSavings + propertySaving + investmentSaving + otherSavings + BusinessShares);
    });

    //Load Data
    self.LoadNetWorth = function () {
        if (self.CIF_PersonalId() > 0) {
            $.getJSON("/IPDC/CIF/LoadNetWorth/?cifPersonId=" + self.CIF_PersonalId(),
                null,
                function (data) {

                    self.SavingsInBank([]);
                    self.Investments([]);
                    self.Properties([]);
                    self.BusinessShares([]);
                    self.Liabilities([]);
                    
                    self.Id(data.Id);
                    self.CIF_PersonalId(data.CIF_PersonalId);
                    self.CashAtHand(data.CashAtHand);

                    $.when(self.LoadBankDepositType())
                        .done(function () {
                            $.each(data.SavingsInBank,
                                function (index, value) {
                                    var aDetail = new SavingsInBankLine();
                                    if (typeof (value) != 'undefined') {
                                        aDetail.LoadData(value);
                                        self.SavingsInBank.push(aDetail);
                                    }
                                });
                        });

                    $.when(self.LoadInvestmentType())
                        .done(function () {
                            $.each(data.Investments,
                                function (index, value) {
                                    var aDetail = new InvestmentLine();
                                    if (typeof (value) != 'undefined') {
                                        aDetail.LoadData(value);
                                        self.Investments.push(aDetail);
                                    }
                                });
                        });

                    $.each(data.Properties,
                        function (index, value) {
                            var aDetail = new PropertiesLine();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.Properties.push(aDetail);
                            }
                        });

                    $.when(self.LoadBusinessShareType())
                        .done(function () {
                        $.each(data.BusinessShares,
                        function (index, value) {
                            var aDetail = new BusinessSharesLine();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.BusinessShares.push(aDetail);
                            }

                        });
                    });
                    
                    $.when(self.LoadLoanType())
                        .done(function () {
                            $.when(self.LoadLiabilitiesType())
                                .done(function () {
                                    $.each(data.Liabilities,
                                        function (index, value) {
                                            var aDetail = new LiabilitiesLine();
                                            if (typeof (value) != 'undefined') {
                                                aDetail.LoadData(value);
                                                self.Liabilities.push(aDetail);
                                            }
                                        });
                                });
                        });

                    self.OtherSavings(data.OtherSavings);
                    self.OfficeLoan(data.OfficeLoan);
                    self.UnpaidTaxes(data.UnpaidTaxes);
                    self.OtherLiabilities(data.OtherLiabilities);
                });
        }
    }

    //Add New Data
    self.Submit = function () {
        //debugger           

        //cifReferenceInfo = {
        //    CIF_PersonalId: self.CIF_PersonalId(),
        //    SavingsInBank: self.Name(),
        //    Investments: self.Designation(),
        //    Properties: self.Department = ko.observable(),
        //    BusinessSharess: self.EnlistedOrganization(),
        //    TotalAsset: self.OrganizationName(),
        //    OrganizationId: self.OrganizationId(),
        //    RelationshipWithApplicant: self.RelationshipWithApplicant(),
        //    ResidenceAddress: self.ResidenceAddress,
        //    PermanentAddress: self.PermanentAddress,
        //    OfficeAddress: self.OfficeAddress

        //};
        $.ajax({
            url: '/IPDC/CIF/SaveCIFNetWorth',
            //cache: false,
            type: 'POST',
            contentType: 'application/json',
            data: ko.toJSON(self),
            success: function (data) {
                //self.CIF_PersonalId()
                $('#NWsuccessModal').modal('show');
                $('#NWsuccessModalText').text(data.Message);

                console.log("data=" + ko.toJSON(data));

                if (data.Id > 0) {
                    self.Id(data.Id);
                    self.LoadNetWorth();
                }
                //self.Id(data.Id);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }


    self.LoadBankDepositType = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/CIF/GetBankDepositTypes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.BankDepositTypes(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.LoadInvestmentType = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/CIF/GetInvestmentTypes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.InvestmentTypes(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.LoadLoanType = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/CIF/GetLoanTypes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.LoanTypes(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.LoadLiabilitiesType = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/CIF/GetLiabilityTypes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.LiabilityTypes(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.LoadBusinessShareType = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/CIF/GetBusinessShareTypes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.BusinessShareTypes(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.Initialize = function () {
        self.LoadBankDepositType();
        self.LoadBusinessShareType();
        self.LoadInvestmentType();
        self.LoadLoanType();
        self.LoadLiabilitiesType();
    }
};

//var alVm = new CIFNetWorthViewModel();

//alVm.CIF_PersonalId(1);
//ko.applyBindings(alVm, document.getElementById("cifNetWorth")[0]);

//})