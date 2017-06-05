//$(function () {
//$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
//$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
//});

$(document).ready(function () {
    function PartiallyDisbursedMemoVm() {
        var self = this;
        self.Id  = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.ApplicationId = ko.observable();
        self.DisbursedDate = ko.observable();
        self.DisbursedDateTxt = ko.observable();
        self.RejectionReason = ko.observable();
        self.MemoDetailEntry = function (data) {
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
                        Name: 'MemoId',
                        Value: data.Id
                    }
                ];
                var menuInfo = {
                    Id: 93,
                    Menu: 'Disbursment Memo Details',
                    Url: '/IPDC/Operations/DisbursementMemoDetails',
                    Parameters: parameters
                }
                window.parent.AddTabFromExternal(menuInfo);
            }
        }
        self.CifSummary = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'Application CIF List',
                Url: '/IPDC/Operations/ApplicationCIFList',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.CbsInfo = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                Id: 15,
                Menu: 'CBS Info',
                Url: '/IPDC/Operations/CBSInfo',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.PrintAppSummary = function (line) {
            if (line.ApplicationId > 0) {
                window.open('/IPDC/Operations/AppSummeryReportLoan?reportTypeId=PDF&AppId=' + line.ApplicationId + '&ProposalId=' + line.ProposalId, '_blank');
            }
        };
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