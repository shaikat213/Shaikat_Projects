/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
/// <reference path="~/Scripts/knockout.validation.min.js" />
/// <reference path="~/Scripts/bootstrap-datetimepicker.js" />
/// <reference path="~/Scripts/knockout-3.4.0.debug.js" />
$(function () {
    $('#ApplicationDateId').datetimepicker({ format: 'DD/MM/YYYY' });
    $('#SubmissionDeadlineId').datetimepicker({ format: 'DD/MM/YYYY' });
    //$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
});
$(document).ready(function () {

    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });
    //ko.validation.rules['minCheck'] = {
    //    validator: function (val, min) {
    //        val = val.replace(/\,/g, '');
    //        return ko.validation.utils.isEmptyVal(val) || val >= min;
    //    },
    //    message: 'Please enter a value greater than or equal to {0}.'
    //};
    function applicationCIFs(data) {
        var self = this;
        self.Id = ko.observable(data ? data.Id : 0);
        self.ApplicationId = ko.observable(data ? data.ApplicationId : 0);
        self.CIF_PersonalId = ko.observable(data ? data.CIF_PersonalId : 0);
        self.ApplicantRole = ko.observable(data ? data.ApplicantRole : 0);
        self.ApplicantRoleName = ko.observable(data ? data.ApplicantRoleName : '');
        self.CIF_OrganizationalId = ko.observable(data ? data.CIF_OrganizationalId : 0);
        self.ApplicantName = ko.observable(data ? data.ApplicantName : '');
        self.Age = ko.observable(data ? data.Age : 0);
        self.ProfessionName = ko.observable(data ? data.ProfessionName : '');
        self.MonthlyIncome = ko.observable(data ? data.MonthlyIncome : 0);
        self.CIFNo = ko.observable(data ? data.CIFNo : 0);

        self.IsAddOption = ko.observable(data ? data.ApplicantRole === 1 || data.ApplicantRole === 2 ? true : false : false);
        self.checkCoApp = function () {
            //
            if (self.ApplicantRole() === 1) {
                self.IsAddOption(true);
            }
            if (self.ApplicantRole() === 2) {
                self.IsAddOption(true);
            }
        }
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.ApplicationId(data ? data.ApplicationId : 0);
            self.CIF_PersonalId(data ? data.CIF_PersonalId : 0);
            self.ApplicantRole(data ? data.ApplicantRole : 0);
            self.ApplicantRoleName(data ? data.ApplicantRoleName : 0);
            self.CIF_OrganizationalId(data ? data.CIF_OrganizationalId : 0);
            self.ApplicantName(data ? data.ApplicantName : '');
            self.Age(data ? data.Age : 0);
            self.ProfessionName(data ? data.ProfessionName : '');
            self.MonthlyIncome(data ? data.MonthlyIncome : 0);
            self.CIFNo(data ? data.CIFNo : 0);
            self.checkCoApp();
        }

    }

    function LppPrimarySecurityVm() {

        var self = this;
        self.AppId = ko.observable();
        self.Id = ko.observable();
        self.SalesLeadId = ko.observable();
        self.ApplicationNo = ko.observable();
        self.LPPrimarySecurityId = ko.observable();
        self.VerificationDate = ko.observable();
        //self.VerifiedByUserId= ko.observable();
        //self.VerifiedByEmpDegMapId= ko.observable();
        self.PropertyTypeName = ko.observable();
        self.FlatSize = ko.observable();
        self.LoanType = ko.observable();
        self.PropornateLand = ko.observable();
        self.PresentMarketRateOfLand = ko.observable(); //puer katha
        self.PresentMarketValueOfLand = ko.pureComputed(function () {
            var commonSpace = 0;
            commonSpace += parseFloat(self.PropornateLand());
            var sftPrc = 0;
            sftPrc += parseFloat(self.PresentMarketRateOfLand());
            return (commonSpace * sftPrc);
        });
        self.FlatSizeWithCommonSpace = ko.observable();
        self.FlatSizeWithoutCommonSpace = ko.observable();
        self.PerSFTMarketPrice = ko.observable();
        self.MarketPriceOfFlat = ko.observable(); // = FlatSizeWithCommonSpace * PerSFTMarketPrice
        self.MarketPriceOfFlat = ko.pureComputed(function () {
            var commonSpace = 0;
            commonSpace += parseFloat(self.FlatSizeWithCommonSpace());
            var sftPrc = 0;
            sftPrc += parseFloat(self.PerSFTMarketPrice());
            return (commonSpace * sftPrc);
        });


        self.CarParkingCount = ko.observable();
        self.CarParkingPrice = ko.observable();

        self.TotalMarketValue = ko.pureComputed(function () {
            var price = 0;
            price += parseFloat(self.MarketPriceOfFlat());
            var parkingPrice = 0;
            parkingPrice += parseFloat(self.CarParkingPrice());
            return (price + parkingPrice);
        });
        self.DistressPercentage = ko.observable();
        self.DistressValue = ko.observable(); // = DistressPercentage * TotalMarketValue
        self.DistressValue = ko.pureComputed(function () {
            var total = 0;
            var distress = 0;
            distress += parseFloat(self.DistressPercentage());
            var marketValue = 0;
            marketValue += parseFloat(self.TotalMarketValue());
            total = (marketValue * distress) / 100;
            return total;
        });


        self.ProjectStatus = ko.observable();
        self.TotalWIPofFullProject = ko.observable().extend({ max: 100 });;
        self.ApplicantsFlatStatus = ko.observable();
        self.TotalWIPofFlat = ko.observable().extend({ max: 100 });;
        self.Remarks = ko.observable();
        self.VerificationState = ko.observable();

        self.AreaOfLandAsPerPlan = ko.observable(); //fetch from project database
        self.AreaOfLandAsPerClient = ko.observable();
        self.PerKathaPriceAsPerRAJUK = ko.observable();
        self.PerKathaPriceAsPerClient = ko.observable();
        self.MarketValueOfLandAsPerRAJUK = ko.computed(function () {
            var total = 0;
            var land = parseFloat(self.AreaOfLandAsPerPlan() ? self.AreaOfLandAsPerPlan() : 0);
            var price = parseFloat(self.PerKathaPriceAsPerRAJUK() ? self.PerKathaPriceAsPerRAJUK() : 0);
            total = land * price;
            return total;
        });
        //= ko.observable(); // = AreaOfLandAsPerPlan * PerKathaPriceAsPerRAJUK
        self.MarketValueOfLandAsPerClient = ko.computed(function () {
            var total = 0;
            var land = parseFloat(self.AreaOfLandAsPerClient() ? self.AreaOfLandAsPerClient() : 0);
            var price = parseFloat(self.PerKathaPriceAsPerClient() ? self.PerKathaPriceAsPerClient() : 0);
            total = land * price;
            return total;
        });
        //= ko.observable(); // = AreaOfLandAsPerClient * PerKathaPriceAsPerClient

        self.BuildUpAreaPerFloorApproved = ko.observable();
        self.TotalBuildUpAreaApproved = ko.observable();
        self.EstimatedConstructionCostPerSFTApproved = ko.observable();
        self.EstimatedConstructionCostApproved = ko.observable();

        self.EstimatedConstructionCostPerSFTPhysical = ko.computed(function () {
            var total = 0;
            var land = parseFloat(self.TotalBuildUpAreaApproved() ? self.TotalBuildUpAreaApproved() : 0);
            var price = parseFloat(self.EstimatedConstructionCostPerSFTApproved() ? self.EstimatedConstructionCostPerSFTApproved() : 0);
            total = land * price;
            return total;
        });
        //= ko.observable(); // = TotalBuildUpAreaApproved * EstimatedConstructionCostPerSFTApproved
        self.LandValueAndEstimatedConstructionCostApproved = ko.computed(function () {
            var total = 0;
            var land = parseFloat(self.MarketValueOfLandAsPerRAJUK() ? self.MarketValueOfLandAsPerRAJUK() : 0);
            var price = parseFloat(self.EstimatedConstructionCostPerSFTPhysical() ? self.EstimatedConstructionCostPerSFTPhysical() : 0);
            total = land + price;
            return total;
        });
        //= ko.observable(); // = MarketValueOfLandAsPerRAJUK  + EstimatedConstructionCostApproved

        self.BuildUpAreaPerFloorPhysical = ko.observable();
        self.TotalBuildUpAreaPhysical = ko.observable();


        self.EstimatedConstructionCostPhysical = ko.computed(function () {
            var total = 0;
            var land = parseFloat(self.TotalBuildUpAreaPhysical() ? self.TotalBuildUpAreaPhysical() : 0);
            var price = parseFloat(self.EstimatedConstructionCostApproved() ? self.EstimatedConstructionCostApproved() : 0);
            total = land * price;
            return total;
        });
        //ko.observable(); // = TotalBuildUpAreaPhysical * EstimatedConstructionCostApproved

        self.LandValueAndEstimatedConstructionCostPhysical = ko.computed(function () {
            var total = 0;
            var land = parseFloat(self.MarketValueOfLandAsPerClient() ? self.MarketValueOfLandAsPerClient() : 0);
            var price = parseFloat(self.EstimatedConstructionCostPhysical() ? self.EstimatedConstructionCostPhysical() : 0);
            total = land + price;
            return total;
        });
        //= ko.observable(); // = MarketValueOfLandAsPerClient  + EstimatedConstructionCostPhysical
        self.Owner = ko.observable('');
        //self.TotalPropertyValue = ko.observable('');
        self.CurrentWorkingStage = ko.observable('');
        self.CompletedFloors = ko.observable('');
        self.ProposedFloors = ko.observable('');
        self.EstimatedConstructionCost = ko.observable('');

        self.IsIndividual = ko.observable(false);
        self.IsDeveloper = ko.observable(false);
        self.IsProjectStatus = ko.observable(false);
        self.LandedPropertySellertype = ko.observable();
        self.LandedPropertySellertype.subscribe(function () {

            if (self.LandedPropertySellertype() === 1) {
                self.IsFlat(true);
                self.IsIndividual(true);
                self.IsIndividualTakeOver(true);
                self.IsDeveloperTakeover(false);
                //self.IsDeveloper(false);
                //self.IsConstruction(false);

                //self.IsProjectStatus(true);
                //self.IsCommon(true);


            } else if (self.LandedPropertySellertype() === 2) {
                self.IsIndividual(false);
                self.IsDeveloper(true);
                self.IsIndividualTakeOver(false);
                self.IsDeveloperTakeover(true);
                //self.IsConstruction(true);
                //self.IsFlat(false);
                //self.IsProjectStatus(true);
                //self.IsCommon(true);

            }
        });
        self.ValuationType = ko.observable();
        self.IsDeveloperTakeover = ko.observable(false);
        self.ValuationType.subscribe(function () {
            if (self.ValuationType() === 1) {
                self.IsConstruction(false);
                self.IsFlat(true);
                self.IsProjectStatus(true);
                self.IsCommon(true);
                self.IsIndividual(true);
                self.IsDeveloper(false);
                if (self.LandedPropertySellertypes() === 2) {
                    self.IsIndividualTakeOver(false);
                    self.IsDeveloperTakeover(true);
                } else {
                    self.IsIndividualTakeOver(true);
                    self.IsDeveloperTakeover(false);
                }

            } else if (self.ValuationType() === 2) {
                self.IsConstruction(true);
                self.IsFlat(false);
                self.IsProjectStatus(true);
                self.IsCommon(true);
                self.IsIndividual(false);
                self.IsIndividualTakeOver(true);
                self.IsDeveloper(true);
                self.IsDeveloperTakeover(false);
            }
        });

        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            var err = self.errors().length;
            if (err == 0)
                return true;
            return false;
        });

        self.PropertyAddress = new address();
        /*LP Primary Security*/
        self.IsTakeover = ko.observable(false);
        self.IsConstEqt = ko.observable(false);
        self.IsFlat = ko.observable(false);
        self.IsCommon = ko.observable(false);
        self.IsConstruction = ko.observable(false);
        self.IsIndividualTakeOver = ko.observable(false);
        self.LandedPropertyLoanType = ko.observable('');
        self.LandedPropertyLoanType.subscribe(function () {
            //debugger;
            //console.log(self.LandedPropertyLoanType());
            if (self.LandedPropertyLoanType() === 1) {
                self.IsFlat(true);
                self.ValuationType(1);
                self.IsConstruction(false);
                self.IsProjectStatus(true);
                self.IsCommon(true);
                self.IsConstEqt(false);
                self.IsTakeover(false);
                //self.IsDeveloperTakeover(false);
            } else if (self.LandedPropertyLoanType() === 2 || self.LandedPropertyLoanType() === 3 || self.LandedPropertyLoanType() === 4) {
                self.IsConstruction(true);
                self.ValuationType(2);
                self.IsFlat(false);
                self.IsProjectStatus(true);
                self.IsCommon(true);
                self.IsConstEqt(true);
                self.IsTakeover(false);
                self.IsDeveloperTakeover(true);
            }
            else if (self.LandedPropertyLoanType() === 6) {
                self.IsConstruction(false);
                self.IsFlat(true);
                self.ValuationType(1);
                self.IsProjectStatus(true);
                self.IsCommon(true);
                self.IsConstEqt(true);
                self.IsTakeover(false);
                self.IsDeveloperTakeover(false);
            }
            else if (self.LandedPropertyLoanType() === 7) {
                console.log(self.ValuationType());
                self.IsTakeover(true);
                self.IsIndividual(false);
                self.IsIndividualTakeOver(true);
            }

        });
        self.TotalCost = ko.observable('');
        self.AmountPaid = ko.observable('');

        self.SourceOfRemainingFund = ko.observable('');
        self.FirstDisbursementExpDate = ko.observable('');
        self.FirstDisbursementExpDateText = ko.observable('');
        //self.LandedPropertySellertype = ko.observable('');
        self.SellerName = ko.observable('');
        self.SellerPhone = ko.observable('');
        self.PropertyAddressId = ko.observable('');
        //self.PropertyAddress = new address();
        self.DeveloperId = ko.observable('');
        self.ProjectId = ko.observable('');
        self.FlatSize = ko.observable('');
        self.FloorNo = ko.observable('');
        self.FlatSide = ko.observable('');
        //takeover
        self.TakeoverFrom = ko.observable('');
        self.PrevCompanyApprovedLoan = ko.observable('');
        self.PrevCompanyOutstandingLoan = ko.observable('');
        self.TopUpLoan = ko.observable('');
        self.CurrentInterestRate = ko.observable('');
        //home equity
        self.Owner = ko.observable('');
        self.TotalPropertyValue = ko.observable('');
        //others
        self.CurrentWorkingStage = ko.observable('');
        self.CompletedFloors = ko.observable('');
        self.ProposedFloors = ko.observable('');
        self.EstimatedConstructionCost = ko.observable('');

        self.DeveloperName = ko.observable('');
        self.ContactPerson = ko.observable('');
        self.ContactPersonDesignation = ko.observable('');
        self.ContactPersonPhone = ko.observable('');
        //public virtual Developer Developer= ko.observable('');

        self.ProjectName = ko.observable('');
        self.ProjectAddress = new address();
        //self.SellersCountryList = ko.observableArray([]);
        //self.SellersDivisionList = ko.observableArray([]);
        //self.SellersDistrictList = ko.observableArray([]);
        //self.SellersThanaList = ko.observableArray([]);

        //self.DealerCountryList = ko.observableArray([]);
        //self.DealerDivisionList = ko.observableArray([]);
        //self.DealerDistrictList = ko.observableArray([]);
        //self.DealerThanaList = ko.observableArray([]);
        self.CountryIdList = ko.observableArray([]);
        self.CIFList = ko.observableArray([]);
        self.PropertyCountryList = ko.observableArray([]);
        self.PropertyDivisionList = ko.observableArray([]);
        self.PropertyDistrictList = ko.observableArray([]);
        self.PropertyThanaList = ko.observableArray([]);
        self.LandedPropertyLoanTypes = ko.observableArray([]);
        self.ShowRooms = ko.observableArray([]);
        self.LandedPropertySellertypes = ko.observableArray([]);
        self.WaiverTypeList = ko.observableArray([]);
        self.WaiverRequestedToList = ko.observableArray([]);
        self.LandedPropertySellertypes = ko.observableArray([]);
        self.GuarantorCifList = ko.observableArray([]);
        self.LoadCountryIDList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CountryIdList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetLandedPropertySellertypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetLandedPropertySellertypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.LandedPropertySellertypes(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetLandedPropertyLoanTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetLandedPropertyLoanTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.LandedPropertyLoanTypes(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        ////////////////////////////////////////////////Latest///////////////////////////////////////////////////////////
        self.ProjectStatuses = ko.observableArray([]);
        self.ApplicantsFlatStatuses = ko.observableArray([]);
        self.VerificationStates = ko.observableArray([]);
        self.ValuationTypes = ko.observableArray([]);
        self.GetProjectStatuses = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetProjectStatuses',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProjectStatuses(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetApplicantsFlatStatuses = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetApplicantsFlatStatuses',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApplicantsFlatStatuses(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetVerificationStates = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetVerificationStates',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.VerificationStates(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetValuationType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetValuationType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ValuationTypes(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        self.LoadPrimarySecurityValuationType = function () {
            //console.log(self.AppId());
            if (self.LPPrimarySecurityId() > 0) {
                $.getJSON("/IPDC/Verification/LoadLPPrimarySecurityValuation/?id=" + self.LPPrimarySecurityId(),
                    null,
                    function (data) {
                        //debugger;
                        self.Id(data.Id);
                        self.LPPrimarySecurityId(data.LPPrimarySecurityId);
                        self.PropertyTypeName(data.PropertyTypeName);
                        self.FlatSize(data.FlatSize);
                        self.LoanType(data.LoanType);
                        self.PropornateLand(data.PropornateLand);
                        self.PresentMarketRateOfLand(data.PresentMarketRateOfLand);
                        //self.PresentMarketValueOfLand(data.PresentMarketValueOfLand);
                        self.FlatSizeWithCommonSpace(data.FlatSizeWithCommonSpace);
                        self.FlatSizeWithoutCommonSpace(data.FlatSizeWithoutCommonSpace);
                        self.PerSFTMarketPrice(data.PerSFTMarketPrice);
                        //self.MarketPriceOfFlat(data.MarketPriceOfFlat) ;
                        self.CarParkingCount(data.CarParkingCount);
                        self.CarParkingPrice(data.CarParkingPrice);
                        //self.TotalMarketValue(data.TotalMarketValue) ;
                        self.DistressPercentage(data.DistressPercentage);
                        //self.DistressValue(data.DistressValue) ;

                        $.when(self.GetProjectStatuses())
                            .done(function () {
                                self.ProjectStatus(data.ProjectStatus);
                            });
                        self.TotalWIPofFullProject(data.TotalWIPofFullProject);
                        $.when(self.GetApplicantsFlatStatuses())
                            .done(function () {
                                self.ApplicantsFlatStatus(data.ApplicantsFlatStatus);
                            });
                        self.TotalWIPofFlat(data.TotalWIPofFlat);
                        self.AreaOfLandAsPerPlan(data.AreaOfLandAsPerPlan);
                        self.AreaOfLandAsPerClient(data.AreaOfLandAsPerClient);
                        self.PerKathaPriceAsPerRAJUK(data.PerKathaPriceAsPerRAJUK);
                        self.PerKathaPriceAsPerClient(data.PerKathaPriceAsPerClient);
                        //self.MarketValueOfLandAsPerRAJUK(data.MarketValueOfLandAsPerRAJUK);
                        //self.MarketValueOfLandAsPerClient(data.MarketValueOfLandAsPerClient);
                        self.BuildUpAreaPerFloorApproved(data.BuildUpAreaPerFloorApproved);
                        self.TotalBuildUpAreaApproved(data.TotalBuildUpAreaApproved);
                        self.EstimatedConstructionCostPerSFTApproved(data.EstimatedConstructionCostPerSFTApproved);
                        self.EstimatedConstructionCostApproved(data.EstimatedConstructionCostApproved);
                        //self.LandValueAndEstimatedConstructionCostApproved(data.LandValueAndEstimatedConstructionCostApproved);
                        self.BuildUpAreaPerFloorPhysical(data.BuildUpAreaPerFloorPhysical);
                        self.TotalBuildUpAreaPhysical(data.TotalBuildUpAreaPhysical);
                        //self.EstimatedConstructionCostPerSFTPhysical(data.EstimatedConstructionCostPerSFTPhysical);
                        //self.EstimatedConstructionCostPhysical(data.EstimatedConstructionCostPhysical);
                        //self.LandValueAndEstimatedConstructionCostPhysical(data.LandValueAndEstimatedConstructionCostPhysical);
                        self.LandedPropertySellertype(data.SellerType);
                        $.when(self.GetValuationType())
                         .done(function () {
                             self.ValuationType(data.ValuationType);
                         });

                        self.Remarks(data.Remarks);
                        $.when(self.GetVerificationStates())
                           .done(function () {
                               self.VerificationState(data.VerificationState);
                           });
                    });
            }
        }
        self.LoadApplicationData = function () {
            //console.log(self.AppId());
            if (self.AppId() > 0) {
                $.getJSON("/IPDC/Application/LoadLoanApplicationByAppId/?AppId=" + self.AppId(),
                    null,
                    function (data) {
                        //debugger;
                        console.log(data);
                        if (data.LPPrimarySecurity) {
                            console.log(data.LPPrimarySecurity);
                            $.when(self.GetLandedPropertyLoanTypes())
                           .done(function () {
                               self.LandedPropertyLoanType(data.LPPrimarySecurity.LandedPropertyLoanType);

                           });
                            self.ApplicationNo(data.ApplicationNo);
                            //self.LppPrimarySecurityId(data.LPPrimarySecurity.Id);
                            self.TotalCost(data.LPPrimarySecurity.TotalCost);
                            self.AmountPaid(data.LPPrimarySecurity.AmountPaid);
                            //self.RemainingClientContribution(data.LPPrimarySecurity.RemainingClientContribution);
                            self.SourceOfRemainingFund(data.LPPrimarySecurity.SourceOfRemainingFund);
                            self.FirstDisbursementExpDate(data.LPPrimarySecurity.FirstDisbursementExpDate);
                            self.FirstDisbursementExpDateText(data.LPPrimarySecurity.FirstDisbursementExpDateText);
                            $.when(self.GetLandedPropertySellertypes())
                            .done(function () {
                                self.LandedPropertySellertype(data.LPPrimarySecurity.LandedPropertySellertype);
                            });
                            self.ContactPerson(data.LPPrimarySecurity.ContactPerson);
                            self.ContactPersonDesignation(data.LPPrimarySecurity.ContactPersonDesignation);
                            self.ContactPersonPhone = ko.observable(data.ContactPersonPhone);
                            self.SellerName(data.LPPrimarySecurity.SellerName);
                            self.SellerPhone(data.LPPrimarySecurity.SellerPhone);
                            self.PropertyAddressId(data.LPPrimarySecurity.PropertyAddressId);
                            self.DeveloperName(data.LPPrimarySecurity.DeveloperName);
                            self.ProjectName(data.LPPrimarySecurity.ProjectName);
                            self.FlatSize(data.LPPrimarySecurity.FlatSize);
                            self.FloorNo(data.LPPrimarySecurity.FloorNo);
                            self.FlatSide(data.LPPrimarySecurity.FlatSide);
                            self.TakeoverFrom(data.LPPrimarySecurity.TakeoverFrom);
                            self.PrevCompanyApprovedLoan(data.LPPrimarySecurity.PrevCompanyApprovedLoan);
                            self.PrevCompanyOutstandingLoan(data.LPPrimarySecurity.PrevCompanyOutstandingLoan);
                            self.TopUpLoan(data.LPPrimarySecurity.TopUpLoan);
                            self.CurrentInterestRate(data.LPPrimarySecurity.CurrentInterestRate);
                            self.Owner(data.LPPrimarySecurity.Owner);
                            self.TotalPropertyValue(data.LPPrimarySecurity.TotalPropertyValue);
                            self.CurrentWorkingStage(data.LPPrimarySecurity.CurrentWorkingStage);
                            self.CompletedFloors(data.LPPrimarySecurity.CompletedFloors);
                            self.ProposedFloors(data.LPPrimarySecurity.ProposedFloors);
                            self.EstimatedConstructionCost(data.LPPrimarySecurity.EstimatedConstructionCost);
                            self.LPPrimarySecurityId(data.LPPrimarySecurity.Id);
                            $.each(data.CIFList, function (index, value) {
                                var aDetail = new applicationCIFs();
                                if (typeof (value) != 'undefined') {
                                    aDetail.LoadData(value);
                                    self.CIFList.push(aDetail);
                                }
                            });
                            $.when(self.LoadCountryIDList())
                            .done(function () {
                                //console.log(data.LPPrimarySecurity.PropertyAddress);
                                self.PropertyAddress.LoadAddress(data.LPPrimarySecurity.PropertyAddress);
                                self.ProjectAddress.LoadAddress(data.LPPrimarySecurity.ProjectAddress);
                            });
                            if (self.LPPrimarySecurityId() > 0) {
                                self.LoadPrimarySecurityValuationType();
                            }
                            $.when(self.GetValuationType())
                            .done(function () {
                           if (self.LandedPropertyLoanType() === 1) {
                               self.ValuationType(1);
                           }
                           if (self.LandedPropertyLoanType() === 2) {
                               self.ValuationType(2);
                           }
                       });


                        }
                    });
            }
        }

        self.Submit = function () {
            var submitData = {
                Id: self.Id(),
                LPPrimarySecurityId: self.LPPrimarySecurityId(),
                //VerificationDate : self.VerificationDate() ,
                //VerifiedByUserId : self.VerifiedByUserId() ,
                //VerifiedByEmpDegMapId : self.VerifiedByEmpDegMapId(), 
                PropertyTypeName: self.PropertyTypeName(),
                FlatSize: self.FlatSize(),
                LoanType: self.LoanType(),
                PropornateLand: self.PropornateLand(),
                PresentMarketRateOfLand: self.PresentMarketRateOfLand(),
                PresentMarketValueOfLand: self.PresentMarketValueOfLand(),
                FlatSizeWithCommonSpace: self.FlatSizeWithCommonSpace(),
                FlatSizeWithoutCommonSpace: self.FlatSizeWithoutCommonSpace(),
                PerSFTMarketPrice: self.PerSFTMarketPrice(),
                MarketPriceOfFlat: self.MarketPriceOfFlat(),
                CarParkingCount: self.CarParkingCount(),
                CarParkingPrice: self.CarParkingPrice(),
                TotalMarketValue: self.TotalMarketValue(),
                DistressPercentage: self.DistressPercentage(),
                DistressValue: self.DistressValue(),
                ProjectStatus: self.ProjectStatus(),
                TotalWIPofFullProject: self.TotalWIPofFullProject(),
                ApplicantsFlatStatus: self.ApplicantsFlatStatus(),
                TotalWIPofFlat: self.TotalWIPofFlat(),
                AreaOfLandAsPerPlan: self.AreaOfLandAsPerPlan(),
                AreaOfLandAsPerClient: self.AreaOfLandAsPerClient(),
                PerKathaPriceAsPerRAJUK: self.PerKathaPriceAsPerRAJUK(),
                PerKathaPriceAsPerClient: self.PerKathaPriceAsPerClient(),
                MarketValueOfLandAsPerRAJUK: self.MarketValueOfLandAsPerRAJUK(),
                MarketValueOfLandAsPerClient: self.MarketValueOfLandAsPerClient(),
                BuildUpAreaPerFloorApproved: self.BuildUpAreaPerFloorApproved(),
                TotalBuildUpAreaApproved: self.TotalBuildUpAreaApproved(),
                EstimatedConstructionCostPerSFTApproved: self.EstimatedConstructionCostPerSFTApproved(),
                EstimatedConstructionCostApproved: self.EstimatedConstructionCostApproved(),
                LandValueAndEstimatedConstructionCostApproved: self.LandValueAndEstimatedConstructionCostApproved(),
                BuildUpAreaPerFloorPhysical: self.BuildUpAreaPerFloorPhysical(),
                TotalBuildUpAreaPhysical: self.TotalBuildUpAreaPhysical(),
                EstimatedConstructionCostPerSFTPhysical: self.EstimatedConstructionCostPerSFTPhysical(),
                EstimatedConstructionCostPhysical: self.EstimatedConstructionCostPhysical(),
                LandValueAndEstimatedConstructionCostPhysical: self.LandValueAndEstimatedConstructionCostPhysical(),
                SellerType: self.LandedPropertySellertype(),
                ValuationType: self.ValuationType(),
                Remarks: self.Remarks(),
                VerificationState: self.VerificationState()
            }
            console.log(ko.toJSON(submitData));
            if (self.IsValid()) {
                $.ajax({
                    type: "POST",
                    url: '/IPDC/Verification/SaveVerification',
                    data: ko.toJSON(submitData),
                    contentType: "application/json",
                    success: function (data) {
                        $('#appSuccessModal').modal('show');
                        $('#appSuccessModalText').text(data.Message);
                    },
                    error: function () {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                self.errors.showAllMessages();
            }
        }


        self.Initialize = function () {
            self.LoadCountryIDList();
            self.GetProjectStatuses();
            self.GetApplicantsFlatStatuses();
            self.GetVerificationStates();
            self.GetValuationType();

            if (self.AppId() > 0) {
                self.LoadApplicationData();
            }
        }

        self.SaveApplication = function () {
            //self.ApplicationDateText($("#ApplicationDateId").val());
            //var applicationCif = ko.observableArray([]);
            //var checkListData = ko.observableArray([]);
            //$.each(self.CIFList(),
            //    function (index, value) {
            //        //console.log("value: " + value);
            //        applicationCif.push({
            //            Id: value.Id(),
            //            ApplicationId: value.ApplicationId(),
            //            CIF_PersonalId: value.CIF_PersonalId(),
            //            ApplicantRole: value.ApplicantRole(),
            //            CIF_OrganizationalId: value.CIF_OrganizationalId(),
            //        });
            //    });
            //$.each(self.DocChecklist(),
            //    function (index, value) {
            //        //console.log("value: " + value);
            //        if (value.IsChecked() === true) {
            //            checkListData.push({
            //                Id: value.Id(),
            //                ApplicationId: value.ApplicationId(),
            //                ProductDocId: value.ProductDocId(),
            //                ProductDoc: value.ProductDoc(),
            //                DocumentStatus: value.DocumentStatus(),
            //                SubmissionDeadline: value.SubmissionDeadline(),
            //                ApprovalRequired: value.ApprovalRequired(),
            //                ApprovedById: value.ApprovedById(),
            //                ProductId: value.ProductId(),
            //                DocName: value.DocName(),
            //                IsChecked: value.IsChecked()
            //            });
            //        }
            //    });
            //var SubmitData = {
            //    Id: self.Id(),
            //    SalesLeadId: self.SalesLeadId(),
            //    ApplicationDate: self.ApplicationDate(),
            //    CustomerType: self.CustomerType(),
            //    ApplicationType: self.ApplicationType(),
            //    ContactPersonId: self.ContactPersonId(),
            //    UseConAddAsGrpAdd: self.UseConAddAsGrpAdd(),
            //    ApplicationDateText: self.ApplicationDateText(),
            //    //ContactPersonList : ko.observableArray([]);
            //    GroupAddress: self.GroupAddress,
            //    AccountTitle: self.AccountTitle(),
            //    AccGroupId: self.AccGroupId(),
            //    ProductId: self.ProductId(),
            //    ProductType: self.ProductType(),
            //    LoanApplicationId: self.LoanApplicationId(),
            //    DepositApplicationId: self.DepositApplicationId(),
            //    Term: self.Term(),
            //    CIFList: applicationCif,
            //    DocChecklist: checkListData
            //}
            //$.ajax({
            //    type: "POST",
            //    url: '/IPDC/Application/SaveApplication',
            //    data: ko.toJSON(SubmitData),
            //    contentType: "application/json",
            //    success: function (data) {
            //        self.Id(data.Id);
            //        $('#appSuccessModal').modal('show');
            //        $('#appSuccessModalText').text(data.Message);
            //        if (typeof (depositAppVM) != 'undefined') {
            //            depositAppVM.Application_Id(data.Id);
            //        }
            //        if (typeof (loanAppVM) != 'undefined') {
            //            loanAppVM.Application_Id(data.Id);
            //            loanAppVM.GetLoanAppColSecurities();
            //        }
            //    },
            //    error: function () {
            //        alert(error.status + "<--and--> " + error.statusText);
            //    }
            //});
        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));

        }
        //var appvm = new ApplicationVm();
    }


    appvm = new LppPrimarySecurityVm();

    var qValue = appvm.queryString('applicationId');
    appvm.AppId(qValue);
    var leadId = appvm.queryString('leadId');
    appvm.SalesLeadId(leadId);
    console.log(qValue);
    appvm.Initialize();
    ko.applyBindings(appvm, $('#LppPrimarySecurityVw')[0]);



});



