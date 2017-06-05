var statuses = [{ 'Id': 2, 'Name': 'Obtained' },
                { 'Id': 3, 'Name': 'Deferred' },
                { 'Id': 4, 'Name': 'Waived' }];
$(document).ready(function () {
    $(function () {
        //$('#OfferLetterDate').datetimepicker({ format: 'DD/MM/YYYY' });
        //$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
    });
    //var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });

    function document(data) {
        var self = this;
        var currentDate = (new Date()).toISOString().split('T')[0];
        self.Id = ko.observable(data ? data.Id : 0);
        self.DCLId = ko.observable(data ? data.DCLId : 0);
        self.DocumentSetupId = ko.observable(data ? data.DocumentSetupId : "");
        self.Name = ko.observable(data ? data.Name : "");
        self.IsRequired = ko.observable(data ? data.IsRequired : 0);
        self.DocumentStatus = ko.observable(data ? data.DocumentStatus : 0);
        self.CollectionDate = ko.observable();
        self.CollectionDateTxt = ko.observable();
        self.Remarks = ko.observable(data ? data.Remarks : "");
        self.IfDererred = function ()
        {
            return self.DocumentStatus() === 3 ? true : false;
        };
        self.IfObtained = function () {
            if (self.DocumentStatus() === 3) {
                self.IfDererred(true);
            } else {
                self.IfDererred(false);
            }
            if (self.DocumentStatus() === 2) {
                self.CollectionDate(moment());
            } else {
                self.CollectionDate('');
            }
        };
        
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.DCLId(data ? data.DCLId : 0);
            self.DocumentSetupId(data ? data.DocumentSetupId : "");
            self.Name(data ? data.Name : "");
            self.IsRequired(data ? data.IsRequired : false);
            self.DocumentStatus(data ? data.DocumentStatus : 0);
            if (data.CollectionDate)
                self.CollectionDate(moment(data.CollectionDate));
            else 
                self.CollectionDate('');
            self.Remarks(data ? data.Remarks : "");
        }
        
    };
    function exception(data) {
        var self = this;
        self.Id = ko.observable(data ? data.Id : 0);
        self.DCLId = ko.observable(data ? data.DCLId : 0);
        self.Description = ko.observable(data ? data.Description : "");
        self.Justification = ko.observable(data ? data.Justification : "");
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.DCLId(data ? data.DCLId : 0);
            self.Description(data ? data.Description : "");
            self.Justification(data ? data.Justification : "");
        }
    }
    function signatory() {
        var self = this;

        self.Id = ko.observable();
        self.DCLId = ko.observable();
        self.Name = ko.observable();

        self.SignatoryId = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.DCLId(data.DCLId);
            self.Name(data.Name);
            self.SignatoryId(data.SignatoryId);

        }
    }
    function DocumentCheckListVm() {
        var self = this;
        self.Id = ko.observable();
        self.DCLNo = ko.observable();
        self.DCLDate = ko.observable().extend({ required: true });
        self.DCLDateTxt = ko.observable();
        self.ApplicationId = ko.observable();
        self.ProposalId = ko.observable();
        self.AccountTitle = ko.observable();
        self.ApplicationNo = ko.observable();
        self.FacilityType = ko.observable();
        self.FacilityTypeName = ko.observable();
        self.ProductId = ko.observable();
        self.ProductName = ko.observable();
        self.Term = ko.observable();
        self.IsApproved = ko.observable(false);
        self.Documents = ko.observableArray([]);
        self.Exceptions = ko.observableArray([]);
        self.DocumentStatusList = ko.observableArray(statuses);
        self.ApprovedByDegId = ko.observable();
        self.ApprovedByEmpId = ko.observable();
        self.ApprovalDate = ko.observable();
        self.PreparedBy = ko.observable();

        self.Link1 = ko.observable();
        self.Link2 = ko.observable();
        self.Link3 = ko.observable();

        self.Title1 = ko.observable('PDF');
        self.Title2 = ko.observable('Excel');
        self.Title3 = ko.observable('Word');

        self.AddExceptions = function () {
            self.Exceptions.push(new exception());
        }
        self.RemovedExceptions = ko.observableArray([]);
        self.RemoveExceptions = function (line) {
            if (line.Id() > 0)
                self.RemovedExceptions.push(line.Id());
            self.Exceptions.remove(line);
        }

        self.AddDocuments = function () {
            self.Documents.push(new document());
        }
        self.RemovedDocuments = ko.observableArray([]);
        self.RemoveDocuments = function (line) {
            if (line.Id() > 0)
                self.RemovedDocuments.push(line.Id());
            self.Documents.remove(line);
        }
        self.SavePrint = function () {
            $.when(self.SaveDocCheckList()).done(function () {
                self.setUrl();
            });

        }
        self.SignatoryList = ko.observableArray([]);
        self.SignatoryListForProp = ko.observableArray([]);
        self.AddSignatories = function () {
            self.SignatoryList.push(new signatory());
        }
        self.RemovedSignatories = ko.observableArray([]);
        self.RemoveSignatories = function (line) {
            if (line.Id() > 0)
                self.RemovedSignatories.push(line.Id());
            self.SignatoryList.remove(line);
        }
        self.GetAllSignatories = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetAllSignatories',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.SignatoryListForProp(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.setUrl = function () {
            if (typeof (self.Id()) != 'undefined') {
                window.open('/IPDC/Operations/DCLReport?reportTypeId=PDF&dclId=' + self.Id(), '_blank');
            }
        };
        self.errors = ko.validation.group(self);
        self.IsValid = ko.computed(function () {
            if (self.errors().length === 0)
                return true;
            else {
                return false;
            }
        });
        self.SaveDocCheckList = function () {
            self.DCLDateTxt(moment(self.DCLDate()).format('DD/MM/YYYY'));
            var docListInfo = ko.observableArray([]);
            var excepListInfo = ko.observableArray([]);
            var securitiesInfo = ko.observableArray([]);
            $.each(self.Documents(),
                function (index, value) {
                    docListInfo.push({
                        Id: value.Id,
                        DCLId: value.DCLId,
                        DocumentSetupId: value.DocumentSetupId,
                        Name: value.Name,
                        IsRequired: value.IsRequired,
                        DocumentStatus: value.DocumentStatus,
                        CollectionDate: value.CollectionDate,
                        CollectionDateTxt: moment(value.CollectionDate()).format("DD/MM/YYYY"),
                        Remarks: value.Remarks
                    });
                });
            $.each(self.Exceptions(),
              function (index, value) {
                  excepListInfo.push({
                      Id: value.Id,
                      DCLId: value.DCLId,
                      Description: value.Description,
                      Justification: value.Justification,
                  });
              });
            var signatoryDetails = ko.observableArray([]);
            $.each(self.SignatoryList(),
            function (index, value) {
                signatoryDetails.push({
                    Id: value.Id(),
                    DCLId: value.DCLId,
                    Name: value.Name,
                    SignatoryId: value.SignatoryId
                });
            });
            var submitData = {
                Id: self.Id(),
                DCLNo: self.DCLNo(),
                DCLDate: self.DCLDate(),
                DCLDateTxt: self.DCLDateTxt(),
                ApplicationId: self.ApplicationId(),
                ProposalId: self.ProposalId(),
                AccountTitle: self.AccountTitle(),
                ApplicationNo: self.ApplicationNo(),
                FacilityType: self.FacilityType(),
                FacilityTypeName: self.FacilityTypeName(),
                ProductId: self.ProductId(),
                Term: self.Term(),
                IsApproved: self.IsApproved(),
                Documents: docListInfo,
                Exceptions: excepListInfo,
                ApprovedByDegId: self.ApprovedByDegId(),
                ApprovedByEmpId: self.ApprovedByEmpId(),
                ApprovalDate: self.ApprovalDate(),
                RemovedDocuments: self.RemovedDocuments(),
                RemovedExceptions: self.RemovedExceptions(),
                Signatories: signatoryDetails,
                RemovedSignatories: self.RemovedSignatories,
                PreparedBy: self.PreparedBy()
            }
            if (self.IsValid()) {
                return $.ajax({
                    type: "POST",
                    url: '/IPDC/Operations/SaveDocumentCheckList',
                    data: ko.toJSON(submitData),
                    contentType: "application/json",
                    success: function (data) {
                        $('#docSuccessModal').modal('show');
                        $('#docSuccessModalText').text(data.Message);

                        if (data.Id > 0) {
                            self.Id(data.Id);
                            self.LoadDocumentCheckList();
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


        self.LoadDocumentCheckList = function () {
            if (self.ApplicationId() > 0) {
                return $.ajax({
                    type: "GET",
                    url: '/IPDC/Operations/LoadDocumentCheckListForDeposit/?AppId=' + self.ApplicationId(),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (data) {
                        self.Documents([]);
                        self.Exceptions([]);
                        self.SignatoryList([]);
                        self.Id(data.Id);
                        self.DCLNo(data.DCLNo);
                        
                        self.DCLDate(data.DCLDate ? moment(data.DCLDate) : '');
                        self.ApplicationId(data.ApplicationId);
                        self.ProposalId(data.ProposalId);
                        self.AccountTitle(data.AccountTitle);
                        self.ApplicationNo(data.ApplicationNo);
                        self.FacilityType(data.FacilityType);
                        self.FacilityTypeName(data.FacilityTypeName);
                        self.ProductId(data.ProductId);
                        self.ProductName(data.ProductName);
                        self.Term(data.Term);
                        self.IsApproved(data.IsApproved),
                        self.ApprovedByDegId(data.ApprovedByDegId);
                        self.ApprovedByEmpId(data.ApprovedByEmpId);
                        self.ApprovalDate(data.ApprovalDate);
                        self.PreparedBy(data.PreparedBy);
                        if (data.Documents != null) {
                            $.each(data.Documents, function (index, value) {
                                var doc = new document();
                                if (typeof (value) != 'undefined') {
                                    doc.LoadData(value);
                                    console.log("doc load " + ko.toJSON(doc));
                                    self.Documents.push(doc);
                                }
                            });
                        }
                        $.each(data.Exceptions, function (index, value) {
                            var excep = new exception();
                            if (typeof (value) != 'undefined') {
                                excep.LoadData(value);
                                self.Exceptions.push(excep);
                            }
                        });
                        $.when(self.GetAllSignatories()).done(function () {
                            $.each(data.Signatories,
                           function (index, value) {
                               var aDetail = new signatory();
                               if (typeof (value) != 'undefined') {
                                   aDetail.LoadData(value);
                                   self.SignatoryList.push(aDetail);
                               }
                           });
                        });
                    }
                });
            }
        };
        self.Initialize = function () {
            self.GetAllSignatories();
            if (self.ApplicationId() > 0) {
                self.LoadDocumentCheckList();
            }
        }
        self.GetDocumentStatusList = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/GetDocumentStatusList',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.DocumentStatusList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
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