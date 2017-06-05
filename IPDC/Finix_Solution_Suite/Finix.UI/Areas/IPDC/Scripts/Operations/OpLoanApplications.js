$(document).ready(function () {
    function OpLoanApplicationVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');

        self.LoadData = ko.observableArray(userInfo);
        self.EntryId = ko.observable();
        self.ApplicationId = ko.observable();
        self.Comment = ko.observable();
        self.HardCopyReceived = ko.observable(false);
        self.HardCopyReceiveDateText = ko.observable();
        self.HardCopyReceiveDate = ko.observable();
        self.RejectionReason = ko.observable();
        
        self.AppicationDetails = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 92,
                Menu: 'Application Details',
                Url: '/IPDC/Operations/OperationApproval',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.GotoDcl = function (data) {

            var parameters = [{
                Name: 'ProposalId',
                Value: data.ProposalId
            },
            {
                Name: 'Id',
                Value: data.DclId
            },
            {
                Name: 'ApplicationId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 97,
                Menu: 'DCL',
                Url: '/IPDC/operations/LoanApplicationDCL',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.GotoDclApp = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 98,
                Menu: 'PO Approve',
                Url: '/IPDC/operations/PreparePOandApproval', //'/IPDC/operations/PreparePOandApproval',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.SendMessage = function (data) {
            var parameters = [{
                Name: 'applicationId',
                //Value: data.ApplicationId
                Value: data.Id
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'New Message',
                Url: '/IPDC/Messaging/NewMessage',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.setEntyId = function (line) {
            self.EntryId(line.Id);
        }
        self.CancelApplication = function () {
            var submitData = {
                Id: self.EntryId(),
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
        self.GotoDclApproval = function (data) {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/SaveDclApproval?id=' + data.DclId + '&appId=' + data.Id,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });

        }
        self.SubmitHardCopy = function () {
            self.HardCopyReceiveDateText();
            var submitHardCopyData = {
                Id: self.EntryId(),
                HardCopyReceived: self.HardCopyReceived(),
                HardCopyReceiveDate: self.HardCopyReceiveDate(),
                HardCopyReceiveDateText: self.HardCopyReceiveDateText()
            };
            $.ajax({
                url: '/IPDC/Operations/SaveOpDepositApplication',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitHardCopyData),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SubmitReleseHolding = function (line) {
            var submitData = {
                Id:line.Id,
                fromApplicationStage: 7 // Under Process At Operations
            };
            $.ajax({
                url: '/IPDC/Operations/SaveReleseApplication',
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

    var opLAvm = new OpLoanApplicationVm();
    var qValue = opLAvm.queryString("entryId");
    opLAvm.EntryId(qValue);
    var appId = opLAvm.queryString("AppId");
    opLAvm.ApplicationId(appId);
    ko.applyBindings(opLAvm, document.getElementById("opsLoanApplications"));
});