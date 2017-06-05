$(function () {
    //$('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#NextFollowUpEdit').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#NextFollowUpDetails').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
});
$(document).ready(function () {

    function SearchCallBMvm() {
        var self = this;
        self.Id = ko.observable();
        self.Call_EntyId = ko.observable();
        self.CallStatus = ko.observable();

        self.CallFailReasonName = ko.observable();
        self.AssignedTo = ko.observable();
        self.AssignedToDegList = ko.observableArray([]);
        //self.AssignedToDeg = ko.observable();
        //self.Remarks = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(callInfo);
        //self.Search = function () {
        //    var data = self.test() ? self.test() : "";

        //    $.ajax({
        //        type: "GET",
        //        url: '/IPDC/CIF/GetClientOrganization?test=' + data,
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //        success: function (data) {
        //            self.LoadData(data);
        //        },
        //        error: function (error) {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}
        self.Details = function (data) {
            var parameters = [{
                Name: 'callEntryId',
                Value: data.Id
            }];
            var menuInfo = {
                //Id: 103,
                Menu: 'Call Details',
                Url: '/IPDC/Call/CallDetailsBM',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        //
        self.SubmitUnSuccessful = function () {
            self.CallStatus(0);
            self.SubmitCallAssign();

        }

        
        self.setCall_EntyId = function (line) {
            self.Call_EntyId(line.Id);
        }

        self.SubmitSuccessful = function (line) {
            self.Call_EntyId(line.Id);
            self.CallStatus(2);
            self.Process();
        }
        self.Process = function () {
            var submitCallData = {
                Id: self.Call_EntyId(),
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
            //}

        }



        self.SubmitCallAssign = function () {
            //if (self.Call_EntyId() > 0) {
            
            var submitAssignedTo = {
                Id: self.Call_EntyId(),
                CallStatus: self.CallStatus(),
                //AssignedTo: self.CallFailReason(),
                AssignedTo: self.AssignedTo()
                //AssignedToDeg: sel.AssignedToDeg()

            }
            $.ajax({
                url: '/IPDC/Call/SaveCallAssigned',
                //cache: false,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitAssignedTo),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
            //}
        }


        
        self.LoadAssignedToDeg = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/OfficeDesignationSetting/GetOffDegSettingsForBMAssignment',
                //url:'/IPDC/Call/GetAllSR',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {                    
                    self.AssignedToDegList(data); //Put the response in ObservableArray
                  
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

    var vm = new SearchCallBMvm();
    
    var qValue = vm.queryString("callEntryId");
    vm.Call_EntyId(qValue);
    vm.LoadAssignedToDeg();
    
    ko.applyBindings(vm, document.getElementById("SearchCallBMVm"));
});