ko.validation.rules['dateValidation'] = {
    validator: function (val, validate) {
        return ko.validation.utils.isEmptyVal(val) || moment(val, 'DD/MM/YYYY').isValid();
    },
    message: 'Invalid date'
};

ko.validation.registerExtenders();

$(document).ready(function () {

    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });

    function NidVerificationVm() {

        var self = this;
        self.CifId = ko.observable();
        self.CIFNo = ko.observable();
        self.Name = ko.observable();
        self.ApplicationId = ko.observable();
        self.Id = ko.observable();
        self.VerificationDateText = ko.observable();
        self.DateOfBirthText = ko.observable();
        self.VerificationPersonRole = ko.observable();
        self.VerificationPersonRoleFrom = ko.observable();
        self.VerificationDate = ko.observable();
        self.VerifiedByUserId = ko.observable();
        self.VerifiedByEmpDegMapId = ko.observable();
        self.NIDNo = ko.observable().extend({ minLength: { params: 13, message: "Please enter Valid NID number" }, maxLength: 17 });
        self.DateOfBirth = ko.observable();
        self.DateOfBirthText = ko.observable();
        self.Finding = ko.observable();
        self.Remarks = ko.observable();
        self.VerificationStatus = ko.observable();
        //self.GuarantorCifList = ko.observableArray([]);
        //self.CountryIdList = ko.observableArray([]);

        //self.ValuationTypes = ko.observableArray([]);
        self.GuarantorCifList = ko.observableArray([]);
        self.CIFList = ko.observableArray([]);
        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            if (self.errors().length === 0)
                return true;
            else {
                return false;
            }

        });


        ////////////////////////////////////////////////Latest///////////////////////////////////////////////////////////

        self.NidHistory = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'CIFPId',
                Value: self.CifId()
            }, {
                Name: 'Id',
                Value: self.Id()
            }];
            var menuInfo = {
                //Id: urlId++,
                Menu: 'NID Verification History',
                Url: '/IPDC/Verification/NIDVerificationHistory',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
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

        self.LoadNidData = function () {
            if (self.Id() > 0 || (self.CifId() > 0 && self.ApplicationId() > 0)) {
                $.getJSON("/IPDC/Verification/LoadNidVerification?AppId=" + self.ApplicationId() + '&CIFPId=' + self.CifId() + '&Id=' + self.Id(),
                    null,
                    function (data) {
                        self.Id(data.Id);
                        self.CifId(data.CifId);
                        self.CIFNo(data.CIFNo);
                        self.ApplicationId(data.ApplicationId);
                        self.VerificationDateText(data.VerificationDateText);
                        self.VerificationDate(data.VerificationDate ? moment(data.VerificationDate) : moment());
                        self.DateOfBirthText(data.DateOfBirthText);
                        self.DateOfBirth(data.DateOfBirth ? moment(data.DateOfBirth) : moment());
                        self.VerificationStatus(data.VerificationStatus + '');
                        if (data.Id > 0)
                            self.VerificationPersonRole(data.VerificationPersonRole + '');
                        
                        if (data.Name)
                            self.Name(data.Name);
                        else
                            self.Name(data.CIF.Name);
                        self.NIDNo(data.NIDNo);
                        self.Finding(data.Finding.toString());
                        self.Remarks(data.Remarks);

                    });
            }
        }

        self.Submit = function () {
            var submitData = {
                Id: self.Id(),
                Name: self.Name(),
                CifId: self.CifId(),
                CIFNo:self.CIFNo(),
                ApplicationId: self.ApplicationId(),
                VerificationDateText: moment(self.VerificationDate()).format('DD/MM/YYYY'),
                DateOfBirthText: moment(self.DateOfBirth()).format('DD/MM/YYYY'),
                VerificationStatus: self.VerificationStatus(),
                VerificationPersonRole: self.VerificationPersonRole(),
                NIDNo: self.NIDNo(),
                Finding: self.Finding(),
                Remarks: self.Remarks()

            }
            if (self.errors().length == 0) {
                $.ajax({
                    type: "POST",
                    url: '/IPDC/Verification/SaveNidVerification',
                    data: ko.toJSON(submitData),
                    contentType: "application/json",
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
        self.SubmitNew = function () {
            self.Id('');
            self.Submit();
            self.VerificationPersonRole(self.VerificationPersonRoleFrom());
        }
        self.Details = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            },
            {
                Name: 'CIFPId',
                Value: self.CifId()
            }];
            var menuInfo = {
                Id: 93,
                Menu: 'Verification',
                Url: '/IPDC/Verification/NIDVerificationHistory',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.Initialize = function () {

            if (self.Id() > 0 || (self.CifId() > 0 && self.ApplicationId() > 0)) {
                self.LoadNidData();
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
    appvm = new NidVerificationVm();
    var qValue = appvm.queryString('AppId');
    appvm.ApplicationId(qValue);
    var cif = appvm.queryString("CIFPId");
    appvm.CifId(cif);
    appvm.Id(appvm.queryString("Id"));
    appvm.VerificationPersonRole(appvm.queryString("VerificationAs"));
    appvm.VerificationPersonRoleFrom(appvm.VerificationPersonRole());
    appvm.Initialize();
    ko.applyBindings(appvm, $('#NidVerificationVw')[0]);
});



