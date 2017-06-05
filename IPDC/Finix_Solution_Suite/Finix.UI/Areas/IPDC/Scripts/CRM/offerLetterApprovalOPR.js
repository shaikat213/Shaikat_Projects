
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
        self.OPSIsApproved = ko.observable(data.OPSIsApproved != null ? data.OPSIsApproved : '');
        self.CUSIsApproved = ko.observable(data.CUSIsApproved != null ? data.CUSIsApproved : '');
    }

    function OfferLetterApprovalOPRVM() {
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
        var SubmitURL;
        self.ApprovalStatus = ko.observable();
        self.OfferLetterId = ko.observable();
        self.OPRApprove = function (data) {
            
            self.ApprovalStatus(true);
            self.OfferLetterId(data.Id);
            SubmitURL = "/IPDC/CRM/OfferLetterApprovalOPS";
            self.Submit();
        }
        self.OPRDisapprove = function (data) {
            
            self.ApprovalStatus(false);
            self.OfferLetterId(data.Id);
            SubmitURL = "/IPDC/CRM/OfferLetterApprovalOPS";
            self.Submit();
        }
        self.CUSApprove = function (data) {
            
            self.ApprovalStatus(true);
            self.OfferLetterId(data.Id);
            SubmitURL = "/IPDC/CRM/OfferLetterApprovalCUS";
            self.Submit();
        }
        self.CUSDisapprove = function (data) {
            
            self.ApprovalStatus(false);
            self.OfferLetterId(data.Id);
            SubmitURL = "/IPDC/CRM/OfferLetterApprovalCUS";
            self.Submit();
        }
        self.Submit = function () {
            
            var submitData = {
                OfferLetterId: self.OfferLetterId(),
                ApprovalStatus: self.ApprovalStatus()
            }
            $.ajax({
                url: SubmitURL,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitData),
                success: function (data) {
                    $('#offerLetterApprovalOPSSuccessModalText').text(data.Message);
                    $('#offerLetterApprovalOPSSuccessModal').modal('show');
                    
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        //self.Edit = function (data) {
        //    var parameters = [
        //        {
        //            Name: 'OfferId',
        //            Value: data.Id()
        //        }];
        //    var menuInfo = {
        //        Id: 1000,
        //        Menu: 'Offer Letter',
        //        Url: '/IPDC/CRM/OfferLetter',
        //        Parameters: parameters
        //    }
        //    window.parent.AddTabFromExternal(menuInfo);
        //}


        self.Reload = function () {
            //window.parent.reloadFrame('frame_118', 0);
        }
        
    }

    var olaOPRvm = new OfferLetterApprovalOPRVM();
    //vm.Search();
    ko.applyBindings(olaOPRvm, document.getElementById("offerLetterApprovalOPSVm"));
});