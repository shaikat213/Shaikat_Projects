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

    function CIBOrganizationalVm() {
        var self = this;
        self.Id = ko.observable();
        self.CIF_OrgId = ko.observable();
        self.CifOrgNo = ko.observable();
        self.CifOrgName = ko.observable();
        self.ApplicationId = ko.observable();
        //self.CifId = ko.observable(); //
        //self.CifNo = ko.observable();
        //self.CifName = ko.observable();
        self.CifList = ko.observableArray([]);
        self.VerificationDate = ko.observable(); //
        self.VerificationDateTxt = ko.observable();//
        self.VerificationPersonRole = ko.observable();//
        self.VerificationPersonRoleName = ko.observable();//

        self.NoOfLivingContactsAsBorrower = ko.observable(); //
        self.TotalOutstandingAsBorrower = ko.observable();//
        self.ClassifiedAmountAsBorrower = ko.observable();//
        self.TotalEMIAsBorrower = ko.observable();//
        self.CIBClassificationStatusAsBorrower = ko.observable();//
        self.CIBClassificationStatusAsBorrowerName = ko.observable();//

        self.NoOfLivingContactsAsGuarantor = ko.observable();//
        self.TotalOutstandingAsGuarantor = ko.observable();//
        self.ClassifiedAmountAsGuarantor = ko.observable();//
        self.TotalEMIAsGuarantor = ko.observable();//
        self.CIBClassificationStatusAsGuarantor = ko.observable();//
        self.CIBClassificationStatusAsGuarantorName = ko.observable();//
        
        self.CIBClassificationStatusOfDirectors = ko.observable();//
        self.CIBClassificationStatusOfDirectorsName = ko.observable();//

        self.CIBClassificationStatusOfDirectorsConcern = ko.observable();//
        self.CIBClassificationStatusOfDirectorsConcernName = ko.observable();//

        self.CIBClassificationStatusAsBorrowerList = ko.observableArray([]);// //
        self.CIBClassificationStatusAsGuarantorList = ko.observableArray([]);// //
        self.CIBClassificationStatusOfDirectorsList = ko.observableArray([]);// //
        self.CIBClassificationStatusOfDirectorsConcernList = ko.observableArray([]);// //

        //self.GetCIFs = function () {
        //    return $.ajax({
        //        type: "GET",
        //        url: '/IPDC/CIF/GetAllCif',
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (data) {
        //            self.CIF_PersonalList(data); //Put the response in ObservableArray
        //        },
        //        error: function (error) {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}

        self.GetCIBClassificationStatusAsBorrower = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetCIBClassificationStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log(data);
                    self.CIBClassificationStatusAsBorrowerList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetCIBClassificationStatusAsGuarantor = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetCIBClassificationStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log(data);
                    self.CIBClassificationStatusAsGuarantorList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetCIBClassificationStatusOfDirectors = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetCIBClassificationStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CIBClassificationStatusOfDirectorsList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetCIBClassificationStatusOfDirectorsConcern = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetCIBClassificationStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CIBClassificationStatusOfDirectorsConcernList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SaveCIBOrganizational = function() {

            var SubmitData = {
                Id: self.Id(),
                ApplicationId: self.ApplicationId(),
                //CifId: self.CifId(),
                //CifNo: self.CifNo(),
                //CifName:self.CifName(),
                CIF_OrgId: self.CIF_OrgId(),
                CifOrgNo: self.CifOrgNo(),
                CifOrgName:self.CifOrgName(),
                VerificationDate:self.VerificationDate(),
                VerificationDateTxt: moment(self.VerificationDate()).format("DD/MM/YYYY"),
                VerificationPersonRole:self.VerificationPersonRole(),//
                VerificationPersonRoleName: self.VerificationPersonRoleName(),//

                NoOfLivingContactsAsBorrower:self.NoOfLivingContactsAsBorrower(),
                TotalOutstandingAsBorrower:self.TotalOutstandingAsBorrower(),
                ClassifiedAmountAsBorrower:self.ClassifiedAmountAsBorrower(),
                TotalEMIAsBorrower:self.TotalEMIAsBorrower(),
                CIBClassificationStatusAsBorrower:self.CIBClassificationStatusAsBorrower(),
                CIBClassificationStatusAsBorrowerName: self.CIBClassificationStatusAsBorrowerName(),

                NoOfLivingContactsAsGuarantor:self.NoOfLivingContactsAsGuarantor(),
                TotalOutstandingAsGuarantor:self.TotalOutstandingAsGuarantor(),//
                ClassifiedAmountAsGuarantor:self.ClassifiedAmountAsGuarantor(),
                TotalEMIAsGuarantor: self.TotalEMIAsGuarantor(),

                CIBClassificationStatusAsGuarantor:self.CIBClassificationStatusAsGuarantor(),
                CIBClassificationStatusAsGuarantorName:self.CIBClassificationStatusAsGuarantorName(),
                CIBClassificationStatusOfDirectors: self.CIBClassificationStatusOfDirectors(),
                CIBClassificationStatusOfDirectorsName: self.CIBClassificationStatusOfDirectorsName(),
                CIBClassificationStatusOfDirectorsConcern: self.CIBClassificationStatusOfDirectorsConcern(),
                CIBClassificationStatusOfDirectorsConcernName: self.CIBClassificationStatusOfDirectorsConcernName()
            }
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveCIBOrganizational',
                data: ko.toJSON(SubmitData),
                contentType: "application/json",
                success: function (data) {
                    $('#cibSuccessModal').modal('show');
                    $('#cibSuccessModalText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadCIBOrganizational = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/LoadCIBOrganizational/?AppId=' + self.ApplicationId() + '&CifOrgId=' + self.CIF_OrgId() + '&Id=' + self.Id(),
                //url: '/IPDC/Verification/LoadCIBOrganizational/?CibOrgId=' + self.Id(),
                contentType: "application/json",
                dataType:"json",
                success: function (data) {
                    console.log("selected data " + data);
                    self.Id(data.Id);
                    self.ApplicationId(data.ApplicationId);
                    //self.CifId(data.CifId);
                    //self.CifNo(data.CifNo);
                    //self.CifName(data.CifName);
                    self.CIF_OrgId(data.CIF_OrgId);
                    self.CifOrgNo(data.CifOrgNo);
                    self.CifOrgName(data.CifOrgName);
                    self.VerificationDate(data.VerificationDate);
                    self.VerificationDateTxt(data.VerificationDateTxt);
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

                    $.when(self.GetCIBClassificationStatusAsBorrower(),
                        self.GetCIBClassificationStatusAsGuarantor(),
                        self.GetCIBClassificationStatusOfDirectors(),
                        self.GetCIBClassificationStatusOfDirectorsConcern()).done(function () {
                        self.CIBClassificationStatusAsBorrower(data.CIBClassificationStatusAsBorrower);
                        self.CIBClassificationStatusAsBorrowerName(data.CIBClassificationStatusAsBorrowerName);
                        self.CIBClassificationStatusAsGuarantor(data.CIBClassificationStatusAsGuarantor);
                        self.CIBClassificationStatusAsGuarantorName(data.CIBClassificationStatusAsGuarantorName);
                        self.CIBClassificationStatusOfDirectors(data.CIBClassificationStatusOfDirectors);
                        self.CIBClassificationStatusOfDirectorsName(data.CIBClassificationStatusOfDirectorsName);
                        self.CIBClassificationStatusOfDirectorsConcern(data.CIBClassificationStatusOfDirectorsConcern);
                        self.CIBClassificationStatusOfDirectorsConcernName(data.CIBClassificationStatusOfDirectorsConcernName);
                    });
                }
            });
        };

        self.Initialize = function () {
            self.GetCIBClassificationStatusAsBorrower();
            self.GetCIBClassificationStatusOfDirectors();
            self.GetCIBClassificationStatusAsGuarantor();
            self.GetCIBClassificationStatusOfDirectorsConcern();
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    appvm = new CIBOrganizationalVm();
    appvm.Initialize();
    var qValue = appvm.queryString('AppId');
    appvm.ApplicationId(qValue);
    //var cifId = appvm.queryString('CifId');
    //appvm.CifId(cifId);
    var cifOrgId = appvm.queryString('CifOrgId');
    appvm.CIF_OrgId(cifOrgId);
    var selfId = appvm.queryString('Id');
    appvm.Id(selfId);
    appvm.LoadCIBOrganizational();
    ko.applyBindings(appvm, $('#ciborganizational')[0]);

});