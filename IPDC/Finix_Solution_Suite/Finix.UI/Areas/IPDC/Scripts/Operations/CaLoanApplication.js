$(document).ready(function () {
    function CaLoanApplicationVm() {
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

        self.Details = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'Fund Confirmation',
                Url: '/IPDC/Operations/FundConfirmation',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.SendMessage = function (data) {
            var parameters = [{
                Name: 'applicationId',
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


        self.SetCurrentHolding = function (line) {
            self.EntryId(line.Id);
            self.SubmitCurrentHolding();
        }

        self.SubmitCurrentHolding = function () {
            var submitData = {
                Id: self.EntryId(),
                fromApplicationStage: 6 // Sent To Operations.
            };
            $.ajax({
                url: '/IPDC/Operations/SaveCaDepositApplication',
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
        
        self.SubmitReleseHolding = function (line) {
            self.EntryId(line.Id);
            self.SubmitRelese();
        }

        self.SubmitRelese = function () {
            var submitData = {
                Id: self.EntryId(),
                fromApplicaitonStage: 7 // Under Process At Operations
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

    var opLAvm = new CaLoanApplicationVm();
    var qValue = opLAvm.queryString("entryId");
    opLAvm.EntryId(qValue);
    var appId = opLAvm.queryString("AppId");
    opLAvm.ApplicationId(appId);
    ko.applyBindings(opLAvm, document.getElementById("caLoanApplications"));
});