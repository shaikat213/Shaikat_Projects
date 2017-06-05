$(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function CaAccountOpeningVm() {
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

        self.AppCifList = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'Fund Confirmation',
                Url: '/IPDC/Operations/ApplicationCIFList',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.CBSInfo = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'Fund Confirmation',
                Url: '/IPDC/Operations/CBSInfo',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }


        self.SetCurrentHolding = function (line) {
            self.EntryId(line.ApplicationId);
            self.SubmitCurrentHolding();
        }

        self.SubmitCurrentHolding = function () {
            var submitData = {
                Id: self.EntryId(),
                fromApplicationStage: 10
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
            //self.EntryId(line.Id);
            self.EntryId(line.ApplicationId);
            self.SubmitRelese();
        }

        self.SubmitRelese = function () {
            var submitData = {
                Id: self.EntryId(),
                fromApplicationStage: 18
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

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new CaAccountOpeningVm();
    var qValue = aabmvm.queryString("entryId");
    aabmvm.EntryId(qValue);
    var appId = aabmvm.queryString("AppId");
    aabmvm.ApplicationId(appId);
    ko.applyBindings(aabmvm, document.getElementById("caAccountOpening"));
});