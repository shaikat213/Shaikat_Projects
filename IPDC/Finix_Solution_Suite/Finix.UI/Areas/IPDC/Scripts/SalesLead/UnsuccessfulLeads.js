$(function () {
    $('#FollowUpCallTimeText').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
});
$(document).ready(function () {

    function UnsuccessfulLeadsVM() {
        var self = this;
        self.Id = ko.observable();
        self.CustomerName = ko.observable();
        self.CustomerPhone = ko.observable();
        self.Call_EntyId = ko.observable();
        self.CallStatus = ko.observable();
        self.CallFailReasonList = ko.observableArray([]);
        self.CallFailReason = ko.observable();
        self.CallFailReasonName = ko.observable();
        self.AssignedTo = ko.observable();
        self.AssignedToDegList = ko.observableArray([]);
        self.FollowUpCallTime = ko.observable('');
        self.FollowUpCallTimeText = ko.observable('');
        self.Remarks = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(callInfo);

        self.Details = function (data) {
            var parameters = [{
                Name: 'leadEntryId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 99,
                Menu: 'Lead Edit',
                Url: '/IPDC/SalesLead/SalesLeadEntry',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.IsSuccessful = function() {
            
            return true;

        };

        self.SubmitUnfinished = function () {
            self.CallStatus(0);
            self.Process();
        };
        self.SubmitUnSuccessful = function () {
            self.CallStatus(1);
            self.SubmitCallFail();
        }

        self.setCall_EntyId = function (line) {
            self.Call_EntyId(line.Id);
        }

        self.SubmitSuccessful = function (line) {
            self.CallStatus(2);
            self.Process();
        }

        self.Process = function () {
            self.FollowUpCallTimeText($('#FollowUpCallTimeText').val());
            var submitCallData = {
                Id: self.Call_EntyId(),
                CustomerName: self.CustomerName(),
                CustomerPhone: self.CustomerPhone(),
                FollowUpCallTime: self.FollowUpCallTime(),
                FollowUpCallTimeText: self.FollowUpCallTimeText(),
                CallStatus: self.CallStatus()
            };
            $.ajax({
                url: '/IPDC/Call/SaveCallEntryToLead',
                //cache: false,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitCallData),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.SubmitCallFail = function () {
            //if (self.Call_EntyId() > 0) {
            //
            var submitFaleDate = {
                Id: self.Call_EntyId(),
                CallStatus: self.CallStatus(),
                CallFailReason: self.CallFailReason(),
                AssignedTo: self.AssignedTo(),
                Remarks: self.Remarks()
            }
            $.ajax({
                url: '/IPDC/Call/SaveCallFailReason',
                //cache: false,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitFaleDate),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadCallFailReason = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Call/GetAllCallFailReason',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CallFailReasonList(data); //Put the response in ObservableArray
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

    var vm = new UnsuccessfulLeadsVM();
    var qValue = vm.queryString("callEntryId");
    vm.Call_EntyId(qValue);
    vm.LoadCallFailReason();
    //vm.Search();
    ko.applyBindings(vm, document.getElementById("unsuccessfulLeadsVm"));
});