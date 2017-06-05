$(document).ready(function () {
    function OpenedDepositApplicationsVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');
        self.AmountFormatted = function (amount) {
            if (amount > 0)
                return amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            return amount;
        }
        self.LoadData = ko.observableArray(userInfo);
        self.EntryId = ko.observable();
        self.DepositApplicationId = ko.observable();
        self.Comment = ko.observable();
        self.HardCopyReceived = ko.observable(false);
        self.HardCopyReceiveDateText = ko.observable();
        self.HardCopyReceiveDate = ko.observable();

        self.Details = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value:data.Id
            },{
                Name: 'DepAppId',
                Value: data.DepositApplicationId
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'Tracking',
                Url: '/IPDC/Operations/DepositApplicationTracking',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }


        self.setEntyId = function (line) {
            self.EntryId(line.Id);
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

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        
    }

    var opLAvm = new OpenedDepositApplicationsVm();
    var qValue = opLAvm.queryString("entryId");
    opLAvm.EntryId(qValue);
    var appId = opLAvm.queryString("AppId");
    opLAvm.Id(appId);
    var depappId = opLAvm.queryString("DepAppId");
    opLAvm.DepositApplicationId(depappId);
    ko.applyBindings(opLAvm, document.getElementById("openedDepositApplications"));
});