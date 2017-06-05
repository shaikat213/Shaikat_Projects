$(function () {
    //$('#NextFollowUp').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#NextFollowUpEdit').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    //$('#NextFollowUpDetails').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
});
$(document).ready(function () {

    function SearchRejectedApplications() {
        var self = this;
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.ReviveApplication = function (data) {
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
        self.Details = function(data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 66,
                Menu: 'Application Details',
                Url: '/IPDC/Application/ApplicationDetails',
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
    }

    var vm = new SearchRejectedApplications();
    //vm.Search();
    ko.applyBindings(vm, document.getElementById("SearchRejectedApplicationsVm"));
});