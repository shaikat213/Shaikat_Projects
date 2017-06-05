/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
/// <reference path="~/Scripts/knockout.validation.min.js" />
$(function () {
    $('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY' });
    $('#NewCommCertIssueDate').datetimepicker({ format: 'DD/MM/YYYY' });
    $('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
});
//$(document).ready(function () {

ko.validation.init({
    errorElementClass: 'has-error',
    errorMessageClass: 'help-block',
    decorateInputElement: true,
    grouping: { deep: true, observable: true }
});
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
function CIFPersonalVm() {

    var self = this;
    self.SalesLeadList = ko.observableArray([]);
    //self.Gender = ko.observableArray([]);

    self.Id = ko.observable('');
    self.CIFNo = ko.observable('');
    self.CustomersHomeBranch = ko.observable('');
    self.RMCode = ko.observable('');
    self.ApplicationSource = ko.observable('');
    self.ApplicationSourceName = ko.observable('');
    self.SourceRefId = ko.observable('');
    self.Title = ko.observable('');

    self.Name = ko.observable('');
    self.FathersTitle = ko.observable('');
    self.FathersName = ko.observable('');
    self.MothersTitle = ko.observable('');
    self.MothersName = ko.observable('');
    self.Gender = ko.observable('').extend({ required: true });
    self.GenderName = ko.observable('');
    self.NationalityId = ko.observable('');
    self.NationalityName = ko.observable('');
    self.BirthCountryId = ko.observable('');
    self.BirthCountryName = ko.observable('');
    self.BirthDistrictId = ko.observable('');
    self.BirthDistrictName = ko.observable('');
    self.BirthDistrictForeign = ko.observable('');
    self.DateOfBirth = ko.observable('');
    self.DateOfBirthText = ko.observable('');

    self.ResidenceAddress = new address();
    self.PermanentAddress = new address();
    self.SpouseWorkAddress = new address();

    self.ContactAddress = ko.observable('');
    self.ContactAddressName = ko.observable('');

    self.NIDNo = ko.observable('');
    self.SmartNIDNo = ko.observable('');
    self.PassportNo = ko.observable('');

    self.PassportIssueCountryId = ko.observable('');
    self.PassportIssueCountryName = ko.observable('');
    self.PassportIssueDate = ko.observable('');
    self.PassportIssueDateText = ko.observable('');

    self.DLNo = ko.observable('');
    self.DLIssueCountryId = ko.observable('');
    self.DLIssueCountryName = ko.observable('');
    self.BirthRegNo = ko.observable('');
    self.CommissionarCertificateNo = ko.observable('');
    self.CommCertIssueDate = ko.observable('');
    self.ETIN = ko.observable('');
    self.MaritalStatus = ko.observable('').extend({ required: true });
    self.MaritalStatusName = ko.observable('');

    self.SpouseTitle = ko.observable('');
    self.SpouseName = ko.observable('');
    self.SpousePhoneNo = ko.observable('');
    self.SpouseProfessionId = ko.observable('');
    self.SpouseDesignation = ko.observable('');
    self.SpouseCompanyId = ko.observable('');
    self.SpouseWorkAddressId = ko.observable('');

    self.NumberOfDependents = ko.observable('').extend({ required: true });
    self.HighestEducationLevel = ko.observable('');
    self.HighestEducationLevelName = ko.observable('');
    self.ResidenceStatus = ko.observable('').extend({ required: true });
    self.ResidenceStatusName = ko.observable('');
    self.HomeOwnership = ko.observable('');
    self.HomeOwnershipName = ko.observable('');
    self.YearsInCurrentResidence = ko.observable('');
    self.YearsInCurrentResidenceName = ko.observable('');

    self.Address = ko.observable('');
    self.ThanaName = ko.observable('');
    self.DistrictName = ko.observable('');
    self.FollowUpCallTime = ko.observable('');
    self.LeadTypeName = ko.observable('');
    self.LoanTypeName = ko.observable('');
    self.CustomerSensitivityName = ko.observable('');
    self.SpouseCompanyEnlisted = ko.observable(false);

    self.SpouseCompanyName = ko.observable('');

    self.myAction = function () {
        return true;
    }
    self.GenderList = ko.observableArray([]);
    self.MaritalStatusList = ko.observableArray([]);
    self.NationalityList = ko.observableArray([]);

    self.CountryList = ko.observableArray([]);
    self.DivisionList = ko.observableArray([]);
    self.DistrictList = ko.observableArray([]);
    self.ThanaList = ko.observableArray([]);

    self.ResidenceCountryList = ko.observableArray([]);
    self.ResidenceDivisionList = ko.observableArray([]);
    self.ResidenceDistrictList = ko.observableArray([]);
    self.ResidenceThanaList = ko.observableArray([]);
    self.PassportIssueDateText = ko.observable();
    self.CommCertIssueDateText = ko.observable();
    self.BirthDistrictList = ko.observableArray([]);

    self.HighestEducationLevelList = ko.observableArray([]);
    self.ResidenceStatusList = ko.observableArray([]);
    self.YearsInCurrentResidenceList = ko.observableArray([]);
    self.HomeOwnershipList = ko.observableArray([]);
    self.SpouseStatusList = ko.observableArray([]);
    self.CompanyList = ko.observableArray([]);

    self.SpouseCountryList = ko.observableArray([]);
    self.SpouseDivisionList = ko.observableArray([]);
    self.SpouseDistrictList = ko.observableArray([]);
    self.SpouseThanaList = ko.observableArray([]);

    self.errors = ko.validation.group(self);
    self.IsValid = ko.computed(function () {
        if (self.errors().length === 0)
            return true;
        else {
            return false;
        }

    });


    self.LoadData = function (cifno) {
        //
        if (typeof cifno !== "undefined") {
            $.getJSON("/IPDC/CIF/GetCIF_Info/?cifNo=" + cifno,
                null,
                function (data) {
                    self.Id(data.Id);
                    self.Name(data.Name);
                    self.FathersTitle(data.FathersTitle);
                    self.FathersName(data.FathersName);
                    self.MothersTitle(data.MothersTitle);
                    self.MothersName(data.MothersName);
                    $.when(self.GetGender())
                          .done(function () {
                              self.Gender(data.Gender);
                          });
                    $.when(self.GetNationality())
                        .done(function () {
                            //self.Gender(data.Gender);
                            self.NationalityId(data.NationalityId);
                        });
                    self.DateOfBirth(data.DateOfBirth);//$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
                    self.PassportIssueDate(data.PassportIssueDate);
                    self.CommCertIssueDate(data.CommCertIssueDate);
                    //$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY', date: new Date(data.DateOfBirth.substring(6, 18)) });
                    $('#NewDateOfBirth').val(data.DateOfBirthText);
                    $('#NextFollowUp').val(data.PassportIssueDateText);
                    $('#NewCommCertIssueDate').val(data.CommCertIssueDateText);

                    self.DateOfBirthText(data.DateOfBirthText);
                    self.BirthCountryId(data.BirthCountryId);
                    self.BirthDistrictId(data.BirthDistrictId);
                    self.BirthDistrictForeign(data.BirthDistrictForeign);
                    self.NIDNo(data.NIDNo);
                    self.SmartNIDNo(data.SmartNIDNo);
                    self.PassportNo(data.PassportNo);
                    self.PassportIssueDateText(data.PassportIssueDateText);
                    self.DLNo(data.DLNo);
                    self.BirthRegNo(data.BirthRegNo);
                    self.CommissionarCertificateNo(data.CommissionarCertificateNo);
                    self.CommCertIssueDateText(data.CommCertIssueDateText);
                    self.ETIN(data.ETIN);
                    $.when(self.GetMaritalStatus())
                        .done(function () {
                            self.MaritalStatus(data.MaritalStatus);
                        });

                    self.SpouseTitle(data.SpouseTitle);
                    self.SpouseName(data.SpouseName);
                    self.SpousePhoneNo(data.SpousePhoneNo);
                    self.SpouseProfessionId(data.SpouseProfessionId);
                    self.SpouseCompanyEnlisted(data.SpouseCompanyEnlisted);
                    self.SpouseCompanyName(data.SpouseCompanyName);
                    self.SpouseDesignation(data.SpouseDesignation);
                    self.SpouseCompanyId(data.SpouseCompanyId);
                    self.NumberOfDependents(data.NumberOfDependents);
                    $.when(self.GetHighestEducationLevel())
                       .done(function () {
                           self.HighestEducationLevel(data.HighestEducationLevel);
                       });
                    $.when(self.GetResidenceStatus())
                     .done(function () {
                         self.ResidenceStatus(data.ResidenceStatus);
                     });
                    $.when(self.GetHomeOwnership())
                     .done(function () {
                         self.HomeOwnership(data.HomeOwnership);
                     });
                    $.when(self.GetYearsCurrentResidence())
                     .done(function () {
                         self.YearsInCurrentResidence(data.YearsInCurrentResidence);
                     });
                    self.ContactAddress(data.ContactAddress);
                    //self.SpouseWorkAddress.Initialize(data.SpouseWorkAddress);
                    if (data.PermanentAddress != null || data.PermanentAddress != undefined) {
                        self.PermanentAddress.Id(data.PermanentAddress.Id);
                        self.PermanentAddress.AddressLine1(data.PermanentAddress.AddressLine1);
                        self.PermanentAddress.AddressLine2(data.PermanentAddress.AddressLine2);
                        self.PermanentAddress.AddressLine3(data.PermanentAddress.AddressLine3);
                        self.PermanentAddress.PostalCode(data.PermanentAddress.PostalCode);
                        self.PermanentAddress.Email(data.PermanentAddress.Email);
                        self.PermanentAddress.PhoneNo(data.PermanentAddress.PhoneNo);
                        self.PermanentAddress.CellPhoneNo(data.PermanentAddress.CellPhoneNo);
                        self.PermanentAddress.PostalCode(data.PermanentAddress.PostalCode);
                        $.when(self.GetCountry())
                            .done(function () {
                                self.PermanentAddress.CountryId(data.PermanentAddress.CountryId);
                                self.PassportIssueCountryId(data.PassportIssueCountryId);
                                self.DLIssueCountryId(data.DLIssueCountryId);

                                $.when(self.LoadDivisionByCountry())
                                    .done(function () {
                                        self.PermanentAddress.DivisionId(data.PermanentAddress.DivisionId);
                                        $.when(self.getDistrictByDivision())
                                            .done(function () {
                                                self.PermanentAddress.DistrictId(data.PermanentAddress.DistrictId);
                                                $.when(self.getUpzilaByDistrict())
                                                    .done(function () {
                                                        self.PermanentAddress.ThanaId(data.PermanentAddress.ThanaId);
                                                        //self.PermanentAddress.Initialize(data.PermanentAddress);
                                                    });
                                            });
                                    });
                            });
                    }
                    if (data.ResidenceAddress != null || data.ResidenceAddress != undefined) {
                        self.ResidenceAddress.Id(data.ResidenceAddress.Id);
                        self.ResidenceAddress.AddressLine1(data.ResidenceAddress.AddressLine1);
                        self.ResidenceAddress.AddressLine2(data.ResidenceAddress.AddressLine2);
                        self.ResidenceAddress.AddressLine3(data.ResidenceAddress.AddressLine3);
                        self.ResidenceAddress.PostalCode(data.ResidenceAddress.PostalCode);
                        self.ResidenceAddress.Email(data.ResidenceAddress.Email);
                        self.ResidenceAddress.PhoneNo(data.ResidenceAddress.PhoneNo);
                        self.ResidenceAddress.CellPhoneNo(data.ResidenceAddress.CellPhoneNo);
                        self.ResidenceAddress.PostalCode(data.ResidenceAddress.PostalCode);
                        $.when(self.GetResidenceCountry())
                            .done(function () {
                                self.ResidenceAddress.CountryId(data.ResidenceAddress.CountryId);
                                $.when(self.LoadResidenceDivisionByCountry())
                                    .done(function () {
                                        self.ResidenceAddress.DivisionId(data.ResidenceAddress.DivisionId);
                                        $.when(self.getResidenceDistrictByDivision())
                                            .done(function () {
                                                self.ResidenceAddress.DistrictId(data.ResidenceAddress.DistrictId);
                                                $.when(self.getResidenceUpzilaByDistrict())
                                                    .done(function () {
                                                        self.ResidenceAddress.ThanaId(data.ResidenceAddress.ThanaId);

                                                    });
                                            });
                                    });
                            });
                    }
                    //
                    if (data.SpouseWorkAddress != null || data.SpouseWorkAddress != undefined) {
                        self.SpouseWorkAddress.Id(data.SpouseWorkAddress.Id);
                        self.SpouseWorkAddress.AddressLine1(data.SpouseWorkAddress.AddressLine1);
                        self.SpouseWorkAddress.AddressLine2(data.SpouseWorkAddress.AddressLine2);
                        self.SpouseWorkAddress.AddressLine3(data.SpouseWorkAddress.AddressLine3);
                        self.SpouseWorkAddress.PostalCode(data.SpouseWorkAddress.PostalCode);
                        self.SpouseWorkAddress.Email(data.SpouseWorkAddress.Email);
                        self.SpouseWorkAddress.PhoneNo(data.SpouseWorkAddress.PhoneNo);
                        self.SpouseWorkAddress.CellPhoneNo(data.SpouseWorkAddress.CellPhoneNo);
                        self.SpouseWorkAddress.PostalCode(data.SpouseWorkAddress.PostalCode);
                        $.when(self.GetSpouseCountry())
                            .done(function () {
                                self.SpouseWorkAddress.CountryId(data.SpouseWorkAddress.CountryId);
                                $.when(self.LoadSpouseDivisionByCountry())
                                    .done(function () {
                                        self.SpouseWorkAddress.DivisionId(data.SpouseWorkAddress.DivisionId);
                                        $.when(self.getSpouseDistrictByDivision())
                                            .done(function () {
                                                self.SpouseWorkAddress.DistrictId(data.SpouseWorkAddress.DistrictId);
                                                $.when(self.getSpouseUpzilaByDistrict())
                                                    .done(function () {
                                                        self.SpouseWorkAddress.ThanaId(data.SpouseWorkAddress.ThanaId);

                                                    });
                                            });
                                    });
                            });
                    }

                });
        }
    }

    self.IsPrAddressChng = function () {
        //
        self.PermanentAddress.IsChanged(true);
    }

    self.IsRsAddressChng = function () {
        //
        self.ResidenceAddress.IsChanged(true);
    }
    self.IsSaAddressChng = function () {
        //
        self.SpouseWorkAddress.IsChanged(true);
    }


    self.GetGender = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Employee/GetGender',
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.GenderList(data);
            },
            error: function (error) {
                alert(error.status + "<-----and----->" + error.statusText);
            }
        });
    }
    self.GetMaritalStatus = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Employee/GetMaritalStatus',
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.MaritalStatusList(data);
            },
            error: function (error) {
                alert(error.status + "<-----and----->" + error.statusText);
            }
        });
    }
    self.GetNationality = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Nationality/GetAllNationality',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.NationalityList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.GetCountry = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetCountries',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.CountryList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.LoadDivisionByCountry = function () {
        var countryId = self.PermanentAddress.CountryId() ? self.PermanentAddress.CountryId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + countryId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.DivisionList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });

    }
    self.getDistrictByDivision = function () {
        var divisionId = self.PermanentAddress.DivisionId() ? self.PermanentAddress.DivisionId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.DistrictList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.getUpzilaByDistrict = function () {
        var districtId = self.PermanentAddress.DistrictId() ? self.PermanentAddress.DistrictId() : 0;

        return $.ajax({
            type: "GET",
            url: '/IPDC/SalesLead/GetThanasByDistrict?districtId=' + districtId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ThanaList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.GetResidenceCountry = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetCountries',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ResidenceCountryList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.LoadResidenceDivisionByCountry = function () {
        var countryId = self.ResidenceAddress.CountryId() ? self.ResidenceAddress.CountryId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + countryId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ResidenceDivisionList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });

    }
    self.getResidenceDistrictByDivision = function () {
        var divisionId = self.ResidenceAddress.DivisionId() ? self.ResidenceAddress.DivisionId() : 0;

        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ResidenceDistrictList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.getResidenceUpzilaByDistrict = function () {
        var districtId = self.ResidenceAddress.DistrictId() ? self.ResidenceAddress.DistrictId() : 0;

        return $.ajax({
            type: "GET",
            url: '/IPDC/SalesLead/GetThanasByDistrict?districtId=' + districtId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ResidenceThanaList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.GetHighestEducationLevel = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetHighestEducationLevel',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.HighestEducationLevelList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.GetResidenceStatus = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetResidenceStatus',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.ResidenceStatusList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    self.GetHomeOwnership = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetHomeOwnership',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.HomeOwnershipList(data); //Put the response in ObservableArray
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
    self.GetSpouseProfessionList = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Profession/GetAllProfession',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.SpouseStatusList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }
    //BirthDistrictList
    self.GetBirthDistrictList = function () {
        $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetDistrictsList',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.BirthDistrictList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.GetSpouseCountry = function () {
        return $.ajax({
            type: "GET",
            url: '/IPDC/Address/GetCountries',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.SpouseCountryList(data); //Put the response in ObservableArray
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.LoadSpouseDivisionByCountry = function () {
        var countryId = self.SpouseWorkAddress.CountryId() ? self.SpouseWorkAddress.CountryId() : 0;
        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDivisionByCountry?id=' + countryId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.SpouseDivisionList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });

    }
    self.getSpouseDistrictByDivision = function () {
        var divisionId = self.SpouseWorkAddress.DivisionId() ? self.SpouseWorkAddress.DivisionId() : 0;

        return $.ajax({
            type: "GET",
            url: '/IPDC/OfficeDesignationArea/GetDistrictByDivision?id=' + divisionId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.SpouseDistrictList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.getSpouseUpzilaByDistrict = function () {
        var districtId = self.SpouseWorkAddress.DistrictId() ? self.SpouseWorkAddress.DistrictId() : 0;

        return $.ajax({
            type: "GET",
            url: '/IPDC/SalesLead/GetThanasByDistrict?districtId=' + districtId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                self.SpouseThanaList(data);
            },
            error: function (error) {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });
    }

    self.Initialize = function () {

        self.GetGender();
        self.GetMaritalStatus();
        self.GetNationality();

        self.GetResidenceCountry();
        self.GetCountry();
        self.GetYearsCurrentResidence();
        self.GetHomeOwnership();
        self.GetResidenceStatus();
        self.GetHighestEducationLevel();
        self.GetSpouseProfessionList();
        self.GetBirthDistrictList();
        self.GetSpouseCountry();
        //self.LoadData();
    }
    self.Submit = function () {

    }

    self.SaveCif = function () {
        self.PassportIssueDateText($("#NextFollowUp").val());
        self.CommCertIssueDateText($("#NewCommCertIssueDate").val());
        self.DateOfBirthText($("#NewDateOfBirth").val());
        
        var clientInfo;
        clientInfo = {
            Id: self.Id(),
            //CIFNo :
            //CustomersHomeBranch : 
            //RMCode :
            //ApplicationSource :
            //ApplicationSourceName :
            //SourceRefId :
            //Title :
            Name: self.Name(),
            FathersTitle: self.FathersTitle(),
            FathersName: self.FathersName(),
            MothersTitle: self.MothersTitle(),
            MothersName: self.MothersName(),
            Gender: self.Gender(),
            //GenderName :
            NationalityId: self.NationalityId(),
            //NationalityName :
            //DateOfBirth : self.DateOfBirthText(),
            DateOfBirthText: self.DateOfBirthText(),
            BirthCountryId: self.BirthCountryId(),
            //BirthCountryName :
            BirthDistrictId: self.BirthDistrictId(),
            //BirthDistrictName :
            BirthDistrictForeign: self.BirthDistrictForeign(),
            //PermanentAddressId :
            //PermanentAddressName :
            //ResidenceAddressId :
            //ResidenceAddressName :
            ContactAddress: self.ContactAddress(),
            //ContactAddressName :
            NIDNo: self.NIDNo(),
            SmartNIDNo: self.SmartNIDNo(),
            PassportNo: self.PassportNo(),
            PassportIssueCountryId: self.PassportIssueCountryId(),
            //PassportIssueCountryName : self.PassportIssueCountryName
            //PassportIssueDate :self.PassportIssueDate(),
            PassportIssueDateText: self.PassportIssueDateText(),
            DLNo: self.DLNo(),
            DLIssueCountryId: self.DLIssueCountryId(),
            //DLIssueCountryName :
            BirthRegNo: self.BirthRegNo(),
            CommissionarCertificateNo: self.CommissionarCertificateNo(),
            //CommCertIssueDate :
            CommCertIssueDateText: self.CommCertIssueDateText(),
            ETIN: self.ETIN(),
            MaritalStatus: self.MaritalStatus(),
            //MaritalStatusName :
            SpouseTitle: self.SpouseTitle(),
            SpouseName: self.SpouseName(),
            SpousePhoneNo: self.SpousePhoneNo(),
            SpouseProfessionId: self.SpouseProfessionId(),
            SpouseCompanyEnlisted: self.SpouseCompanyEnlisted(),
            SpouseCompanyName: self.SpouseCompanyName(),
            SpouseDesignation: self.SpouseDesignation(),
            SpouseCompanyId: self.SpouseCompanyId(),
            //SpouseWorkAddressId :
            NumberOfDependents: self.NumberOfDependents(),
            HighestEducationLevel: self.HighestEducationLevel(),
            //HighestEducationLevelName :
            ResidenceStatus: self.ResidenceStatus(),
            //ResidenceStatusName :
            HomeOwnership: self.HomeOwnership(),
            //HomeOwnershipName :
            YearsInCurrentResidence: self.YearsInCurrentResidence(),
            //YearsInCurrentResidenceName :
            ResidenceAddress: self.ResidenceAddress, //ko.toJSON(self.ResidenceAddress),
            PermanentAddress: self.PermanentAddress, //ko.toJSON(self.PermanentAddress)
            SpouseWorkAddress: self.SpouseWorkAddress
        }
        //if (self.errors().length == 0) {
        $.ajax({
            type: "POST",
            url: '/IPDC/CIF/SaveCifIffo',
            data: ko.toJSON(clientInfo),
            contentType: "application/json",
            success: function (data) {
                $('#successModal').modal('show');
                $('#successModalText').text(data.Message);
            },
            error: function () {
                alert(error.status + "<--and--> " + error.statusText);
            }
        });

    }


}

//});



