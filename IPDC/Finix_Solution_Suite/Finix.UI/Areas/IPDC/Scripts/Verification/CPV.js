
$(document).ready(function () {

    var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });

    function bankAccount(data) {

        var self = this;
        //self.Id = ko.observable(data ? data.Id : '');
        self.BankName = ko.observable(data ? data.BankName : '');
        self.AccountNo = ko.observable(data ? data.AccountNo : '');
        self.AccountVerification = ko.observable(data ? data.AccountVerification ? data.AccountVerification.toString() : 1 : 1);

        self.LoadData = function (data) {
            //self.Id(data ? data.Id : '');
            self.BankName(data ? data.BankName : '');
            self.AccountNo(data ? data.AccountNo : '');
            self.AccountVerification(data ? data.AccountVerification ? data.AccountVerification.toString() : 1 : 1);
        }
    }

    function reference(data) {
        var self = this;
        //self.Id = ko.observable(data ? data.Id : 1);
        self.Name = ko.observable(data ? data.Name : '');
        self.CifReferenceId = ko.observable(data ? data.CifReferenceId : 0);
        self.CifReferenceName = ko.observable(data ? data.CifReferenceName : '');
        self.Mobile = ko.observable(data ? data.Mobile : 1);
        self.Phone = ko.observable(data ? data.Phone : 1);
        self.ResidenceStatus = ko.observable(data ? data.ResidenceStatus : 1);
        self.ProfessionalInformation = ko.observable(data ? data.ProfessionalInformation : 1);
        self.RelationshipWithApplicant = ko.observable(data ? data.RelationshipWithApplicant : 1);
        self.LoadData = function (data) {
            //self.Id(data ? data.Id : 1);
            self.Name(data ? data.Name : "");
            self.CifReferenceId(data ? data.CifReferenceId : 0);
            self.CifReferenceName(data ? data.CifReferenceName : '');
            self.Mobile(data ? data.Mobile : 1);
            self.Phone(data ? data.Phone : 1);
            self.ResidenceStatus(data ? data.ResidenceStatus : 1);
            self.ProfessionalInformation(data ? data.ProfessionalInformation : 1);
            self.RelationshipWithApplicant(data ? data.RelationshipWithApplicant : 1);
        }
    }

    function ContactPointVm() {

        var self = this;
        self.Id = ko.observable();
        self.PhotoName = ko.observable();
        self.SignaturePhotoName = ko.observable();
        self.ApplicationId = ko.observable();
        self.CifId = ko.observable();
        self.CIFNo = ko.observable();
        self.CIFName = ko.observable();
        self.ApplicationNo = ko.observable();
        self.AccountTitle = ko.observable();
        self.VerificationDate = ko.observable();
        self.VerificationDateText = ko.observable();
        self.VerificationPersonRole = ko.observable();
        self.VerificationPersonRoleFrom = ko.observable();
        self.VerificationPersonRoleName = ko.observable();
        self.Photo = ko.observable('1');
        self.Name = ko.observable('1');
        self.MobileNo = ko.observable('1');
        self.ResidencePhone = ko.observable('1');
        self.HomeOwnership = ko.observable();
        self.HomeOwnerships = ko.observableArray([]);
        self.GetHomeOwnerships = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetHomeOwnerships',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.HomeOwnerships(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.OfficePhone = ko.observable('1');
        self.SignatureOfPhotoId = ko.observable('1');
        self.SignatureOfApplication = ko.observable('1');
        self.PresentAddress = ko.observable('1');
        self.ResidenceStatus = ko.observable('1');
        self.LocationOfResidence = ko.observable();
        self.MontylyRentAndUtilityExp = ko.observable();
        self.YearInPresentAddress = ko.observable();
        self.YearInPresentAddressList = ko.observableArray([]);
        self.LivingWith = ko.observable();
        self.PermanentAddress = ko.observable('1');
        self.PersonContacted = ko.observable();
        self.RelationshipWithApplicant = ko.observable();
        self.Relationships = ko.observableArray([]);
        self.ProfessionDeclared = ko.observable('1');
        self.ProfessionAssessedId = ko.observable();
        self.Professions = ko.observableArray([]);
        self.LocationsOfResidence = ko.observableArray([]);
        self.NameOfOrganization = ko.observable('1');
        self.AddressOfOrganization = ko.observable('1');
        self.NatureOfBusiness = ko.observable();
        self.YearOfEstablishment = ko.observable().extend({
            digit: true,
            minLength: { params: 4, message: "Please enter Year" },
            maxLength: { params: 4, message: "Please enter Year" }
        });
        self.TradeLicence = ko.observable('1');
        self.SalaryCertificate = ko.observable('1');
        self.PaySlip = ko.observable('1');
        self.OtherIncomeSource = ko.observable();
        self.OtherIncomeDeclared = ko.observable();
        self.OtherIncomeVerified = ko.observable();
        self.OtherIncomeVariance = ko.pureComputed(function () {
            var value = 0;
            if (self.OtherIncomeDeclared() > 0)
                value += parseFloat(self.OtherIncomeDeclared());
            if (self.OtherIncomeVerified() > 0)
                value -= parseFloat(self.OtherIncomeVerified());
            return value;
        });
        self.PersonContactedAtOffice = ko.observable();
        self.PersonContactedAtOfficeDetails = ko.observable();
        self.BankAccounts = ko.observableArray([]);
        self.AddBankAccount = function () {
            self.BankAccounts.push(new bankAccount());
        }
        self.RemovedBankAccounts = ko.observableArray([]);
        self.RemoveBankAccount = function (line) {
            if (line.Id() > 0)
                self.RemovedBankAccounts.push(line.Id());
            self.BankAccounts.remove(line);
        }
        self.References = ko.observableArray([]);
        self.AddReference = function () {
            self.References.push(new reference());
        }
        self.RemovedReferences = ko.observableArray([]);
        self.RemoveReference = function (line) {
            if (line.Id() > 0)
                self.RemovedReferences.push(line.Id());
            self.References.remove(line);
        }
        self.Remarks = ko.observable();
        self.VerificationStatus = ko.observable('0');
        self.VerificationStatuses = ko.observableArray([]);

        self.GetVerificationStatuses = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetVerificationStatuses',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.VerificationStatuses(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetLocationFindibility = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetLocationFindibility',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.LocationsOfResidence(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetYearsCurrentResidence = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetYearsCurrentResidence',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.YearInPresentAddressList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetRelationshipWithApplicant = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetRelationshipWithApplicant',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Relationships(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetAllProfession = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetAllProfession',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Professions(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.LoadCPVVerificationData = function () {
            $.getJSON("/IPDC/Verification/LoadCpvById/?AppId=" + self.ApplicationId() + '&CifId=' + self.CifId() + '&CpvId=' + self.Id(),
                null,
                function (data) {
                    $.when(self.GetVerificationStatuses())
                        .done(function () {
                            $.each(data.References, function (index, value) {
                                var aDetail = new reference();
                                if (typeof (value) != 'undefined') {
                                    aDetail.LoadData(value);
                                    self.References.push(aDetail);
                                }
                            });
                        });
                    $.when(self.GetLocationFindibility())
                      .done(function () {
                          self.LocationOfResidence(data.LocationOfResidence);
                      });
                    $.when(self.GetYearsCurrentResidence())
                      .done(function () {
                          self.YearInPresentAddress(data.YearInPresentAddress);
                      });
                    $.when(self.GetRelationshipWithApplicant())
                      .done(function () {
                          self.RelationshipWithApplicant(data.RelationshipWithApplicant);
                      });
                    $.when(self.GetAllProfession())
                      .done(function () {
                          self.ProfessionAssessedId(data.ProfessionAssessedId);
                      });
                    $.when(self.GetHomeOwnerships())
                      .done(function () {
                          self.HomeOwnership(data.HomeOwnership);
                      });
                    self.Id(data.Id);
                    self.VerificationDate(data.VerificationDate ? moment(data.VerificationDate) : moment());
                    self.CifId(data.CifId);
                    self.CIFNo(data.CIFNo);
                    self.CIFName(data.CIFName);
                    if (data.ApplicationId && data.ApplicationId > 0)
                        self.ApplicationId(data.ApplicationId);
                    self.ApplicationNo(data.ApplicationNo);
                    self.AccountTitle(data.AccountTitle);
                    if (data.Id > 0)
                        self.VerificationPersonRole(data.VerificationPersonRole + '');
                    self.Photo(data.Photo ? data.Photo + '' : '1');
                    self.Name(data.Name ? data.Name + '' : '1');
                    self.MobileNo(data.MobileNo ? data.MobileNo + '' : '1');
                    self.ResidencePhone(data.ResidencePhone ? data.ResidencePhone + '' : '1');
                    self.OfficePhone(data.OfficePhone ? data.OfficePhone + '' : '1');
                    self.SignatureOfPhotoId(data.SignatureOfPhotoId ? data.SignatureOfPhotoId + '' : '1');
                    self.SignatureOfApplication(data.SignatureOfApplication ? data.SignatureOfApplication + '' : '1');
                    self.PresentAddress(data.PresentAddress ? data.PresentAddress + '' : '1');
                    self.ResidenceStatus(data.ResidenceStatus ? data.ResidenceStatus + '' : '1');
                    self.MontylyRentAndUtilityExp(data.MontylyRentAndUtilityExp);
                    self.LivingWith(data.LivingWith ? data.LivingWith + '' : '');
                    self.PermanentAddress(data.PermanentAddress ? data.PermanentAddress + '' : '1');
                    self.PersonContacted(data.PersonContacted ? data.PersonContacted : "");
                    self.PhotoName(data.PhotoName);
                    self.SignaturePhotoName(data.SignaturePhotoName);
                    //occupation information
                    self.ProfessionDeclared(data.ProfessionDeclared != null ? data.ProfessionDeclared : "");
                    self.VerificationDateText(data.VerificationDateText);
                    self.NameOfOrganization(data.NameOfOrganization ? data.NameOfOrganization + '' : '1');
                    self.AddressOfOrganization(data.AddressOfOrganization ? data.AddressOfOrganization + '' : '1');
                    self.NatureOfBusiness(data.NatureOfBusiness);
                    self.YearOfEstablishment(data.YearOfEstablishment);
                    self.TradeLicence(data.TradeLicence ? data.TradeLicence + '' : '1');
                    self.SalaryCertificate(data.SalaryCertificate ? data.SalaryCertificate + '' : '1');
                    self.PaySlip(data.PaySlip ? data.PaySlip + '': '1');
                    self.OtherIncomeSource(data.OtherIncomeSource);
                    self.OtherIncomeDeclared(data.OtherIncomeDeclared);
                    self.OtherIncomeVerified(data.OtherIncomeVerified);
                    self.PersonContactedAtOffice(data.PersonContactedAtOffice);
                    self.PersonContactedAtOfficeDetails(data.PersonContactedAtOfficeDetails);
                    self.Remarks(data.Remarks);
                    self.VerificationStatus(data.VerificationStatus + '');
                    $.each(data.BankAccounts, function (index, value) {
                        var aDetail = new bankAccount();
                        if (typeof (value) != 'undefined') {
                            aDetail.LoadData(value);
                            self.BankAccounts.push(aDetail);
                        }

                    });
                });
        }

        self.CpvHistory = function () {
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
                Menu: 'CPV History',
                Url: '/IPDC/Verification/CPVVerificationHistory',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.Submit = function () {
            self.VerificationDateText(moment(self.VerificationDate()).format('DD/MM/YYYY'));
            
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveCpvVerification',
                data: ko.toJSON(self),
                contentType: "application/json",
                success: function (data) {
                    $('#SuccessModal').modal('show');
                    $('#SuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.Save = function () {
            self.Submit();
        }
        self.SaveAs = function () {
            self.Id('');
            self.VerificationPersonRole(self.VerificationPersonRoleFrom());
            self.Submit();
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
                Url: '/IPDC/Verification/CPVVerificationHistory',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.Initialize = function () {
            if (self.CifId() > 0) {
                self.LoadCPVVerificationData();
            } else {
                self.GetLocationFindibility();
                self.GetYearsCurrentResidence();
                self.GetRelationshipWithApplicant();
                self.GetVerificationStatuses();
                self.GetAllProfession();
                self.GetHomeOwnerships();
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


    appvm = new ContactPointVm();
    var qValue = appvm.queryString('AppId');
    appvm.ApplicationId(qValue);
    var cifId = appvm.queryString('CIFPId');
    appvm.CifId(cifId);
    var cpvId = appvm.queryString('Id');
    appvm.VerificationPersonRole(appvm.queryString("VerificationAs"));
    appvm.VerificationPersonRoleFrom(appvm.VerificationPersonRole());
    appvm.Id(cpvId);
    appvm.Initialize();
    ko.applyBindings(appvm, $('#contactPointVerification')[0]);

});



