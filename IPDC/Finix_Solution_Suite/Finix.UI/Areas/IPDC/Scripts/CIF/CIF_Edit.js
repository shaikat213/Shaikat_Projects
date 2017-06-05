/// <reference path="../knockout-3.4.0.debug.js" />
/// <reference path="../jquery-2.1.4.js" />
/// <reference path="../finix.util.js" />
/// <reference path="~/Scripts/knockout.validation.min.js" />
/// <reference path="~/Scripts/knockout-3.4.0.debug.js" />
ko.validation.rules['dateValidation'] = {
    validator: function (val, validate) {
        return !(ko.validation.utils.isEmptyVal(val)) && moment(val, 'DD/MM/YYYY').isValid();
    },
    message: 'Invalid date'
};

ko.validation.registerExtenders();
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#cifPhoto').attr('src', e.target.result);
            $('#cifPhoto').show();
            $('#cifPhotoOld').hide();
        }
        reader.readAsDataURL(input.files[0]);
    }
}
function readSigURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#cifSigPhoto').attr('src', e.target.result);
            $('#cifSigPhoto').show();
            $('#cifSigPhotoOld').hide();
        }
        reader.readAsDataURL(input.files[0]);
    }
}

var cifpvm;
$(document).ready(function () {


    var isVm;
    var nwVm;
    var occVm;
    var refVm;
    var cifCards;

    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });

    function CIF_PersonalVM() {

        var self = this;
        self.IsForeign = ko.observable(true);
        self.IsDisableCountry = ko.observable(true);
        self.IsPassport = ko.observable(false);
        self.IsDL = ko.observable(false);
        self.CIF_PersonalId = ko.observable('');
        self.SalesLeadList = ko.observableArray([]);
        self.OccupationId = ko.observable();

        self.colPassportInfo = function () {
            self.IsPassport(true);
        }

        self.colDriverLcs = function () {
            self.IsDL(true);
        }

        self.ForeignCheck = function () {
            if (self.BirthCountryId() > 0) {
                $.ajax({
                    type: "GET",
                    url: '/IPDC/CIF/CheckIfHomeCountry?id=' + self.BirthCountryId(),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data === true) {
                            self.IsForeign(false);
                            self.IsDisableCountry(true);
                        } else {
                            self.IsForeign(true);
                            self.IsDisableCountry(false);
                        }
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                self.IsForeign(true);
                self.IsDisableCountry(true);
            }
        }

        self.Id = ko.observable('');
        self.CIFNo = ko.observable('');
        self.CBSCIFNo = ko.observable('');
        self.CustomersHomeBranch = ko.observable('');
        self.RMCode = ko.observable('');
        self.ApplicationSource = ko.observable('');
        self.ApplicationSourceName = ko.observable('');
        self.SourceRefId = ko.observable('');

        self.Photo = ko.observableArray([]);
        self.SignaturePhoto = ko.observableArray([]);
        self.Name = ko.observable().extend({ required: true, maxLength: { params: 70, message: "Please enter no more than 70 characters. " } });
        self.Title = ko.observable().extend({ maxLength: { params: 20, message: "Please enter no more than 20 characters. " } });
        self.FathersTitle = ko.observable().extend({ maxLength: { params: 20, message: "Please enter no more than 20 characters. " } });
        self.FathersName = ko.observable().extend({ required: true, maxLength: { params: 70, message: "Please enter no more than 70 characters. " } });
        self.MothersTitle = ko.observable().extend({ maxLength: { params: 20, message: "Please enter no more than 20 characters. " } });
        self.MothersName = ko.observable().extend({ required: true, maxLength: { params: 70, message: "Please enter no more than 70 characters. " } });
        self.Gender = ko.observable().extend({ required: { message: 'Please give gender information' } });
        self.GenderName = ko.observable('');
        self.NationalityId = ko.observable().extend({ required: { message: 'Please provide nationality ' } });
        self.NationalityName = ko.observable('');
        self.BirthCountryId = ko.observable().extend({ required: { message: 'Please give bith country ' } });

        self.BirthCountryName = ko.observable('');
        self.BirthDistrictId = ko.observable('');
        self.BirthDistrictName = ko.observable('');
        self.BirthDistrictForeign = ko.observable();
        self.DateOfBirth = ko.observable();
        self.DateOfBirthText = ko.observable();

        self.ResidenceAddressId = ko.observable();
        self.ResidenceAddress = ko.validatedObservable(new address())();
        self.ResidenceAddress.CountryId.extend({ required: true });
        self.PermanentAddressId = ko.observable();
        self.PermanentAddress = new address();
        self.SpouseWorkAddressId = ko.observable();
        self.SpouseWorkAddress = new address();


        self.ContactAddress = ko.observable('');
        self.ContactAddressName = ko.observable('');

        self.NIDNo = ko.observable('');
        self.SmartNIDNo = ko.observable('');
        self.PassportNo = ko.observable('');
        self.DLNo = ko.observable('');
        self.CommissionarCertificateNo = ko.observable('');
        self.BirthRegNo = ko.observable('');

        self.NIDNo.extend({
            minLength: { params: 13, message: "Please enter Valid NID number" },
            maxLength: 17,
            required: {
                message: "NID/Smart NID/Passport No./Driver's License/Com. Certificate/Birth Reg. No. is required",
                onlyIf: function () {
                    return (self.SmartNIDNo().length === 0 &&
                            self.PassportNo().length === 0 &&
                            self.DLNo().length === 0 &&
                            self.CommissionarCertificateNo().length === 0 &&
                            self.BirthRegNo().length === 0);

                }
            }
        });
        self.SmartNIDNo.extend({
            minLength: { params: 10, message: "Please enter Valid Cmart Card number" },
            required: {
                message: "NID/Smart NID/Passport No./Driver's License/Com. Certificate/Birth Reg. No. is required",
                onlyIf: function () {
                    return (self.NIDNo().length === 0 &&
                            self.PassportNo().length === 0 &&
                            self.DLNo().length === 0 &&
                            self.CommissionarCertificateNo().length === 0 &&
                            self.BirthRegNo().length === 0);
                }
            }
        });
        self.PassportNo.extend({
            pattern: {
                message: 'Passport Number can only contain a-z, A-Z, 0-9',
                params: /^([A-Za-z0-9\-])+$/
            },
            required: {
                message: "NID/Smart NID/Passport No./Driver's License/Com. Certificate/Birth Reg. No. is required",
                onlyIf: function () {
                    return (self.NIDNo().length === 0 &&
                            self.SmartNIDNo().length === 0 &&
                            self.DLNo().length === 0 &&
                            self.CommissionarCertificateNo().length === 0 &&
                            self.BirthRegNo().length === 0);
                }
            }
        });
        self.DLNo.extend({
            required: {
                message: "NID/Smart NID/Passport No./Driver's License/Com. Certificate/Birth Reg. No. is required",
                onlyIf: function () {
                    return (self.NIDNo().length === 0 &&
                            self.SmartNIDNo().length === 0 &&
                            self.PassportNo().length === 0 &&
                            self.CommissionarCertificateNo().length === 0 &&
                            self.BirthRegNo().length === 0);
                }
            }
        });
        self.CommissionarCertificateNo.extend({
            required: {
                message: "NID/Smart NID/Passport No./Driver's License/Com. Certificate/Birth Reg. No. is required",
                onlyIf: function () {
                    return (self.NIDNo().length === 0 &&
                            self.SmartNIDNo().length === 0 &&
                            self.PassportNo().length === 0 &&
                            self.DLNo().length === 0 &&
                            self.BirthRegNo().length === 0);
                }
            }
        });
        self.BirthRegNo.extend({
            pattern: {
                message: 'Birth Reg Number can only contain a-z, A-Z, 0-9',
                params: /^([A-Za-z0-9])+$/
            },
            required: {
                message: "NID/Smart NID/Passport No./Driver's License/Com. Certificate/Birth Reg. No. is required",
                onlyIf: function () {
                    return (self.NIDNo().length === 0 &&
                            self.SmartNIDNo().length === 0 &&
                            self.PassportNo().length === 0 &&
                            self.DLNo().length === 0 &&
                            self.CommissionarCertificateNo().length === 0);
                }
            }
        });

        self.PassportIssueCountryId = ko.observable('');
        self.PassportIssueCountryName = ko.observable('');
        self.PassportIssueDate = ko.observable();
        self.PassportIssueDateText = ko.observable('');
        self.PhotoName = ko.observable('');
        self.SignaturePhotoName = ko.observable('');
        self.DLIssueCountryId = ko.observable('');
        self.DLIssueCountryName = ko.observable('');


        self.CommCertIssueDate = ko.observable();
        self.ETIN = ko.observable('');
        self.MaritalStatus = ko.observable().extend({});
        self.MaritalStatusName = ko.observable('');

        self.SpouseTitle = ko.observable('');
        self.SpouseName = ko.observable('');
        self.SpousePhoneNo = ko.observable().extend({ digit: true });
        self.SpouseProfessionId = ko.observable('');
        self.SpouseDesignation = ko.observable('');
        self.SpouseCompanyId = ko.observable('');
        self.SpouseWorkAddressId = ko.observable('');

        self.NumberOfDependents = ko.observable().extend({ required: true });
        self.HighestEducationLevel = ko.observable('');
        self.HighestEducationLevelName = ko.observable('');
        self.ResidenceStatus = ko.observable().extend({ required: true });
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

        self.ResidenceDivisionList = ko.observableArray([]);
        self.ResidenceDistrictList = ko.observableArray([]);
        self.ResidenceThanaList = ko.observableArray([]);
        self.PassportIssueDateText = ko.observable();
        self.CommCertIssueDateText = ko.observable();
        self.DLIssueDate = ko.observable();
        self.DLIssueDateTxt = ko.observable();
        self.BirthDistrictList = ko.observableArray([]);

        self.HighestEducationLevelList = ko.observableArray([]);
        self.ResidenceStatusList = ko.observableArray([]);
        self.YearsInCurrentResidenceList = ko.observableArray([]);
        self.HomeOwnershipList = ko.observableArray([]);
        self.SpouseStatusList = ko.observableArray([]);
        self.CompanyList = ko.observableArray([]);

        self.SpouseDivisionList = ko.observableArray([]);
        self.SpouseDistrictList = ko.observableArray([]);
        self.SpouseThanaList = ko.observableArray([]);

        // Copy Address Residence to Permanent

        self.UseResidenceAddress = function () {
            var id;
            if (self.ResidenceAddress.Id() > 0)
                id = self.ResidenceAddress.Id();
            var addressCopy = {
                Id: self.ResidenceAddress.Id(),
                ThanaId: self.ResidenceAddress.ThanaId(),
                DistrictId: self.ResidenceAddress.DistrictId(),
                DivisionId: self.ResidenceAddress.DivisionId(),
                CountryId: self.ResidenceAddress.CountryId(),
                AddressLine1: self.ResidenceAddress.AddressLine1(),
                AddressLine2: self.ResidenceAddress.AddressLine2(),
                AddressLine3: self.ResidenceAddress.AddressLine3(),
                PostalCode: self.ResidenceAddress.PostalCode(),
                PhoneNo: self.ResidenceAddress.PhoneNo(),
                CellPhoneNo: self.ResidenceAddress.CellPhoneNo(),
                Email: self.ResidenceAddress.Email(),
                IsChanged: self.ResidenceAddress.IsChanged()
            }
            self.PermanentAddress.LoadAddress(addressCopy);
            if (typeof (id) != 'undefined') {
                self.PermanentAddress.Id(id);
                self.PermanentAddressId(id);
            }
        }

        // Copy Address Residence to Permanent

        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {

            if (self.errors().length === 0)
                return true;
            else {
                return false;
            }

        });
        self.LoadData = function () {

            if (self.CIF_PersonalId() > 0) {
                $.getJSON("/IPDC/CIF/GetCIF_Info/?cifId=" + self.CIF_PersonalId(),
                    null,
                    function (data) {
                        self.Id(data.Id);
                        self.CIFNo(data.CIFNo);
                        self.CBSCIFNo(data.CBSCIFNo);
                        self.Name(data.Name);
                        self.Title(data.Title);
                        self.FathersTitle(data.FathersTitle);
                        self.FathersName(data.FathersName);
                        self.MothersTitle(data.MothersTitle);
                        self.MothersName(data.MothersName);
                        self.OccupationId(data.OccupationId);
                        $.when(self.GetGender())
                            .done(function () {
                                self.Gender(data.Gender);
                            });
                        $.when(self.GetNationality())
                            .done(function () {
                                //self.Gender(data.Gender);
                                self.NationalityId(data.NationalityId);
                            });
                        self.DateOfBirth(moment(data.DateOfBirth)); //$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
                        self.PassportIssueDate(moment(data.PassportIssueDate));
                        self.CommCertIssueDate(moment(data.CommCertIssueDate));
                        self.DLIssueDate(moment(data.DLIssueDate));
                        self.DateOfBirthText(data.DateOfBirthText);
                        self.BirthDistrictForeign(data.BirthDistrictForeign);
                        self.NIDNo(data.NIDNo ? data.NIDNo : '');
                        self.SmartNIDNo(data.SmartNIDNo ? data.SmartNIDNo : '');
                        self.PassportNo(data.PassportNo ? data.PassportNo : '');
                        self.PassportIssueDateText(data.PassportIssueDateText);
                        self.DLNo(data.DLNo ? data.DLNo : '');
                        self.BirthRegNo(data.BirthRegNo ? data.BirthRegNo : '');
                        self.CommissionarCertificateNo(data.CommissionarCertificateNo ? data.CommissionarCertificateNo : '');
                        self.CommCertIssueDateText(data.CommCertIssueDateText);
                        self.ETIN(data.ETIN);
                        self.PhotoName(data.PhotoName);
                        self.SignaturePhotoName(data.SignaturePhotoName);
                        $.when(self.GetBirthDistrictList()).done(function () {
                            self.BirthDistrictId(data.BirthDistrictId);
                        });
                        $.when(self.GetMaritalStatus())
                            .done(function () {
                                self.MaritalStatus(data.MaritalStatus);
                            });

                        self.SpouseTitle(data.SpouseTitle);
                        self.SpouseName(data.SpouseName);
                        self.SpousePhoneNo(data.SpousePhoneNo);

                        $.when(self.GetSpouseProfessionList())
                            .done(function () {
                                self.SpouseProfessionId(data.SpouseProfessionId);
                        });

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
                        self.ContactAddress(data.ContactAddress + "");

                        $.when(self.GetCountry()).done(function () {
                            self.PassportIssueCountryId(data.PassportIssueCountryId);
                            self.BirthCountryId(data.BirthCountryId);
                            self.DLIssueCountryId(data.DLIssueCountryId);
                            self.PermanentAddressId(data.PermanentAddressId);
                            if (data.PermanentAddress != null && typeof (data.PermanentAddress) != 'undefined') {
                                self.PermanentAddress.LoadAddress(data.PermanentAddress);
                            }
                            self.ResidenceAddressId(data.ResidenceAddressId);
                            if (data.ResidenceAddress != null && typeof (data.ResidenceAddress) != 'undefined') {
                                self.ResidenceAddress.LoadAddress(data.ResidenceAddress);
                            }
                            self.SpouseWorkAddressId(data.SpouseWorkAddressId);
                            if (data.SpouseWorkAddress != null && typeof (data.SpouseWorkAddress) != 'undefined') {
                                self.SpouseWorkAddress.LoadAddress(data.SpouseWorkAddress);
                            }
                        });

                    });
            }
            else {
                self.Initialize();
            }
        }


        //self.getData = function () {
        //    return $.ajax({
        //        type: "GET",
        //        url: '/IPDC/SalesLead/GetDataForLeadAssignment',
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (data) {
        //            self.SalesLeadList(data); //Put the response in ObservableArray
        //        },
        //        error: function (error) {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}
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
            return $.ajax({
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

        self.GetAllOrganizations = function () {
            //
            return $.ajax({
                type: "GET",
                url: '/IPDC/Organization/GetOrganizations',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CompanyList(data); //Put the response in ObservableArray
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
            self.GetAllOrganizations();
            self.GetCountry();
            self.GetYearsCurrentResidence();
            self.GetHomeOwnership();
            self.GetResidenceStatus();
            self.GetHighestEducationLevel();
            self.GetSpouseProfessionList();
            self.GetBirthDistrictList();

        }
        self.Submit = function () {
        }
        $("#imgInp").change(function () {
            readURL(this);
        });
        $("#imgSigInp").change(function () {
            readSigURL(this);
        });
        self.Upload = function () {
            var file_data = $('#imgInp').prop('files')[0];
            var file_data1 = $('#imgSigInp').prop('files')[0];
            var formData = new FormData();
            formData.append('Id', self.CIF_PersonalId());
            formData.append('Photo', file_data);
            formData.append('SignaturePhoto', file_data1);
            $.ajax({
                type: "POST",
                url: '/IPDC/CIF/UploadPicture',
                data: formData,
                contentType: false,
                processData: false,
                cache: false,
                success: function (data) {
                    $('#SuccessModal').modal('show');
                    $('#SuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SaveCif = function () {
            //
            self.PassportIssueDateText(moment(self.PassportIssueDate()).format('DD/MM/YYYY'));
            self.CommCertIssueDateText(moment(self.CommCertIssueDate()).format('DD/MM/YYYY'));
            self.DateOfBirthText(moment(self.DateOfBirth()).format('DD/MM/YYYY'));
            self.DLIssueDateTxt(moment(self.DLIssueDate()).format('DD/MM/YYYY'));
            var clientInfo = {
                Id: self.Id(),
                Name: self.Name(),
                CIFNo: self.CIFNo(),
                CBSCIFNo: self.CBSCIFNo(),
                Title: self.Title(),
                FathersTitle: self.FathersTitle(),
                FathersName: self.FathersName(),
                MothersTitle: self.MothersTitle(),
                MothersName: self.MothersName(),
                Gender: self.Gender(),
                NationalityId: self.NationalityId(),
                DateOfBirthText: self.DateOfBirthText(),
                BirthCountryId: self.BirthCountryId(),
                BirthDistrictId: self.BirthDistrictId(),
                BirthDistrictForeign: self.BirthDistrictForeign(),
                ContactAddress: self.ContactAddress(),
                NIDNo: self.NIDNo(),
                SmartNIDNo: self.SmartNIDNo(),
                PassportNo: self.PassportNo(),
                PassportIssueCountryId: self.PassportIssueCountryId(),
                PassportIssueDateText: self.PassportIssueDateText(),
                DLNo: self.DLNo(),
                DLIssueDateTxt: self.DLIssueDateTxt(),
                DLIssueCountryId: self.DLIssueCountryId(),
                BirthRegNo: self.BirthRegNo(),
                CommissionarCertificateNo: self.CommissionarCertificateNo(),
                CommCertIssueDateText: self.CommCertIssueDateText(),
                ETIN: self.ETIN(),
                MaritalStatus: self.MaritalStatus(),
                SpouseTitle: self.SpouseTitle(),
                SpouseName: self.SpouseName(),
                SpousePhoneNo: self.SpousePhoneNo(),
                SpouseProfessionId: self.SpouseProfessionId(),
                SpouseCompanyEnlisted: self.SpouseCompanyEnlisted(),
                SpouseCompanyName: self.SpouseCompanyName(),
                SpouseDesignation: self.SpouseDesignation(),
                SpouseCompanyId: self.SpouseCompanyId(),
                NumberOfDependents: self.NumberOfDependents(),
                HighestEducationLevel: self.HighestEducationLevel(),
                ResidenceStatus: self.ResidenceStatus(),
                HomeOwnership: self.HomeOwnership(),
                YearsInCurrentResidence: self.YearsInCurrentResidence(),
                ResidenceAddressId: self.ResidenceAddressId(),
                ResidenceAddress: self.ResidenceAddress, //ko.toJSON(self.ResidenceAddress),
                PermanentAddressId: self.PermanentAddressId(),
                PermanentAddress: self.PermanentAddress, //ko.toJSON(self.PermanentAddress)
                SpouseWorkAddressId: self.SpouseWorkAddressId(),
                SpouseWorkAddress: self.SpouseWorkAddress,
                OccupationId: self.OccupationId()
            };
            if (self.IsValid()) {
                $.ajax({
                    type: "POST",
                    url: '/IPDC/CIF/SaveCifIffo',
                    data: ko.toJSON(clientInfo),
                    contentType: "application/json",
                    success: function (data) {
                        $('#cifpSuccessModal').modal('show');
                        $('#cifpSuccessModalText').text(data.Message);
                        if (data.Id > 0) {
                            self.Id(data.Id);
                            self.CIF_PersonalId(data.Id);
                            self.Upload();
                            if (typeof (isVm) != 'undefined') {
                                isVm.CIF_PersonalId(data.Id);
                                isVm.LoadIncomeStatement();
                            }
                            if (typeof (nwVm) != 'undefined') {
                                nwVm.Initialize();
                                nwVm.CIF_PersonalId(data.Id);
                                nwVm.LoadNetWorth();
                            }
                            if (typeof (refVm) != 'undefined') {
                                refVm.Initialize();
                                refVm.CIF_PersonalId(data.Id);
                                refVm.LoadReference();
                            }
                            if (typeof (occVm) != 'undefined') {
                                occVm.Initialize();
                                occVm.CIF_PersonalId(data.Id);
                                occVm.LoadOccupation();
                            }
                            if (typeof (cifCards) != 'undefined') {
                                cifCards.CIFId(data.Id);
                                cifCards.LoadData();
                            }
                        }
                    },
                    error: function () {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                self.errors.showAllMessages();
            }

        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));

        }


    }
    cifpvm = new CIF_PersonalVM();
    var qValue = cifpvm.queryString('cifpid');
    cifpvm.CIF_PersonalId(qValue);
    //cifpvm.getData();

    $.getScript("/Areas/IPDC/Scripts/CIF/cifIncomeStatement.js", function () {
        isVm = new CIFIncomeStatementViewModel();
        if (typeof (isVm) != 'undefined') {
            isVm.CIF_PersonalId(qValue);
            isVm.LoadIncomeStatement();
        }
        ko.applyBindings(isVm, document.getElementById('cifIncomeStatement'));
    });


    $.getScript("/Areas/IPDC/Scripts/CIF/cifNetWorth.js", function () {
        nwVm = new CIFNetWorthViewModel();
        //
        nwVm.Initialize(); if (typeof (nwVm) != 'undefined') {
            nwVm.CIF_PersonalId(qValue);
            nwVm.LoadNetWorth();
        }
        //nwVm.CIF_PersonalId(1);
        ko.applyBindings(nwVm, document.getElementById('cifNetWorth'));
    });


    $.getScript("/Areas/IPDC/Scripts/CIF/cifReference.js", function () {
        refVm = new CIFRefenceViewModel();

        refVm.Initialize();
        if (typeof (refVm) != 'undefined') {
            refVm.CIF_PersonalId(qValue);
            refVm.LoadReference();
        }
        //refVm.CIF_PersonalId(1);
        ko.applyBindings(refVm, document.getElementById('cifReference'));
    });
    //

    $.getScript("/Areas/IPDC/Scripts/CIF/CIF_Occupation.js", function () {

        occVm = new ClientOccupationVm();
        occVm.Initialize();
        if (typeof (occVm) != 'undefined') {
            occVm.CIF_PersonalId(qValue);
            occVm.LoadOccupation();
        }
        ko.applyBindings(occVm, document.getElementById('cifOccupation'));
    });
    $.getScript("/Areas/IPDC/Scripts/CIF/cifCards.js", function () {

        cifCards = new CIFCardsViewModel();
        if (typeof (cifCards) != 'undefined') {
            cifCards.CIFId(qValue);
            cifCards.LoadData();
        }
        ko.applyBindings(cifCards, document.getElementById('cifCards'));
    });

    cifpvm.LoadData();
    ko.applyBindings(cifpvm, document.getElementById('cif_personal'));

});
