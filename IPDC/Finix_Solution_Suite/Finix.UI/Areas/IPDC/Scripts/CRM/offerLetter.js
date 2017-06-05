var accountList = [{ 'Id': 1, 'Name': 'Personal Account' },
                    { 'Id': 2, 'Name': 'Salary Account' },
                    { 'Id': 3, 'Name': 'Business Account' }];
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

    function offerLetterText(data) {
        var self = this;
        self.Id = ko.observable(data ? data.Id : 0);
        self.Text = ko.observable(data ? data.Text : '');
        self.OfferTextType = ko.observable(data ? data.OfferTextType : '');
        self.PrinterFiltering = ko.observable(data ? data.PrinterFiltering : 0);
        self.LoadData = function (data) {
            self.Id(data ? data.Id : 0);
            self.Text(data ? data.Text : '');
            self.OfferTextType(data ? data.OfferTextType : 0);
            self.PrinterFiltering(data ? data.PrinterFiltering : 0);
        }
    }
    function OfferLetterVm() {
        var self = this;
        self.Id = ko.observable();
        self.ProposalId = ko.observable();
        self.ApplicationId = ko.observable();
        self.FacilityType = ko.observable();
        self.FacilityTypeName = ko.observable();
        self.OfferLetterNo = ko.observable();
        self.OfferLetterDate = ko.observable();
        self.OfferLetterDateTxt = ko.observable();
        self.PenalInterest = ko.observable();
        //OfferLetterTexts  = ko.observable();
        self.LoanAdvance = ko.observable();
        self.AcceptancePeriod = ko.observable();
        self.Purpose = ko.observable();
        self.CibAndProcessingFee = ko.observable();
        self.BankAccount = ko.observable();

        self.OfferLetterTextList = ko.observableArray([]);
        self.FacilityTypeList = ko.observableArray([]);
        self.PurposeList = ko.observableArray([]);
        self.OfferTextTypeList = ko.observableArray([]);
        self.PrinterFilteringList = ko.observableArray([]);
        self.ModeOfDisbursmentList = ko.observableArray([]);
        self.DisbursmentConditionList = ko.observableArray([]);
        self.EarlySettlement = ko.observableArray([]);
        self.PartialPayment = ko.observableArray([]);
        self.DocumentationList = ko.observableArray([]);
        self.BankAccountList = ko.observableArray(accountList);

        self.Link1 = ko.observable();
        self.Link2 = ko.observable();
        self.Link3 = ko.observable();

        self.Title1 = ko.observable('PDF');
        self.Title2 = ko.observable('Excel');
        self.Title3 = ko.observable('Word');

        self.GetOfferTextType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetOfferTextType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.OfferTextTypeList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.GetPrinterFiltering = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetPrinterFiltering',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.PrinterFilteringList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        //self.Owners = ko.observableArray([new OwnersLine()]);
        ////self.CIF_PersonalList = ko.observableArray([]);
        self.RemovedOfferLetterTexts = ko.observableArray([]);
        self.AddEarlySettlementText = function () {
            var text = new offerLetterText();
            text.OfferTextType(1);
            self.EarlySettlement.push(text);
        }
        self.RemoveEarlySettlementText = function (line) {
            if (line.Id() > 0)
                self.RemovedOfferLetterTexts.push(line.Id());
            self.EarlySettlement.remove(line);
        }

        self.AddPartialText = function () {
            var text = new offerLetterText();
            text.OfferTextType(2);
            self.PartialPayment.push(text);
        }
        self.RemovePartialText = function (line) {
            if (line.Id() > 0)
                self.RemovedOfferLetterTexts.push(line.Id());
            self.PartialPayment.remove(line);
        }

        self.AddDisbursmentModetext = function () {
            var text = new offerLetterText();
            text.OfferTextType(3);
            self.ModeOfDisbursmentList.push(text);
        }
        self.RemoveDisbursmentModeText = function (line) {
            if (line.Id() > 0)
                self.RemovedOfferLetterTexts.push(line.Id());
            self.ModeOfDisbursmentList.remove(line);
        }

        self.AddDisbursmentCndtext = function () {
            var text = new offerLetterText();
            text.OfferTextType(4);
            self.DisbursmentConditionList.push(text);
        }
        self.RemoveDisbursmentCndText = function (line) {
            if (line.Id() > 0)
                self.RemovedOfferLetterTexts.push(line.Id());
            self.DisbursmentConditionList.remove(line);
        }

        self.AddDocumentationtext = function () {
            var text = new offerLetterText();
            text.OfferTextType(5);
            self.DocumentationList.push(text);
        }
        self.RemoveDocumentationText = function (line) {
            if (line.Id() > 0)
                self.RemovedOfferLetterTexts.push(line.Id());
            self.DocumentationList.remove(line);
        }


        self.GetFacilityType = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetFacilityType',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.FacilityTypeList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.GetPurposes = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/GetPurposes',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.PurposeList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SaveOfferLetter = function () {
            self.OfferLetterDateTxt($("#OfferLetterDate").val());
            var offerTextInfo = ko.observableArray([]);
            $.each(self.EarlySettlement(),
                function (index, value) {
                    offerTextInfo.push({
                        Id: value.Id,
                        Text: value.Text,
                        OfferTextType: value.OfferTextType,
                        PrinterFiltering: value.PrinterFiltering
                    });
                });
            $.each(self.PartialPayment(),
              function (index, value) {
                  offerTextInfo.push({
                      Id: value.Id,
                      Text: value.Text,
                      OfferTextType: value.OfferTextType,
                      PrinterFiltering: value.PrinterFiltering
                  });
              });
            $.each(self.ModeOfDisbursmentList(),
              function (index, value) {
                  offerTextInfo.push({
                      Id: value.Id,
                      Text: value.Text,
                      OfferTextType: value.OfferTextType,
                      PrinterFiltering: value.PrinterFiltering
                  });
              });
            $.each(self.DisbursmentConditionList(),
              function (index, value) {
                  offerTextInfo.push({
                      Id: value.Id,
                      Text: value.Text,
                      OfferTextType: value.OfferTextType,
                      PrinterFiltering: value.PrinterFiltering
                  });
              });
            //DocumentationList
            $.each(self.DocumentationList(),
            function (index, value) {
                offerTextInfo.push({
                    Id: value.Id,
                    Text: value.Text,
                    OfferTextType: value.OfferTextType,
                    PrinterFiltering: value.PrinterFiltering
                });
            });
            var submitData = {
                Id: self.Id(),
                ProposalId: self.ProposalId(),
                FacilityType: self.FacilityType(),
                BankAccount :self.BankAccount(),
                OfferLetterNo: self.OfferLetterNo(),
                OfferLetterDate: self.OfferLetterDate(),
                OfferLetterDateTxt: self.OfferLetterDateTxt(), //moment(self.OfferLetterDate()).format("DD/MM/YYYY"),
                PenalInterest: self.PenalInterest(),
                OfferLetterTexts: offerTextInfo,
                LoanAdvance: self.LoanAdvance(),
                AcceptancePeriod: self.AcceptancePeriod(),
                Purpose: self.Purpose(),
                CibAndProcessingFee: self.CibAndProcessingFee(),
                RemovedOfferLetterTexts: self.RemovedOfferLetterTexts()
            }
            $.ajax({
                type: "POST",
                url: '/IPDC/CRM/SaveOfferLetter',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#offSuccessModal').modal('show');
                    $('#offSuccessModalText').text(data.Message);
                    console.log(data);
                    if (data.Id > 0) {
                        self.Id(data.Id);
                        self.LoadOfferLetter();
                    }
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SavePrint = function () {
            $.when(self.SaveOfferLetter()).done(function () {
                if (self.FacilityType() > 0 && self.Id() > 0) {
                    if (self.FacilityType() === 2 || self.FacilityType() === 3 || self.FacilityType() === 4) {
                        var url = "/IPDC/CRM/AutoLoanOfferLetterReport?reportTypeId=PDF&OfferLetterId=" + self.Id();
                        window.open(url, '_blank');
                    } else if (self.FacilityType() === 1) {
                        var url = "/IPDC/CRM/HomeLoanOfferLetterReport?reportTypeId=PDF&OfferLetterId=" + self.Id();
                        window.open(url, '_blank');
                    }
                }
            });
        }

        self.setUrl = ko.computed(function () {
            if (self.FacilityType() > 0) {
                if (self.FacilityType() === 2 || self.FacilityType() === 3 || self.FacilityType() === 4) {
                    self.Link1('/IPDC/CRM/AutoLoanOfferLetterReport?reportTypeId=PDF&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
                    self.Link2('/IPDC/CRM/AutoLoanOfferLetterReport?reportTypeId=Excel&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
                    self.Link3('/IPDC/CRM/AutoLoanOfferLetterReport?reportTypeId=Word&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
                }
                else if (self.FacilityType() === 1) {
                    self.Link1('/IPDC/CRM/HomeLoanOfferLetterReport?reportTypeId=PDF&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
                    self.Link2('/IPDC/CRM/HomeLoanOfferLetterReport?reportTypeId=Excel&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
                    self.Link3('/IPDC/CRM/HomeLoanOfferLetterReport?reportTypeId=Word&ProposalId=' + self.ProposalId() + '&OfferLetterId=' + self.Id());
                }
            }

        });
        
        self.LoadOfferLetter = function () {

            return $.ajax({
                type: "GET",
                url: '/IPDC/CRM/LoadOfferLetter/?proposalId=' + self.ProposalId() + '&id=' + self.Id(),
                contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    self.OfferLetterTextList([]);
                    self.ModeOfDisbursmentList([]);
                    self.DisbursmentConditionList([]);
                    self.EarlySettlement([]);
                    self.PartialPayment([]);
                    self.DocumentationList([]);
                    self.Id(data.Id);
                    self.ProposalId(data.ProposalId);
                    $.when(self.GetFacilityType()).done(function () {
                        self.FacilityType(data.FacilityType);
                    });
                    self.FacilityTypeName(data.FacilityTypeName);
                    self.OfferLetterNo(data.OfferLetterNo);
                    self.OfferLetterDate(data.OfferLetterDate);
                    self.OfferLetterDateTxt(data.OfferLetterDateTxt); //moment(self.OfferLetterDate()).format("DD/MM/YYYY");
                    self.PenalInterest(data.PenalInterest);
                    //self.offerTextInfo;
                    self.LoanAdvance(data.LoanAdvance);
                    self.AcceptancePeriod(data.AcceptancePeriod);
                    $.when(self.GetPurposes()).done(function () {
                        self.Purpose(data.Purpose);
                    });
                    self.CibAndProcessingFee(data.CibAndProcessingFee);
                    self.BankAccount(data.BankAccount);
                    //self.GetOfferTextType();
                    //self.GetPrinterFiltering();
                    $.when(self.GetOfferTextType()).done(function () {
                        $.when(self.GetPrinterFiltering()).done(function () {
                            if (data.ModeOfDisbursment != null) {
                                $.each(data.ModeOfDisbursment, function (index, value) {
                                    var disbursment = new offerLetterText();
                                    if (typeof (value) != 'undefined') {
                                        disbursment.LoadData(value);
                                        self.ModeOfDisbursmentList.push(disbursment);
                                    }
                                });
                            }
                            if (data.DisbursmentCondition != null) {
                                $.each(data.DisbursmentCondition, function (index, value) {
                                    var disbursment = new offerLetterText();
                                    if (typeof (value) != 'undefined') {
                                        disbursment.LoadData(value);
                                        self.DisbursmentConditionList.push(disbursment);
                                    }
                                });
                            }
                            if (data.OfferLetterTexts != null) {
                                $.each(data.OfferLetterTexts, function (index, value) {
                                    var disbursment = new offerLetterText();
                                    if (typeof (value) != 'undefined') {
                                        disbursment.LoadData(value);
                                        if (value.OfferTextType ===1) {
                                            self.EarlySettlement.push(disbursment);
                                        }
                                        else if (value.OfferTextType === 2) {
                                            self.PartialPayment.push(disbursment);
                                        }
                                        else if (value.OfferTextType === 3) {
                                            self.ModeOfDisbursmentList.push(disbursment);
                                        }
                                        else if (value.OfferTextType === 4) {
                                            self.DisbursmentConditionList.push(disbursment);
                                        }
                                        else if (value.OfferTextType === 5) {
                                            self.DocumentationList.push(disbursment);
                                        }
                                    }
                                });
                            }
                            //DocumentationList
                        });
                    });
                    //$.when(self.GetJointVenturedAgreementandPOA()).done(function () {
                    //    self.JointVenturedAgreement(data.JointVenturedAgreement);
                    //    self.JointVenturedAgreementName(data.JointVenturedAgreementName);
                    //});
                }
            });
        };

        self.Initialize = function () {
            self.GetFacilityType();
            self.GetPurposes();
            self.GetOfferTextType();
            self.GetPrinterFiltering();
            if (self.ProposalId() > 0 || self.Id() > 0) {
                self.LoadOfferLetter();
            }
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var oflvm = new OfferLetterVm();
    var qValue = oflvm.queryString('ProposalId');
    oflvm.ProposalId(qValue);
    var selfId = oflvm.queryString('ApplicationId');
    oflvm.ApplicationId(selfId);
    var offerLetterId = oflvm.queryString('OfferId');
    oflvm.Id(offerLetterId);
    oflvm.Initialize();
    ko.applyBindings(oflvm, $('#OfferLetter')[0]);

});