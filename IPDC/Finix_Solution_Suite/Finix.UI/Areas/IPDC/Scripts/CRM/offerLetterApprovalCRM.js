
$(document).ready(function () {

    function offerLetter(data) {
        var self = this;
        self.Id = ko.observable(data.Id);
        self.ApplicationDate = ko.observable(moment(data.ApplicationDate).format('DD/MM/YYYY'));
        self.ApplicationNo = ko.observable(data.ApplicationNo);
        self.ProposalNo = ko.observable(data.ProposalNo);
        self.OfferLetterNo = ko.observable(data.OfferLetterNo);
        self.ApplicationTitle = ko.observable(data.ApplicationTitle);
        self.OfferLetterDate = ko.observable(moment(data.OfferLetterDate).format('DD/MM/YYYY'));
        self.CRMIsApproved = ko.observable(data.CRMIsApproved != null ? data.CRMIsApproved : '');
    }

    function OfferLetterApprovalVM() {
        var self = this;
        self.test = ko.observable('');
        self.OfferLetterList = ko.observableArray([]);
        $.each(userInfo, function (index, value) {
            
            var memo = new offerLetter(value);
            self.OfferLetterList.push(memo);
        });
        self.Print = function (data) {
            var url = "/IPDC/CRM/OfferLetterReport?reportTypeId=PDF&OfferLetterId=" + data.Id();
            window.open(url, '_blank');
        }
        self.ApprovalStatus = ko.observable();
        self.OfferLetterId = ko.observable();
        self.Approve = function (data) {
            self.ApprovalStatus(true);
            self.OfferLetterId(data.Id);
            self.Submit();
        }
        self.Disapprove = function (data) {
            self.ApprovalStatus(false);
            self.OfferLetterId(data.Id);
            self.Submit();
        }
        self.Submit = function () {
            var submitData = {
                OfferLetterId: self.OfferLetterId(),
                ApprovalStatus: self.ApprovalStatus()
            }
            $.ajax({
                url: '/IPDC/CRM/OfferLetterApprovalCRM',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitData),
                success: function (data) {
                    $('#offerLetterApprovalSuccessModalText').text(data.Message);
                    $('#offerLetterApprovalSuccessModal').modal('show');
                    
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.Edit = function (data) {
            var parameters = [
                {
                    Name: 'OfferId',
                    Value: data.Id()
                }];
            var menuInfo = {
                Id: 1000,
                Menu: 'Offer Letter',
                Url: '/IPDC/CRM/OfferLetter',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }


        self.Reload = function () {
            window.parent.reloadFrame('frame_118', 0);
        }
        
    }

    var olavm = new OfferLetterApprovalVM();
    //vm.Search();
    ko.applyBindings(olavm, document.getElementById("offerLetterApprovalVm"));
});