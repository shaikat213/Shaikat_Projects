    $(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function PreparedDisbursmentMemoVm() {
        var self = this;
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.ApprovalDate = ko.observable();
        self.ApprovalDateTxt = ko.observable();
        self.MemoApproval = function (data) {
            
            console.log(data);
            self.Id(data.Id);
            self.ApplicationId(data.ApplicationId);
            
        }
        self.SaveMemoApproval = function () {
            var submitData = {
                Id: self.Id(),
                ApprovalDate: self.ApprovalDate(),
                ApprovalDateTxt: moment(self.ApprovalDate()).format("DD/MM/YYYY"),
                ApplicationId: self.ApplicationId
            }
            return $.ajax({
                type: "POST",
                url: '/IPDC/Operations/SaveMemoApproval',
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
        //self.DisMemo = function (data) {
        //    
        //    console.log(data);
        //    if (data.DmId > 0 || data.ProposalId > 0) {
        //        var parameters = [
        //            {
        //                Name: 'ApplicationId',
        //                Value: data.Id
        //            },
        //            {
        //                Name: 'ProposalId',
        //                Value: data.ProposalId
        //            },
        //            {
        //                Name: 'Id',
        //                Value: data.DmId
        //            }
        //        ];
        //        var menuInfo = {
        //            Id: 93,
        //            Menu: 'Disbursment Memo',
        //            Url: '/IPDC/Operations/DisbursementMemo',
        //            Parameters: parameters
        //        }
        //        window.parent.AddTabFromExternal(menuInfo);
        //    }
        //}
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }
    var aabmvm = new PreparedDisbursmentMemoVm();
    ko.applyBindings(aabmvm, document.getElementById("PreparedDisbursmentMemoVW"));
});