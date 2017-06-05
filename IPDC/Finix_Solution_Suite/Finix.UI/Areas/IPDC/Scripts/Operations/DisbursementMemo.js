
$(document).ready(function () {
    $(function () {
        $('#OfferLetterDate').datetimepicker({ format: 'DD/MM/YYYY' });

        //$('#NewDateOfBirth').datetimepicker({ format: 'DD/MM/YYYY' });
    });
    //var appvm;
    ko.validation.init({
        errorElementClass: 'has-error',
        errorMessageClass: 'help-block',
        decorateInputElement: true,
        grouping: { deep: true, observable: true }
    });

    function DMText(data) {
        var self = this;
        self.Id = ko.observable(data ? data.Id : 0);
        self.DMId = ko.observable(data ? data.DMId : 0);
        self.DisbursementTextType = ko.observable(data ? data.DisbursementTextType : 0);
        self.DisbursementTextTypeName = ko.observable(data ? data.DisbursementTextTypeName : 0);
        self.Text = ko.observable(data ? data.Text : "");
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.DMId(data ? data.DMId : 0);
            self.DisbursementTextType(data ? data.DisbursementTextType : 0);
            self.DisbursementTextTypeName(data ? data.DisbursementTextTypeName : 0);
            self.Text(data ? data.Text : "");
        }
    }
    function signatory() {
        var self = this;

        self.Id = ko.observable();
        self.DMId = ko.observable();
        self.Name = ko.observable();
        self.SignatoryId = ko.observable();
        self.LoadData = function (data) {
            self.Id(data.Id);
            self.DMId(data.DCLId);
            self.Name(data.Name);
            self.SignatoryId(data.SignatoryId);

        }
    }
    function DisbursmentMemoVm() {
        var self = this;
        self.Id = ko.observable();
        self.DMNo = ko.observable();
        self.DMDate = ko.observable().extend({ required: true });
        self.DMDateTxt = ko.observable();
        self.ApplicationId = ko.observable();
        self.ProposalId = ko.observable();
        self.ParentId = ko.observable();
        self.IsPartial = ko.observable();
        self.TrenchNo = ko.observable();
        self.TotalLoanAmount = ko.observable();
        self.CurrentDisbursementAmount = ko.observable().extend({
            max: {
                params: ko.pureComputed(function () {
                    if (self.TotalLoanAmount() > 0) {
                        var maxlimit = parseFloat(self.TotalLoanAmount() ? self.TotalLoanAmount() : 0);
                        var disbursed = 0;
                        if (self.TotalDisbursedAmount() > 0) {
                            disbursed = parseFloat(self.TotalDisbursedAmount() ? self.TotalDisbursedAmount() : 0);
                            if (self.Id() > 0) {
                                disbursed = parseFloat(self.TotalDisbursedAmountOld()) - parseFloat(self.CurrentDisbursementAmountOld());
                            }
                            maxlimit -= disbursed;
                        }
                        return maxlimit;
                    }
                    return 0;
                }),
                message: "Invalid Amount"
            }

        });
        self.CurrentDisbursementAmountOld = ko.observable();
        self.TotalDisbursedAmount = ko.observable();
        self.TotalDisbursedAmountOld = ko.observable();
        //self.Texts = ko.observable();
        //self.Id = ko.observable();
        self.Texts = ko.observableArray([]);

        self.SecurityTexts = ko.observableArray([]);
        self.AddSecurityText = function () {
            var txt = new DMText();
            txt.DisbursementTextType(1);
            self.SecurityTexts.push(txt);
        }
        self.RemovedTexts = ko.observableArray([]);
        self.RemoveSecurityText = function (line) {
            if (line.Id() > 0)
                self.RemovedTexts.push(line.Id());
            self.SecurityTexts.remove(line);
        }
        self.Disburses = ko.observableArray([]);
        self.AddDisburseTo = function () {
            var txt = new DMText();
            txt.DisbursementTextType(2);
            self.Disburses.push(txt);
        }

        self.RemoveDisburseTo = function (line) {
            if (line.Id() > 0)
                self.RemovedTexts.push(line.Id());
            self.Disburses.remove(line);
        }

        self.Exceptions = ko.observableArray([]);
        self.AddException = function () {
            var txt = new DMText();
            txt.DisbursementTextType(3);
            self.Exceptions.push(txt);
        }
        self.RemoveException = function (line) {
            if (line.Id() > 0)
                self.RemovedTexts.push(line.Id());
            self.Exceptions.remove(line);
        }

        self.DocStatuses = ko.observableArray([]);
        self.AddDocStatus = function () {
            var txt = new DMText();
            txt.DisbursementTextType(4);
            self.DocStatuses.push(txt);
        }
        self.RemoveDocStatus = function (line) {
            if (line.Id() > 0)
                self.RemovedTexts.push(line.Id());
            self.DocStatuses.remove(line);
        }

        self.Recommandations = ko.observableArray([]);
        self.AddRecommand = function () {
            var txt = new DMText();
            txt.DisbursementTextType(5);
            self.Recommandations.push(txt);
        }
        self.Removerecommand = function (line) {
            if (line.Id() > 0)
                self.RemovedTexts.push(line.Id());
            self.Recommandations.remove(line);
        }

        self.Exclosures = ko.observableArray([]);
        self.AddExclosure = function () {
            var txt = new DMText();
            txt.DisbursementTextType(6);
            self.Exclosures.push(txt);
        }
        self.RemoveExclosure = function (line) {
            if (line.Id() > 0)
                self.RemovedTexts.push(line.Id());
            self.Exclosures.remove(line);
        }
        self.SignatoryListForProp = ko.observableArray([]);
        self.AddSignatories = function () {
            self.SignatoryList.push(new signatory());
        }
        self.SignatoryList = ko.observableArray([]);
        self.RemovedSignatories = ko.observableArray([]);
        self.RemoveSignatories = function (line) {
            if (line.Id() > 0)
                self.RemovedSignatories.push(line.Id());
            self.SignatoryList.remove(line);
        }
        self.GetAllSignatories = function () {
            self.SignatoryListForProp([]);
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
        self.errors = ko.validation.group(self);

        self.IsValid = ko.computed(function () {
            if (self.errors().length === 0)
                return true;
            else {
                return false;
            }
        });

        self.Link1 = ko.observable();
        self.Link2 = ko.observable();
        self.Link3 = ko.observable();

        self.Title1 = ko.observable('PDF');
        self.Title2 = ko.observable('Excel');
        self.Title3 = ko.observable('Word');


        //self.AddOfferLetterText = function () {
        //    self.OfferLetterTextList.push(new offerLetterText());
        //}
        //self.RemovedOfferLetterTexts = ko.observableArray([]);
        //self.RemoveOfferLetterText = function (line) {
        //    if (line.Id() > 0)
        //        self.RemovedOfferLetterTexts.push(line.Id());
        //    self.OfferLetterTextList.remove(line);
        //}
        self.PrintMemo = function () {
            if (self.Id() > 0) {
                var url = '/IPDC/Operations/DisbursmentMemoReport?reportTypeId=PDF&proposalId=' + self.ProposalId() + '&appId=' + self.ApplicationId() + '&id=' + self.Id();
                window.open(url, '_blank');
            } else {
                $('#offSuccessModal').modal('show');
                $('#offSuccessModalText').text("Enable To Find Memo");
            }
        }
        self.SaveNew = function () {

            self.Id(0);
            self.SaveMemo();
        }
        self.SaveMemo = function () {

            var text = ko.observableArray([]);
            $.each(self.SecurityTexts(),
                function (index, value) {
                    text.push({
                        Id: value.Id,
                        DMId: value.DMId,
                        DisbursementTextType: value.DisbursementTextType,
                        DisbursementTextTypeName: value.DisbursementTextTypeName,
                        Text: value.Text
                    });
                });
            $.each(self.Exclosures(),
               function (index, value) {
                   text.push({
                       Id: value.Id,
                       DMId: value.DMId,
                       DisbursementTextType: value.DisbursementTextType,
                       DisbursementTextTypeName: value.DisbursementTextTypeName,
                       Text: value.Text
                   });
               });
            $.each(self.Disburses(),
              function (index, value) {
                  text.push({
                      Id: value.Id,
                      DMId: value.DMId,
                      DisbursementTextType: value.DisbursementTextType,
                      DisbursementTextTypeName: value.DisbursementTextTypeName,
                      Text: value.Text
                  });
              });

            $.each(self.Exceptions(),
                function (index, value) {
                    text.push({
                        Id: value.Id,
                        DMId: value.DMId,
                        DisbursementTextType: value.DisbursementTextType,
                        DisbursementTextTypeName: value.DisbursementTextTypeName,
                        Text: value.Text
                    });
                });
            $.each(self.DocStatuses(),
               function (index, value) {
                   text.push({
                       Id: value.Id,
                       DMId: value.DMId,
                       DisbursementTextType: value.DisbursementTextType,
                       DisbursementTextTypeName: value.DisbursementTextTypeName,
                       Text: value.Text
                   });
               });
            $.each(self.Recommandations(),
              function (index, value) {
                  text.push({
                      Id: value.Id,
                      DMId: value.DMId,
                      DisbursementTextType: value.DisbursementTextType,
                      DisbursementTextTypeName: value.DisbursementTextTypeName,
                      Text: value.Text
                  });
              });
            var signatoryDetails = ko.observableArray([]);
            $.each(self.SignatoryList(),
            function (index, value) {
                signatoryDetails.push({
                    Id: value.Id(),
                    DMId: value.DMId,
                    Name: value.Name,
                    SignatoryId: value.SignatoryId
                });
            });//signatoryDetails
            if (self.IsValid()) {
                var submitData = {
                    Id: self.Id(),
                    DMNo: self.DMNo(),
                    DMDate: self.DMDate(),
                    DMDateTxt: moment(self.DMDate()).format("DD/MM/YYYY"),
                    ApplicationId: self.ApplicationId(),
                    ProposalId: self.ProposalId(),
                    ParentId: self.ParentId(),
                    IsPartial: self.IsPartial(),
                    TrenchNo: self.TrenchNo(),
                    TotalLoanAmount: self.TotalLoanAmount(),
                    CurrentDisbursementAmount: self.CurrentDisbursementAmount(),
                    TotalDisbursedAmount: self.TotalDisbursedAmount(),
                    Texts: text,
                    RemovedTexts: self.RemovedTexts,
                    Signatories: signatoryDetails,
                    RemovedSignatories: self.RemovedSignatories
                }
                $.ajax({
                    type: "POST",
                    url: '/IPDC/Operations/SaveDisbursmentMemo',
                    data: ko.toJSON(submitData),
                    contentType: "application/json",
                    success: function (data) {
                        $('#offSuccessModal').modal('show');
                        $('#offSuccessModalText').text(data.Message);
                        self.Id(data.Id);
                        self.LoadDisbursmentMemo();
                    },
                    error: function () {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            } else {
                self.errors.showAllMessages();
            }
        }
        self.SavePrint = function () {
        }
        //self.setUrl = ko.computed(function () {
        //    if (self.FacilityType() > 0) {
        //        if (self.FacilityType() === 2 || self.FacilityType() === 3 || self.FacilityType() === 4) {
        //            self.Link1('/IPDC/CRM/AutoLoanOfferLetterReport?reportTypeId=PDF&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
        //            self.Link2('/IPDC/CRM/AutoLoanOfferLetterReport?reportTypeId=Excel&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
        //            self.Link3('/IPDC/CRM/AutoLoanOfferLetterReport?reportTypeId=Word&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
        //        }
        //        else if (self.FacilityType() === 1) {
        //            self.Link1('/IPDC/CRM/HomeLoanOfferLetterReport?reportTypeId=PDF&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
        //            self.Link2('/IPDC/CRM/HomeLoanOfferLetterReport?reportTypeId=Excel&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
        //            self.Link3('/IPDC/CRM/HomeLoanOfferLetterReport?reportTypeId=Word&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
        //        }
        //    }
        //});

        self.LoadDisbursmentMemo = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/LoadDisbursmentMemo/?proposalId=' + self.ProposalId() + '&appId=' + self.ApplicationId() + '&id=' + self.Id() + '&parentId=' + self.ParentId(),
                contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    self.SecurityTexts([]);
                    self.Disburses([]);
                    self.Exceptions([]);
                    self.DocStatuses([]);
                    self.Recommandations([]);
                    self.Exclosures([]);
                    self.SignatoryList([]);
                    self.Id(data.Id);
                    self.DMNo(data.DMNo);
                    self.DMDate(moment(data.DMDate));
                    self.DMDateTxt(data.DMDateTxt);
                    self.ApplicationId(data.ApplicationId);
                    self.ProposalId(data.ProposalId);
                    self.ParentId(data.ParentId);
                    self.IsPartial(data.IsPartial + "");
                    self.TrenchNo(data.TrenchNo);
                    self.TotalLoanAmount(data.TotalLoanAmount);
                    self.CurrentDisbursementAmount(data.CurrentDisbursementAmount);
                    self.CurrentDisbursementAmountOld(data.CurrentDisbursementAmount);
                    self.TotalDisbursedAmount(data.TotalDisbursedAmount);
                    self.TotalDisbursedAmountOld(data.TotalDisbursedAmount);
                    $.each(data.Texts, function (index, value) {
                        var txt = new DMText();
                        if (typeof (value) != 'undefined') {
                            txt.LoadData(value);
                            if (value.DisbursementTextType === 1) {
                                self.SecurityTexts.push(txt);
                            }
                            if (value.DisbursementTextType === 2) {
                                self.Disburses.push(txt);
                            }
                            if (value.DisbursementTextType === 3) {
                                self.Exceptions.push(txt);
                            }
                            if (value.DisbursementTextType === 4) {
                                self.DocStatuses.push(txt);
                            }
                            if (value.DisbursementTextType === 5) {
                                self.Recommandations.push(txt);
                            }
                            if (value.DisbursementTextType === 6) {
                                self.Exclosures.push(txt);
                            }
                            //self.Texts.push(txt);
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
        };

        self.Initialize = function () {
            self.GetAllSignatories();
            if (self.ProposalId() || self.ApplicationId() || self.Id()) {
                self.LoadDisbursmentMemo();
            }
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var oflvm = new DisbursmentMemoVm();
    var qValue = oflvm.queryString('ProposalId');
    oflvm.ProposalId(qValue);
    var selfId = oflvm.queryString('ApplicationId');
    oflvm.ApplicationId(selfId);
    var id = oflvm.queryString('Id');
    oflvm.Id(id);
    var parentId = oflvm.queryString('parentId');
    oflvm.ParentId(parentId);//ParentId
    oflvm.Initialize();
    ko.applyBindings(oflvm, $('#DisbursmentMemoVW')[0]);

});