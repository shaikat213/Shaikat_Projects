$(function () {
    $('#VarificationDateId').datetimepicker({ format: 'DD/MM/YYYY' });

});
$(document).ready(function () {

    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });


    function CIBPersonalVm() {

        var self = this;
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.CIF_PersonalId = ko.observable();
        self.CIFNo = ko.observable();
        self.CIFName = ko.observable();
        self.CIF_PersonalList = ko.observableArray([]);
        self.VerificationDate = ko.observable();
        self.VerificationDateTxt = ko.observable();
        self.VerificationPersonRole = ko.observable();
        self.VerificationPersonRoleFrom = ko.observable();
        self.VerificationPersonRoleName = ko.observable();

        self.NoOfLivingContactsAsBorrower = ko.observable();
        self.TotalOutstandingAsBorrower = ko.observable();
        self.ClassifiedAmountAsBorrower = ko.observable();
        self.TotalEMIAsBorrower = ko.observable();
        self.CIBClassificationStatusAsBorrower = ko.observable();
        self.CIBClassificationStatusAsBorrowerName = ko.observable();

        self.NoOfLivingContactsAsGuarantor = ko.observable();
        self.TotalOutstandingAsGuarantor = ko.observable();
        self.ClassifiedAmountAsGuarantor = ko.observable();
        self.TotalEMIAsGuarantor = ko.observable();
        self.CIBClassificationStatusAsGuarantor = ko.observable();
        self.CIBClassificationStatusAsGuarantorName = ko.observable();

        self.ExposureInBusiness = ko.observable();
        self.CIBClassificationStatusOfBusiness = ko.observable();
        self.CIBClassificationStatusOfBusinessName = ko.observable();
        self.CIBReport = ko.observable('');
        self.CIBReportFile = ko.observable('');
        self.VerificationPersonRoleList = ko.observableArray([]);
        self.CIBClassificationStatusAsBorrowerList = ko.observableArray([]);
        self.CIBClassificationStatusAsGuarantorList = ko.observableArray([]);
        self.CIBClassificationStatusOfBusinessList = ko.observableArray([]);
        self.CibType = ko.observable(2);

        //
        self.CIFFathersName = ko.observable();
        self.CIFMothersName = ko.observable();
        self.AppliedAmount = ko.observable();
        self.DateOfBirth = ko.observable();
        self.BirthDistrictName = ko.observable();
        self.NIDNo = ko.observable();
        self.PassportNo = ko.observable();
        self.DLNo = ko.observable();
        self.PermanentAddress = ko.observable();
        self.CIBReportFileName = ko.observable();
        self.BirthRegNo = ko.observable();
        self.SmartNIDNo = ko.observable();
        self.PassportIssueCountryId = ko.observable();
        self.PassportIssueCountryName = ko.observable();
        self.PassportIssueDate = ko.observable();
        //

        self.CibHistory = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'CIFPId',
                Value: self.CIF_PersonalId()
            }, {
                Name: 'Id',
                Value: self.Id()
            }, {
                Name: 'CibType',
                Value: 1
            }];
            var menuInfo = {
                //Id: urlId++,
                Menu: 'CIB History',
                Url: '/IPDC/Verification/CIBVerificationHistory',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.GetCIBClassificationStatusAsBorrower = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetCIBClassificationStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    
                    self.CIBClassificationStatusAsBorrowerList(data);
                    self.CIBClassificationStatusAsGuarantorList(data);
                    self.CIBClassificationStatusOfBusinessList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SaveCIBPersonal = function () {
            
            if (typeof (self.CIBReportFile()) != 'undefined') {
                var file_data = $('#CIBReportFile').prop('files')[0];
            }
            var formData = new FormData();
            formData.append('Id', self.Id());
            formData.append('ApplicationId', self.ApplicationId());
            formData.append('CIF_PersonalId', self.CIF_PersonalId());
            formData.append('CIFNo', self.CIFNo());
            formData.append('CIFName', self.CIFName());
            formData.append('VerificationDate', self.VerificationDate());
            formData.append('VerificationDateTxt', moment(self.VerificationDate()).format("DD/MM/YYYY"));
            formData.append('VerificationPersonRole', self.VerificationPersonRole());
            formData.append('VerificationPersonRoleName', self.VerificationPersonRoleName());
            formData.append('NoOfLivingContactsAsBorrower', self.NoOfLivingContactsAsBorrower());
            formData.append('TotalOutstandingAsBorrower', self.TotalOutstandingAsBorrower());
            formData.append('ClassifiedAmountAsBorrower', self.ClassifiedAmountAsBorrower());
            formData.append('CIBClassificationStatusAsBorrowerName', self.CIBClassificationStatusAsBorrowerName());
            formData.append('CIBClassificationStatusAsBorrower', self.CIBClassificationStatusAsBorrower());
            formData.append('NoOfLivingContactsAsGuarantor', self.NoOfLivingContactsAsGuarantor());
            formData.append('TotalOutstandingAsGuarantor', self.TotalOutstandingAsGuarantor());
            formData.append('ClassifiedAmountAsGuarantor', self.ClassifiedAmountAsGuarantor());
            formData.append('TotalEMIAsGuarantor', self.TotalEMIAsGuarantor());
            formData.append('CIBClassificationStatusAsGuarantor', self.CIBClassificationStatusAsGuarantor());
            formData.append('CIBClassificationStatusAsGuarantorName', self.CIBClassificationStatusAsGuarantorName());
            formData.append('ExposureInBusiness', self.ExposureInBusiness());
            formData.append('CIBClassificationStatusOfBusiness', self.CIBClassificationStatusOfBusiness());
            formData.append('CIBClassificationStatusOfBusinessName', self.CIBClassificationStatusOfBusinessName());
            formData.append('CIBReportFile', file_data);
            formData.append('TotalEMIAsBorrower', self.TotalEMIAsBorrower());
            formData.append('CIBReport', self.CIBReport());
           
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveCIBPersonal',
                data: formData,
                contentType: false,//"application/json",
                processData: false,
                cache: false,
                success: function (data) {
                    $('#cibSuccessModal').modal('show');
                    $('#cibSuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SaveNewCIBPersonal = function () {
            self.Id('');
            self.VerificationPersonRole(self.VerificationPersonRoleFrom());
            self.SaveCIBPersonal();
        }
        self.LoadCIBPersonal = function () {
            //if (self.CIF_PersonalId() > 0) {
            $.getJSON('/IPDC/Verification/LoadCIBPersonal/?AppId=' + self.ApplicationId() + '&CIFPId=' + self.CIF_PersonalId() + '&Id=' + self.Id(),
                null,
                function (data) {
                    
                    self.Id(data.Id);
                    self.ApplicationId(data.ApplicationId);
                    self.CIF_PersonalId(data.CIF_PersonalId);
                    self.CIFNo(data.CIFNo);
                    self.CIFName(data.CIFName);
                    self.VerificationDate(data.VerificationDate ? moment(data.VerificationDate) : moment());
                    self.VerificationDateTxt(data.VerificationDateTxt);
                    if (data.Id > 0)
                        self.VerificationPersonRole(data.VerificationPersonRole.toString());
                    self.VerificationPersonRoleName(data.VerificationPersonRoleName);
                    self.NoOfLivingContactsAsBorrower(data.NoOfLivingContactsAsBorrower);
                    self.TotalOutstandingAsBorrower(data.TotalOutstandingAsBorrower);
                    self.ClassifiedAmountAsBorrower(data.ClassifiedAmountAsBorrower);
                    self.TotalEMIAsBorrower(data.TotalEMIAsBorrower);
                    self.NoOfLivingContactsAsGuarantor(data.NoOfLivingContactsAsGuarantor);
                    self.TotalOutstandingAsGuarantor(data.TotalOutstandingAsGuarantor);
                    self.ClassifiedAmountAsGuarantor(data.ClassifiedAmountAsGuarantor);
                    self.TotalEMIAsGuarantor(data.TotalEMIAsGuarantor);
                    self.ExposureInBusiness(data.ExposureInBusiness);
                    //
                    self.CIFFathersName(data.CIFFathersName);
                    self.CIFMothersName(data.CIFMothersName);
                    self.AppliedAmount(data.AppliedAmount);
                    self.DateOfBirth(data.DateOfBirth ? moment(data.DateOfBirth).format('DD/MM/YYYY') : '');
                    self.BirthDistrictName(data.BirthDistrictName);
                    self.NIDNo(data.NIDNo);
                    self.PassportNo(data.PassportNo);
                    self.DLNo(data.DLNo);
                    self.PermanentAddress(data.PermanentAddress);
                    self.CIBReport(data.CIBReport);
                    self.CIBReportFileName(data.CIBReportFileName);
                    self.BirthRegNo(data.BirthRegNo);
                    self.SmartNIDNo(data.SmartNIDNo);
                    self.PassportIssueCountryId(data.PassportIssueCountryId);
                    self.PassportIssueCountryName(data.PassportIssueCountryName);
                    self.PassportIssueDate(data.PassportIssueDate ? moment(data.PassportIssueDate).format('DD/MM/YYYY') : '');
                    $.when(self.GetCIBClassificationStatusAsBorrower()).done(function () {
                        self.CIBClassificationStatusAsBorrower(data.CIBClassificationStatusAsBorrower);
                        self.CIBClassificationStatusAsBorrowerName(data.CIBClassificationStatusAsBorrowerName);
                        self.CIBClassificationStatusAsGuarantor(data.CIBClassificationStatusAsGuarantor);
                        self.CIBClassificationStatusAsGuarantorName(data.CIBClassificationStatusAsGuarantorName);
                        self.CIBClassificationStatusOfBusiness(data.CIBClassificationStatusOfBusiness);
                        self.CIBClassificationStatusOfBusinessName(data.CIBClassificationStatusOfBusinessName);
                    });
                });
            //}
        }
        self.getEditUrl = function (data) {
            return '/IPDC/Verification/Download?fileBytes=' + self.CIBReport() + '&fileName=' + self.CIBReportFileName();
        }
        self.Initialize = function () {
            self.GetCIBClassificationStatusAsBorrower();
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    appvm = new CIBPersonalVm();
    appvm.Initialize();
    var qValue = appvm.queryString('AppId');
    appvm.ApplicationId(qValue);
    var cifId = appvm.queryString('CIFPId');
    appvm.CIF_PersonalId(cifId);
    var selfId = appvm.queryString('Id');
    appvm.VerificationPersonRole(appvm.queryString("VerificationAs"));
    appvm.VerificationPersonRoleFrom(appvm.VerificationPersonRole());
    appvm.Id(selfId);
    appvm.LoadCIBPersonal();
    ko.applyBindings(appvm, $('#cibpersonal')[0]);

});