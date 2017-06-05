
$(document).ready(function () {

    function OwnCallsVM() {
        var self = this;
        //self.Id = ko.observable();
        //self.CustomerName = ko.observable();
        //self.CustomerPhone = ko.observable();
        //self.Call_EntyId = ko.observable();
        //self.CallStatus = ko.observable();
        //self.CallFailReasonList = ko.observableArray([]);
        //self.CallFailReason = ko.observable();
        //self.CallFailReasonName = ko.observable();
        //self.AssignedTo = ko.observable();
        //self.AssignedToDegList = ko.observableArray([]);
        //self.FollowUpCallTime = ko.observable('');
        //self.FollowUpCallTimeText = ko.observable('');
        //self.Remarks = ko.observable();
        //self.test = ko.observable('');
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(callInfo);

    }

    var vm = new OwnCallsVM();
    ko.applyBindings(vm, document.getElementById("ownCallList"));
});