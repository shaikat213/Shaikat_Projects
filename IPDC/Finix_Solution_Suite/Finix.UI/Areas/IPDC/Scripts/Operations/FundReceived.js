$(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function FundReceivedVm() {
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
        self.AmountFormatted = function (amount) {
            if (amount > 0)
                return amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            return amount;
        }
        //self.Details = function (data) {
        //    //console.log('load data-' + data.ApplicationId);
        //    var parameters = [{
        //        Name: 'AppId',
        //        Value: data.ApplicationId
        //    }];
        //    var menuInfo = {
        //        //Id: 89,
        //        Menu: 'Received Funds',
        //        Url: '/IPDC/Operations/FundConfirmation',
        //        Parameters: parameters
        //    }
        //    window.parent.AddTabFromExternal(menuInfo);
        //}

        self.FullApplication = function (data) {

            var parameters = [{
                Name: 'applicationId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 127,
                Menu: 'Application',
                Url: '/IPDC/Application/ApplicationUneditable',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);

        }

        self.GotoDcl = function (data) {
            //console.log('load data-' + data.ApplicationId);
            var parameters = [{
                Name: 'AppId',
                Value: data.Id
            }];
            //{
            //    Name: 'ProposalId',
            //    Value: data.ProposalId
            //}
            var menuInfo = {
                Id: 89,
                Menu: 'Document Check list',
                Url: '/IPDC/Operations/DocumentCheckList',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }


        self.setEntyId = function (line) {
            self.EntryId(line.Id);
        }

        //self.SubmitHardCopy = function() {
        //    self.HardCopyReceived(true);
        //    self.Process();
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


        self.GotoDclApproval = function (data) {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/SaveDclApproval?id=' + data.DclId + '&appId=' + data.Id,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });

        }

        self.SubmitHardCopy = function () {
            self.HardCopyReceiveDateText($('#ReceivedDateTxt').val());
            var submitHardCopyData = {
                Id: self.EntryId(),
                HardCopyReceived: self.HardCopyReceived(),
                HardCopyReceiveDate: self.HardCopyReceiveDate(),
                HardCopyReceiveDateText: self.HardCopyReceiveDateText()
            };
            $.ajax({
                url: '/IPDC/Operations/SaveOpDepositApplication',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitHardCopyData),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);

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

    var aabmvm = new FundReceivedVm();
    var qValue = aabmvm.queryString("entryId");
    aabmvm.EntryId(qValue);
    var appId = aabmvm.queryString("AppId");
    aabmvm.ApplicationId(appId);
    ko.applyBindings(aabmvm, document.getElementById("opFundReceived"));
});