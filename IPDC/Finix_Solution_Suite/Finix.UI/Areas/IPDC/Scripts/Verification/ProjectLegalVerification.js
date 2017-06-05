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

    function bankAccount(data) {

        var self = this;
        self.Id = ko.observable(data ? data.Id : '');
        self.BankName = ko.observable(data ? data.BankName : '');
        self.AccountNo = ko.observable(data ? data.AccountNo : '');
        self.AccountVerification = ko.observable(data ? data.AccountVerification ? data.AccountVerification.toString() : 1 : 1);

        self.LoadData = function (data) {
            self.Id(data ? data.Id : '');
            self.BankName(data ? data.BankName : '');
            self.AccountNo(data ? data.AccountNo : '');
            self.AccountVerification(data ? data.AccountVerification ? data.AccountVerification.toString() : 1 : 1);
        }
    }

    var OwnersLine = function () {
        var self = this;
        self.Id = ko.observable();
        self.ProjectLegalId = ko.observable();
        self.Name = ko.observable();
        self.TitleDeedNo = ko.observable();
        self.TitleDeedDate = ko.observable();
        self.TitleDeedDateTxt = ko.observable();//

        self.LoadOwners = function (data) {
            self.Id(data.Id);
            self.ProjectLegalId(data.ProjectLegalId);
            self.Name(data.Name);
            self.TitleDeedNo(data.TitleDeedNo);
            self.TitleDeedDate(data.TitleDeedDate);
            self.TitleDeedDateTxt(data.TitleDeedDateTxt);
        }
    };

    function ProjectLegalVerificationVm() {

        var self = this;
        self.Id = ko.observable();
        self.ProjectId = ko.observable();
        self.LandType = ko.observable();
        self.LandTypeName = ko.observable(); //
        self.AreaOfLandTD = ko.observableArray([]);
        self.AreaOfLandTDUomId = ko.observable(); //
        self.IsEncumbered = ko.observable('false');//
        self.JointVenturedAgreement = ko.observable();//
        self.JointVenturedAgreementName = ko.observable();//

        self.POA = ko.observable(); //
        self.POAName = ko.observable();//
        self.ScheduleOfProperty = ko.observable();//
        self.VerificationReport = ko.observable();//
        self.VerifiedByEmpId = ko.observable();//
        self.VerifiedByOffDegId = ko.observable();//

        self.VettingReport = ko.observable();//
        self.VettedBy = ko.observable();//
        self.LegalStageComment = ko.observable();//
        self.LegalApprovalStatus = ko.observable();//
        self.LegalApprovalStatusName = ko.observable();//

        self.LandTypeList = ko.observableArray([]);// //
        self.JointVenturedAgreementList = ko.observableArray([]);// //
        self.POAList = ko.observableArray([]);// //
        self.LegalApprovalStatusList = ko.observableArray([]);// //


        self.Owners = ko.observableArray([new OwnersLine()]);
        //self.CIF_PersonalList = ko.observableArray([]);
        self.AddOwnersLine = function () {
            self.Owners.push(new OwnersLine());
        }
        self.RemovedOwners = ko.observableArray([]);
        self.RemoveOwnersLine = function (line) {
            if (line.Id() > 0)
                self.RemovedOwners.push(line.Id());
            self.Owners.remove(line);
        }
        self.getEditUrl = function () {
            return '/IPDC/Verification/Download?fileBytes=' + self.VerificationReportPath() + '&fileName=' + self.VerificationReportFileName();
        }
        self.getEditUrlVet = function () {
            return '/IPDC/Verification/Download?fileBytes=' + self.VettingReportPath() + '&fileName=' + self.VettingReportFileName();
        }
        self.VerificationReportFile = ko.observable('');
        self.VerificationReportFileName = ko.observable('');
        self.VettingReportFile = ko.observable();
        self.VettingReportFileName = ko.observable();
        self.VerificationReportPath = ko.observable();
        self.VettingReportPath = ko.observable();

        self.GetLandTypes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetLandTypes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    //console.log(data);
                    self.LandTypeList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        //self.GetJointVenturedAgreementandPOA = function () {
        //    return $.ajax({
        //        type: "GET",
        //        url: '/IPDC/Verification/GetDocumentStatusList',
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (data) {
        //            //console.log(data);
        //            self.JointVenturedAgreementList(data);
        //            self.POAList(data);
        //        },
        //        error: function (error) {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}

        self.GetLegalApprovalStatus = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/GetApprovalStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.LegalApprovalStatusList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.IsEdit = ko.observable(false);
        self.Create = function () {
            self.IsEdit(false);
            self.SaveProjectLegalVerification();
        }
        self.Edit = function () {
            //
            self.IsEdit(true);
            self.SaveProjectLegalVerification();
            console.log(self.IsEdit());
        }
        self.FileSubmission = function () {
            var file_data;
            var file_data1;
            if (typeof (self.VerificationReportFile()) != 'undefined') {
                file_data = $('#VerificationReportFile').prop('files')[0];
            }
            if (typeof (self.VettingReportFile()) != 'undefined') {
                file_data1 = $('#VettingReportFile').prop('files')[0];
            }
            var formData = new FormData();
            formData.append('Id', self.Id());
            formData.append('VerificationReportFile', file_data);
            formData.append('VettingReportFile', file_data1);
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveProjectLegalVerification',
                data: formData,
                contentType: false,
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


        self.SaveProjectLegalVerification = function () {
            if (self.IsEdit() === false) {
                self.Id(0);
            }
            var ownersInfo = ko.observableArray([]);

            $.each(self.Owners(),
                function (index, value) {
                    ownersInfo.push({
                        Id: value.Id(),
                        ProjectLegalId: value.ProjectLegalId(),
                        Name: value.Name(),
                        TitleDeedNo: value.TitleDeedNo(),
                        TitleDeedDate: value.TitleDeedDate(),
                        TitleDeedDateTxt: moment(value.TitleDeedDateTxt()).format("DD/MM/YYYY")
                    });
                });


            var SubmitData = {
                Id:self.Id(),
                ProjectId:self.ProjectId(),
                LandType:self.LandType(),
                LandTypeName:self.LandTypeName(),
                AreaOfLandTD:self.AreaOfLandTD(),
                AreaOfLandTDUomId:self.AreaOfLandTDUomId(),
                IsEncumbered:self.IsEncumbered(),
                JointVenturedAgreement:self.JointVenturedAgreement(),
                JointVenturedAgreementName:self.JointVenturedAgreementName(),
                
                POA:self.POA(),
                POAName:self.POAName(),
                ScheduleOfProperty: self.ScheduleOfProperty(),

                Owners: ownersInfo,

                VerificationReport:self.VerificationReport(),
                VerifiedByEmpId:self.VerifiedByEmpId(),
                VerifiedByOffDegId:self.VerifiedByOffDegId(),

                VettingReport:self.VettingReport(),
                VettedBy:self.VettedBy(), //
                LegalStageComment:self.LegalStageComment(),
                LegalApprovalStatus:self.LegalApprovalStatus(),
                LegalApprovalStatusName: self.LegalApprovalStatusName(),
                RemovedOwners: self.RemovedOwners()
            }
            $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveProjectLegalVerification',
                data: ko.toJSON(SubmitData),
                contentType: "application/json",
                success: function (data) {
                    //$('#cibSuccessModal').modal('show');
                    //$('#cibSuccessModalText').text(data.Message);
                    self.Id(data.Id);
                    if (self.Id() >0) {
                        self.FileSubmission();
                    }
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadProjectLegalVerification = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Verification/LoadProjectLegalVerification/?ProjectId=' + self.ProjectId() + '&ProjectLegalId=' + self.Id(),
                contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    self.Owners([]);

                    if (data.Owners.length > 0) {
                        $.each(data.Owners, function (index, value) {
                            var owners = new OwnersLine();
                            if (typeof (value) != 'undefined') {
                                owners.LoadOwners(value);
                                self.Owners.push(owners);
                            }
                        });
                    }

                    //console.log("selected data " + data);
                    self.Id(data.Id);
                    self.ProjectId(data.ProjectId);
                    self.LandType(data.LandType.toString());//.toString());
                    self.LandTypeName(data.LandTypeName);
                    self.AreaOfLandTD(data.AreaOfLandTD);
                    self.AreaOfLandTDUomId(data.AreaOfLandTDUomId);
                    self.IsEncumbered(data.IsEncumbered);
                    self.JointVenturedAgreement(data.JointVenturedAgreement.toString());
                    self.JointVenturedAgreementName(data.JointVenturedAgreementName);

                    self.POA(data.POA.toString());//.toString());
                    self.POAName(data.POAName);
                    self.ScheduleOfProperty(data.ScheduleOfProperty);
                    self.VerificationReportFileName(data.VerificationReportFileName);
                    self.VerificationReport(data.VerificationReport);
                    self.VerifiedByEmpId(data.VerifiedByEmpId);
                    self.VerifiedByOffDegId(data.VerifiedByOffDegId);
                    self.VerificationReportPath(data.VerificationReportPath);
                    self.VettingReportPath(data.VettingReportPath);
                    self.VettingReport(data.VettingReport);
                    self.VettingReportFileName(data.VettingReportFileName);
                    self.VettedBy(data.VettedBy);
                    self.LegalStageComment(data.LegalStageComment);

                    $.when(self.GetLegalApprovalStatus()).done(function () {
                        self.LegalApprovalStatus(data.LegalApprovalStatus);
                        self.LegalApprovalStatusName(data.LegalApprovalStatusName);
                    });
                    
                }
            });
        };

        self.Initialize = function () {
            //self.GetLandTypes();
            //self.GetJointVenturedAgreementandPOA();
            self.GetLegalApprovalStatus();
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    appvm = new ProjectLegalVerificationVm();
    appvm.Initialize();
    var qValue = appvm.queryString('ProjectId');
    appvm.ProjectId(qValue);
    var selfId = appvm.queryString('ProjectLegalId');
    appvm.Id(selfId);
    appvm.LoadProjectLegalVerification();
    ko.applyBindings(appvm, $('#projectlegalverification')[0]);

});