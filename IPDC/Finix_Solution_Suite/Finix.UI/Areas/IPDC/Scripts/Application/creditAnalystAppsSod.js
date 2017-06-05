$(document).ready(function () {

    function CreditAnalystAppsVm() {
        var self = this;
        self.Id = ko.observable();
        self.EntryId = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);

        self.Details = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 93,
                Menu: 'Application Details',
                Url: '/IPDC/Application/ApplicationDetailsSOD',
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
                Id: self.EntryId()
            };
            $.ajax({
                url: '/IPDC/Operations/SaveApprovedCreditMemoCurrentHoldings',
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

    var vm = new CreditAnalystAppsVm();
    ko.applyBindings(vm, document.getElementById("creditAnalyst"));
});