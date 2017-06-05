//$(function () {
//$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
//$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
//});
$(document).ready(function () {
    function PartiallyDisbursedMemoVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.ApplicationId = ko.observable();
        self.ApprovalDate = ko.observable();
        self.ApprovalDateTxt = ko.observable();
        self.RejectionReason = ko.observable();

        self.MemoEdit = function (data) {
            if (data.Id > 0 || data.ProposalId > 0) {
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
                        Name: 'Id',
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
        self.MemoApproval = function (data) {
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
        self.SaveDisbursmentMemoApproval = function () {
            var submitData = {
                Id: self.Id(),
                DisbursedDate: self.ApprovalDate(),
                DisbursedDateTxt: moment(self.ApprovalDate()).format("DD/MM/YYYY"),
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
        self.SendMessage = function (data) {
            var parameters = [{
                Name: 'applicationId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'New Message',
                Url: '/IPDC/Messaging/NewMessage',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
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
        //self.SaveDMDetails = function() { 
        //}
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }
    var aabmvm = new PartiallyDisbursedMemoVm();
    ko.applyBindings(aabmvm, document.getElementById("PartiallyDisbursedMemoVW"));
});