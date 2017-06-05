$(document).ready(function () {

    function OutboxVM() {
        var self = this;
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.ApplicationId = ko.observable();
        //self.Comment = ko.observable();
        self.Details = function (data) {
            var parameters = [{
                Name: 'applicationId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'Application',
                Url: '/IPDC/Application/Application',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.SetAppId = function (data) {
            self.ApplicationId(data.Id);
        }
        //self.SubmitApplication = function (data) {
        //    var submitData = {
        //        ApplicationId: self.ApplicationId(),
        //        Comment: self.Comment()
        //    }
        //    return $.ajax({
        //        type: "POST",
        //        url: '/IPDC/Application/SubmitApplicationToCRM',
        //        data: ko.toJSON(submitData),
        //        contentType: "application/json",
        //        success: function (data) {
        //            $('#successModalAppApprovalBM').modal('show');
        //            $('#successModalAppApprovalBMText').text(data.Message);
        //        },
        //        error: function () {
        //            alert(error.status + "<--and--> " + error.statusText);
        //        }
        //    });
        //}
    }

    var aabmvm = new OutboxVM();
    ko.applyBindings(aabmvm, document.getElementById("outboxvm"));
});