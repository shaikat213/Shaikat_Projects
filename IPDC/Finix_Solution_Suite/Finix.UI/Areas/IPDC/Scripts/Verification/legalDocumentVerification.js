var actions = [{ 'Id': 1, 'Name': 'Not Verified' },
                { 'Id': 2, 'Name': 'Ok' },
                { 'Id': 3, 'Name': 'Not Ok' }];
$(document).ready(function () {
    $(function () {
        //$('#OfferLetterDate').datetimepicker({ format: 'DD/MM/YYYY' });
        //$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
    });
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });
    function document(data) {
        var self = this;
        self.Id = ko.observable();
        self.LegalDocumentVerificationId = ko.observable();
        self.LegalDocumentId = ko.observable();
        self.LegalDocumentName = ko.observable();
        self.VerificationStatus = ko.observable();
        self.LoadData = function (data) {
            self.Id(data ? data.Id : null);
            self.LegalDocumentVerificationId(data ? data.LegalDocumentVerificationId : null);
            self.LegalDocumentId(data ? data.LegalDocumentId : null);
            self.LegalDocumentName(data ? data.LegalDocumentName : '');
            self.VerificationStatus(data ? data.VerificationStatus : null);
        }

    };

    function DocumentCheckListVm() {
        var self = this;
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.ApplicationNo = ko.observable();
        self.ApplicationTitle = ko.observable();

        self.LandType = ko.observable();
        self.ProjectId = ko.observable();
        self.LandTypeName = ko.observable();
        //public  List<LegalDocumentStatusDto> LegalDocuments { get; set; }
        self.Documents = ko.observableArray([]);
        self.DocumentStatusList = ko.observableArray(actions);
        //self.Exceptions = ko.observableArray([]);
        //self.ApprovalDate = ko.observable();

        self.Link1 = ko.observable();
        self.Link2 = ko.observable();
        self.Link3 = ko.observable();

        self.Title1 = ko.observable('PDF');
        self.Title2 = ko.observable('Excel');
        self.Title3 = ko.observable('Word');

        self.AddDocuments = function () {
            self.Documents.push(new document());
        }
        self.RemovedDocuments = ko.observableArray([]);
        self.RemoveDocuments = function (line) {
            if (line.Id() > 0)
                self.RemovedDocuments.push(line.Id());
            self.Documents.remove(line);
        }

        //self.setUrl = function () {
        //    console.log(self.Id());
        //    console.log(typeof (self.Id()));
        //    console.log(typeof (self.Id()) != 'undefined');
        //    if ( typeof(self.Id()) != 'undefined') {
        //        window.open('/IPDC/Operations/DCLReport?reportTypeId=PDF&dclId=' + self.Id(), '_blank');
        //    }
        //};
        self.SaveDocCheckList = function () {
            //self.OfferLetterDateTxt($("#OfferLetterDate").val());
            var docListInfo = ko.observableArray([]);
            $.each(self.Documents(),
                function (index, value) {
                    docListInfo.push({
                        Id: value.Id,
                        LegalDocumentVerificationId: value.LegalDocumentVerificationId,
                        LegalDocumentId: value.LegalDocumentId,
                        LegalDocumentName: value.LegalDocumentName,
                        VerificationStatus: value.VerificationStatus
                    });
                });
            var submitData = {
                Id: self.Id(),
                LandType: self.LandType(),
                ProjectId: self.ProjectId(),
                LandTypeName: self.LandTypeName(),
                LegalDocuments: docListInfo,
                RemovedDocuments : self.RemovedDocuments
            }
            return $.ajax({
                type: "POST",
                url: '/IPDC/Verification/SaveLegalDocumentVerification',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#docSuccessModal').modal('show');
                    $('#docSuccessModalText').text(data.Message);
                    self.Id(data.Id);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }


        self.LoadLegalDocumentCheckList = function () {
            if (self.ApplicationId() > 0 || self.Id()>0) {
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/Verification/LoadLegalDocumentCheckList/?AppId=' + self.ApplicationId()+'&id='+self.Id(),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (data) {
                        self.Id(data.Id);
                        self.ApplicationId(data.ApplicationId);
                        self.ApplicationNo(data.ApplicationNo);
                        self.ApplicationTitle(data.ApplicationTitle);
                        self.LandType(data.LandType);
                        self.ProjectId(data.ProjectId);
                        self.LandTypeName(data.LandTypeName);
                        ////self.ApprovalDate(data.ApprovalDate);
                        ////$.when(self.GetDocumentStatusList()).done(function () {
                        if (data.LegalDocuments != null) {
                            $.each(data.LegalDocuments, function (index, value) {
                                var doc = new document();
                                if (typeof (value) != 'undefined') {
                                    doc.LoadData(value);
                                    self.Documents.push(doc);
                                }
                            });
                        }
                    }
                });
            }
        };
        self.Initialize = function () {
           
            if (self.ApplicationId() > 0 || self.Id() > 0) {
                self.LoadLegalDocumentCheckList();
            }
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var oflvm = new DocumentCheckListVm();
    var appId = oflvm.queryString('AppId');
    oflvm.ApplicationId(appId);
    var id = oflvm.queryString('Id');
    oflvm.Id(id);
    oflvm.Initialize();
    ko.applyBindings(oflvm, $('#DocCheckListVW')[0]);
});