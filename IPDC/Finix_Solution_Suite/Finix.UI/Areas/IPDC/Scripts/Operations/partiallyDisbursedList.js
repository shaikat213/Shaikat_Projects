    $(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function PartiallyDisbursedMemoVm() {
        var self = this;
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
         self.DisMemo = function (data) {
            if (data.DmId > 0 || data.ProposalId > 0) {
                var parameters = [
                    {
                        Name: 'ApplicationId',
                        Value: data.ApplicationId
                    },
                    {
                        Name: 'ProposalId',
                        Value: data.ProposalId
                    },
                    {
                        Name: 'parentId',
                        Value: data.Id
                    }
                ];
                var menuInfo = {
                    Id: 93,
                    Menu: 'Disbursment Memo',
                    Url: '/IPDC/Operations/DisbursementMemo',
                    Parameters: parameters
                }
                window.parent.AddTabFromExternal(menuInfo);
            }
         }
         self.SendMessage = function (data) {
             var parameters = [{
                 Name: 'applicationId',
                 Value: data.ApplicationId
             }];
             var menuInfo = {
                 Id: 30,
                 Menu: 'New Message',
                 Url: '/IPDC/Messaging/NewMessage',
                 Parameters: parameters
             }
             window.parent.AddTabFromExternal(menuInfo);
         }
         self.ApplicationId = ko.observable();
         self.RejectionReason = ko.observable();
         self.setId = function (line) {
             self.ApplicationId(line.ApplicationId);
         }
         self.CancelApplication = function (ApplicationId) {
             var submitData = {
                 Id: self.ApplicationId(),
                 toApplicationStage: -6, // rejected by operations
                 RejectionReason: self.RejectionReason()
             };
             $.ajax({
                 url: '/IPDC/Application/CloseApplication',
                 type: 'POST',
                 contentType: 'application/json',
                 data: ko.toJSON(submitData),
                 success: function (data) {
                     $('#successModal').modal('show');
                     $('#successModalText').text(data.Message);

                 },
                 error: function (error) {
                     alert(error.status + "<--and--> " + error.statusText);
                 }
             });
         }
    }
    var aabmvm = new PartiallyDisbursedMemoVm();
    ko.applyBindings(aabmvm, document.getElementById("PartiallyDisbursedMemoVW"));
});