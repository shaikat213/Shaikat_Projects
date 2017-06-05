var ChequeDepositsLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.DepositApplicationId = ko.observable();
    self.ChequeNo = ko.observable();
    self.ChequeDate = ko.observable('');
    self.ChequeDate.subscribe(function () {
        self.ChequeDateTxt(moment(self.ChequeDate()).format("DD/MM/YYYY"));
    });
    self.ChequeDateTxt = ko.observable('');
    self.ChequeBank = ko.observable();
    self.DepositedTo = ko.observable();
    self.DepositDate = ko.observable();
    self.DepositDate.subscribe(function () {
        self.DepositDateTxt(moment(self.DepositDate()).format("DD/MM/YYYY"));
    });
    self.DepositDateTxt = ko.observable('');
    self.DepositAmount = ko.observable().extend({ required: true });
    self.ChequDepositAmountFormatted = ko.pureComputed({
        read: function () {
            if (self.DepositAmount() > 0)
                return self.DepositAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.DepositAmount(isNaN(value) ? 0 : value);
        },
        owner: self
    });

    self.LoadChqueDepositData = function (data) {
        self.Id(data.Id);
        self.DepositApplicationId(data.DepositApplicationId);
        self.ChequeNo(data.ChequeNo);
        self.ChequeDate(data.ChequeDate);
        self.ChequeDateTxt(data.ChequeDateTxt);
        self.ChequeBank(data.ChequeBank);
        self.DepositedTo(data.DepositedTo);
        self.DepositDate(data.DepositDate);
        self.DepositDateTxt(data.DepositDateTxt);
        self.DepositAmount(data.DepositAmount);
    };
};

var TransferDepositsLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.DepositApplicationId = ko.observable();
    self.TransferDate = ko.observable('');
    self.TransferDate.subscribe(function () {
        self.TransferDateTxt(moment(self.TransferDate()).format("DD/MM/YYYY"));
    });
    self.TransferDateTxt = ko.observable('');
    self.SourceBank = ko.observable();
    self.DepositedTo = ko.observable();
    self.DepositAmount = ko.observable().extend({ required: true });
    self.TransDepositAmountFormatted = ko.pureComputed({
        read: function () {
            if (self.DepositAmount() > 0)
                return self.DepositAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.DepositAmount(isNaN(value) ? 0 : value);
        },
        owner: self
    });
    self.LoadTransferDepositData = function (data) {
        self.Id(data.Id);
        self.DepositApplicationId(data.DepositApplicationId);
        self.TransferDate(data.TransferDate);
        self.TransferDateTxt(data.TransferDateTxt);
        self.SourceBank(data.SourceBank);
        self.DepositedTo(data.DepositedTo);
        self.DepositAmount(data.DepositAmount);
    };
};

var CashDepositsLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.DepositApplicationId = ko.observable();
    self.DepositedTo = ko.observable();
    self.DepositorName = ko.observable().extend({ required: true });
    self.DepositorPhone = ko.observable().extend({ required: true });
    self.CashDepositDate = ko.observable('');
    self.CashDepositDate.subscribe(function () {
        self.CashDepositDateTxt(moment(self.CashDepositDate()).format("DD/MM/YYYY"));
    });
    self.CashDepositDateTxt = ko.observable('');
    self.DepositAmount = ko.observable().extend({ required: true });
    self.CashDepositAmountFormatted = ko.pureComputed({
        read: function () {
            if (self.DepositAmount() > 0)
                return self.DepositAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.DepositAmount(isNaN(value) ? 0 : value);
        },
        owner: self
    });
    self.LoadCashDepositData = function (data) {
        self.Id(data.Id);
        self.DepositApplicationId(data.DepositApplicationId);
        self.DepositedTo(data.DepositedTo);
        self.DepositorName(data.DepositorName);
        self.DepositorPhone(data.DepositorPhone);
        self.CashDepositDate(data.CashDepositDate);
        self.CashDepositDateTxt(data.CashDepositDateTxt);
        self.DepositAmount(data.DepositAmount);
    };
};

var NomineesLine = function () {
    var self = this;
    self.Id = ko.observable();
    self.DepositApplicationId = ko.observable();
    self.NomineeCif = ko.observable();
    self.NomineeCifId = ko.observable();
    self.NomineeCif.subscribe(function () {
        self.NomineeCifId(self.NomineeCif().key);
    });
    self.RelationshipWithApplicant = ko.observable();
    self.PercentageShare = ko.observable();
    self.GuiardianCif = ko.observable();
    self.GuiardianCifId = ko.observable();
    self.GuiardianCif.subscribe(function () {
        if(self.GuiardianCif() != 'null')
        self.GuiardianCifId(self.GuiardianCif().key);
    });
    self.GuidRelationWithNom = ko.observable();

    self.LoadNomineesData = function (data) {
        
        self.Id(data.Id);
        self.DepositApplicationId(data.DepositApplicationId);
        if(data.NomineeCifId)
            self.NomineeCif(data.NomineeCif);
        //self.NomineeCifId(data.NomineeCifId);
        self.RelationshipWithApplicant(data.RelationshipWithApplicant);
        self.PercentageShare(data.PercentageShare);
        if (data.GuiardianCifId)
            self.GuiardianCif(data.GuiardianCif);
        //self.GuiardianCifId(data.GuiardianCifId);
        self.GuidRelationWithNom(data.GuidRelationWithNom);
    };
};

var DepositApplicationViewModel = function () {
    var self = this;

    self.Id = ko.observable();

    self.Office_Id = ko.observable();

    self.Application_Id = ko.observable();

    self.ModeOfDeposits = ko.observableArray([]);// Blocked or Not used

    self.ModeOfDeposit = ko.observable('');// Blocked or Not used

    self.ChequeDeposits = ko.observableArray([new ChequeDepositsLine()]);
    self.ChequeIPDCBankAccounts = ko.observableArray([]);
    self.AddChequeDeposits = function () {
        self.ChequeDeposits.push(new ChequeDepositsLine());
    };
    self.RemovedChequeDeposits = ko.observableArray([]);
    self.RemoveChequeDeposits = function (line) {
        if (line.Id() > 0)
            self.RemovedChequeDeposits.push(line.Id());
        self.ChequeDeposits.remove(line);
    };
    var chequeDepositsAmnt = 0;
    $.each(self.ChequeDeposits(), function (index, value) {
        if (value.DepositAmount() > 0)
            chequeDepositsAmnt += parseFloat(value.DepositAmount());
    });
    self.TransferDeposits = ko.observableArray([new TransferDepositsLine()]);
    self.TransferIPDCBankAccounts = ko.observableArray([]);
    self.AddTransferDeposits = function () {
        self.TransferDeposits.push(new TransferDepositsLine());
    };
    self.RemovedTransferDeposits = ko.observableArray([]);
    self.RemoveTransferDeposits = function (line) {
        if (line.Id() > 0)
            self.RemovedTransferDeposits.push(line.Id());
        self.TransferDeposits.remove(line);
    };
    var transferDepositsAmnt = 0;
    $.each(self.TransferDeposits(), function (index, value) {
        if (value.DepositAmount() > 0)
            transferDepositsAmnt += parseFloat(value.DepositAmount());
    });
    self.CashDeposits = ko.observableArray([new CashDepositsLine()]);
    self.CashIPDCBankAccounts = ko.observableArray([]);
    self.AddCashDepositsLine = function () {
        self.CashDeposits.push(new CashDepositsLine());
    };
    self.RemovedCashDeposits = ko.observableArray([]);
    self.RemoveCashDepositsLine = function (line) {
        if (line.Id() > 0)
            self.RemovedCashDeposits.push(line.Id());
        self.CashDeposits.remove(line);
    };
    var cashDepositsAmnt = 0;
    $.each(self.CashDeposits(), function (index, value) {
        if (value.DepositAmount() > 0)
            cashDepositsAmnt += parseFloat(value.DepositAmount());
    }); //Total Deposit Amount

    self.TotalDepositAmount = ko.pureComputed(function () {
        var chqDepositAmount = 0;
        $.each(self.ChequeDeposits(), function (index, value) {

            if (value.DepositAmount() > 0)
                chqDepositAmount += parseFloat(value.DepositAmount());
        });
        var transferDepositAmount = 0;
        $.each(self.TransferDeposits(), function (index, value) {

            if (value.DepositAmount() > 0)
                transferDepositAmount += parseFloat(value.DepositAmount());
        });
        var cashDepositAmount = 0;
        $.each(self.CashDeposits(), function (index, value) {

            if (value.DepositAmount() > 0)
                cashDepositAmount += parseFloat(value.DepositAmount());
        });
        return (chqDepositAmount + transferDepositAmount + cashDepositAmount);
    });

    self.TotalDepositAmountFormatted = ko.pureComputed({
        read: function () {
            if (self.TotalDepositAmount() > 0)
                return self.TotalDepositAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.TotalDepositAmount(isNaN(value) ? 0 : value);
        },
        owner: self
    });

    self.ModeOfOperations = ko.observableArray([]);
    self.ModeOfOperation = ko.observable();

    // End of Deposited Computation 

    self.SpecialInstructions = ko.observable();

    self.DepositClasses = ko.observableArray([]);
    self.DepositClass = ko.observable();

    self.Term = ko.observable();
    self.CardRate = ko.observable();
    self.OfferRate = ko.observable();
    self.RateVariance = ko.observable();

    self.ApprovedByDegList = ko.observableArray([]);
    self.ApprovedBy = ko.observable();

    self.RenewalOpts = ko.observableArray([]);
    self.RenewalOpt = ko.observable();

    self.WithdrawalModes = ko.observableArray([]);
    self.WithdrawalMode = ko.observable();


    self.CIFAccountTitle = ko.observable();
    self.CIFBankAccNo = ko.observable();
    self.CIFRoutingNo = ko.observable();
    self.CIFBankBranch = ko.observable();
    self.CIFBank = ko.observable();
    self.InitialDeposit = ko.observable();
    self.InstallmentSize = ko.observable();
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
    self.SourceOfFund = ko.observable();


    // Nominees 
    self.Nominees = ko.observableArray([new NomineesLine()]);
    self.NomineeCif = ko.observableArray([]);
    self.GuiardianCif = ko.observableArray([]);
    self.AddNomineesLine = function () {
        self.Nominees.push(new NomineesLine());
    };
    self.RemovedNominees = ko.observableArray([]);
    self.RemoveNomineesLine = function (line) {
        if (line.Id() > 0)
            self.RemovedNominees.push(line.Id());
        self.Nominees.remove(line);
    };
    self.Relationships = ko.observableArray([]);
    self.GetRelationships = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetRelationshipOptions',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.Relationships(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.ApplicantAge = ko.observable();
    self.GetApplicantAge = function () {
        if (self.Application_Id() > 0) {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetApplicantYoungestAge?AppId=' + self.Application_Id(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApplicantAge(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
    }
    
    self.AppGuiardianCifList = ko.observableArray([]);
    self.AppGuiardianCif = ko.observable();
    self.GuiardianCifId = ko.observable();
    self.AppGuiardianCif.subscribe(function () {
        self.GuiardianCifId(self.AppGuiardianCif().key);
        //self.CIFName(self.GuiardianCif().value);
    });

    self.RelationshipWithApplicant = ko.observable();
    self.BenificialOwner = ko.observable();
    self.SourceOfFundDetail = ko.observable();
    self.SourceOfFundVerificationMethod = ko.observable();
    self.SourceOfFundConsistency = ko.observable();

    self.RiskLevels = ko.observableArray([]);
    self.RiskLevel = ko.observable('');

    self.Remarks = ko.observable();

    self.FundRealization = ko.observable(false);
    self.FundRealizationDate = ko.observable('');
    self.FundRealizationDateTxt = ko.observable('');

    self.ApplicationStatuses = ko.observableArray([]);
    self.ApplicationStatus = ko.observable('');

    self.ApprovedByDegList = ko.observableArray([]);
    self.ApprovedBy = ko.observable();

    self.TaskAssignedToList = ko.observableArray([]);
    self.TaskAssignedToId = ko.observable();

    self.SanctionChecks = ko.observableArray([]);
    self.SanctionCheck = ko.observable('');
    self.SanctionRemarks = ko.observable();

    self.AccountOpenDate = ko.observable('');
    self.AccountOpenDateTxt = ko.observable('');

    self.MaturityDate = ko.observable('');
    self.MaturityDateTxt = ko.observable('');

    self.CBSAccountNo = ko.observable();
    self.InstrumentNo = ko.observable();

    self.InstrumentDispatchStatuses = ko.observableArray([]);
    self.InstrumentDispatchStatus = ko.observable('');

    self.WelcomeLetterStatuses = ko.observableArray([]);
    self.WelcomeLetterStatus = ko.observable('');
    self.IsFixed = ko.observable(false);
    self.IsRecurring = ko.observable(false);
    // Get Old Data
    self.chkDepositClass = function () {
        if (self.DepositClass() === 1) {
            self.IsFixed(true);
            self.IsRecurring(false);
        }
        else {
            self.IsFixed(false);
            self.IsRecurring(true);
        }
    };
    self.IsEFT = ko.observable(false);
    self.chkEFT = function () {
        if (self.WithdrawalMode() === 2) {
            self.IsEFT(true);
        } else {
            self.IsEFT(false);
        }
    };
    self.LoadDepositAppData = function () {
        if (self.Application_Id() > 0) {
            $.getJSON("/IPDC/Application/LoadDepositApplicationByAppId/?AppId=" + self.Application_Id(),
            null,
            function (data) {
                self.ChequeDeposits([]);
                self.TransferDeposits([]);
                self.CashDeposits([]);
                self.Nominees([]);

                self.Id(data.Id);
                self.Term(data.Term);
                self.Office_Id(data.Office_Id);
                
                if (data.ChequeDeposits.length > 0) {
                    $.when(self.LoadChequeIPDCBankAccount()).done(function () {
                        $.each(data.ChequeDeposits, function (index, value) {
                            var chequeDeposits = new ChequeDepositsLine();
                            if (typeof (value) != 'undefined') {
                                chequeDeposits.LoadChqueDepositData(value);
                                self.ChequeDeposits.push(chequeDeposits);
                            }
                        });
                    });
                }

                if (data.TransferDeposits.length > 0) {
                    $.when(self.LoadTransferIPDCBankAccount()).done(function () {
                        $.each(data.TransferDeposits, function (index, value) {
                            var transferDeposits = new TransferDepositsLine();
                            if (typeof (value) != 'undefined') {
                                transferDeposits.LoadTransferDepositData(value);
                                self.TransferDeposits.push(transferDeposits);
                            }
                        });
                    });
                }

                if (data.CashDeposits.length > 0) {
                    $.when(self.LoadCashIPDCBankAccount()).done(function () {
                        $.each(data.CashDeposits, function (index, value) {
                            var cashDeposits = new CashDepositsLine();
                            if (typeof (value) != 'undefined') {
                                cashDeposits.LoadCashDepositData(value);
                                self.CashDeposits.push(cashDeposits);
                            }
                        });
                    });
                }

                //Total Deposit Amount

                //self.TotalDepositAmount(data.TotalDepositAmount);

                $.when(self.LoadModeOfOperations())
                    .done(function () {
                        self.ModeOfOperation(data.ModeOfOperation);
                    });

                self.SpecialInstructions(data.SpecialInstructions);

                $.when(self.LoadDepositClasses())
                    .done(function () {
                        self.DepositClass(data.DepositClass);
                    });

                self.CardRate(data.CardRate);
                self.OfferRate(data.OfferRate);
                self.RateVariance(data.RateVariance);

                $.when(self.LoadApprovedByDegs())
                   .done(function () {
                       self.ApprovedBy(data.ApprovedBy);
                   });


                $.when(self.LoadRenewalOptions())
                   .done(function () {
                       self.RenewalOpt(data.RenewalOpt);
                   });

                $.when(self.LoadWithdrawalModes())
                   .done(function () {
                       self.WithdrawalMode(data.WithdrawalMode);
                   });

                self.CIFAccountTitle(data.CIFAccountTitle);
                self.CIFBankAccNo(data.CIFBankAccNo);
                self.CIFRoutingNo(data.CIFRoutingNo);
                self.CIFBankBranch(data.CIFBankBranch);
                self.CIFBank(data.CIFBank);
                self.InitialDeposit(data.InitialDeposit);
                self.InstallmentSize(data.InstallmentSize);
                self.MaturityAmount(data.MaturityAmount);
                self.SourceOfFund(data.SourceOfFund);

                // Nominees 
                $.when(self.GetRelationships()).done(function () {
                    self.RelationshipWithApplicant(data.RelationshipWithApplicant);
                    if (data.Nominees.length > 0) {
                        
                        //$.when(self.LoadNomineeGuiardianCif()).done(function () {
                           // $.when(self.LoadNomineesCif()).done(function () {
                                $.each(data.Nominees, function (index, value) {
                                    var nominees = new NomineesLine();
                                    if (typeof (value) != 'undefined') {
                                        nominees.LoadNomineesData(value);
                                        self.Nominees.push(nominees);
                                    }
                                });
                            //});
                        //});
                    }
                });

                //$.when(self.LoadApplicantGuiardianCif())
                //   .done(function () {
                
                if (data.GuiardianCifId > 0) {
                    self.GuiardianCifId(data.GuiardianCifId);
                    self.AppGuiardianCif(data.GuiardianCif);
                }
                

                
                self.BenificialOwner(data.BenificialOwner);
                self.SourceOfFundDetail(data.SourceOfFundDetail);
                self.SourceOfFundVerificationMethod(data.SourceOfFundVerificationMethod);
                self.SourceOfFundConsistency(data.SourceOfFundConsistency);

                $.when(self.LoadRiskLevels())
                   .done(function () {
                       self.RiskLevel(data.RiskLevel);
                   });

                self.Remarks(data.Remarks);

                self.FundRealization(data.FundRealization);

                self.FundRealizationDate(moment(data.FundRealizationDate));
                //$('#txtFundRealizationDate').val(data.FundRealizationDateTxt);
                self.FundRealizationDateTxt(data.FundRealizationDateTxt);

                $.when(self.LoadApplicationStatusList())
                   .done(function () {
                       self.ApplicationStatus(data.ApplicationStatus);
                   });

                $.when(self.LoadApprovedByDegs())
                   .done(function () {
                       self.ApprovedBy(data.ApprovedBy);
                   });

                $.when(self.LoadTaskAssignedToDegs())
                   .done(function () {
                       self.TaskAssignedToId(data.TaskAssignedToId);
                   });

                $.when(self.LoadSectionCheckList())
                   .done(function () {
                       self.SanctionCheck(data.SanctionCheck);
                   });

                self.SanctionRemarks(data.SanctionRemarks);

                self.AccountOpenDate(moment(data.AccountOpenDate).format('DD/MM/YYYY'));

                self.MaturityDate(moment(data.MaturityDate).format('DD/MM/YYYY'));
                self.MaturityDateTxt(data.MaturityDateTxt);

                self.CBSAccountNo(data.CBSAccountNo);
                self.InstrumentNo(data.InstrumentNo);

                $.when(self.LoadInstrumentDispatchStatusList())
                   .done(function () {
                       self.InstrumentDispatchStatus(data.InstrumentDispatchStatus);
                   });

                //$.when(self.LoadWelcomeLetterStatusList())
                //   .done(function () {
                //       self.WelcomeLetterStatus(data.WelcomeLetterStatus);
                //   });
                self.chkDepositClass();
                self.chkEFT();
            });
        }
    }; //Add New Data
    self.LoadChequeIPDCBankAccount = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetAllIpdcBankAccntiWithName',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ChequeIPDCBankAccounts(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadTransferIPDCBankAccount = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetAllIpdcBankAccntiWithName',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.TransferIPDCBankAccounts(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadCashIPDCBankAccount = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetAllIpdcBankAccntiWithName',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.CashIPDCBankAccounts(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadModeOfDeposit = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetModeOfDeposit',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ModeOfDeposits(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadModeOfOperations = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetModeOfOperations',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ModeOfOperations(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadDepositClasses = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetDepositClasses',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.DepositClasses(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadApprovedByDegs = function () {
        //var officeId = self.ResidenceAddress.CountryId();
        //if (officeId > 0) {
        return $.ajax({
            type: "GET",
            //url: '/IPDC/OfficeDesignationSettings/GetDesignationSettingListByOffice?officeId=' + ResidencecountryId,
            url: '/IPDC/OfficeDesignationSetting/GetAllDesignationSettings',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ApprovedByDegList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //}
    };
    self.LoadRenewalOptions = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetRenewalOptions',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.RenewalOpts(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadWithdrawalModes = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetWithdrawalModes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.WithdrawalModes(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.GetCIFList = function (searchTerm, callback) {
        var submitData = {
            prefix: searchTerm,
            exclusionList: appvm.ExistingCifIds()
        };
        $.ajax({
            type: "POST",
            url: '/IPDC/CIF/GetCIFPListForAutoFill',
            data: ko.toJSON(submitData),
            contentType: "application/json",
            success: function () {
            },
            error: function () {
                alert(error.status + "<--and--> " + error.statusText);
            }
        }).done(callback);
    };
    self.LoadRiskLevels = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetRiskLevels',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.RiskLevels(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadApplicationStatusList = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetAllApplicationStatus',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ApplicationStatuses(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadTaskAssignedToDegs = function () {
        //var officeId = self.ResidenceAddress.CountryId();
        //if (officeId > 0) {
        return $.ajax({
            type: "GET",
            //url: '/IPDC/OfficeDesignationSettings/GetDesignationSettingListByOffice?officeId=' + ResidencecountryId,
            url: '/IPDC/OfficeDesignationSetting/GetAllDesignationSettings',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.TaskAssignedToList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
        //}
    };
    self.LoadSectionCheckList = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetSectionCheckList',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.SanctionChecks(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    self.LoadInstrumentDispatchStatusList = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetDispatchStatusList',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.InstrumentDispatchStatuses(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    };
    //self.LoadWelcomeLetterStatusList = function () {
    //    return $.ajax({
    //        type: "GET",
    //        url: '/IPDC/Application/GetWelcomeLeterStatusList',
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        success: function (data) {
    //            self.WelcomeLetterStatuses(data); //Put the response in ObservableArray
    //        },
    //        error: function (error) {
    //            alert(error.status + "<--and--> " + error.statusText);
    //        }
    //    });
    //};
    self.Reset = function () {
        //self.CashAtHand("");
        //self.

    };
    self.Initializer = function () {
        self.LoadModeOfDeposit();
        self.LoadChequeIPDCBankAccount();
        self.LoadTransferIPDCBankAccount();
        self.LoadCashIPDCBankAccount();
        self.LoadModeOfOperations();
        self.LoadDepositClasses();
        self.LoadApprovedByDegs();
        self.LoadRenewalOptions();
        self.LoadWithdrawalModes();
        //self.LoadNomineesCif();
        //self.LoadNomineeGuiardianCif();
        //self.LoadApplicantGuiardianCif();
        self.LoadRiskLevels();
        self.LoadApplicationStatusList();
        self.LoadTaskAssignedToDegs();
        self.LoadSectionCheckList();
        self.LoadInstrumentDispatchStatusList();
        //self.LoadWelcomeLetterStatusList(); //self.LoadDepositAppData();
        self.GetRelationships();
        self.GetApplicantAge();
    };
    self.queryString = function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    };
};
