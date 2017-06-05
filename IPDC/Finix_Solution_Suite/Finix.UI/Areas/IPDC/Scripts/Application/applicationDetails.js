/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
/// <reference path="~/Scripts/knockout.validation.min.js" />
$(function () {
    $('#ApplicationDateId').datetimepicker({ format: 'DD/MM/YYYY' });
    $('#SubmissionDeadlineId').datetimepicker({ format: 'DD/MM/YYYY' });
    //$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
});
$(document).ready(function () {
    var depositAppVM;
    var loanAppVM;
    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });
    function ownerCif() {
        var self = this;
        self.Id = ko.observable('');
        self.CIF_OrganizationalId = ko.observable('');
        self.CIF_PersonalId = ko.observable('');
        self.CIFNo = ko.observable('');
        self.Name = ko.observable('');
        self.CIF_Org_OwnersRoleName = ko.observable('');
        self.Age = ko.observable('');
        self.ProfessionName = ko.observable('');
        self.MonthlyIncome = ko.observable('');
        self.ApplicationId = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.CIF_OrganizationalId(data.CIF_OrganizationalId);
            self.CIF_PersonalId(data.CIF_PersonalId);
            self.CIFNo(data.CIFNo);
            self.Name(data.Name);
            self.CIF_Org_OwnersRoleName(data.CIF_Org_OwnersRoleName);
            self.Age(data.Age);
            self.ProfessionName(data.ProfessionName);
            self.MonthlyIncome(data.MonthlyIncome);
            self.ApplicationId(data ? data.ApplicationId : 0);
        }

        self.Details = function () {
            var parameters = [
                {
                    Name: 'AppId',
                    Value: self.ApplicationId()
                },
                {
                    Name: 'CIFPId',
                    Value: self.CIF_PersonalId()
                }, {
                    Name: 'CIFOId',
                    Value: self.CIF_OrganizationalId()
                }];
            var menuInfo = {
                Id: 67,
                Menu: 'Verifications for CIF',
                Url: '/IPDC/Application/ApplicationCifs',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        //self.Initialize = function () {
        //    self.GetGuarantorCifLists();
        //}
    }
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
        self.OccupationTypeName = ko.observable(data ? data.OccupationTypeName : '');

        self.IsAddOption = ko.observable(data ? data.ApplicantRole === 1 || data.ApplicantRole === 2 ? true : false : false);
        self.IncomeAmount = ko.pureComputed({
            read: function () {
                if (self.MonthlyIncome() > 0)
                    return self.MonthlyIncome().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.MonthlyIncome(isNaN(value) ? 0 : value);
            },
            owner: self
        });
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
            self.OccupationTypeName(data ? data.OccupationTypeName : "");
        }

        self.Details = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'CIFPId',
                Value: self.CIF_PersonalId()
            }, {
                Name: 'CIFOId',
                Value: self.CIF_OrganizationalId()
            }, {
                Name: 'VerificationAs',
                Value: self.ApplicantRole()
            }];
            var menuInfo = {
                Id: 67,
                Menu: 'Verifications for CIF',
                Url: '/IPDC/Application/ApplicationCifs',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.Message = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }];
            var menuInfo = {
                Id: 67,
                Menu: 'Verifications for CIF',
                Url: '/IPDC/Messaging/NewMessage',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
    }


    function guarantor() {
        var self = this;
        self.Id = ko.observable();
        self.GuarantorCifId = ko.observable('');
        self.GuaranteeAmount = ko.observable('');
        self.GuarantorName = ko.observable('');
        self.Age = ko.observable('');
        self.CIFNo = ko.observable('');
        self.ApplicationId = ko.observable();
        self.RelationshipWithApplicantName = ko.observable('');
        self.IncomeAmount = ko.pureComputed({
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
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.GuarantorCifId(data.GuarantorCifId);
            self.GuaranteeAmount(data.GuaranteeAmount);
            self.GuarantorName(data.GuarantorName);
            self.Age(data.Age);
            self.CIFNo(data ? data.CIFNo : 0);
            self.ApplicationId(data ? data.ApplicationId : 0);
            self.RelationshipWithApplicantName(data ? data.RelationshipWithApplicantName : '');
        }
        self.GuarantorDetails = function () {
            var parameters = [
            {
                Name: 'AppId',
                Value: self.ApplicationId()
            },
            {
                Name: 'CIFPId',
                Value: self.GuarantorCifId()
            }, {
                Name: 'VerificationAs',
                Value: 3
            }
            //, {
            //    Name: 'CIFOId',
            //    Value: self.CIF_OrganizationalId()
            //}
            ];
            var menuInfo = {
                Id: 67,
                Menu: 'Verification For Guarantors',
                Url: '/IPDC/Application/ApplicationCifs',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
    }
    function applicationCheckList(data) {
        var self = this;
        self.Id = ko.observable(data ? data.Id : 0);
        self.ApplicationId = ko.observable(data ? data.ApplicationId : 0);
        self.ProductDocId = ko.observable(data ? data.ProductDocId : 0);
        self.ProductDoc = ko.observable(data ? data.ProductDoc : "");
        self.DocumentStatus = ko.observable(data ? data.DocumentStatus : "");
        self.SubmissionDeadline = ko.observable();
        self.ApprovalRequired = ko.observable(data ? data.ApprovalRequired : "");
        self.ApprovedById = ko.observable(data ? data.ApprovedById : 0);
        self.ProductId = ko.observable(data ? data.ProductId : 0);
        self.DocName = ko.observable(data ? data.DocName : "");
        self.SubmissionDeadlinetext = ko.observable(data ? data.SubmissionDeadlinetext : "");
        self.IsChecked = ko.observable(data ? data.IsChecked : false);
        self.DocumentStatusList = ko.observableArray([]);

        self.IfDererred = ko.observable(data ? data.DocumentStatus === 3 ? true : false : false);
        self.CheckIfDererred = function () {
            if (self.DocumentStatus() === 3) {
                self.IfDererred(true);
            } else {
                self.IfDererred(false);
            }
        }
        self.LoadData = function (data) {
            //
            self.Id(data ? data.Id : 0);
            self.ApplicationId(data ? data.ApplicationId : 0);
            self.ProductDocId(data ? data.ProductDocId : 0);
            self.ProductDoc(data ? data.ProductDoc : "");
            self.DocumentStatus(data ? data.DocumentStatus : "");
            self.SubmissionDeadline(data.SubmissionDeadline);
            self.ApprovalRequired(data ? data.ApprovalRequired : "");
            self.ApprovedById(data ? data.ApprovedById : 0);
            self.ProductId(data ? data.ProductId : 0);
            self.DocName(data ? data.DocName : "");
            self.SubmissionDeadlinetext(moment(data.SubmissionDeadlinetext).format('DD/MM/YYYY'));

            self.IsChecked(data ? data.IsChecked : false);
            self.CheckIfDererred();
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
    function valuation() {
        var self = this;
        self.Id = ko.observable();
        self.PropertyTypeName = ko.observable();
        self.Remarks = ko.observable();
        self.MarketValueOfLandAsPerRAJUK = ko.observable();
        self.MarketValueOfLandAsPerClient = ko.observable();
        self.FlatSize = ko.observable();
        self.ProjectId = ko.observable();
        self.LegalDocId = ko.observable();
        self.ProjectTechnicalId = ko.observable();
        self.DeveloperId = ko.observable();
        self.ProjectLegalId = ko.observable();
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.PropertyTypeName(data ? data.PropertyTypeName : "");
            self.Remarks(data ? data.Remarks : "");
            self.MarketValueOfLandAsPerRAJUK(data ? data.MarketValueOfLandAsPerRAJUK : 0);
            self.MarketValueOfLandAsPerClient(data ? data.MarketValueOfLandAsPerClient : 0);
            self.FlatSize(data ? data.FlatSize : 0);
            self.ProjectId(data ? data.ProjectId : null);
            self.LegalDocId(data ? data.LegalDocId : null);
            self.DeveloperId(data ? data.DeveloperId : null);
            self.ProjectTechnicalId(data ? data.ProjectTechnicalId : null);
            self.ProjectLegalId(data ? data.ProjectLegalId : null);
        }
    }
    var CIFReferenceLine = function () {
        var self = this;
        self.Id = ko.observable();
        self.CIF_PersonalId = ko.observable();
        self.Name = ko.observable().extend({ required: true });
        self.Designation = ko.observable();
        self.Department = ko.observable();
        self.OrganizationName = ko.observable();
        self.OrganizationId = ko.observable('');
        self.RelationshipWithApplicant = ko.observable();
        self.CIFNo = ko.observable();
        self.Load = function (data) {
            self.Id(data ? data.Id : 0);
            self.CIF_PersonalId(data ? data.CIF_PersonalId : 0);
            self.Name(data ? data.Name : "");
            self.Designation(data ? data.Designation : "");
            self.Department(data ? data.Department : "");
            self.OrganizationName(data ? data.OrganizationName : "");
            self.OrganizationId(data ? data.OrganizationId : 0);
            self.RelationshipWithApplicant(data ? data.RelationshipWithApplicant : "");
            self.CIFNo(data ? data.CIFNo : 0);

        }


    };
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

    //vehicleDetails
    function vehicleDetails() {
        var self = this;
        self.Id = ko.observable();
        self.VehicleStatusName = ko.observable();
        self.VehicleTypeName = ko.observable();
        self.VehicleDetail = ko.observable();
        self.VendorDetail = ko.observable();
        self.Price = ko.observable();
        self.ProjectId = ko.observable();
        self.VerificationId = ko.observable();
        self.LoadData = function (data) {
            console.log(data);
            self.Id(data ? data.Id : 0);
            self.VehicleStatusName(data ? data.VehicleStatusName : "");
            self.VehicleTypeName(data ? data.VehicleTypeName : "");
            self.VehicleDetail(data ? data.VehicleDetail : 0);
            self.VendorDetail(data ? data.VendorDetail : 0);
            self.Price(data ? data.Price : 0);
            self.ProjectId(data ? data.ProjectId : null);
            self.VerificationId(data ? data.VerificationId : 1);
        }
    }

    function consumerGoodsDetails() {
        var self = this;
        self.Id = ko.observable();
        self.Item = ko.observable();
        self.VendorDetail = ko.observable();
        self.Price = ko.observable();
        self.ProjectId = ko.observable();
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.Item(data ? data.Item : "");
            self.VendorDetail(data ? data.VendorDetail : 0);
            self.Price(data ? data.Price : 0);
            self.ProjectId(data ? data.ProjectId : 0);
        }
    }

    function CRMIndexVm() {

        var self = this;
        self.Application_Id = ko.observable();
        self.SalesLeadId = ko.observable();
        self.ProposalId = ko.observable();
        self.ApplicationNo = ko.observable();
        self.ApplicationDate = ko.observable('');
        self.ApplicationDateText = ko.observable('');
        self.AccountTitle = ko.observable();
        self.ApplicationType = ko.observable();
        self.ApplicationTypeName = ko.observable();
        self.LoanAmountApplied = ko.observable(0);
        self.DepositApplication = ko.observable();
        self.Term = ko.observable('');
        self.Rate = ko.observable();
        self.ServiceChargeAmount = ko.observable();
        self.ServiceChargeRate = ko.observable();
        self.CIF_PersonalList = ko.observableArray([]);
        self.ApplicantRoles = ko.observableArray([]);
        self.ApplicationRoleName = ko.observable();
        self.Organizations = ko.observableArray([]);
        self.ConsumerGoodsPrimarySecurities = ko.observableArray([]);
        self.ProductName = ko.observable();
        self.FacilityType = ko.observable();
        self.RejectionReason = ko.observable();
        self.RejectedByEmpName = ko.observable();
        self.RejectedOn = ko.observable();
        self.BranchName = ko.observable();
        self.RMName = ko.observable();
        self.LeadPriorityName = ko.observable();
        //GetOrganizations
        self.GetOrganizations = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetAllCifOrgList',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Organizations(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetCIFs = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetAllCif',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CIF_PersonalList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetApplicantRoles = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetApplicantRoles',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApplicantRoles(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }


        self.Id = ko.observable('');

        self.CustomerType = ko.observable().extend({ required: true, message: 'Please Select Customer Type.' });// true });


        self.ContactPersonId = ko.observable().extend({ required: true, message: 'Please Select Customer Type.' });
        self.UseConAddAsGrpAdd = ko.observable(false);

        self.UseConAddAsGrpAdd.subscribe(function () {
            //IsAddFieldsEnabled
            if (self.UseConAddAsGrpAdd())
                self.IsAddFieldsEnabled(false);
            else
                self.IsAddFieldsEnabled(true);
        });

        self.ContactPersonList = ko.observableArray([]);
        self.GroupAddress = new address();
        self.DocChecklist = ko.observableArray([]);
        self.GroupCountryList = ko.observableArray([]);
        self.GroupDivisionList = ko.observableArray([]);
        self.GroupDistrictList = ko.observableArray([]);
        self.GroupThanaList = ko.observableArray([]);
        self.ProductTypes = ko.observableArray([]);
        self.DocumentStatusList = ko.observableArray([]);
        self.VehiclePrimarySecurities = ko.observableArray([]);
        //self.Products = ko.observableArray([]);
        //self.ApplicationCIFsDto = ko.observableArray([]);
        self.IsGrpAddressChng = function () {
            self.GroupAddress.IsChanged(true);
        }


        self.AccGroupId = ko.observable();//.extend({ required: true });
        self.ProductId = ko.observable().extend({ required: true });
        self.ProductType = ko.observable().extend({ required: true, message: 'Please Select Customer Type.' });
        self.ProductType.subscribe(function () {
            //document.getElementById('depAppTab')
            if (self.ProductType() === 1) {
                $("#depAppTab").show();
                $("#lonAppTab").hide();
            }
            else if (self.ProductType() === 2) {
                $("#depAppTab").hide();
                $("#lonAppTab").show();
            }
        });
        self.LoanApplicationId = ko.observable('');
        self.DepositApplicationId = ko.observable('');

        self.IsIndividual = ko.observable(false);
        self.IsOrganizational = ko.observable(false);
        //self.IsIndividual.subscribe(function () {
        //    if (self.IsIndividual(true)) {
        //        $("#tblOrganizatioList").hide();
        //        $("#tblIndividualCifList").show();
        //    }
        //    else if (self.IsOrganizational(true)) {
        //        $("#tblOrganizatioList").show();
        //        $("#tblIndividualCifList").hide();
        //    }
        //});
        //self.IsOrganizational.subscribe(function () {  
        //    if (self.IsOrganizational(true))
        //        self.ApplicationType(1);
        //});
        self.IsLppAddVisible = ko.observable(false);
        self.IsAddFieldsEnabled = ko.observable(true);
        self.LoanAppColSecurities = ko.observableArray([]);
        self.CIFList = ko.observableArray([]);
        self.ApplicationCustomerTypes = ko.observableArray([]);
        self.ApplicationTypes = ko.observableArray([]);
        self.ProductTypes = ko.observableArray([]);
        self.CIFReference = ko.observableArray([]);
        self.ApplicantLoans = ko.observableArray([]);
        self.Products = ko.observableArray([]);
        self.Guarantors = ko.observableArray([]);
        self.Valuations = ko.observableArray([]);
        self.OwnerList = ko.observableArray([]);
        self.IsSelfSubmitted = ko.observable(false);
        self.AmountFormatted = ko.pureComputed({
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

        self.PropertyDetails = function (data) {
            var parameters = [{
                Name: 'applicationId',
                Value: self.Id()
            },
            {
                Name: 'leadId',
                Value: self.SalesLeadId()
            }];
            var menuInfo = {
                Id: 82,
                Menu: 'LP Valuation',
                Url: '/IPDC/Verification/LppPrimarySecurityValuation',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.VehicleSecurityDetails = function (data) {
            var parameters = [{
                Name: 'applicationId',
                Value: self.Id()
            }];
            var menuInfo = {
                Id: 83,
                Menu: 'Verification',
                Url: '/IPDC/Verification/VehiclePrimarySecurityValuation',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.ConsumerDetails = function (data) {
            var parameters = [{
                Name: 'applicationId',
                Value: self.Id()
            },
            //{
            //    Name: 'CIFPId',
            //    Value: self.CIF_PersonalId()
            //}
            ];
            var menuInfo = {
                Id: 84,
                Menu: 'Verification',
                Url: '/IPDC/Verification/ConsumerPrimarySecurityValuation',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        //LegalDoc
        self.LegalDoc = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: self.Id()
            },
            {
                Name: 'Id',
                Value: data.LegalDocId()
            }];
            var menuInfo = {
                Id: 104,
                Menu: 'Title Search',
                Url: '/IPDC/Verification/LegalDocumentVerification',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.DeveloperApproval = function (data) {
            console.log(data);
            var parameters = [{
                Name: 'Id',
                Value: data.DeveloperId()
            }];
            var menuInfo = {
                Id: 104,
                Menu: 'Developer Approval',
                Url: '/IPDC/Developer/DeveloperApproval',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.addDocCheckListDetail = function () {
            self.DocChecklist([]);
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetAllDocCheckList?prodId=' + self.ProductId(),
                contentType: "application/json",
                success: function (data) {
                    $.each(data, function (index, value) {
                        self.DocChecklist.push(new applicationCheckList(value));
                    });
                },
                error: function () {
                    alert(error.status + "<--and-->" + error.statusText);
                }
            });
        }
        self.GetDocumentStatusList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetDocumentStatusList',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.DocumentStatusList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            var err = self.errors().length;
            if (err == 0)
                return true;
            return false;
        });
        self.addDetailOfClient = function () {
            self.CIFList([]);

            var aDetail = new applicationCIFs();
            if (self.CustomerType() === 1) {
                self.IsIndividual(true);
                self.IsOrganizational(false);
                //aDetail.InitializeClient();
                self.CIFList.push(aDetail);
            }
            if (self.CustomerType() === 2) {
                self.IsIndividual(false);
                self.IsOrganizational(true);
                //aDetail.InitializeClient();
                self.CIFList.push(aDetail);
            }
        }
        self.removeDetialOfClient = function (line) {
            self.CIFList.remove(line);
        }
        self.addDetailOfClientRow = function () {
            var aDetail = new applicationCIFs();
            if (self.CustomerType() === 1) {
                self.IsIndividual(true);
                self.IsOrganizational(false);
                //aDetail.InitializeClient();
                self.CIFList.push(aDetail);
            }
            if (self.CustomerType() === 2) {
                self.IsIndividual(false);
                self.IsOrganizational(true);
                //aDetail.InitializeClient();
                self.CIFList.push(aDetail);
            }
        }
        self.GetApplicationCustomerTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetApplicationCustomerTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ApplicationCustomerTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetApplicationTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetApplicationTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    self.ApplicationTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetProductTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProductTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProductTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetProducts = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetAllProducts',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Products(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetContactPerson = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetAllCif',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ContactPersonList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetGroupCountry = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.GroupCountryList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadGroupDivisionByCountry = function () {
            var countryId = self.GroupAddress.CountryId() ? self.GroupAddress.CountryId() : 0;
            return $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + countryId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.GroupDivisionList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });

        }
        self.getGroupDistrictByDivision = function () {
            var divisionId = self.GroupAddress.DivisionId() ? self.GroupAddress.DivisionId() : 0;

            return $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.GroupDistrictList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.getGroupUpzilaByDistrict = function () {
            var districtId = self.GroupAddress.DistrictId() ? self.GroupAddress.DistrictId() : 0;

            return $.ajax({
                type: "GET",
                url: '/IPDC/SalesLead/GetThanasByDistrict?districtId=' + districtId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.GroupThanaList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetProductTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProductTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProductTypes(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetProductByType = function () {
            var productType = self.ProductType() ? self.ProductType() : 0;

            return $.ajax({
                type: "GET",
                url: '/IPDC/Application/GetProductByType?typeId=' + productType,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Products(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadApplicationData = function () {
            //;
            if (self.Id() > 0) {
                $.getJSON("/IPDC/Application/LoadApplicationByAppIdForCRM/?AppId=" + self.Id(),
                    null,
                    function (data) {
                        self.Id(data.Id);
                        self.ApplicationNo(data.ApplicationNo);
                        self.BranchName(data.BranchName);
                        self.RMName(data.RMName);
                        self.LeadPriorityName(data.LeadPriorityName);
                        self.ProductName(data.ProductName);
                        self.FacilityType(data.Product.FacilityType);
                        self.ProposalId(data.ProposalId);
                        self.IsSelfSubmitted(data.IsSelfSubmitted);
                        self.RejectionReason(data.RejectionReason);
                        self.RejectedByEmpName(data.RejectedByEmpName);

                        self.RejectedOn(data.RejectedOn ? moment(data.RejectedOn).format('DD/MM/YYYY') : "");
                        if (data.LoanApplication != null) {
                            self.LoanAmountApplied(data.LoanApplication.LoanAmountApplied);
                            self.ServiceChargeAmount(data.LoanApplication.ServiceChargeAmount);
                            self.Term(data.LoanApplication.Term);
                            self.Rate(data.LoanApplication.Rate);
                            self.ServiceChargeRate(data.LoanApplication.ServiceChargeRate);
                            $.each(data.LoanApplication.Guarantors,
                                function (index, value) {
                                    var aDetail = new guarantor();
                                    if (typeof (value) != 'undefined') {
                                        aDetail.LoadData(value);
                                        self.Guarantors.push(aDetail);
                                    }
                                });
                            $.each(data.LoanApplication.LoanAppColSecurities,
                               function (index, value) {
                                   var aDetail = new loanAppColSecurity();
                                   if (typeof (value) != 'undefined') {
                                       aDetail.LoadData(value);
                                       self.LoanAppColSecurities.push(aDetail);
                                   }
                               });

                            if (data.LoanApplication.LPPrimarySecurity != null) {
                                if (data.LoanApplication.LPPrimarySecurity.Valuations.length > 0) {
                                    $.each(data.LoanApplication.LPPrimarySecurity.Valuations,
                                        function (index, value) {
                                            var aDetail = new valuation();
                                            if (typeof (value) != 'undefined') {
                                                aDetail.LoadData(value);
                                                self.Valuations.push(aDetail);
                                            }
                                        });
                                } else {
                                    self.IsLppAddVisible(true);
                                }

                            }
                            if (data.LoanApplication.VehiclePrimarySecurity != null) {

                                var aDetail = new vehicleDetails();
                                aDetail.LoadData(data.LoanApplication.VehiclePrimarySecurity);
                                console.log(ko.toJSON(data.LoanApplication.VehiclePrimarySecurity));
                                self.VehiclePrimarySecurities.push(aDetail);
                            }
                            if (data.LoanApplication.ConsumerGoodsPrimarySecurity != null) {
                                var aDetail = new consumerGoodsDetails();
                                aDetail.LoadData(data.LoanApplication.ConsumerGoodsPrimarySecurity);
                                self.ConsumerGoodsPrimarySecurities.push(data.LoanApplication.ConsumerGoodsPrimarySecurity);
                            }
                            //consumerGoodsDetails
                        }
                        $.each(data.References,
                             function (index, value) {
                                 var cifRef = new CIFReferenceLine();
                                 cifRef.Load(value);
                                 self.CIFReference.push(cifRef);
                             });
                        $.when(self.GetApplicationTypes())
                            .done(function () {
                                self.ApplicationType(data.ApplicationType);
                                self.ApplicationTypeName(data.ApplicationTypeName);
                            });
                        self.UseConAddAsGrpAdd(data.UseConAddAsGrpAdd);
                        self.ApplicationDateText(data.ApplicationDateText);
                        $('#ApplicationDateId').val(data.ApplicationDateText);

                        if (data.GroupAddress != null || data.GroupAddress != undefined) {
                            self.GroupAddress.Id(data.GroupAddress.Id);
                            self.GroupAddress.AddressLine1(data.GroupAddress.AddressLine1);
                            self.GroupAddress.AddressLine2(data.GroupAddress.AddressLine2);
                            self.GroupAddress.AddressLine3(data.GroupAddress.AddressLine3);
                            self.GroupAddress.PostalCode(data.GroupAddress.PostalCode);
                            self.GroupAddress.Email(data.GroupAddress.Email);
                            self.GroupAddress.PhoneNo(data.GroupAddress.PhoneNo);
                            self.GroupAddress.CellPhoneNo(data.GroupAddress.CellPhoneNo);
                            self.GroupAddress.PostalCode(data.GroupAddress.PostalCode);
                            $.when(self.GetGroupCountry())
                                .done(function () {
                                    self.GroupAddress.CountryId(data.GroupAddress.CountryId);
                                    $.when(self.LoadGroupDivisionByCountry())
                                        .done(function () {
                                            self.GroupAddress.DivisionId(data.GroupAddress.DivisionId);
                                            $.when(self.getGroupDistrictByDivision())
                                                .done(function () {
                                                    self.GroupAddress.DistrictId(data.GroupAddress.DistrictId);
                                                    $.when(self.getGroupUpzilaByDistrict())
                                                        .done(function () {
                                                            self.GroupAddress
                                                                .ThanaId(data.GroupAddress.ThanaId);

                                                        });
                                                });
                                        });
                                });
                        }
                        $.when(self.GetApplicationCustomerTypes())
                           .done(function () {
                               self.CustomerType(data.CustomerType);
                               if (data.CustomerType === 1) {
                                   self.IsIndividual(true);
                                   self.IsOrganizational(false);
                               } else {
                                   self.IsIndividual(false);
                                   self.IsOrganizational(true);
                               }
                           });
                        $.when(self.GetContactPerson())
                          .done(function () {
                              self.ContactPersonId(data.ContactPersonId);
                          });
                        $.when(self.GetProductTypes())
                         .done(function () {
                             self.ProductType(data.ProductType);
                         });
                        $.when(self.GetProducts())
                          .done(function () {
                              self.ProductId(data.ProductId);
                          });
                        $.when(self.GetDocumentStatusList())
                         .done(function () {

                             $.each(data.DocChecklist,
                                  function (index, value) {
                                      var aDetail = new applicationCheckList();
                                      if (typeof (value) != 'undefined') {
                                          aDetail.LoadData(value);
                                          self.DocChecklist.push(aDetail);
                                      }

                                  });
                         });
                        $.when(self.GetCIFs())
                            .done(function () {
                                $.when(self.GetApplicantRoles()).done(function () {
                                    $.when(self.GetOrganizations()).done(function () {
                                        $.each(data.CIFList, function (index, value) {
                                            var aDetail = new applicationCIFs();
                                            if (typeof (value) != 'undefined') {
                                                aDetail.LoadData(value);
                                                self.CIFList.push(aDetail);
                                            }
                                        });
                                    });
                                });
                            });
                        //data.OwnerList
                        self.AccountTitle(data.AccountTitle);
                        self.AccGroupId(data.AccGroupId);
                        self.LoanApplicationId(data.LoanApplicationId);
                        self.DepositApplicationId(data.DepositApplicationId);
                        $.each(data.OwnerList,
                           function (index, value) {
                               var aDetail = new ownerCif();
                               if (typeof (value) != 'undefined') {
                                   aDetail.LoadData(value);
                                   self.OwnerList.push(aDetail);
                               }

                           });

                        $.each(data.DocChecklist,
                              function (index, value) {
                                  var aDetail = new applicationCheckList();
                                  if (typeof (value) != 'undefined') {
                                      aDetail.LoadData(value);
                                      self.DocChecklist.push(aDetail);
                                  }
                              });
                    });

            }
        }
        self.Legal = function (data) {
            if (data.ProjectId() > 0) {
                var parameters = [{
                    Name: 'ProjectId',
                    Value: data.ProjectId()
                }, {
                    Name: 'ProjectLegalId',
                    Value: data.ProjectLegalId()
                }];
                var menuInfo = {
                    Id: 91,
                    Menu: 'Project Legal Valuation',
                    Url: '/IPDC/Verification/ProjectLegalVerification',
                    Parameters: parameters
                }
                window.parent.AddTabFromExternal(menuInfo);
            }

        }
        self.Technical = function (data) {

            if (data.ProjectId() > 0) {
                var parameters = [
                    {
                        Name: 'projectId',
                        Value: data.ProjectId()
                    },
                     {
                         Name: 'id',
                         Value: data.ProjectTechnicalId()
                     }
                ];
                var menuInfo = {
                    Id: 90,
                    Menu: 'Project Technical Valuation',
                    Url: '/IPDC/Verification/ProjectTechnicalVerification',
                    Parameters: parameters
                }
                window.parent.AddTabFromExternal(menuInfo);
            }
        }
        self.SubmitCurrentHolding = function () {
            var submitData = {
                Id: self.Id(),
                fromApplicationStage: 4 //application stage for sent to CRM
            };
            $.ajax({
                url: '/IPDC/Operations/SaveCaDepositApplication',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitData),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SubmitRelese = function () {
            var submitData = {
                Id: self.Id(),
                fromApplicationStage: 5 //application stage for underprocess at CRM
            };
            $.ajax({
                url: '/IPDC/Operations/SaveReleseApplication',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitData),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.CreateProposal = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: self.Id()
            },
            {
                Name: 'Id',
                Value: self.ProposalId()
            }];
            var menuInfo = {
                Id: 85,
                Menu: 'Proposal',
                Url: '/IPDC/CRM/Proposal',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.SetAppId = function (data) {
            self.Application_Id(data.Id);
        }
        self.FullApplication = function (data) {

            if (data.Application_Id() > 0) {
                var parameters = [{
                    Name: 'applicationId',
                    Value: data.Application_Id()//ko.toJSON(data.Application_Id())
                }];
                var menuInfo = {
                    Id: 127,
                    Menu: 'Application',
                    Url: '/IPDC/Application/ApplicationUneditable',
                    Parameters: parameters
                }
                window.parent.AddTabFromExternal(menuInfo);
            }

        }
        self.CancelApplication = function () {
            var submitData = {
                Id: self.Application_Id(),
                toApplicationStage: -4, // rejected by bm
                RejectionReason: self.RejectionReason()
            };
            $.ajax({
                url: '/IPDC/Application/CloseApplication',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitData),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        //self.SendMessage = function (data) {
        //    var parameters = [{
        //        Name: 'applicationId',
        //        Value: data.Application_Id
        //    }];
        //    var menuInfo = {
        //        //Id: 89,
        //        Menu: 'New Message',
        //        Url: '/IPDC/Messaging/NewMessage',
        //        Parameters: parameters
        //    }
        //    window.parent.AddTabFromExternal(menuInfo);
        //}

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));

        }
    }

    appvm = new CRMIndexVm();
    //vm.getData();
    var qValue = appvm.queryString('AppId');
    appvm.Id(qValue);// For Testing - qValue
    appvm.LoadApplicationData();
    appvm.Application_Id(qValue);
    ko.applyBindings(appvm, $('#crmIndexVw')[0]);
});