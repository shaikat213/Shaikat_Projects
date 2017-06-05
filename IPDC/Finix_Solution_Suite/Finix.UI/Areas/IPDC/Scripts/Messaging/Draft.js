$(document).ready(function () {

    function DraftVM() {
        var self = this;
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.Id = ko.observable();
        //self.MessageType = ko.MessageType();
        self.ApplicationId = ko.observable();
        //self.Comment = ko.observable();
        self.Details = function (data) {
            var parameters = [{
                Name: 'msgId',
                Value: data.Id
            },
            {
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'Reply Message',
                //Url: '/IPDC/Messaging/ReplyForwardMessage?msgId=' + Value + '&AppId=' + self.ApplicationId(),
                Url: '/IPDC/Messaging/LoadDraftMessage',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.SetAppId = function (data) {
            self.Id(data.Id);
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

    var aabmvm = new DraftVM();
    ko.applyBindings(aabmvm, document.getElementById("draftvm"));
});