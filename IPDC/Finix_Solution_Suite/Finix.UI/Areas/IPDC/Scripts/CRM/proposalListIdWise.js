$(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function DocumentCheckVm() {
        var self = this;
        self.Id = ko.observable();
        self.LoadData = ko.observableArray(userInfo);
        self.OfferLetterType = ko.observable();
        self.IsApproved = ko.observable(false);
        self.QuotationDate = ko.observable();
        self.QuotationDateTxt = ko.observable();
        self.ProposalId = ko.observable();
        self.ApplicationId = ko.observable();
        self.ApplicationNo = ko.observable();
        self.Link1 = ko.observable();
        self.Title1 = ko.observable('PDF');
        self.Details = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'Received Funds',
                Url: '/IPDC/Operations/FundConfirmation',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }


        self.setEntyId = function (line) {
            //debugger;
            self.Id(line.Id);
            //self.EntryAppId(line.ApplicationId);
            //self.ProductTypeId(line.ProductTypeId);
            self.setUrl();
        }
        self.setUrl = function () {
            var parameters = [{
                Name: 'ProposalId',
                Value: self.ProposalId()
            },
            {
                Name: 'OfferId',
                Value: self.Id()
            }];
            var menuInfo = {
                Id: 93,
                Menu: 'Offer Letter',
                Url: '/IPDC/CRM/OfferLetter',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.setOfferLetter = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.IsApproved(false);
            self.setUrl();
        }
        self.setCrm = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.IsApproved(true);
            self.OfferLetterType(1);
            self.ApplicationId(data.ApplicationId);
            self.ApplicationNo(data.ApplicationNo);
            //self.SaveOfferLetterApproval();
        }
        self.setOpperation = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.IsApproved(true);
            self.OfferLetterType(2);
            self.ApplicationId(data.ApplicationId);
            self.ApplicationNo(data.ApplicationNo);
            //self.SaveOfferLetterApproval();
        }
        self.setCustomer = function (data) {
            self.Id(data.Id);
            self.ProposalId(data.ProposalId);
            self.IsApproved(true);
            self.OfferLetterType(3);
            self.ApplicationId(data.ApplicationId);
            self.ApplicationNo(data.ApplicationNo);
            //self.SaveOfferLetterApproval();
        }

        self.SaveOfferLetterApproval = function () {
            var submitData = {
                Id: self.Id(),
                IsApproved: self.IsApproved(),
                QuotationDate: self.QuotationDate(),
                QuotationDateTxt: moment(self.QuotationDate()).format("DD/MM/YYYY"),
                OfferLetterType: self.OfferLetterType(),
                ApplicationId:self.ApplicationId(),
                ApplicationNo:self.ApplicationNo()
            }
            return $.ajax({
                type: "POST",
                url: '/IPDC/CRM/SaveOfferLetterApproval',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    self.Id(data.Id);
                },
                error: function () {
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

    var aabmvm = new DocumentCheckVm();
    //var qValue = aabmvm.queryString("entryId");
    //aabmvm.EntryId(qValue);
    //var appId = aabmvm.queryString("AppId");
    //aabmvm.ApplicationId(appId);
    ko.applyBindings(aabmvm, document.getElementById("DocumentCheckVW"));
});