ko.validation.rules['dateValidation'] = {
    validator: function (val, validate) {
        return ko.validation.utils.isEmptyVal(val) || moment(val, 'DD/MM/YYYY').isValid();
    },
    message: 'Invalid date'
};

ko.validation.registerExtenders();
$(function () {
    $('#VarificationDateId').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });

});
$(document).ready(function () {

    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });

    function VisitReportVm() {

        var self = this;
        self.Id = ko.observable();
        self.CIFId = ko.observable();
        self.CIF = ko.observable();
        self.CIFNo = ko.observable();
        self.CIFName = ko.observable();
        self.ApplicationId = ko.observable();
        self.Application = ko.observable();
        self.VerificationPersonRole = ko.observable();
        self.VerificationPersonRoleFrom = ko.observable();
        self.VerificationPersonRoleName = ko.observable();
        self.VisitTime = ko.observable();
        self.VisitTimeText = ko.observable();
        self.VisitedById = ko.observable();
        self.VisitedBy = ko.observable();
        self.OfficeAddress = new address();
        self.CompanyName = ko.observable();
        self.TypeOfOrg = ko.observable();
        self.NatureOfBusiness = ko.observable();
        self.OfficeAddressId = ko.observable();
        //self.OfficeAddress = ko.observable();
        self.ContactedPersonName = ko.observable();
        self.ContactedPersonDetails = ko.observable();
        self.Description = ko.observable();
        self.Observation = ko.observable();
        self.LegalStatuses = ko.observableArray([]);
        self.CountryIdList = ko.observableArray([]);
        self.VisitReportFileName = ko.observable();
        self.VisitReportFile = ko.observable();
        self.VisitReportPath = ko.observable();
        self.VerificationStatus = ko.observable();
        self.VisitHistory = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'CIFPId',
                Value: self.CIFId()
            }, {
                Name: 'Id',
                Value: self.Id()
            }];
            var menuInfo = {
                //Id: urlId++,
                Menu: 'Visit Report History',
                Url: '/IPDC/Verification/VisitReportHistory',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        ////////////////////////////////////////////////Latest///////////////////////////////////////////////////////////
        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            if (self.errors().length === 0)
                return true;
            else {
                return false;
            }

        });

        self.GetAllCifOrgLegalStatus = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CIF/GetAllCifOrgLegalStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.LegalStatuses(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        self.LoadVisitReport = function () {
            if (self.Id() > 0 || (self.CIFId() > 0 && self.ApplicationId() > 0)) {
                $.getJSON("/IPDC/Verification/LoadVisitReport?AppId=" + self.ApplicationId() + '&CIFPId=' + self.CIFId() + '&Id=' + self.Id(),
                    null,
                    function (data) {

                        self.Id(data.Id);
                        self.CIFId(data.CIFId);
                        self.CIFName(data.CIFName);
                        self.CIFNo(data.CIFNo);
                        self.ApplicationId(data.ApplicationId);
                        if (data.Id > 0)
                            self.VerificationPersonRole(data.VerificationPersonRole + '');
                        self.VisitTime(data.VisitTime);
                        self.VisitTimeText(data.VisitTimeText);
                        self.CompanyName(data.CompanyName);
                        $.when(self.GetAllCifOrgLegalStatus())
                          .done(function () {
                              self.TypeOfOrg(data.TypeOfOrg);
                          });

                        self.NatureOfBusiness(data.NatureOfBusiness?data.NatureOfBusiness:'');
                        self.OfficeAddressId(data.OfficeAddressId);
                        if (data.OfficeAddressId)
                            self.OfficeAddress.LoadAddress(data.OfficeAddress);
                        //console.log(ko.toJSON(self.OfficeAddress));
                        self.ContactedPersonName(data.ContactedPersonName);
                        self.ContactedPersonDetails(data.ContactedPersonDetails);
                        self.VisitReportFileName(data.VisitReportFileName);
                        self.VisitReportFile(data.VisitReportFile);
                        self.VisitReportPath(data.VisitReportPath);
                        self.Description(data.Description);
                        self.Observation(data.Observation);
                        self.VerificationStatus(data.VerificationStatus + '');
                    });
            }
        }
        self.IsEdit = ko.observable(false);
        self.Create = function () {
            //
            //self.IsEdit(false);
            self.Submit();
        }
        self.Edit = function () {
            self.IsEdit(true);
            self.Submit();
        }
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
        self.getEditUrl = function (data) {

            return '/IPDC/Verification/Download?fileBytes=' + self.VisitReportPath() + '&fileName=' + self.VisitReportFileName();
        }
        self.Submit = function () {

            if (typeof (self.VisitReportFile()) != 'undefined') {
                var file_data = $('#VisitReportFile').prop('files')[0];
            }
            self.VisitTimeText($("#VarificationDateId").val());
            //if (self.IsEdit() === false) {
            //    self.Id('');
            //    self.VerificationPersonRole(self.VerificationPersonRoleFrom());
            //}
            var formData = new FormData();
            formData.append('Id', self.Id());
            formData.append('CIFId', self.CIFId());
            formData.append('ApplicationId', self.ApplicationId());
            formData.append('VerificationPersonRole', self.VerificationPersonRole());
            formData.append('VisitTimeText', self.VisitTimeText());
            formData.append('CompanyName', self.CompanyName());
            formData.append('TypeOfOrg', self.TypeOfOrg());
            formData.append('NatureOfBusiness', self.NatureOfBusiness());
            formData.append('OfficeAddress.Id', self.OfficeAddress.Id());
            formData.append('OfficeAddress.ThanaId', self.OfficeAddress.ThanaId());
            formData.append('OfficeAddress.ThanaName', self.OfficeAddress.ThanaName());
            formData.append('OfficeAddress.DistrictId', self.OfficeAddress.DistrictId());
            formData.append('OfficeAddress.DistrictName', self.OfficeAddress.DistrictName());
            formData.append('OfficeAddress.DivisionId', self.OfficeAddress.DivisionId());
            formData.append('OfficeAddress.DivisionName', self.OfficeAddress.DivisionName());
            formData.append('OfficeAddress.CountryId', self.OfficeAddress.CountryId());
            formData.append('OfficeAddress.CountryName', self.OfficeAddress.CountryName());
            formData.append('OfficeAddress.AddressLine1', self.OfficeAddress.AddressLine1());
            formData.append('OfficeAddress.AddressLine2', self.OfficeAddress.AddressLine2());
            formData.append('OfficeAddress.AddressLine3', self.OfficeAddress.AddressLine3());
            formData.append('OfficeAddress.PostalCode', self.OfficeAddress.PostalCode());
            formData.append('OfficeAddress.PhoneNo', self.OfficeAddress.PhoneNo());
            formData.append('OfficeAddress.CellPhoneNo', self.OfficeAddress.CellPhoneNo());
            formData.append('OfficeAddress.Email', self.OfficeAddress.Email());
            formData.append('OfficeAddress.IsChanged', self.OfficeAddress.IsChanged());
            //formData.append('OfficeAddress', JSON.stringify(self.OfficeAddress));
            formData.append('ContactedPersonName', self.ContactedPersonName());
            formData.append('ContactedPersonDetails', self.ContactedPersonDetails());
            formData.append('Description', self.Description());
            formData.append('Observation', self.Observation());
            formData.append('VisitReportFileName', self.VisitReportFileName());
            formData.append('VisitReportPath', self.VisitReportPath());
            formData.append('VisitReportFile', file_data);
            formData.append('VerificationStatus', self.VerificationStatus());

            //var submitData = {              
            //Id: self.Id(),
            //CIFId: self.CIFId(),
            //ApplicationId: self.ApplicationId(),
            //VerificationPersonRole: self.VerificationPersonRole(),
            //VisitTimeText: self.VisitTimeText(),
            //CompanyName: self.CompanyName(),
            //TypeOfOrg: self.TypeOfOrg(),
            //NatureOfBusiness: self.NatureOfBusiness(),
            //OfficeAddress: self.OfficeAddress,
            //ContactedPersonName: self.ContactedPersonName(),
            //ContactedPersonDetails: self.ContactedPersonDetails(),
            //Description: self.Description(),
            //Observation: self.Observation()
            //}

            if (self.errors().length === 0) {
                $.ajax({
                    type: "POST",
                    url: '/IPDC/Verification/SaveVisitReport',
                    data: formData,
                    contentType: false,//"application/json",
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
            } else {
                self.errors.showAllMessages();
            }
        }

        self.Initialize = function () {
            self.GetAllCifOrgLegalStatus();
            self.LoadCountryIDList();
            if (self.Id() > 0 || (self.CIFId() > 0 && self.ApplicationId() > 0)) {
                self.LoadVisitReport();
            }
        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));

        }
        //var appvm = new ApplicationVm();
    }
    appvm = new VisitReportVm();
    var qValue = appvm.queryString('AppId');
    appvm.ApplicationId(qValue);
    var cif = appvm.queryString("CIFPId");
    appvm.CIFId(cif);
    appvm.Id(appvm.queryString("Id"));
    appvm.VerificationPersonRole(appvm.queryString("VerificationAs"));
    appvm.VerificationPersonRoleFrom(appvm.VerificationPersonRole);
    appvm.Initialize();
    ko.applyBindings(appvm, $('#VisitReportVw')[0]);
});



