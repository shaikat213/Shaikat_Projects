$(document).ready(function () {
    function CaFundReceivedVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.EntryId = ko.observable();
        self.ApplicationId = ko.observable();
        self.CurrentHolding = ko.observable();

        self.SetCurrentHolding = function (line) {
            //console.log('AppId=' + ko.toJSON(line.ApplicationId));
            self.EntryId(line.ApplicationId);
            self.SubmitCurrentHolding();
        }

        self.SubmitCurrentHolding = function () {
            var submitData = {

                Id: self.EntryId(),
                fromApplicationStage: 7 // under process at operation
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
                Id: self.EntryId()
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
        self.Details = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            },{
                Name: 'DepAppId',
                Value: data.DepositApplicationId
            }];
            var menuInfo = {
                Id: 18,
                Menu: 'Tracking',
                Url: '/IPDC/Operations/DepositApplicationTracking',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new CaFundReceivedVm();
    var qValue = aabmvm.queryString("entryId");
    aabmvm.EntryId(qValue);
    var appId = aabmvm.queryString("AppId");
    aabmvm.ApplicationId(appId);
    ko.applyBindings(aabmvm, document.getElementById("caFundReceived"));
});