/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
/// <reference path="~/Scripts/knockout.validation.min.js" />
$(function () {
    $('#ApplicationDateId').datetimepicker({ format: 'DD/MM/YYYY' });
    //$('#MaturityDateText').datetimepicker({ format: 'DD/MM/YYYY' });
    $('#FirstDisbursementExpDateTxt').datetimepicker({ format: 'DD/MM/YYYY' });
    $('#SubmissionDeadlineId').datetimepicker({ format: 'DD/MM/YYYY' });
});
//$(document).ready(function () {

ko.validation.init({
    errorElementClass: 'has-error',
    errorMessageClass: 'help-block',
    decorateInputElement: true,
    grouping: { deep: true, observable: true }
});
function loanAppWaiverReq() {
    var self = this;
    self.Id = ko.observable();
    self.WaiverType = ko.observable('');
    self.WaiverRequestedToId = ko.observable('');
    self.LoadData = function (data) {
        self.Id(data ? data.Id : '');
        self.WaiverType(data ? data.WaiverType : 0);
        self.WaiverRequestedToId(data ? data.WaiverRequestedToId : 0);
    }
}
function loanAppColSecurity(data) {
    var self = this;
    self.Id = ko.observable();
    self.ColSecurityId = ko.observable(data ? data.ColSecurityId : 0);
    self.ProductId = ko.observable(data ? data.ProductId : 0);
    self.ProductName = ko.observable(data ? data.ProductName : "");
    self.SecurityDescription = ko.observable(data ? data.SecurityDescription : "");
    self.IsChecked = ko.observable(data ? data.IsChecked : false);
    self.LoadData = function (data) {
        self.Id(data ? data.Id : '');
        self.ColSecurityId(data ? data.ColSecurityId : 0);
        self.ProductId(data ? data.ProductId : 0);
        self.ProductName(data ? data.ProductName : "");
        self.SecurityDescription(data ? data.SecurityDescription : "");
        self.IsChecked(data ? data.IsChecked : false);
    }
}

function fDRPSDetail(data) {
    var self = this;
    self.Id = ko.observable(data ? data.Id : '');
    self.FDRAccountNo = ko.observable(data ? data.FDRAccountNo : "");
    self.Amount = ko.observable(data ? data.Amount : 0);
    self.FDRAmountFormatted = ko.pureComputed({
        read: function () {
            if (self.Amount() > 0)
                return self.Amount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.Amount(isNaN(value) ? 0 : value);
        },
        owner: self
    });
    self.Depositor = ko.observable(data ? data.Depositor : "");
    self.MaturityDate = ko.observable(data ? data.MaturityDate : null);
    self.MaturityDateTxt = ko.observable(data ? data.MaturityDateTxt : "");
    self.RelationshipWithApplicant = ko.observable(data ? data.RelationshipWithApplicant : "");
    self.DisbursementTo = ko.observable(data ? data.DisbursementTo : '');
    self.InstituteName = ko.observable(data ? data.InstituteName : '');
    self.BranchName = ko.observable(data ? data.BranchName : '');
    self.LoadData = function (data) {
        self.Id(data ? data.Id : '');
        self.FDRAccountNo(data ? data.FDRAccountNo : "");
        self.Amount(data ? data.Amount : 0);
        self.Depositor(data ? data.Depositor : "");
        self.MaturityDate(data ? data.MaturityDate : null);
        self.MaturityDateTxt(data ? data.MaturityDateTxt : "");
        self.DisbursementTo(data ? data.DisbursementTo : '');
        self.RelationshipWithApplicant(data ? data.RelationshipWithApplicant : "");
        self.InstituteName(data ? data.InstituteName : '');
        self.BranchName(data ? data.BranchName : '');
    }
}

function address() {
    var self = this;
    self.Id = ko.observable('');
    self.ThanaId = ko.observable('');
    self.ThanaName = ko.observable('');
    self.DistrictId = ko.observable('');
    self.DistrictName = ko.observable('');
    self.DivisionId = ko.observable('');
    self.DivisionName = ko.observable('');
    self.CountryId = ko.observable('');
    self.CountryName = ko.observable('');
    self.AddressLine1 = ko.observable('');
    self.AddressLine2 = ko.observable('');
    self.AddressLine3 = ko.observable('');
    self.PostalCode = ko.observable('');
    self.PhoneNo = ko.observable('');
    self.CellPhoneNo = ko.observable('');
    self.Email = ko.observable('');
    self.IsChanged = ko.observable(false);
    self.Initialize = function (data) {

        self.ThanaId(data ? data.ThanaId : "");
        self.ThanaName(data ? data.ThanaName : "");
        self.DistrictId(data ? data.DistrictId : "");
        self.DistrictName(data ? data.DistrictName : "");
        self.DivisionId(data ? data.DivisionId : "");
        self.DivisionName(data ? data.DivisionName : "");
        self.CountryId(data ? data.CountryId : "");
        self.CountryName(data ? data.CountryName : "");
        self.AddressLine1(data ? data.AddressLine1 : "");
        self.AddressLine2(data ? data.AddressLine2 : "");
        self.AddressLine3(data ? data.AddressLine3 : "");
        self.PostalCode(data ? data.PostalCode : "");
        self.PhoneNo(data ? data.PhoneNo : "");
        self.CellPhoneNo(data ? data.CellPhoneNo : "");
        self.Email(data ? data.Email : "");
    }
}
function guarantor() {
    var self = this;
    self.Id = ko.observable();
    self.GuarantorCifId = ko.observable();
    self.GuarantorCif = ko.observable();
    self.GuarantorCif.subscribe(function () {
        self.GuarantorCifId(self.GuarantorCif().key);
    });
    self.GuaranteeAmount = ko.observable('');
    self.GuaranteeAmountFormatted = ko.pureComputed({
        read: function () {
            if (self.GuaranteeAmount() > 0)
                return self.GuaranteeAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.GuaranteeAmount(isNaN(value) ? 0 : value);
        },
        owner: self
    });
    self.RelationshipWithApplicant = ko.observable();
    self.LoadData = function (data) {
        self.Id(data.Id);
        self.GuarantorCif(data.GuarantorCif);
        self.GuarantorCifId(data.GuarantorCifId);
        self.GuaranteeAmount(data.GuaranteeAmount);
        self.RelationshipWithApplicant(data.RelationshipWithApplicant);
    }
    //self.Initialize = function () {
    //    self.GetGuarantorCifLists();
    //}
}
function otherSecurities() {
    var self = this;

    self.Id = ko.observable();
    self.LoanApplicationId = ko.observable();
    self.SecurityDescription = ko.observable();

    self.LoadData = function (data) {
        self.Id(data.Id);
        self.LoanApplicationId(data.LoanApplicationId);
        self.SecurityDescription(data.SecurityDescription);
    }
}

function LoanApplicationVm() {
    var self = this;

    var i = 0;
    for (i = new Date().getFullYear() ; i > new Date().getFullYear() - 100; i--) {
        $('#MnufacturingYear').append($('<option />').val(i).html(i));
        $('#YearModel').append($('<option />').val(i).html(i));

    }
    for (i = new Date().getFullYear() + 1 ; i > new Date().getFullYear() - 10; i--) {
        $('#RegistrationYear').append($('<option />').val(i).html(i));
    }

    self.Application_Id = ko.observable();
    self.Term = ko.observable();
    self.VehicleId = ko.observable('');
    self.LppPrimarySecurityId = ko.observable('');
    self.FDRId = ko.observable('');
    self.ConsumerGoodsId = ko.observable('');

    self.IsDealer = ko.observable(false);
    self.IsVendor = ko.observable(true);
    self.IsAmount = ko.observable();


    self.id = ko.observable('');
    self.Id = ko.observable('');
    self.LoanAmountApplied = ko.observable();
    self.LoanAmountFormatted = ko.pureComputed({
        read: function () {
            if (self.LoanAmountApplied() > 0)
                return self.LoanAmountApplied().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.LoanAmountApplied(isNaN(value) ? 0 : value);
        },
        owner: self
    });
    self.Rate = ko.observable('');

    //self.Rate.subscribe(function () {
    //    self.DeveloperId('');
    //    self.SellerName('');
    //    self.SellerPhone('');

    //    if (typeof (self.SelectedDeveloperId()) != 'undefined' && self.SelectedDeveloperId().Id > 0) {

    //        self.DeveloperId(self.SelectedDeveloperId().Id);
    //        self.SellerName(self.SelectedDeveloperId().ContactPerson);
    //        self.SellerPhone(self.SelectedDeveloperId().ContactPersonPhone);
    //    }
    //});

    self.ServiceChargeRate = ko.observable('');
    self.ServiceChargeAmount = ko.observable();
    //self.ServiceChargeFormatted = ko.pureComputed({
    //    read: function () {
    //        if (self.ServiceChargeAmount() > 0)
    //            return self.ServiceChargeAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    //    },
    //    write: function (value) {
    //        value = parseFloat(value.replace(/,/g, ""));
    //        self.ServiceChargeAmount(isNaN(value) ? 0 : value);
    //    },
    //    owner: self
    //});
    //self.ChkProcessingFee = ko.computed(function () {
    //    //self.IsAmount();
    //    if (self.ServiceChargeRate() > 0 || self.ServiceChargeAmount() <=0 ) {
    //        self.IsAmount(1);
    //    } else if (self.ServiceChargeAmount() > 0 || self.ServiceChargeRate()<=0) {
    //        self.IsAmount(2);
    //    }
    //});
    self.ChkProcessingFeeRate = ko.computed(function () {
        //self.IsAmount();
        if (self.ServiceChargeRate() > 0 && self.LoanAmountApplied() > 0) {
            var loanAmt = parseFloat(self.LoanAmountApplied() ? self.LoanAmountApplied() : 0);
            var rate = parseFloat(self.ServiceChargeRate() ? self.ServiceChargeRate() : 0);
            var result = ((loanAmt * rate) / 100).toFixed(2);
            self.ServiceChargeAmount(result);
        }
    });
    self.DocumentationFee = ko.observable();
    self.DocumentFeeFormatted = ko.pureComputed({
        read: function () {
            if (self.DocumentationFee() > 0)
                return self.DocumentationFee().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.DocumentationFee(isNaN(value) ? 0 : value);
        },
        owner: self
    });
    self.OtherFees = ko.observable('');
    self.OtherFeesFormatted = ko.pureComputed({
        read: function () {
            if (self.OtherFees() > 0)
                return self.OtherFees().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.OtherFees(isNaN(value) ? 0 : value);
        },
        owner: self
    });
    self.HasOtherPurpose = ko.observable(false);
    self.addPurpose = function () {
        self.HasOtherPurpose(true);
    }
    self.Purpose = ko.observable('').extend({ required: true });
    self.LoanPrimarySecurityType = ko.observable('');
    self.LoanAppColSecurities = ko.observableArray([]);
    self.Relationships = ko.observableArray([]);
    self.OtherSecurity = ko.observable('');
    self.OtherSecurities = ko.observableArray([]);
    self.DisbursementMode = ko.observable('');
    self.PayeesAccountNo = ko.observable('');
    self.PayeesName = ko.observable('');
    self.Bank = ko.observable('');
    self.Branch = ko.observable('');
    self.RoutingNo = ko.observable('').extend({ digit: true });
    self.LoanChequeDeliveryOption = ko.observable('');
    self.WaiverRequests = ko.observableArray([]);
    self.BeneficialOwner = ko.observable('').extend({ required: true });
    self.SourceOfFund = ko.observable('').extend({ required: true });
    self.SourceOfFundVerificationMethod = ko.observable('').extend({ required: true });
    self.SourceOfFundConsistency = ko.observable('').extend({ required: true });
    self.RiskLevel = ko.observable('');
    self.RiskLevels = ko.observableArray([]);
    self.Comment = ko.observable('');
    self.Guarantors = ko.observableArray([]);

    self.IsVehicle = ko.observable(false);
    self.IsConsumerPrimarySecurity = ko.observable(false);
    self.IsFDRPrimarySecurity = ko.observable(false);
    self.IsLpPrimarySecurity = ko.observable(false);
    self.changeVendor = function () {
        self.IsVendor(false);
    }

    self.errors = ko.validation.group(self);
    self.IsValid = ko.computed(function () {
        var err = self.errors().length;
        if (err == 0)
            return true;
        return false;
    });

    /*Vehicle*/
    self.VehicleStatus = ko.observable('');
    self.VendorType = ko.observable('');
    self.SellersName = ko.observable('');
    self.SellersAddressId = ko.observable('');
    self.SellersAddress = new address();
    self.SellersPhone = ko.observable('');
    self.VendorId = ko.observable('');
    self.VehicleType = ko.observable('');
    self.VehicleName = ko.observable('');
    self.Manufacturer = ko.observable('');
    self.MnufacturingYear = ko.observable('');
    self.YearModel = ko.observable('');
    self.RegistrationYear = ko.observable('');
    self.RegistrationNo = ko.observable('').extend({
        pattern: {
            message: 'Vehicle Reg Number can only contain a-z, A-Z, 0-9',
            params: /^([A-Za-z0-9\-])+$/
        }
    });
    self.CC = ko.observable().extend({ maxLength: 4 });
    self.Colour = ko.observable('');
    self.ChassisNo = ko.observable('').extend({
        pattern: {
            message: 'Chassis Number can only contain a-z, A-Z, 0-9',
            params: /^([A-Za-z0-9\-])+$/
        }
    });
    self.EngineNo = ko.observable('').extend({
        pattern: {
            message: 'Engine Number can only contain a-z, A-Z, 0-9',
            params: /^([A-Za-z0-9\-])+$/
        }
    });
    self.Price = ko.observable('');
    /*Consumer Goods Primary Security*/
    self.LoanApplicationId = ko.observable('');
    self.Item = ko.observable('');
    self.Brand = ko.observable('');
    self.Dealer = ko.observable('');
    self.DealerAddressId = ko.observable('');

    self.DealerAddress = new address();
    self.ShowRoomId = ko.observable('');
    self.ConsumerGoodsPrimarySecurityPrice = ko.observable('');

    /*LP Primary Security*/
    self.LandedPropertyLoanType = ko.observable('');
    self.TotalCost = ko.observable('');
    self.TotalCostFormatted = ko.pureComputed({
        read: function () {
            if (self.TotalCost() > 0)
                return self.TotalCost().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.TotalCost(isNaN(value) ? 0 : value);
        },
        owner: self
    });
    self.AmountPaid = ko.observable('');
    self.PaidAmountFormatted = ko.pureComputed({
        read: function () {
            if (self.AmountPaid() > 0)
                return self.AmountPaid().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        write: function (value) {
            value = parseFloat(value.replace(/,/g, ""));
            self.AmountPaid(isNaN(value) ? 0 : value);
        },
        owner: self
    });
    self.LandType = ko.observable();
    //self.RemainingClientContribution = ko.observable('');//total cost - amound paid - loan amount applied
    self.RemainingClientContribution = ko.pureComputed(function () {

        var totalCost = 0;
        totalCost += parseFloat(self.TotalCost());

        var amountPaid = 0;
        amountPaid += parseFloat(self.AmountPaid());

        var appliedLoan = 0;
        appliedLoan += parseFloat(self.LoanAmountApplied());

        return (totalCost - amountPaid - appliedLoan);

    });
    self.RemainingClientContributionFormatted = ko.pureComputed({
        read: function () {
            if (self.RemainingClientContribution() > 0)
                return self.RemainingClientContribution().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },
        owner: self
    });
    self.SourceOfRemainingFund = ko.observable('');
    self.FirstDisbursementExpDate = ko.observable('');
    self.FirstDisbursementExpDateText = ko.observable('');
    self.LandedPropertySellertype = ko.observable('');
    self.SellerName = ko.observable('');


    self.SellerPhone = ko.observable('');
    self.PropertyAddressId = ko.observable('');
    self.PropertyAddress = new address();
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



    self.GroupAddress = new address();
    self.SellersCountryList = ko.observableArray([]);
    self.SellersDivisionList = ko.observableArray([]);
    self.SellersDistrictList = ko.observableArray([]);
    self.SellersThanaList = ko.observableArray([]);

    self.DealerCountryList = ko.observableArray([]);
    self.DealerDivisionList = ko.observableArray([]);
    self.DealerDistrictList = ko.observableArray([]);
    self.DealerThanaList = ko.observableArray([]);

    self.PropertyCountryList = ko.observableArray([]);
    self.PropertyDivisionList = ko.observableArray([]);
    self.PropertyDistrictList = ko.observableArray([]);
    self.PropertyThanaList = ko.observableArray([]);

    self.ShowRooms = ko.observableArray([]);
    self.LandedPropertySellertypes = ko.observableArray([]);
    self.WaiverTypeList = ko.observableArray([]);
    self.WaiverRequestedToList = ko.observableArray([]);

    //self.GuarantorCifList = ko.observableArray([]);
    //self.GetGuarantorCifLists = function () {
    //    return $.ajax({
    //        type: "GET",
    //        url: '/IPDC/CIF/GetAllCif',
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        success: function (data) {
    //            self.GuarantorCifList(data); //Put the response in ObservableArray
    //        },
    //        error: function (error) {
    //            alert(error.status + "<--and--> " + error.statusText);
    //        }
    //    });
    //}
    self.SaveVendor = function () {
        //var parameters = [{
        //    Name: 'ProposalId',
        //    Value: self.ProposalId()
        //},
        //   {
        //       Name: 'OfferId',
        //       Value: self.Id()
        //   }];
        var menuInfo = {
            Id: 93,
            Menu: 'Vendor Entry',
            Url: '/IPDC/Vendor/VendorEntry'
            //Parameters: parameters
        }
        window.parent.AddTabFromExternal(menuInfo);
    }
    self.GetWaiverTypes = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetWaiverTypes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.WaiverTypeList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.GetWaiverRequestedToLists = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationSetting/GetAllDesignationSettings',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.WaiverRequestedToList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

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

    self.ViewCardRate = function () {
        return '/IPDC/Application/Download';
    }
    self.GetShowRooms = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Vendor/GetAllVendorShowrooms?vendorId=' + self.VendorId(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ShowRooms(data); //Put the response in ObservableArray

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.DisbursementModes = ko.observableArray([]);
    self.LoanChequeDeliveryOptions = ko.observableArray([]);
    self.IsAddFieldsEnabled = ko.observable(true);
    self.LoanPrimarySecurityTypes = ko.observableArray([]);
    self.VehicleStatuses = ko.observableArray([]);
    self.VendorTypes = ko.observableArray([]);
    self.Vendors = ko.observableArray([]);
    self.VehicleTypes = ko.observableArray([]);
    self.LandedPropertyLoanTypes = ko.observableArray([]);
    self.Developers = ko.observableArray([]);
    self.SelectedDeveloperId = ko.observable();

    self.SelectedDeveloperId.subscribe(function () {
        self.DeveloperId('');
        self.SellerName('');
        self.SellerPhone('');
        if (typeof (self.SelectedDeveloperId()) != 'undefined' && self.SelectedDeveloperId().Id > 0) {
            self.DeveloperId(self.SelectedDeveloperId().Id);
            self.SellerName(self.SelectedDeveloperId().ContactPerson);
            self.SellerPhone(self.SelectedDeveloperId().ContactPersonPhone);
        }
    });
    self.Projects = ko.observableArray([]);
    self.FDRPSDetails = ko.observableArray([]);
    self.IsSeller = ko.observable(false);
    //self.IsVendor = ko.observable(false);
    self.VendorType.subscribe(function () {
        if (self.VendorType() === 1) {
            self.IsSeller(true);
            self.IsVendor(false);
        } else {
            self.IsSeller(false);
            self.IsVendor(true);
        }
    });
    self.IsFlat = ko.observable(false);
    self.IsTakeover = ko.observable(false);
    self.IsHome = ko.observable(false);
    self.IsPlot = ko.observable(false);
    self.IsConstruction = ko.observable(false);
    self.IsExtension = ko.observable(false);
    self.IsPropAddress = ko.observable(false);
    self.IsCommonSeller = ko.observable(false);
    self.IsCommonFlat = ko.observable(false);
    self.total = ko.observable(false);
    self.owner = ko.observable(false);
    self.IsFlatAndPlot = ko.observable(false);

    self.IsIndividual = ko.observable(false);
    self.IsDeveloper = ko.observable(false);
    self.LandedPropertySellertype.subscribe(function () {
        if (self.LandedPropertySellertype() === 1) {
            self.IsCommonSeller(true);
            //self.IsIndividual(true);
            //self.IsDeveloper(false);
        }

        if (self.LandedPropertySellertype() === 2) {
            self.IsCommonSeller(false);
            //self.IsIndividual(false);
            self.IsDeveloper(true);
        }
    });

    self.chkLoanType = function () {
        if (self.LandedPropertyLoanType() === 1) {
            self.IsFlat(true);
            self.IsTakeover(false);
            self.IsHome(false);
            self.IsPlot(false);
            self.IsConstruction(false);
            self.IsPropAddress(true);
            self.IsCommonSeller(true);
            self.IsCommonFlat(true);
            self.total(true);
            self.owner(false);
            self.IsFlatAndPlot(true);
        }
        if (self.LandedPropertyLoanType() === 2 || self.LandedPropertyLoanType() === 3 || self.LandedPropertyLoanType() === 4) {
            self.IsFlat(false);
            self.IsTakeover(false);
            self.IsHome(false);
            self.IsPlot(false);
            self.IsConstruction(true);
            self.IsPropAddress(true);
            self.IsCommonSeller(false);
            self.IsCommonFlat(false);
            self.total(false);
            self.owner(true);
            self.IsFlatAndPlot(false);
        }
        if (self.LandedPropertyLoanType() === 7) {
            self.IsFlat(false);
            self.IsTakeover(true);
            self.IsHome(false);
            self.IsPlot(false);
            self.IsConstruction(false);
            self.IsPropAddress(true);
            self.IsCommonSeller(false);
            self.IsCommonFlat(true);
            self.total(true);
            self.owner(false);
            self.IsFlatAndPlot(false);
        }
        if (self.LandedPropertyLoanType() === 6) {
            self.IsFlat(false);
            self.IsTakeover(false);
            self.IsHome(true);
            self.IsPlot(false);
            self.IsConstruction(false);
            self.IsPropAddress(true);
            self.IsCommonSeller(false);
            self.IsCommonFlat(true);
            self.total(false);
            self.owner(true);
            self.IsFlatAndPlot(false);
        }
        if (self.LandedPropertyLoanType() === 5) {
            self.IsFlat(false);
            self.IsTakeover(false);
            self.IsHome(false);
            self.IsPlot(true);
            self.IsConstruction(false);
            self.IsPropAddress(true);
            self.IsCommonSeller(true);
            self.IsCommonFlat(false);
            self.total(true);
            self.owner(false);
            self.IsFlatAndPlot(true);
        }
    }

    self.GetProjectsByDevelopers = function () {
        if (self.DeveloperId() > 0) {
            var developerId = self.DeveloperId();
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProjectsByDevelopers?id=' + developerId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Projects(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

    }

    self.GetAllDevelopers = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetAllDevelopers',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.Developers(data); //Put the response in ObservableArray

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
    self.GetVendors = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Vendor/GetAllVendors',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.Vendors(data); //Put the response in ObservableArray

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.GetVehicleTypes = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetVehicleTypes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.VehicleTypes(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.GetLoanPrimarySecurityTypes = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetLoanPrimarySecurityTypes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.LoanPrimarySecurityTypes(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.GetDisbursementModes = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetDisbursementModes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.DisbursementModes(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.GetLoanChequeDeliveryOptions = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetLoanChequeDeliveryOptions',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.LoanChequeDeliveryOptions(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.GetRiskLevels = function () {
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
    }

    self.GetVehicleStatuses = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetVehicleStatus',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.VehicleStatuses(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.GetVendorTypes = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetVendorTypes',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.VendorTypes(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.AddFDRPSDetail = function () {
        var aDetail = new fDRPSDetail();
        //aDetail.Initialize();
        self.FDRPSDetails.push(aDetail);
    }


    self.GetLoanAppColSecurities = function () {
        self.LoanAppColSecurities([]);
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetLoanAppColSecurities?appId=' + self.Application_Id(), //self.id(),
            contentType: "application/json",
            success: function (data) {
                $.each(data, function (index, value) {
                    self.LoanAppColSecurities.push(new loanAppColSecurity(value));
                });
            },
            error: function () {
                alert(error.status + "<--and-->" + error.statusText);
            }
        });
    }
    //OtherSecurities
    self.addOtherSecurities = function () {
        self.OtherSecurities.push(new otherSecurities());
    }
    self.RemovedOtherSecurities = ko.observableArray([]);
    self.removeOtherSecurities = function (line) {
        if (line.Id() > 0)
            self.RemovedOtherSecurities.push(line.Id());
        self.OtherSecurities.remove(line);
    }

    self.addWaiverRequests = function () {
        var aDetail = new loanAppWaiverReq();
        //aDetail.Initialize();
        self.WaiverRequests.push(aDetail);
    }
    self.RemovedWaiverRequests = ko.observableArray([]);
    self.removeWaiverRequests = function (line) {
        if (line.Id() > 0)
            self.RemovedWaiverRequests.push(line.Id());
        self.WaiverRequests.remove(line);
    }
    self.RemovedFDRPSDetails = ko.observableArray([]);
    self.removeFDRPSDetail = function (line) {
        if (line.Id() > 0)
            self.RemovedFDRPSDetails.push(line.Id());
        self.FDRPSDetails.remove(line);
    }
    self.addGuarantors = function () {
        var aDetail = new guarantor();
        self.Guarantors.push(aDetail);
    }
    self.RemovedGuarantors = ko.observableArray([]);
    self.removeGuarantors = function (line) {
        if (line.Id() > 0)
            self.RemovedGuarantors.push(line.Id());
        self.Guarantors.remove(line);
    };
    self.selectPrimarySecurity = function () {
        if (self.LoanPrimarySecurityType() === 1) {
            self.IsVehicle(true);
            self.IsConsumerPrimarySecurity(false);
            self.IsFDRPrimarySecurity(false);
            self.IsLpPrimarySecurity(false);
            $("#primarySecurity").show();
        }
        else if (self.LoanPrimarySecurityType() === 2) {
            self.IsVehicle(false);
            self.IsConsumerPrimarySecurity(true);
            self.IsFDRPrimarySecurity(false);
            self.IsLpPrimarySecurity(false);
            $("#primarySecurity").show();
        }
        else if (self.LoanPrimarySecurityType() === 3) {
            self.IsVehicle(false);
            self.IsConsumerPrimarySecurity(false);
            self.IsFDRPrimarySecurity(true);
            self.IsLpPrimarySecurity(false);
            $("#primarySecurity").show();
        }
        else if (self.LoanPrimarySecurityType() === 4) {
            self.IsVehicle(false);
            self.IsConsumerPrimarySecurity(false);
            self.IsFDRPrimarySecurity(false);
            self.IsLpPrimarySecurity(true);
            $("#primarySecurity").show();
        }
        else {
            $("#primarySecurity").hide();
        }
    }

    self.GetSellersCountry = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetCountries',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.SellersCountryList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.LoadSellersDivisionByCountry = function () {
        var countryId = self.SellersAddress.CountryId() ? self.SellersAddress.CountryId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + countryId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                self.SellersDivisionList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.getSellersDistrictByDivision = function () {
        var divisionId = self.SellersAddress.DivisionId() ? self.SellersAddress.DivisionId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                self.SellersDistrictList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.getSellersUpzilaByDistrict = function () {
        var districtId = self.SellersAddress.DistrictId() ? self.SellersAddress.DistrictId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/SalesLead/GetThanasByDistrict?districtId=' + districtId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.SellersThanaList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }


    /////////////////////////////////////////////////////////////////////////
    self.GetDealerCountry = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetCountries',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.DealerCountryList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.LoadDealerDivisionByCountry = function () {
        var countryId = self.DealerAddress.CountryId() ? self.DealerAddress.CountryId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + countryId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                self.DealerDivisionList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.getDealerDistrictByDivision = function () {
        var divisionId = self.DealerAddress.DivisionId() ? self.DealerAddress.DivisionId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                self.DealerDistrictList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.getDealerUpzilaByDistrict = function () {
        var districtId = self.DealerAddress.DistrictId() ? self.DealerAddress.DistrictId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/SalesLead/GetThanasByDistrict?districtId=' + districtId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.DealerThanaList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    ////////////////////////////////////////////////////////////////////////

    self.GetPropertyCountry = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetCountries',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.PropertyCountryList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.LoadPropertyDivisionByCountry = function () {
        var countryId = self.PropertyAddress.CountryId() ? self.PropertyAddress.CountryId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + countryId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                self.PropertyDivisionList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.getPropertyDistrictByDivision = function () {
        var divisionId = self.PropertyAddress.DivisionId() ? self.PropertyAddress.DivisionId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                self.PropertyDistrictList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.getPropertyUpzilaByDistrict = function () {
        var districtId = self.PropertyAddress.DistrictId() ? self.PropertyAddress.DistrictId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/SalesLead/GetThanasByDistrict?districtId=' + districtId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                self.PropertyThanaList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.GetOnlyShowRooms = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Vendor/GetOnlyShowRooms',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ShowRooms(data); //Put the response in ObservableArray

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.IsSellerAddressChanged = function () {
        self.SellersAddress.IsChanged(true);
    }

    self.IsPropertyAddressChanged = function () {
        self.PropertyAddress.IsChanged(true);
    }

    self.RelationshipWithApplicantList = ko.observableArray([]);
    self.GetRelationshipWithApplicantList = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetRelationshipWithApplicant',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.RelationshipWithApplicantList(data); //Put the response in ObservableArray

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });

    }
    self.DisbursementToList = ko.observableArray([]);
    self.GetDisbursementToList = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Application/GetDisbursementToList',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.DisbursementToList(data); //Put the response in ObservableArray

            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });

    }
    self.IsProjectStatus = ko.computed(function () {
        if (self.ProjectId() > 0) {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProjectAddress?projectId=' + self.ProjectId(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    if (data != null) {
                        //self.ConsumerGoodsPrimarySecurity.PropertyAddress.Id(data.ConsumerGoodsPrimarySecurity.PropertyAddress.Id);
                        self.PropertyAddress.AddressLine1(data.AddressLine1);
                        self.PropertyAddress.AddressLine2(data.AddressLine2);
                        self.PropertyAddress.AddressLine3(data.AddressLine3);
                        self.PropertyAddress.PostalCode(data.PostalCode);
                        self.PropertyAddress.Email(data.Email);
                        self.PropertyAddress.PhoneNo(data.PhoneNo);
                        self.PropertyAddress.CellPhoneNo(data.CellPhoneNo);
                        self.PropertyAddress.PostalCode(data.PostalCode);
                        $.when(self.GetPropertyCountry())
                            .done(function () {
                                self.PropertyAddress.CountryId(data.CountryId);
                                $.when(self.LoadPropertyDivisionByCountry())
                                    .done(function () {
                                        self.PropertyAddress.DivisionId(data.DivisionId);
                                        $.when(self.getPropertyDistrictByDivision())
                                            .done(function () {
                                                self.PropertyAddress.DistrictId(data.DistrictId);
                                                $.when(self.getPropertyUpzilaByDistrict())
                                                    .done(function () {
                                                        self.PropertyAddress.ThanaId(data.ThanaId);

                                                    });
                                            });
                                    });
                            });
                        self.IsPropAddress(true);
                    }

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
    });

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

    self.LoadLoanApplicationData = function () {
        if (self.Application_Id() > 0) {
            $.getJSON("/IPDC/Application/LoadLoanApplicationByAppId/?AppId=" + self.Application_Id(),
                null,
                function (data) {
                    self.LoanAppColSecurities([]);
                    self.WaiverRequests([]);
                    self.Guarantors([]);
                    self.LoanAppColSecurities([]);
                    self.OtherSecurities([]);

                    self.Term(data.Term);
                    self.Id(data.Id);
                    self.LoanAmountApplied(data.LoanAmountApplied);
                    self.Rate(data.Rate);
                    self.ServiceChargeRate(data.ServiceChargeRate);
                    self.ServiceChargeAmount(data.ServiceChargeAmount);
                    self.DocumentationFee(data.DocumentationFee);
                    self.OtherFees(data.OtherFees);
                    self.Purpose(data.Purpose);
                    $.when(self.GetLoanPrimarySecurityTypes())
                        .done(function () {
                            self.LoanPrimarySecurityType(data.LoanPrimarySecurityType);
                            $.when(self.selectPrimarySecurity())
                                .done(function () {
                                    //self.selectPrimarySecurity();
                                });
                        });
                    $.when(self.GetDisbursementModes())
                       .done(function () {
                           self.DisbursementMode(data.DisbursementMode);
                       });
                    $.when(self.GetLoanChequeDeliveryOptions())
                       .done(function () {
                           self.LoanChequeDeliveryOption(data.LoanChequeDeliveryOption);
                       });
                    $.when(self.GetRiskLevels())
                       .done(function () {
                           self.RiskLevel(data.RiskLevel);
                       });
                    self.OtherSecurity(data.OtherSecurity);
                    self.PayeesAccountNo(data.PayeesAccountNo);
                    self.PayeesName(data.PayeesName);
                    self.Bank(data.Bank);
                    self.Branch(data.Branch);
                    self.RoutingNo(data.RoutingNo);
                    self.BeneficialOwner(data.BeneficialOwner);
                    self.SourceOfFund(data.SourceOfFund);
                    self.SourceOfFundVerificationMethod(data.SourceOfFundVerificationMethod);
                    self.SourceOfFundConsistency(data.SourceOfFundConsistency);
                    self.RiskLevels(data.RiskLevels);
                    self.Comment(data.Comment);



                    $.when(self.GetWaiverTypes())
                        .done(function () {
                            $.when(self.GetWaiverRequestedToLists())
                                .done(function () {
                                    $.each(data.WaiverRequests,
                                    function (index, value) {
                                        var aDetail = new loanAppWaiverReq();
                                        if (typeof (value) != 'undefined') {
                                            aDetail.LoadData(value);
                                            self.WaiverRequests.push(aDetail);
                                        }

                                    });
                                });
                        });

                    //$.when(self.GetGuarantorCifLists())
                    //  .done(function () {
                    $.when(self.GetRelationships())
                    .done(function () {
                        //
                        $.each(data.Guarantors,
                        function (index, value) {
                            var aDetail = new guarantor();
                            if (typeof (value) != 'undefined') {
                                aDetail.LoadData(value);
                                self.Guarantors.push(aDetail);
                            }

                        });
                    })
                    //});


                    $.each(data.LoanAppColSecurities,
                              function (index, value) {
                                  var aDetail = new loanAppColSecurity();
                                  if (typeof (value) != 'undefined') {
                                      aDetail.LoadData(value);
                                      self.LoanAppColSecurities.push(aDetail);
                                  }
                              });
                    $.each(data.OtherSecurities,
                              function (index, value) {
                                  var aDetail = new otherSecurities();
                                  if (typeof (value) != 'undefined') {
                                      aDetail.LoadData(value);
                                      self.OtherSecurities.push(aDetail);
                                  }
                              });
                    if (data.VehiclePrimarySecurity != null) {
                        $.when(self.GetVehicleStatuses())
                          .done(function () {
                              self.VehicleStatus(data.VehiclePrimarySecurity.VehicleStatus);
                          });
                        $.when(self.GetVendorTypes())
                          .done(function () {
                              self.VendorType(data.VehiclePrimarySecurity.VendorType);
                          });
                        $.when(self.GetVendors())
                          .done(function () {
                              self.VendorId(data.VehiclePrimarySecurity.VendorId);
                          });
                        $.when(self.GetVehicleTypes())
                          .done(function () {
                              self.VehicleType(data.VehiclePrimarySecurity.VehicleType);
                          });
                        self.VehicleName(data.VehiclePrimarySecurity.VehicleName);
                        self.SellersName(data.VehiclePrimarySecurity.SellersName);
                        self.SellersAddressId(data.VehiclePrimarySecurity.SellersAddressId);
                        self.SellersPhone(data.VehiclePrimarySecurity.SellersPhone);
                        self.Manufacturer(data.VehiclePrimarySecurity.Manufacturer);
                        self.MnufacturingYear(data.VehiclePrimarySecurity.MnufacturingYear);
                        self.YearModel(data.VehiclePrimarySecurity.YearModel);
                        self.RegistrationYear(data.VehiclePrimarySecurity.RegistrationYear);
                        self.RegistrationNo(data.VehiclePrimarySecurity.RegistrationNo);
                        self.CC(data.VehiclePrimarySecurity.CC);
                        self.Colour(data.VehiclePrimarySecurity.Colour);
                        self.ChassisNo(data.VehiclePrimarySecurity.ChassisNo);
                        self.EngineNo(data.VehiclePrimarySecurity.EngineNo);
                        self.Price(data.VehiclePrimarySecurity.Price);
                        self.VehicleId(data.VehiclePrimarySecurity.Id);



                        if (data.VehiclePrimarySecurity.SellersAddress != null || data.VehiclePrimarySecurity.SellersAddress != undefined) {
                            //self.VehiclePrimarySecurity.SellersAddress.Id(data.VehiclePrimarySecurity.SellersAddress.Id);
                            self.SellersAddress.AddressLine1(data.VehiclePrimarySecurity.SellersAddress.AddressLine1);
                            self.SellersAddress.AddressLine2(data.VehiclePrimarySecurity.SellersAddress.AddressLine2);
                            self.SellersAddress.AddressLine3(data.VehiclePrimarySecurity.SellersAddress.AddressLine3);
                            self.SellersAddress.PostalCode(data.VehiclePrimarySecurity.SellersAddress.PostalCode);
                            self.SellersAddress.Email(data.VehiclePrimarySecurity.SellersAddress.Email);
                            self.SellersAddress.PhoneNo(data.VehiclePrimarySecurity.SellersAddress.PhoneNo);
                            self.SellersAddress.CellPhoneNo(data.VehiclePrimarySecurity.SellersAddress.CellPhoneNo);
                            self.SellersAddress.PostalCode(data.VehiclePrimarySecurity.SellersAddress.PostalCode);
                            $.when(self.GetSellersCountry())
                                .done(function () {
                                    self.SellersAddress.CountryId(data.VehiclePrimarySecurity.SellersAddress.CountryId);
                                    $.when(self.LoadSellersDivisionByCountry())
                                        .done(function () {
                                            self.SellersAddress.DivisionId(data.VehiclePrimarySecurity.SellersAddress.DivisionId);
                                            $.when(self.getSellersDistrictByDivision())
                                                .done(function () {
                                                    self.SellersAddress.DistrictId(data.VehiclePrimarySecurity.SellersAddress.DistrictId);
                                                    $.when(self.getSellersUpzilaByDistrict())
                                                        .done(function () {
                                                            self.SellersAddress.ThanaId(data.VehiclePrimarySecurity.SellersAddress.ThanaId);

                                                        });
                                                });
                                        });
                                });
                        }
                    }
                    if (data.ConsumerGoodsPrimarySecurity != null) {
                        if (data.ConsumerGoodsPrimarySecurity.DealerAddress != null || data.ConsumerGoodsPrimarySecurity.DealerAddress != undefined) {
                            self.IsDealer(true);
                            //self.ConsumerGoodsPrimarySecurity.DealerAddress.Id(data.ConsumerGoodsPrimarySecurity.DealerAddress.Id);
                            self.DealerAddress.AddressLine1(data.ConsumerGoodsPrimarySecurity.DealerAddress.AddressLine1);
                            self.DealerAddress.AddressLine2(data.ConsumerGoodsPrimarySecurity.DealerAddress.AddressLine2);
                            self.DealerAddress.AddressLine3(data.ConsumerGoodsPrimarySecurity.DealerAddress.AddressLine3);
                            self.DealerAddress.PostalCode(data.ConsumerGoodsPrimarySecurity.DealerAddress.PostalCode);
                            self.DealerAddress.Email(data.ConsumerGoodsPrimarySecurity.DealerAddress.Email);
                            self.DealerAddress.PhoneNo(data.ConsumerGoodsPrimarySecurity.DealerAddress.PhoneNo);
                            self.DealerAddress.CellPhoneNo(data.ConsumerGoodsPrimarySecurity.DealerAddress.CellPhoneNo);
                            self.DealerAddress.PostalCode(data.ConsumerGoodsPrimarySecurity.DealerAddress.PostalCode);
                            $.when(self.GetDealerCountry())
                                .done(function () {
                                    self.DealerAddress.CountryId(data.ConsumerGoodsPrimarySecurity.DealerAddress.CountryId);
                                    $.when(self.LoadDealerDivisionByCountry())
                                        .done(function () {
                                            self.DealerAddress.DivisionId(data.ConsumerGoodsPrimarySecurity.DealerAddress.DivisionId);
                                            $.when(self.getDealerDistrictByDivision())
                                                .done(function () {
                                                    self.DealerAddress.DistrictId(data.ConsumerGoodsPrimarySecurity.DealerAddress.DistrictId);
                                                    $.when(self.getDealerUpzilaByDistrict())
                                                        .done(function () {
                                                            self.DealerAddress.ThanaId(data.ConsumerGoodsPrimarySecurity.DealerAddress.ThanaId);

                                                        });
                                                });
                                        });
                                });
                        }
                        //self.ConsumerGoodsId = ko.observable('');
                        self.ConsumerGoodsId(data.ConsumerGoodsPrimarySecurity.Id);
                        self.Item(data.ConsumerGoodsPrimarySecurity.Item);
                        self.Brand(data.ConsumerGoodsPrimarySecurity.Brand);
                        self.Dealer(data.ConsumerGoodsPrimarySecurity.Dealer);
                        self.DealerAddressId(data.ConsumerGoodsPrimarySecurity.DealerAddressId);
                        //DealerAddress: self.DealerAddress,
                        $.when(self.GetVendorTypes())
                        .done(function () {
                            self.VendorType(data.ConsumerGoodsPrimarySecurity.VendorType);
                        });
                        $.when(self.GetOnlyShowRooms())
                        .done(function () {
                            //self.VehicleType(data.VehiclePrimarySecurity.VehicleType);
                            self.ShowRoomId(data.ConsumerGoodsPrimarySecurity.ShowRoomId);
                        });
                        self.ConsumerGoodsPrimarySecurityPrice(data.ConsumerGoodsPrimarySecurity.Price);


                    }
                    if (data.FDRPrimarySecurity != null) {
                        //FDRPSDetails: self.FDRPSDetails()
                        //self.FDRId = ko.observable('');

                        self.FDRId(data.FDRPrimarySecurity.Id);
                        $.each(data.FDRPrimarySecurity.FDRPSDetails,
                            function (index, value) {
                                var aDetail = new fDRPSDetail();
                                if (typeof (value) != 'undefined') {
                                    aDetail.LoadData(value);
                                    self.FDRPSDetails.push(aDetail);
                                }

                            });
                    }
                    if (data.LPPrimarySecurity) {
                        //GetLandedPropertySellertypes
                        $.when(self.GetLandedPropertyLoanTypes())
                       .done(function () {
                           self.LandedPropertyLoanType(data.LPPrimarySecurity.LandedPropertyLoanType);
                           self.chkLoanType();
                       });
                        //self.LppPrimarySecurityId = ko.observable('');
                        self.LppPrimarySecurityId(data.LPPrimarySecurity.Id);
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

                        self.SellerName(data.LPPrimarySecurity.SellerName);
                        self.SellerPhone(data.LPPrimarySecurity.SellerPhone);
                        self.PropertyAddressId(data.LPPrimarySecurity.PropertyAddressId);
                        self.LandType(data.LPPrimarySecurity.LandType + "");
                        //self.PropertyAddress,
                        $.when(self.GetAllDevelopers())
                        .done(function () {
                            if (data.LPPrimarySecurity.DeveloperId > 0) {
                                $.each(self.Developers(), function (index, value) {
                                    if (value.Id === data.LPPrimarySecurity.DeveloperId)
                                        self.SelectedDeveloperId(value);
                                })
                            }
                            //self.DeveloperId(data.LPPrimarySecurity.DeveloperId);
                            $.when(self.GetProjectsByDevelopers())
                            .done(function () {
                                self.ProjectId(data.LPPrimarySecurity.ProjectId);
                            });
                        });


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
                        if (data.LPPrimarySecurity.PropertyAddress != null || data.LPPrimarySecurity.PropertyAddress != undefined) {

                            //self.ConsumerGoodsPrimarySecurity.PropertyAddress.Id(data.ConsumerGoodsPrimarySecurity.PropertyAddress.Id);
                            self.PropertyAddress.AddressLine1(data.LPPrimarySecurity.PropertyAddress.AddressLine1);
                            self.PropertyAddress.AddressLine2(data.LPPrimarySecurity.PropertyAddress.AddressLine2);
                            self.PropertyAddress.AddressLine3(data.LPPrimarySecurity.PropertyAddress.AddressLine3);
                            self.PropertyAddress.PostalCode(data.LPPrimarySecurity.PropertyAddress.PostalCode);
                            self.PropertyAddress.Email(data.LPPrimarySecurity.PropertyAddress.Email);
                            self.PropertyAddress.PhoneNo(data.LPPrimarySecurity.PropertyAddress.PhoneNo);
                            self.PropertyAddress.CellPhoneNo(data.LPPrimarySecurity.PropertyAddress.CellPhoneNo);
                            self.PropertyAddress.PostalCode(data.LPPrimarySecurity.PropertyAddress.PostalCode);
                            $.when(self.GetPropertyCountry())
                                .done(function () {
                                    self.PropertyAddress.CountryId(data.LPPrimarySecurity.PropertyAddress.CountryId);
                                    $.when(self.LoadPropertyDivisionByCountry())
                                        .done(function () {
                                            self.PropertyAddress.DivisionId(data.LPPrimarySecurity.PropertyAddress.DivisionId);
                                            $.when(self.getPropertyDistrictByDivision())
                                                .done(function () {
                                                    self.PropertyAddress.DistrictId(data.LPPrimarySecurity.PropertyAddress.DistrictId);
                                                    $.when(self.getPropertyUpzilaByDistrict())
                                                        .done(function () {
                                                            self.PropertyAddress.ThanaId(data.LPPrimarySecurity.PropertyAddress.ThanaId);

                                                        });
                                                });
                                        });
                                });
                        }
                        self.chkLoanType();
                    }
                })
            .fail(function () {
                self.GetLoanAppColSecurities();
            });
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    self.Initialize = function () {
        self.GetOnlyShowRooms();
        self.GetLoanPrimarySecurityTypes();
        //if (self.Application_Id() > 0) {
        //    self.GetLoanAppColSecurities();
        //}

        self.GetDisbursementModes();
        self.GetLoanChequeDeliveryOptions();
        //self.addWaiverRequests();
        self.GetRiskLevels();
        //self.addGuarantors();
        self.GetVehicleStatuses();
        self.GetVendorTypes();
        self.GetSellersCountry();
        self.GetDealerCountry();
        self.GetVehicleTypes();
        self.GetVendors();
        self.GetLandedPropertyLoanTypes();
        self.GetLandedPropertySellertypes();
        self.GetAllDevelopers();
        self.GetPropertyCountry();
        //self.AddFDRPSDetail();
        self.selectPrimarySecurity();
        self.GetWaiverTypes();
        self.GetWaiverRequestedToLists();
        //self.GetGuarantorCifLists();
        self.GetRelationships();
        self.GetDisbursementToList();
        self.GetRelationshipWithApplicantList();
    }

    self.SaveLoanApplication = function () {
        self.FirstDisbursementExpDateText($("#FirstDisbursementExpDateTxt").val());

        var consumerGoodsPrimarySecurity = {
            Id: self.ConsumerGoodsId(),
            LoanApplicationId: self.LoanApplicationId(),
            Item: self.Item(),
            Brand: self.Brand(),
            Dealer: self.Dealer(),
            DealerAddressId: self.DealerAddressId(),
            DealerAddress: self.DealerAddress,
            ShowRoomId: self.ShowRoomId(),
            Price: self.ConsumerGoodsPrimarySecurityPrice()
        }
        var vehiclePrimarySecurity = {
            Id: self.VehicleId(),
            VehicleStatus: self.VehicleStatus(),
            VendorType: self.VendorType(),
            SellersName: self.SellersName(),
            SellersAddressId: self.SellersAddressId(),
            SellersAddress: self.SellersAddress,
            SellersPhone: self.SellersPhone(),
            VendorId: self.VendorId(),
            VehicleType: self.VehicleType(),
            VehicleName: self.VehicleName(),
            Manufacturer: self.Manufacturer(),
            MnufacturingYear: self.MnufacturingYear(),
            YearModel: self.YearModel(),
            RegistrationYear: self.RegistrationYear(),
            RegistrationNo: self.RegistrationNo(),
            CC: self.CC(),
            Colour: self.Colour(),
            ChassisNo: self.ChassisNo(),
            EngineNo: self.EngineNo(),
            Price: self.Price()
        }
        var details = ko.observableArray([]);
        $.each(self.FDRPSDetails(),
            function (index, value) {
                details.push({
                    Id: value.Id(),
                    FDRAccountNo: value.FDRAccountNo(),
                    Amount: value.Amount(),
                    Depositor: value.Depositor(),
                    MaturityDate: value.MaturityDate(),
                    MaturityDateTxt: moment(value.MaturityDate()).format("DD/MM/YYYY"),
                    RelationshipWithApplicant: value.RelationshipWithApplicant(),
                    InstituteName: value.InstituteName(),
                    BranchName: value.BranchName()
                });
            });
        var fDRPrimarySecurity = {
            Id: self.FDRId(),

            FDRPSDetails: details//fDRPSDetail
        }


        var lPPrimarySecurity = {
            Id: self.LppPrimarySecurityId(),
            LandedPropertyLoanType: self.LandedPropertyLoanType(),
            TotalCost: self.TotalCost(),
            AmountPaid: self.AmountPaid(),
            RemainingClientContribution: self.RemainingClientContribution(),
            SourceOfRemainingFund: self.SourceOfRemainingFund(),
            FirstDisbursementExpDate: self.FirstDisbursementExpDate(),
            FirstDisbursementExpDateText: self.FirstDisbursementExpDateText(),
            LandedPropertySellertype: self.LandedPropertySellertype(),
            LandType: self.LandType(),
            SellerName: self.SellerName(),
            SellerPhone: self.SellerPhone(),
            PropertyAddressId: self.PropertyAddressId(),
            PropertyAddress: self.PropertyAddress,
            DeveloperId: self.DeveloperId(),
            ProjectId: self.ProjectId(),
            FlatSize: self.FlatSize(),
            FloorNo: self.FloorNo(),
            FlatSide: self.FlatSide(),
            TakeoverFrom: self.TakeoverFrom(),
            PrevCompanyApprovedLoan: self.PrevCompanyApprovedLoan(),
            PrevCompanyOutstandingLoan: self.PrevCompanyOutstandingLoan(),
            TopUpLoan: self.TopUpLoan(),
            CurrentInterestRate: self.CurrentInterestRate(),
            Owner: self.Owner(),
            TotalPropertyValue: self.TotalPropertyValue(),
            CurrentWorkingStage: self.CurrentWorkingStage(),
            CompletedFloors: self.CompletedFloors(),
            ProposedFloors: self.ProposedFloors(),
            EstimatedConstructionCost: self.EstimatedConstructionCost()
        }
        var submitData = {
            Application_Id: self.Application_Id(),
            Term: self.Term(),
            Id: self.Id(),
            LoanAmountApplied: self.LoanAmountApplied(),
            Rate: self.Rate(),
            ServiceChargeRate: self.ServiceChargeRate(),
            ServiceChargeAmount: self.ServiceChargeAmount(),
            DocumentationFee: self.DocumentationFee(),
            OtherFees: self.OtherFees(),
            Purpose: self.Purpose(),
            LoanPrimarySecurityType: self.LoanPrimarySecurityType(),
            LoanAppColSecurities: self.LoanAppColSecurities(),
            OtherSecurities: self.OtherSecurities(),
            RemovedOtherSecurities: self.RemovedOtherSecurities(),
            OtherSecurity: self.OtherSecurity(),
            DisbursementMode: self.DisbursementMode(),
            PayeesAccountNo: self.PayeesAccountNo(),
            PayeesName: self.PayeesName(),
            Bank: self.Bank(),
            Branch: self.Branch(),
            RoutingNo: self.RoutingNo(),
            LoanChequeDeliveryOption: self.LoanChequeDeliveryOption(),
            WaiverRequests: self.WaiverRequests(),
            BeneficialOwner: self.BeneficialOwner(),
            SourceOfFund: self.SourceOfFund(),
            SourceOfFundVerificationMethod: self.SourceOfFundVerificationMethod(),
            SourceOfFundConsistency: self.SourceOfFundConsistency(),
            RiskLevel: self.RiskLevel(),
            RiskLevels: self.RiskLevels(),
            Comment: self.Comment(),
            Guarantors: self.Guarantors(),
            ConsumerGoodsPrimarySecurity: consumerGoodsPrimarySecurity,
            VehiclePrimarySecurity: vehiclePrimarySecurity,
            FDRPrimarySecurity: fDRPrimarySecurity,
            LPPrimarySecurity: lPPrimarySecurity,
            RemovedWaiverRequests: self.RemovedWaiverRequests,
            RemovedFDRPSDetails: self.RemovedFDRPSDetails,
            RemovedGuarantors: self.RemovedGuarantors
        }
        if (self.errors().length === 0) {
            $.ajax({
                type: "POST",
                url: '/IPDC/Application/SaveLoanApplication',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    //debugger;
                    $('#lonSuccessModal').modal('show');
                    $('#lonSuccessModalText').text(data.Message);
                    if (data.Id > 0) {
                        self.LoadLoanApplicationData();
                    }
                    self.Id(data.Id);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        } else {
            self.errors.showAllMessages();
        }

    }

    self.CIFPDetails = function (data) {
        var parameters = [{
            Name: 'cifpid',
            Value: data.GuarantorCifId()
        }];
        var menuInfo = {
            Id: 27,
            Menu: 'CIF Personal',
            Url: '/IPDC/CIF/CIF_Personal',
            Parameters: parameters
        }
        window.parent.AddTabFromExternal(menuInfo);
    }

    self.queryString = function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));

    }
}
//var loanAppVM = new LoanApplicationVm();
//var qValue = loanAppVM.queryString('applicationId');
//loanAppVM.Application_Id(qValue);
//loanAppVM.Initialize();
//ko.applyBindings(vm, $('#LoanApplicationVw')[0]);
//});



