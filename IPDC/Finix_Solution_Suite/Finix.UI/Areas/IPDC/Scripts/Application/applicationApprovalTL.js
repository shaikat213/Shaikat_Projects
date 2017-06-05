$(document).ready(function () {

    function ApplicationApprovalVM() {
        var self = this;
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.EntryId = ko.observable();
        self.Comment = ko.observable();
        self.RejectionReason = ko.observable();
        self.Details = function(data) {
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
        self.ApplicationSendToRM = function (data) {
            $.ajax({
                type: "GET",
                url: '/IPDC/Application/ApplicationSendToRM?ApplicaitonId=' + data.Id,
                contentType: "application/json",
                success: function (serverData) {
                    
                    $('#successModalAppApprovalTL').modal('show');
                    $('#successModalAppApprovalTLText').text(serverData.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SetAppId = function (data) {
            self.ApplicationId(data.Id);
        }
        self.SubmitApplication = function (data) {
            var submitData = {
                ApplicationId: self.ApplicationId(),
                Comment: self.Comment(),
                IsTL: true
            }
            return $.ajax({
                type: "POST",
                url: '/IPDC/Application/SubmitApplicationToBm',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#successModalAppApprovalTL').modal('show');
                    $('#successModalAppApprovalTLText').text(data.Message);
                    self.Comment('');
                    self.ApplicationId('');
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        //self.Cancel = function (data) {

        //    self.EntryId(data.Id);

        //}
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

        self.CancelApplication = function () {
            var submitData = {
                Id: self.ApplicationId(),
                toApplicationStage: -2, // rejected by tl
                RejectionReason: self.RejectionReason()
            };
            $.ajax({
                url: '/IPDC/Application/CloseApplication',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitData),
                success: function (data) {                
                    $('#successModalAppApprovalTL').modal('show');
                    $('#successModalAppApprovalTLText').text(data.Message);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
    }

    var aavm = new ApplicationApprovalVM();
    ko.applyBindings(aavm, document.getElementById("ApplicationApprovalVM"));
});