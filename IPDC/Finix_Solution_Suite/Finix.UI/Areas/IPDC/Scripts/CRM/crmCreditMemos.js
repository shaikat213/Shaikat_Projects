
$(document).ready(function () {

    function creditMemo(data) {
        var self = this;
        self.Id = ko.observable(data.Id);
        self.ApplicationNo = ko.observable(data.ApplicationNo);
        self.AccountTitle = ko.observable(data.AccountTitle);
        self.ApplicationReceiveDate = ko.observable(moment(data.ApplicationReceiveDate).format('DD/MM/YYYY'));
        self.ProposalDate = ko.observable(moment(data.ProposalDate).format('DD/MM/YYYY'));
        self.IsApproved = ko.observable(data.IsApproved != null ? data.IsApproved : '');
    }

    function CreditMemos() {
        var self = this;
        self.test = ko.observable('');
        self.CreditMemoList = ko.observableArray();
        $.each(userInfo, function (index, value) {
            
            var memo = new creditMemo(value);
            self.CreditMemoList.push(memo);
        });
        self.Print = function (data) {
            var url = "/IPDC/CRM/CreditMemoReport?reportTypeId=PDF&ProposalId=" + data.Id();
            window.open(url, '_blank');
        }
        self.ApprovalStatus = ko.observable();
        self.ProposalId = ko.observable();
        self.Approve = function (data) {
            self.ApprovalStatus(true);
            self.ProposalId(data.Id);
            self.Submit();
        }
        self.Disapprove = function (data) {
            self.ApprovalStatus(false);
            self.ProposalId(data.Id);
            self.Submit();
        }
        self.Submit = function () {
            var submitData = {
                ProposalId: self.ProposalId(),
                ApprovalStatus: self.ApprovalStatus()
            }
            $.ajax({
                url: '/IPDC/CRM/CreditMemoApproval',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitData),
                success: function (data) {
                    $('#CreditMemoApprovalSuccessModalText').text(data.Message);
                    $('#CreditMemoApprovalSuccessModal').modal('show');
                    
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.Edit = function (data) {
            var parameters = [
                {
                    Name: 'Id',
                    Value: data.Id()
                }];
            var menuInfo = {
                Id: 00,
                Menu: 'Proposal',
                Url: '/IPDC/CRM/Proposal',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.OfferLetter = function (data) {
            var parameters = [
                {
                    Name: 'ProposalId',
                    Value: data.Id()
                }];
            var menuInfo = {
                Id: 00,
                Menu: 'Offer Letter',
                Url: '/IPDC/CRM/OfferLetter',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.Reload = function () {
            window.parent.reloadFrame('frame_116', 0);
        }
        
    }

    var vm = new CreditMemos();
    //vm.Search();
    ko.applyBindings(vm, document.getElementById("crmCreditMemoApprovalVm"));
});