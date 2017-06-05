
$(document).ready(function () {

    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });


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

    function NetWorhVerificationVm() {

        var self = this;
        self.Id = ko.observable();
        self.NetWorthId = ko.observable();
        self.CifNetWorth = ko.observable();
        self.ApplicationId = ko.observable();
        self.CIF_PersonalId = ko.observable(); //
        self.CIFNo = ko.observable();
        self.CIFName = ko.observable();
        self.CIF_PersonalList = ko.observableArray([]);
        self.VerificationDate = ko.observable(); //
        self.VerificationDateTxt = ko.observable();//
        self.VerificationPersonRole = ko.observable();
        self.VerificationPersonRoleFrom = ko.observable();
        self.VerificationPersonRoleName = ko.observable();//
        self.VerificationState = ko.observable();//
        self.VerificationStateName = ko.observable();//
        self.Remarks = ko.observable();

        self.CashAtHand = ko.observable();
        self.OtherSavings = ko.observable();

        //##############################################

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

        self.OfficeLoan = ko.observable();
        self.UnpaidTaxes = ko.observable();
        self.OtherLiabilities = ko.observable();

        //##############################################

        self.NWHistory = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'NetWorthId',
                Value: self.NetWorthId()
            }, {
                Name: 'CIFPId',
                Value: self.CIF_PersonalId()
            }];
            var menuInfo = {
                //Id: urlId++,
                Menu: 'Net Worth Verification History',
                Url: '/IPDC/Verification/NetWorthVerificationHistory',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.AddNetWorth = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'CIFPId',
                Value: self.CIF_PersonalId()
            }, {
                Name: 'Id',
                Value: null
            }];
            var menuInfo = {
                //Id: urlId++,
                Menu: 'Net Worth',
                Url: '/IPDC/Verification/NetWorthVerification',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.SaveAsNew = function () {
            self.Id('');
            self.VerificationPersonRole(self.VerificationPersonRoleFrom());
            self.SaveNetWorthVerification();
        }
        self.Save = function() {
            self.SaveNetWorthVerification();
        }

        self.SaveNetWorthVerification = function () {
            self.VerificationDateTxt(moment(self.VerificationDate).format('DD/MM/YYYY'));
            //var savingsInBank = ko.observableArray([]);
            //var properties = ko.observableArray([]);
            //var investMent = ko.observableArray([]);
            //var businessShares = ko.observableArray([]);
            //var liabilities = ko.observableArray([]);
            //$.each(self.SavingsInBank(),
            //        function (index, value) {
            //            savingsInBank.push({
            //                Id: value.Id(),
            //                BankName: value.BankName(),
            //                BankDepositType: value.BankDepositType(),
            //                CurrentBalance: value.CurrentBalance()
            //            });
            //        });
            //$.each(self.Investments(),
            //        function (index, value) {
            //            investMent.push({
            //                Id: value.Id(),
            //                InvestmentType: value.InvestmentType(),
            //                Amount: value.Amount()
            //            });
            //        });
            //$.each(self.Properties(),
            //        function (index, value) {
            //            properties.push({
            //                Id: value.Id(),
            //                Description: value.Description(),
            //                MarketValue: value.MarketValue(),
            //                Encumbered: value.Encumbered()
            //            });
            //        });
            //$.each(self.BusinessShares(),
            //        function (index, value) {
            //            businessShares.push({
            //                Id: value.Id(),
            //                BusinessShareType: value.BusinessShareType(),
            //                ValueOfOwnership: value.ValueOfOwnership()
            //            });
            //        });
            //$.each(self.Liabilities(),
            //        function (index, value) {
            //            liabilities.push({
            //                Id: value.Id(),
            //                LoanType: value.LoanType(),
            //                LoanAmountOrLimit: value.LoanAmountOrLimit(),
            //                InstallmentAmount:value.InstallmentAmount(),
            //                Term:value.Term(),
            //                RemainingTerm:value.RemainingTerm(),
            //                OutstandingAmount:value.OutstandingAmount(),
            //                BankOrFIName:value.BankOrFIName(),
            //                LiabilityType:value.LiabilityType()
            //            });
            //        });
            //var submitData = {
            //    Id: self.Id(),
            //    NetWorthId: self.NetWorthId(),
            //    CifNetWorth: self.CifNetWorth(),
            //    ApplicationId: self.ApplicationId(),
            //    CIF_PersonalId: self.CIF_PersonalId(),
            //    CIFNo: self.CIFNo(),
            //    CIFName: self.CIFName(),
            //    VerificationDate:self.VerificationDate(),
            //    VerificationDateTxt:self.VerificationDateTxt(),
            //    VerificationPersonRole:self.VerificationPersonRole(),
            //    VerificationPersonRoleFrom:self.VerificationPersonRoleFrom(),
            //    VerificationPersonRoleName:self.VerificationPersonRoleName(),
            //    VerificationState: self.VerificationState(),
            //    VerificationStateName: self.VerificationStateName(),
            //    CashAtHand:self.CashAtHand(),
            //    OtherSavings: self.OtherSavings(),
            //    RemoveBankSavings: self.RemoveBankSavings(),
            //    SavingsInBank: savingsInBank,
            //    RemovedInvestments:self.RemoveInvestments(),
            //    Investments: investMent,
            //    RemoveProperties: self.RemoveProperties(),
            //    Properties: properties,
            //    RemoveShareinBusines: self.RemoveShareinBusines(),
            //    BusinessShares: businessShares,
            //    RemoveLiabilities: self.RemoveLiabilities(),
            //    Liabilities: liabilities,
            //    OfficeLoan:self.OfficeLoan(),
            //    UnpaidTaxes:self.UnpaidTaxes(),
            //    OtherLiabilities:self.OtherLiabilities()
            //};
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveNetWorthVerification',
                data: ko.toJSON(self),
                contentType: "application/json",
                success: function (data) {
                    $('#cibSuccessModal').modal('show');
                    $('#cibSuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadNetWorthVerification = function () {
            return $.ajax({
                type: "GET",
                //url: '/IPDC/Verification/LoadNetWorthVerification/?AppId=' + self.ApplicationId() + '&NetWorthId=' + self.NetWorthId() + '&CifId=' + self.CIF_PersonalId() + '&NWVId=' + self.Id(),
                url: '/IPDC/Verification/LoadNetWorthVerification/?AppId=' + self.ApplicationId() + '&CIFPId=' + self.CIF_PersonalId() + '&Id=' + self.Id(),
                contentType: "application/json",
                dataType:"json",
                success: function (data) {
                    
                    self.Id(data.Id);
                    self.NetWorthId(data.NetWorthId);
                    self.ApplicationId(data.ApplicationId);
                    self.CIF_PersonalId(data.CIF_PersonalId);
                    self.CIFNo(data.CIFNo);
                    self.CIFName(data.CIFName);
                    self.VerificationDate(data.VerificationDate ? (moment(data.VerificationDate)) : moment());

                    if(data.Id > 0)
                        self.VerificationPersonRole(data.VerificationPersonRole + '');
                    
                    self.VerificationState(data.VerificationState + '');
                    self.VerificationStateName(data.VerificationStateName);

                    self.CashAtHand(data.CashAtHand);
                    self.Remarks(data.Remarks);

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

                }
            });
        };
        
        ///// ######################################
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
        //##########################################

        self.Initialize = function () {
            self.LoadBankDepositType();
            self.LoadBusinessShareType();
            self.LoadInvestmentType();
            self.LoadLoanType();
            self.LoadLiabilitiesType();
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    appvm = new NetWorhVerificationVm();

    appvm.Initialize();

    var qValue = appvm.queryString('AppId');
    appvm.ApplicationId(qValue);
    //var networthId = appvm.queryString('NetWorthId');
    //appvm.NetWorthId(networthId);
    var cifId = appvm.queryString('CIFPId');
    appvm.CIF_PersonalId(cifId);
    var selfId = appvm.queryString('Id');
    appvm.Id(selfId);
    appvm.VerificationPersonRole(appvm.queryString("VerificationAs"));
    appvm.VerificationPersonRoleFrom(appvm.VerificationPersonRole());
    appvm.LoadNetWorthVerification();
    ko.applyBindings(appvm, $('#networhverification')[0]);

});