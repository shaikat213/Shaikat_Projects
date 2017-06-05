$(document).ready(function () {

    function ApplicationApprovalBMVM() {
        var self = this;
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.Comment = ko.observable();
        self.RejectionReason = ko.observable();
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

        self.SetAppId = function (data) {
            self.ApplicationId(data.Id);
        }
        self.ApplicationSendToRM = function (data) {
            $.ajax({
                type: "GET",
                url: '/IPDC/Application/ApplicationSendToRM?ApplicaitonId=' + data.Id,
                contentType: "application/json",
                success: function (serverData) {
                    
                    $('#successModalAppApprovalBM').modal('show');
                    $('#successModalAppApprovalBMText').text(serverData.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }
        self.SubmitApplication = function (data) {
            var submitData = {
                ApplicationId: self.ApplicationId(),
                Comment: self.Comment()
            }
            return $.ajax({
                type: "POST",
                url: '/IPDC/Application/SubmitApplicationToCRM',
                data: ko.toJSON(submitData),
                contentType: "application/json",
                success: function (data) {
                    $('#successModalAppApprovalBM').modal('show');
                    $('#successModalAppApprovalBMText').text(data.Message);
                },
                error: function () {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.CancelApplication = function () {
            var submitData = {
                Id: self.ApplicationId(),
                toApplicationStage: -3, // rejected by bm
                RejectionReason: self.RejectionReason()
            };
            $.ajax({
                url: '/IPDC/Application/CloseApplication',
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

    var aabmvm = new ApplicationApprovalBMVM();
    ko.applyBindings(aabmvm, document.getElementById("ApplicationApprovalBMVM"));
});