/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
/// <reference path="~/Scripts/knockout.validation.min.js" />
$(function () {
    //$('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY' });
    //$('#NewCommCertIssueDate').datetimepicker({ format: 'DD/MM/YYYY' });
    //$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
});
//$(document).ready(function () {

//    ko.validation.init({
//        errorElementClass: 'has-error',
//        errorMessageClass: 'help-block',
//        decorateInputElement: true,
//        grouping: { deep: true, observable: true }
//    });

    function cifOccAddress() {
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
        self.PostalCode = ko.observable().extend({ digit: true });
        self.PhoneNo = ko.observable().extend({ digit: true });
        self.CellPhoneNo = ko.observable().extend({ digit: true, minLength: 11 });
        self.Email = ko.observable().extend({ email: true });
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
    function landOwnerPropertyDetails() {
        var self = this;
        self.Id = ko.observable('');
        self.NumberOfFloorsRented = ko.observable('');
        self.RentedPropertyType = ko.observable('');
        self.RentedAreaInSqf = ko.observable('');
        self.ConstructionCompletionYear = ko.observable('');
        self.IsChanged = ko.observable(false);
        
        self.InitializeLandOwnerProperty = function () {
            //self.GetPropertyType();
        }

        self.Load = function (data) {
            self.Id(data.Id);
            self.NumberOfFloorsRented(data.NumberOfFloorsRented);
            self.RentedPropertyType(data.RentedPropertyType);
            self.RentedAreaInSqf(data.RentedAreaInSqf);
            self.ConstructionCompletionYear(data.ConstructionCompletionYear);
        }
    }
    //LandOwnerProperty


    function ClientOccupationVm() {

        var self = this;
        var i = 0;
        for (i = new Date().getFullYear() + 50 ; i > new Date().getFullYear() - 100; i--) {
            $('#ConstructionCompletionYear').append($('<option />').val(i).html(i));
        }
        self.Id = ko.observable('');
        self.Occupation_Id = ko.observable('');
        self.CIF_PersonalId = ko.observable('');
        self.OccupationType = ko.observable().extend({required : true});
        self.EmploymentStatus = ko.observable('');
        self.ProfessionId = ko.observable('');
        self.Designation = ko.observable('');
        self.OrganizationId = ko.observable('');
        self.EnlistedOrganization = ko.observable(false);
        self.OrganizationName = ko.observable('');
        self.OfficeAddressId = ko.observable('');
        self.MonthsInCurrentJob = ko.observable('');
        self.NameOfPreviousEmployeer = ko.observable('');
        self.YearsOfExpInPrevEmp = ko.observable().extend({ digit: true });
        self.TotalYearOfServieExp = ko.observable('');
        self.CompanyRetirementAge = ko.observable().extend({ digit: true });
        self.LegalStatus = ko.observable('');
        self.NumberOfEmployees = ko.observable('');
        self.EquityOrShare = ko.observable('');
        self.MainProduct = ko.observable('');
        self.MainClient = ko.observable('');
        self.PrimaryIncomeSource = ko.observable().extend({
            required: {
                onlyIf: function () {
                    return self.OccupationType() === 7;
                }
            }
        });
        self.OtherIncomeSource = ko.observable();
        self.DescriptionOfOccupation = ko.observable('');
        self.IsDirectorOfAnyBankOrFL = ko.observable(false);
        self.BankOrFLName = ko.observable('');
        self.RoleInBankOrFL = ko.observable('');
        self.RelatedPartyWithIPDC = ko.observable('');
        self.RelationshipWithIPDC = ko.observable('');
        self.OccupationTypeList = ko.observableArray([]);
        self.EmployeeStatusList = ko.observableArray([]);
        self.ProfessionList = ko.observableArray([]);
        self.OrganizationList = ko.observableArray([]);
        self.LegalStatusList = ko.observableArray([]);
        self.LandOwnerProperties = ko.observableArray([]);
        self.RemovedLandOwnerProp = ko.observableArray([]);
        self.IsGovtPrivateService = ko.observable(false);
        self.Common = ko.observable(false);
        self.IsBusiness = ko.observable(false);
        self.IsHW = ko.observable(false);
        self.IsOther = ko.observable(false);
        self.IsRetired = ko.observable(false);
        self.IsSelfEmpl = ko.observable(false);
        self.IsLandOwner = ko.observable(false);
        self.IsDirector = ko.observable(false);
        self.OfficeAddress = new cifOccAddress();
        self.OfficeCountryList = ko.observableArray([]);
        self.OfficeDivisionList = ko.observableArray([]);
        self.OfficeDistrictList = ko.observableArray([]);
        self.OfficeThanaList = ko.observableArray([]);
        self.LegalStatusList = ko.observableArray([]);
        self.RoleInBankOrFLList = ko.observableArray([]);
        self.RelationshipWithIPDCList = ko.observableArray([]);
        self.IsDirectorChecked = ko.observable(false);
        self.IsOfcAddressChng = function () {

            self.OfficeAddress.IsChanged(true);
        }

        self.PropertyTypeList = ko.observableArray([]);
        self.GetPropertyType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetPropertyType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.PropertyTypeList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        //self.errors = ko.validation.group(self);
        //self.IsValid = ko.computed(function () {
        //    if (self.errors().length === 0)
        //        return true;
        //    else {
        //        return false;
        //    }
        //});

        self.LoadOccupation = function () {
            if (self.CIF_PersonalId() > 0) {
                self.LandOwnerProperties([]);
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/CIF/GetCIF_Occupation/?personId=' + self.CIF_PersonalId(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        self.Id(data.Id);
                        self.OrganizationName(data.OrganizationName);
                        $.when(self.GetOccupationType()).done(function () {
                            self.OccupationType(data.OccupationType);
                        });
                        self.EmploymentStatus(data.EmploymentStatus);
                        $.when(self.GetProfessionList()).done(function () {
                            self.ProfessionId(data.ProfessionId);
                        });
                        self.Designation(data.Designation);

                        $.when(self.GetOrganizations().done(function () {
                            self.OrganizationId(data.OrganizationId);
                            self.OrganizationName(data.OrganizationName);
                        }));

                        //self.OrganizationId(data.OrganizationId);

                        self.EnlistedOrganization(data.EnlistedOrganization);
                        self.OfficeAddressId(data.OfficeAddressId);
                        self.MonthsInCurrentJob(data.MonthsInCurrentJob);
                        self.NameOfPreviousEmployeer(data.NameOfPreviousEmployeer);
                        self.YearsOfExpInPrevEmp(data.YearsOfExpInPrevEmp);
                        self.TotalYearOfServieExp(data.TotalYearOfServieExp);
                        self.CompanyRetirementAge(data.CompanyRetirementAge);
                        self.LegalStatus(data.LegalStatus);
                        self.NumberOfEmployees(data.NumberOfEmployees);
                        self.EquityOrShare(data.EquityOrShare);
                        self.MainProduct(data.MainProduct);
                        self.MainClient(data.MainClient);
                        self.PrimaryIncomeSource(data.PrimaryIncomeSource);
                        self.OtherIncomeSource(data.OtherIncomeSource);
                        //self.LandOwnerProperties(data.landDetails);
                        $.when(self.GetPropertyType()).done(function () {
                            $.each(data.LandOwnerProperties,
                                function(index, value) {
                                    var landData = new landOwnerPropertyDetails();
                                    landData.Load(value);
                                    self.LandOwnerProperties.push(landData);
                                });
                        });
                        self.DescriptionOfOccupation(data.DescriptionOfOccupation);
                        self.IsDirectorOfAnyBankOrFL(data.IsDirectorOfAnyBankOrFL);
                        self.BankOrFLName(data.BankOrFLName);
                        self.RoleInBankOrFL(data.RoleInBankOrFL);
                        self.RelatedPartyWithIPDC(data.RelatedPartyWithIPDC);
                        self.RelationshipWithIPDC(data.RelationshipWithIPDC);
                        //self.OfficeAddress(data.OfficeAddress);; //Put the response in ObservableArray
                        if (data.OfficeAddress != null || data.OfficeAddress != undefined) {
                            self.OfficeAddress.Id(data.OfficeAddress.Id);
                            self.OfficeAddress.AddressLine1(data.OfficeAddress.AddressLine1);
                            self.OfficeAddress.AddressLine2(data.OfficeAddress.AddressLine2);
                            self.OfficeAddress.AddressLine3(data.OfficeAddress.AddressLine3);
                            self.OfficeAddress.PostalCode(data.OfficeAddress.PostalCode);
                            self.OfficeAddress.Email(data.OfficeAddress.Email);
                            self.OfficeAddress.PhoneNo(data.OfficeAddress.PhoneNo);
                            self.OfficeAddress.CellPhoneNo(data.OfficeAddress.CellPhoneNo);
                            self.OfficeAddress.PostalCode(data.OfficeAddress.PostalCode);
                            $.when(self.GetOfficeCountry())
                                .done(function () {
                                    self.OfficeAddress.CountryId(data.OfficeAddress.CountryId);
                                    $.when(self.LoadOfficeDivisionByCountry())
                                        .done(function () {
                                            self.OfficeAddress.DivisionId(data.OfficeAddress.DivisionId);
                                            $.when(self.getOfficeDistrictByDivision())
                                                .done(function () {
                                                    self.OfficeAddress.DistrictId(data.OfficeAddress.DistrictId);
                                                    $.when(self.getOfficeUpzilaByDistrict())
                                                        .done(function () {
                                                            self.OfficeAddress
                                                                .ThanaId(data.OfficeAddress.ThanaId);

                                                        });
                                                });
                                        });
                                });
                        }
                        self.EnableFields();
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                self.Initialize();
            }
           
        }

        self.EnableFields = function () {
            //
            var occupationType = self.OccupationType() ? self.OccupationType() : 0;
            if (occupationType === 1 ) {
                self.IsGovtPrivateService(true);
                self.Common(true);
                self.IsRetired(false);
                self.IsBusiness(false);
                self.IsHW(false);
                self.IsOther(false);
                self.IsSelfEmpl(false);
                self.IsLandOwner(false);
                self.IsDirectorChecked(true);
            }
            else if (occupationType == 2) {
                self.IsGovtPrivateService(true);
                self.Common(true);
                self.IsRetired(false);
                self.IsBusiness(false);
                self.IsHW(false);
                self.IsOther(false);
                self.IsSelfEmpl(false);
                self.IsLandOwner(false);
                self.IsDirectorChecked(true);

            }
            else if (occupationType === 3) {
                self.IsGovtPrivateService(false);
                self.IsRetired(false);
                self.IsHW(false);
                self.IsBusiness(true);
                self.Common(true);
                self.IsOther(false);
                self.IsSelfEmpl(false);
                self.IsLandOwner(false);
                self.IsDirectorChecked(true);
               
            }
            else if (occupationType === 4) {
                self.IsGovtPrivateService(false);
                self.IsBusiness(false);
                self.Common(false);
                self.IsRetired(false);
                self.IsOther(false);
                self.IsHW(true);
                self.IsSelfEmpl(false);
                self.IsLandOwner(false);
                self.IsDirectorChecked(false);
            }
            else if (occupationType === 5) {
                self.IsGovtPrivateService(false);
                self.IsBusiness(false);
                self.Common(false);
                self.IsRetired(true);
                self.IsOther(false);
                self.IsHW(false);
                self.IsSelfEmpl(false);
                self.IsLandOwner(false);
                self.IsDirectorChecked(true);
            }
            else if (occupationType === 6) {
                self.IsGovtPrivateService(false);
                self.IsBusiness(false);
                self.Common(false);
                self.IsRetired(false);
                self.IsHW(false);
                self.IsOther(true);
                self.IsSelfEmpl(false);
                self.IsLandOwner(false);
                self.IsDirectorChecked(true);
            }
            else if (occupationType === 7) {
                self.IsGovtPrivateService(false);
                self.IsBusiness(false);
                self.Common(true);
                self.IsRetired(false);
                self.IsHW(false);
                self.IsOther(false);
                self.IsSelfEmpl(true);
                self.IsLandOwner(false);
                self.IsDirectorChecked(true);
            }
            else if (occupationType === 8) {
                self.IsGovtPrivateService(false);
                self.IsBusiness(false);
                self.Common(false);
                self.IsRetired(false);
                self.IsHW(false);
                self.IsOther(false);
                self.IsSelfEmpl(false);
                self.IsLandOwner(true);
                self.IsDirectorChecked(true);
            }
        }

        self.addDetailOfLandProperty = function () {

            var aDetail = new landOwnerPropertyDetails();
            aDetail.InitializeLandOwnerProperty();
            self.LandOwnerProperties.push(aDetail);
        }

        self.removeDetailOfLandProperty = function (line) {
            
            if (line.Id()>0) {
                self.RemovedLandOwnerProp.push(line.Id());
            }
            self.LandOwnerProperties.remove(line);
        }

        self.GetOccupationType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Profession/GetOccupationType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.OccupationTypeList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetEmployeeStatus = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Employee/GetEnumEmployeementStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.EmployeeStatusList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetOrganizations = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Organization/GetOrganizations',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.OrganizationList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetYearsCurrentResidence = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetYearsCurrentResidence',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.YearsInCurrentResidenceList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetProfessionList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Profession/GetAllProfession',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.ProfessionList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        //BirthDistrictList
        self.GetLegalStatusList = function () {
            $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetLegalStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.LegalStatusList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        //GetRoleInBankOrFL
        self.GetRoleInBankOrFLList = function () {
            $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetRoleInBankOrFL',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.RoleInBankOrFLList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        //GetRelationshipWithIPDC
        self.GetRelationshipWithIPDC = function () {
            $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetRelationshipWithIPDC',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.RelationshipWithIPDCList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetOfficeCountry = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.OfficeCountryList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadOfficeDivisionByCountry = function () {
            var countryId = self.OfficeAddress.CountryId() ? self.OfficeAddress.CountryId() : 0;
            return $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + countryId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    
                    self.OfficeDivisionList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });

        }
        self.getOfficeDistrictByDivision = function () {
            var divisionId = self.OfficeAddress.DivisionId() ? self.OfficeAddress.DivisionId() : 0;

            return $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    
                    self.OfficeDistrictList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.getOfficeUpzilaByDistrict = function () {
            var districtId = self.OfficeAddress.DistrictId() ? self.OfficeAddress.DistrictId() : 0;

            return $.ajax({
                type: "GET",
                url: '/IPDC/SalesLead/GetThanasByDistrict?districtId=' + districtId,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    
                    self.OfficeThanaList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.Initialize = function () {
            self.GetOccupationType();
            self.GetEmployeeStatus();
            self.GetProfessionList();
            self.GetOrganizations();
            self.GetOfficeCountry();
            self.GetLegalStatusList();
            self.GetRoleInBankOrFLList();
            self.GetRelationshipWithIPDC();
            self.addDetailOfLandProperty();
            self.GetPropertyType();
        }
        
        self.SaveClientOrg = function () {
            //
            var landDetails = ko.observableArray([]);
            $.each(self.LandOwnerProperties(), function (index, value) {
                landDetails.push({
                    Id : value.Id(),
                    NumberOfFloorsRented: value.NumberOfFloorsRented(),
                    RentedPropertyType: value.RentedPropertyType(),
                    RentedAreaInSqf: value.RentedAreaInSqf(),
                    ConstructionCompletionYear: value.ConstructionCompletionYear()
                });
            });
            var submitData = {
                Id: self.Id(),
                CIF_PersonalId: self.CIF_PersonalId(),
                OccupationType: self.OccupationType(),
                EmploymentStatus: self.EmploymentStatus(),
                ProfessionId: self.ProfessionId(),
                //ProfessionName:
                Designation: self.Designation(),
                OrganizationId: self.OrganizationId(),
                EnlistedOrganization: self.EnlistedOrganization(),
                OrganizationName: self.OrganizationName(),
                OfficeAddressId: self.OfficeAddressId(),
                MonthsInCurrentJob: self.MonthsInCurrentJob(),
                NameOfPreviousEmployeer: self.NameOfPreviousEmployeer(),
                YearsOfExpInPrevEmp: self.YearsOfExpInPrevEmp(),
                TotalYearOfServieExp: self.TotalYearOfServieExp(),
                CompanyRetirementAge: self.CompanyRetirementAge(),
                LegalStatus: self.LegalStatus(),
                NumberOfEmployees: self.NumberOfEmployees(),
                EquityOrShare: self.EquityOrShare(),
                MainProduct: self.MainProduct(),
                MainClient: self.MainClient(),
                PrimaryIncomeSource:self.PrimaryIncomeSource(),
                OtherIncomeSource: self.OtherIncomeSource(),
                LandOwnerProperties: landDetails,
                DescriptionOfOccupation: self.DescriptionOfOccupation(),
                IsDirectorOfAnyBankOrFL: self.IsDirectorOfAnyBankOrFL(),
                BankOrFLName: self.BankOrFLName(),
                RoleInBankOrFL: self.RoleInBankOrFL(),
                RelatedPartyWithIPDC: self.RelatedPartyWithIPDC(),
                RelationshipWithIPDC: self.RelationshipWithIPDC(),
                OfficeAddress: self.OfficeAddress,
                RemovedLandOwnerProp: self.RemovedLandOwnerProp()
            };
          
            $.ajax({
                type: "POST",
                url: '/IPDC/CIF/SaveClientOccupation',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#ciopsuccessModal').modal('show');
                    $('#ciopsuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });

        }


    }
    //var vm = new ClientOccupationVm();
    ////vm.getData();
    //vm.Initialize();
    //ko.applyBindings(vm, $('#ClientOccupationVw')[0]);
//});



