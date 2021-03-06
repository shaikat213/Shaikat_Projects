﻿/// <reference path="../knockout-3.4.0.debug.js" />
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
    function guarantor() {
        var self = this;
        self.Id = ko.observable();
        self.GuarantorCifId = ko.observable('');
        self.GuaranteeAmount = ko.observable('');
        self.GuarantorName = ko.observable('');
        self.Age = ko.observable('');
        self.CIFNo = ko.observable('');
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.GuarantorCifId(data.GuarantorCifId);
            self.GuaranteeAmount(data.GuaranteeAmount);
            self.GuarantorName(data.GuarantorName);
            self.Age(data.Age);
            self.CIFNo(data ? data.CIFNo : 0);
        }
        //self.Initialize = function () {
        //    self.GetGuarantorCifLists();
        //}
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
        self.PropertyTypeName = ko.observable();
        self.Remarks = ko.observable();
        self.MarketValueOfLandAsPerRAJUK = ko.observable();
        self.MarketValueOfLandAsPerClient = ko.observable();
        self.FlatSize = ko.observable();
        self.LoadData = function (data) {
            self.PropertyTypeName(data ? data.PropertyTypeName : "");
            self.Remarks(data ? data.Remarks : "");
            self.MarketValueOfLandAsPerRAJUK(data ? data.MarketValueOfLandAsPerRAJUK : 0);
            self.MarketValueOfLandAsPerClient(data ? data.MarketValueOfLandAsPerClient : 0);
            self.FlatSize(data ? data.FlatSize : 0);
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
        self.VehicleStatusName = ko.observable();
        self.VehicleTypeName = ko.observable();
        self.VehicleDetail = ko.observable();
        self.VendorDetail = ko.observable();
        self.Price = ko.observable();
        self.LoadData = function (data) {
            self.VehicleStatusName(data ? data.VehicleStatusName : "");
            self.VehicleTypeName(data ? data.VehicleTypeName : "");
            self.VehicleDetail(data ? data.VehicleDetail : 0);
            self.VendorDetail(data ? data.VendorDetail : 0);
            self.Price(data ? data.Price : 0);
        }
    }

    function consumerGoodsDetails() {
        var self = this;
        self.Item = ko.observable();
        self.VendorDetail = ko.observable();
        self.Price = ko.observable();
        self.LoadData = function (data) {
            self.Item(data ? data.Item : "");
        
            self.VendorDetail(data ? data.VendorDetail : 0);
            self.Price(data ? data.Price : 0);
        }
    }

    function CRMIndexVm() {

        var self = this;
        self.Application_Id = ko.observable();
        self.SalesLeadId = ko.observable();
        self.ApplicationNo = ko.observable();
        self.ApplicationDate = ko.observable('');
        self.ApplicationDateText = ko.observable('');
        self.AccountTitle = ko.observable();
        self.ApplicationType = ko.observable();
        self.ApplicationTypeName = ko.observable();
        self.LoanAmountApplied = ko.observable();
        self.Term = ko.observable('');
        self.Rate = ko.observable();
        self.ServiceChargeAmount = ko.observable();
        self.CIF_PersonalList = ko.observableArray([]);
        self.ApplicantRoles = ko.observableArray([]);
        self.ApplicationRoleName = ko.observable();
        self.Organizations = ko.observableArray([]);
        self.ConsumerGoodsPrimarySecurities = ko.observableArray([]);


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
        //;
        self.LoadApplicationData = function () {
            //;
            if (self.Id() > 0) {
                $.getJSON("/IPDC/CRM/LoadApplicationByAppIdForCRM/?AppId=" + self.Id(),
                    null,
                    function (data) {
                        
                        self.Id(data.Id);
                        self.ApplicationNo(data.ApplicationNo);
                        //self.SalesLeadId(data.SalesLeadId);
                        //  $.when(self.GetGuarantorCifLists())
                        //.done(function () {
                        if (data.LoanApplication != null) {
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
                                if (data.LoanApplication.LPPrimarySecurity.Valuations != null) {
                                    $.each(data.LoanApplication.LPPrimarySecurity.Valuations,
                                        function (index, value) {
                                            var aDetail = new valuation();
                                            if (typeof (value) != 'undefined') {
                                                aDetail.LoadData(value);
                                                self.Valuations.push(aDetail);
                                            }
                                        });
                                }

                            }
                            if (data.LoanApplication.VehiclePrimarySecurity != null) {
                              
                                var aDetail = new vehicleDetails();
                                aDetail.LoadData(data.LoanApplication.VehiclePrimarySecurity);
                                self.VehiclePrimarySecurities.push(data.LoanApplication.VehiclePrimarySecurity);  
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
                        self.AccountTitle(data.AccountTitle);
                        self.AccGroupId(data.AccGroupId);
                        self.LoanApplicationId(data.LoanApplicationId);
                        self.DepositApplicationId(data.DepositApplicationId);
                        self.Term(data.Term);

                        $.each(data.DocChecklist,
                              function (index, value) {
                                  var aDetail = new applicationCheckList();
                                  if (typeof (value) != 'undefined') {
                                      aDetail.LoadData(value);
                                      self.DocChecklist.push(aDetail);
                                  }

                              });


                        //LoanAppColSecurities

                    });

            }
        }

        //self.LoadLoanApplicationData = function () {
        //    if (self.Application_Id() > 0) {
        //        $.getJSON("/IPDC/Application/LoadLoanApplicationByAppId/?AppId=" + self.Application_Id(),
        //            null,
        //            function (data) {
        //                // ;
        //                self.Application_Id(data.Application_Id);
        //                self.Id(data.Id);
        //                self.LoanAmountApplied(data.LoanAmountApplied);
        //                self.Rate(data.Rate);
        //                self.ServiceChargeAmount(data.ServiceChargeAmount);

        //                $.when(self.GetWaiverTypes())
        //                    .done(function () {
        //                        $.when(self.GetWaiverRequestedToLists())
        //                            .done(function () {
        //                                $.each(data.WaiverRequests,
        //                                function (index, value) {
        //                                    var aDetail = new loanAppWaiverReq();
        //                                    if (typeof (value) != 'undefined') {
        //                                        aDetail.LoadData(value);
        //                                        self.WaiverRequests.push(aDetail);
        //                                    }

        //                                });
        //                            });
        //                    });

        //                $.when(self.GetGuarantorCifLists())
        //                  .done(function () {
        //                      $.each(data.Guarantors,
        //                      function (index, value) {
        //                          var aDetail = new guarantor();
        //                          if (typeof (value) != 'undefined') {
        //                              aDetail.LoadData(value);
        //                              self.Guarantors.push(aDetail);
        //                          }

        //                      });
        //                  });
        //                //$.each(data,
        //                //    function (index, value) {
        //                //        var cifRef = new CIFReferenceLine();
        //                //        cifRef.Load(value);
        //                //        self.CIFReference.push(cifRef);
        //                //    });


        //            });
        //       }
        //}
        self.GetAllApplicationCIF = function () {

        }



        self.Initialize = function () {

            self.GetApplicationCustomerTypes();
            self.GetApplicationTypes();
            self.GetContactPerson();
            self.GetGroupCountry();
            self.GetProductTypes();
            self.GetDocumentStatusList();
            if (self.Id() > 0) {
                self.LoadApplicationData();
            }
            self.GetCIFs();
            self.GetApplicantRoles();
            self.GetOrganizations();
        }
        self.Submit = function () {

        }

        self.SaveApplication = function () {

            self.ApplicationDateText($("#ApplicationDateId").val());
            var applicationCif = ko.observableArray([]);
            var checkListData = ko.observableArray([]);
            $.each(self.CIFList(),
                function (index, value) {
                    applicationCif.push({
                        Id: value.Id(),
                        ApplicationId: value.ApplicationId(),
                        CIF_PersonalId: value.CIF_PersonalId(),
                        ApplicantRole: value.ApplicantRole(),
                        CIF_OrganizationalId: value.CIF_OrganizationalId(),
                    });
                });
            $.each(self.DocChecklist(),
                function (index, value) {
                    if (value.IsChecked() === true) {
                        checkListData.push({
                            Id: value.Id(),
                            ApplicationId: value.ApplicationId(),
                            ProductDocId: value.ProductDocId(),
                            ProductDoc: value.ProductDoc(),
                            DocumentStatus: value.DocumentStatus(),
                            SubmissionDeadline: value.SubmissionDeadline(),
                            ApprovalRequired: value.ApprovalRequired(),
                            ApprovedById: value.ApprovedById(),
                            ProductId: value.ProductId(),
                            DocName: value.DocName(),
                            IsChecked: value.IsChecked()
                        });
                    }
                });
            var SubmitData = {
                Id: self.Id(),
                SalesLeadId: self.SalesLeadId(),
                ApplicationDate: self.ApplicationDate(),
                CustomerType: self.CustomerType(),
                ApplicationType: self.ApplicationType(),
                ContactPersonId: self.ContactPersonId(),
                UseConAddAsGrpAdd: self.UseConAddAsGrpAdd(),
                ApplicationDateText: self.ApplicationDateText(),
                //ContactPersonList : ko.observableArray([]);
                GroupAddress: self.GroupAddress,
                AccountTitle: self.AccountTitle(),
                AccGroupId: self.AccGroupId(),
                ProductId: self.ProductId(),
                ProductType: self.ProductType(),
                LoanApplicationId: self.LoanApplicationId(),
                DepositApplicationId: self.DepositApplicationId(),
                Term: self.Term(),
                CIFList: applicationCif,
                DocChecklist: checkListData
            }

            $.ajax({
                type: "POST",
                url: '/IPDC/Application/SaveApplication',
                data: ko.toJSON(SubmitData),
                contentType: "application/json",
                success: function (data) {
                    self.Id(data.Id);
                    $('#appSuccessModal').modal('show');
                    $('#appSuccessModalText').text(data.Message);
                    if (typeof (depositAppVM) != 'undefined') {
                        depositAppVM.Application_Id(data.Id);
                    }
                    if (typeof (loanAppVM) != 'undefined') {
                        loanAppVM.Application_Id(data.Id);
                        loanAppVM.GetLoanAppColSecurities();
                    }
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });

        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));

        }
        //var appvm = new ApplicationVm();
    }

    appvm = new CRMIndexVm();
    //vm.getData();
    var qValue = appvm.queryString('AppId');
    appvm.Id(qValue);
    appvm.LoadApplicationData();
    appvm.Application_Id(qValue);
    //appvm.LoadLoanApplicationData();
    //var leadId = appvm.queryString('leadId');
    //appvm.SalesLeadId(leadId);
    //appvm.Initialize();
    ko.applyBindings(appvm, $('#crmIndexVw')[0]);
});



