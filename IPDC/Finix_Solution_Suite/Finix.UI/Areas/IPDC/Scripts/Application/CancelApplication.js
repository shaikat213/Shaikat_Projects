$(document).ready(function () {

    function CanelApplication() {
        var self = this;

        self.Id = ko.observable();
        self.EntryId = ko.observable();
        self.DepositApplicationId = ko.observable();
        self.LoanApplicationId = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.RejectionReason = ko.observable();
        self.Cancel = function(data) {

            self.EntryId(data.Id);
            //self.CancelApplication();
//var parameters = [{
            //    Name: 'applicationId',
            //    Value: data.Id
            //}];
            //var menuInfo = {
            //    Id: 93,
            //    Menu: 'Application',
            //    Url: '/IPDC/Application/Application',
            //    Parameters: parameters
            //}
            //window.parent.AddTabFromExternal(menuInfo);
        }

        self.CancelApplication = function () {
            var submitData = {
                Id: self.EntryId(),
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

    var vm = new CanelApplication();
    //vm.Search();
    ko.applyBindings(vm, document.getElementById("cancelApplication"));
});