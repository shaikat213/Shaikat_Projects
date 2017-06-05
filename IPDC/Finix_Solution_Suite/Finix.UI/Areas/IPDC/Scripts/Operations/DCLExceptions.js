var actions = [{ 'Id': 1, 'Name': 'Rectify' },
                { 'Id': 2, 'Name': 'Replace' },
                { 'Id': 3, 'Name': 'Waiver' },
                { 'Id': 4, 'Name': 'Deferral' },
                { 'Id': 5, 'Name': 'Others' },
                 { 'Id': 6, 'Name': 'Obtained' }];
$(document).ready(function () {
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
        self.DocumentSetupId = ko.observable(data ? data.DocumentSetupId : null);
        self.Name = ko.observable(data ? data.Name : "");
        self.IsRequired = ko.observable(data ? data.IsRequired : false);
        self.IsObtained = ko.observable(data ? data.IsObtained : false);
        self.IsObtained.subscribe(function () {
            if (self.IsObtained())
                self.DocumentStatus(2);
            else
                self.DocumentStatus(null);
            return;
        });
        self.DocumentStatus = ko.observable(data ? data.DocumentStatus : 0);
        self.CollectionDate = ko.observable();
        self.CollectionDateTxt = ko.observable(data ? data.CollectionDateTxt : "");
        self.Remarks = ko.observable(data ? data.Remarks : "");

        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.DCLId(data ? data.DCLId : 0);
            self.DocumentSetupId(data ? data.DocumentSetupId : 0);
            self.Name(data ? data.Name : "");
            self.IsRequired(data ? data.IsRequired : false);
            self.IsObtained(data ? data.IsObtained : false);
            self.DocumentStatus(data ? data.DocumentStatus : 0);
            self.CollectionDate(data.CollectionDate ? moment(data.CollectionDate) : moment());//(data ? data.CollectionDate : null);
            self.CollectionDateTxt(data ? data.CollectionDateTxt : "");
            self.Remarks(data ? data.Remarks : "");
        }

        self.IfObtained = ko.computed(function () {

            if (self.DocumentStatus() === 2) {
                self.CollectionDate(currentDate);
            } else {
                self.CollectionDate(' ');
            }
        });
    }

    function exception(data) {
        var self = this;
        self.Id = ko.observable(data ? data.Id : 0);
        self.DCLId = ko.observable(data ? data.DCLId : 0);
        self.Description = ko.observable(data ? data.Description : "");
        self.Justification = ko.observable(data ? data.Justification : "");
        self.CollectionDate = ko.observable(moment());
        self.CollectionDateTxt = ko.observable(data ? data.CollectionDateTxt : "");
        self.Action = ko.observable(data ? data.Action : '')
        .extend({
            required: {
                message: 'Please Select Action'
            }
        });
        self.ObtainedDate = ko.observable(moment());
        self.ObtainedDateTxt = ko.observable(data ? data.ObtainedDateTxt : "");
        self.IfWaiver = ko.observable(data ? data.Action === 3 ? true : false : false);
        self.IfObtained = ko.observable(data ? data.Action === 6 ? true : false : false);
        self.CheckIfWaiver = function () {
            if (self.Action() != 3 && self.Action() != 6) {
                self.IfWaiver(true);
            }
            else if (self.Action() != 3 && self.Action() == 6) {
                self.IfObtained(true);
            }
            else {
                self.IfWaiver(false);
                self.IfObtained(false);
            }
        }

        self.LoadData = function (data) {
            self.Id(data ? data.Id : '');
            self.DCLId(data ? data.DCLId : '');
            self.Description(data ? data.Description : "");
            self.Justification(data ? data.Justification : "");
            self.CollectionDate(data.CollectionDate ? moment(data.CollectionDate) : "");
            self.CollectionDateTxt(data ? data.CollectionDateTxt : "");
            self.ObtainedDate(data.ObtainedDate ? moment(data.ObtainedDate) : "");
            self.Action(data ? data.Action : 0);
        }
        //self.IfEnabled = ko.computed(function () {
        //    if (self.Action() === 3) {
        //        self.CollectionDate('');
        //        return false;
        //    } else {
        //        return true;
        //    }
        //});
    }
    function security(data) {
        var self = this;
        self.Id = ko.observable(data ? data.Id : 0);
        self.DCLId = ko.observable(data ? data.DCLId : 0);
        self.SecurityDescription = ko.observable(data ? data.SecurityDescription : "");
        self.Value = ko.observable(data ? data.Value : "");

        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.DCLId(data ? data.DCLId : 0);
            self.SecurityDescription(data ? data.SecurityDescription : "");
            self.Value(data ? data.Value : "");

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
        //self.Id = ko.observable();
        //self.ProposalId = ko.observable();
        //self.PrinterFilteringList = ko.observableArray([]);
        self.Id = ko.observable();
        self.DCLNo = ko.observable();
        self.DCLDate = ko.observable();
        self.DCLDateTxt = ko.observable();
        self.ApplicationId = ko.observable();
        self.ProposalId = ko.observable();
        self.ApplicationTitle = ko.observable();
        self.ApplicationNo = ko.observable();
        self.FacilityType = ko.observable();
        self.FacilityTypeName = ko.observable();
        self.ProductId = ko.observable();//[ForeignKey("ProductId")]
        self.Term = ko.observable();
        self.IsApproved = ko.observable(false);
        self.Documents = ko.observableArray([]);
        self.Exceptions = ko.observableArray([]);
        self.Securities = ko.observableArray([]);
        self.ActionList = ko.observableArray(actions);

        self.ApprovedByDegId = ko.observable(); //[ForeignKey("ApprovedByDegId")] public  OfficeDesignationSettingDto OfficeDesignationSetting = ko.observable();
        self.ApprovedByEmpId = ko.observable();//[ForeignKey("ApprovedByEmpId")]
        self.ApprovalDate = ko.observable();

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
        //AddSecurities
        self.AddSecurities = function () {
            self.Securities.push(new security());
        }
        self.RemovedSecurities = ko.observableArray([]);
        self.RemoveSecurities = function (line) {
            if (line.Id() > 0)
                self.RemovedSecurities.push(line.Id());
            self.Securities.remove(line);
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
                    self.SignatoryListForProp(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SaveDocCheckList = function () {
            var docListInfo = ko.observableArray([]);
            var excepListInfo = ko.observableArray([]);
            var securityInfo = ko.observableArray([]);
            $.each(self.Documents(),
                function (index, value) {
                    docListInfo.push({
                        Id: value.Id,
                        DCLId: value.DCLId,
                        DocumentSetupId: value.DocumentSetupId,
                        Name: value.Name,
                        IsRequired: value.IsRequired,
                        IsObtained: value.IsObtained,
                        DocumentStatus: value.DocumentStatus,
                        CollectionDate: value.CollectionDate,
                        CollectionDateTxt: moment(value.CollectionDate).format("DD/MM/YYYY"),//moment(value.CollectionDate()).format("DD/MM/YYYY"),
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
                      Action: value.Action,
                      CollectionDate: value.CollectionDate,
                      CollectionDateTxt: moment(value.CollectionDate).format("DD/MM/YYYY"), //moment(value.CollectionDate()).format("DD/MM/YYYY")
                      ObtainedDate: value.CollectionDate,
                      ObtainedDateTxt: moment(value.ObtainedDate).format("DD/MM/YYYY")

                  });
              });
            $.each(self.Securities(),
           function (index, value) {
               securityInfo.push({
                   Id: value.Id,
                   DCLId: value.DCLId,
                   SecurityDescription: value.SecurityDescription,
                   Value: value.Value
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
                DCLDateTxt: moment(self.DCLDate()).format("DD/MM/YYYY"),
                ApplicationId: self.ApplicationId(),
                ProposalId: self.ProposalId(),
                ApplicationTitle: self.ApplicationTitle(),
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
                Securities: securityInfo,
                RemovedSecurities: self.RemovedSecurities(),
                Signatories: signatoryDetails,
                RemovedSignatories: self.RemovedSignatories()
            }
            $.ajax({
                type: "POST",
                url: '/IPDC/Operations/SaveDocumentCheckList',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#loanAppDCLResponseModal').modal('show');
                    $('#loanAppDCLResponseModalText').text(data.Message);
                    if (data.Id > 0) {
                        self.Id(data.Id);
                        self.LoadDocumentCheckList();
                    }
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SavePrint = function () {

            $.when(self.SaveDocCheckList()).done(function () {
                self.setUrl();
            });
        }

        self.setUrl = function () {
            //
            //console.log(self.Id());
            console.log(typeof (self.Id()));
            console.log(typeof (self.Id()) != 'undefined');
            if (typeof (self.Id()) != 'undefined') {
                window.open('/IPDC/Operations/DCLLoanReport?reportTypeId=PDF&dclId=' + self.Id(), '_blank');
            }
        };
        self.LoadException = function () {

            self.Exceptions([]);
            $.each(self.Documents(), function (index, value) {
                if (value.IsRequired() && !value.IsObtained()) {
                    var exc = new exception();
                    exc.DCLId(value.DCLId());
                    exc.Description(value.Name());
                    self.Exceptions.push(exc);
                }
            });
        }
        self.LoadDocumentCheckList = function () {
            if (self.ApplicationId() > 0 || self.Id() > 0) {
                return $.ajax({
                    type: "GET",
                    //url: '/IPDC/Operations/LoadDocumentCheckList/?AppId=' + self.ApplicationId() +'&proposalId=' + self.ProposalId() + '&id=' + self.Id(),
                    url: '/IPDC/Operations/LoadDocumentCheckList/?AppId=' + self.ApplicationId(),
                    contentType: "application/json",
                    dataType: "json",
                    success: function (data) {

                       self.Documents([]);
                        self.Exceptions([]);
                        self.Securities([]);
                        self.SignatoryList([]);
                        self.Id(data.Id);
                        self.DCLNo(data.DCLNo);
                        self.DCLDate(data.DCLDate ? moment(data.DCLDate) : moment());
                        //(data.VerificationDate ? moment(data.VerificationDate) : moment());
                        self.ApplicationId(data.ApplicationId);
                        self.ProposalId(data.ProposalId);
                        self.ApplicationTitle(data.ApplicationTitle);
                        self.ApplicationNo(data.ApplicationNo);
                        self.FacilityType(data.FacilityType);
                        self.FacilityTypeName(data.FacilityTypeName);
                        self.ProductId(data.ProductId);
                        self.Term(data.Term);
                        self.IsApproved(data.IsApproved),
                        self.ApprovedByDegId(data.ApprovedByDegId);
                        self.ApprovedByEmpId(data.ApprovedByEmpId);
                        self.ApprovalDate(data.ApprovalDate);
                        //$.when(self.GetDocumentStatusList()).done(function () {
                        if (data.Documents != null) {
                            $.each(data.Documents, function (index, value) {
                                var doc = new document();
                                if (typeof (value) != 'undefined') {
                                    doc.LoadData(value);
                                    self.Documents.push(doc);
                                }
                            });
                        }
                        //});
                        $.each(data.Exceptions, function (index, value) {
                            var excep = new exception();
                            if (typeof (value) != 'undefined') {
                                if (value.Action != 6) {
                                    excep.LoadData(value);
                                    self.Exceptions.push(excep);
                                }
                            }
                        });
                        $.each(data.Securities, function (index, value) {
                            var secrt = new security();
                            if (typeof (value) != 'undefined') {
                                secrt.LoadData(value);
                                self.Securities.push(secrt);
                            }
                        });
                        //
                        console.log(data.Signatories);
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
            if (self.ApplicationId() > 0 || self.Id() > 0) {
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
                    self.DocumentStatusList(data); //Put the response in ObservableArray
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
    var laDclVm = new DocumentCheckListVm();
    var appId = laDclVm.queryString('ApplicationId');
    laDclVm.ApplicationId(appId);
    var propId = laDclVm.queryString('ProposalId');
    laDclVm.ProposalId(propId);
    var id = laDclVm.queryString('Id');
    laDclVm.Id(id);
    laDclVm.Initialize();
    ko.applyBindings(laDclVm, $('#DocCheckListLoanVW')[0]);

});