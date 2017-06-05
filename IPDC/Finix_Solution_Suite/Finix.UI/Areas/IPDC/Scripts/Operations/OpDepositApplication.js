$(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function OpDepositApplicationVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.EntryId = ko.observable();
        self.RejectionReason = ko.observable();
        //self.ApplicationId = ko.observable();
        //self.Comment = ko.observable();
        self.HardCopyReceived = ko.observable(false);
        self.HardCopyReceiveDateText = ko.observable();
        self.HardCopyReceiveDate = ko.observable();
        self.AmountFormatted = function (amount) {
            if (amount > 0)
                return amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            return amount;
        }
        self.Details = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.Id
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'Fund Confirmation',
                Url: '/IPDC/Operations/FundConfirmation',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        

        // Has a menu so no need this button

        //self.ReceivedFunds = function (data) {
        //    var parameters = [{
        //        Name: 'AppId',
        //        //Value: data.ApplicationId
        //        Value: data.Id
        //    }];
        //    var menuInfo = {
        //        Id: 89,
        //        Menu: 'Fund Received',
        //        Url: '/IPDC/Operations/FundReceivedList',
        //        Parameters: parameters
        //    }
        //    window.parent.AddTabFromExternal(menuInfo);
        //}


        self.setEntyId = function (line) {
            self.EntryId(line.Id);
        }

        //self.SubmitHardCopy = function() {
        //    self.HardCopyReceived(true);
        //    self.Process();
        //}

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
        self.SubmitReleseHolding = function (data) {
            //console.log(ko.toJSON(line.Id));
            self.EntryId(data.Id);
            self.SubmitRelese();
        }

        self.SubmitRelese = function () {
            //console.log('app id=' + data.Id);
            var submitData = {
                Id: self.EntryId(),
                fromApplicationStage: 7
            };
            $.ajax({
                url: '/IPDC/Operations/SaveReleseApplication',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitData),
                success: function (data) {
                    //console.log('app id=' + data.Id);
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
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

        self.CancelApplication = function () {
            var submitData = {
                Id: self.EntryId(),
                toApplicationStage: -6, // rejected by operations
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

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new OpDepositApplicationVm();
    var qValue = aabmvm.queryString("entryId");
    aabmvm.EntryId(qValue);
    var appId = aabmvm.queryString("AppId");
    aabmvm.Id(appId);
    ko.applyBindings(aabmvm, document.getElementById("opDepositApplication"));
});