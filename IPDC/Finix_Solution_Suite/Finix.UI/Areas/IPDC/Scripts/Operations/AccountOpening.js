$(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function AccountOpeningVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.EntryId = ko.observable();
        self.EntryAppId = ko.observable();
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
        self.AppCifList = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'Application CIF List',
                Url: '/IPDC/Operations/ApplicationCIFList',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        self.CBSInfo = function (data) {
            var parameters = [{
                Name: 'AppId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                Id: 89,
                Menu: 'CBS Info',
                Url: '/IPDC/Operations/CBSInfo',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.SendMessage = function (data) {
            var parameters = [{
                Name: 'applicationId',
                Value: data.ApplicationId
            }];
            var menuInfo = {
                //Id: 89,
                Menu: 'New Message',
                Url: '/IPDC/Messaging/NewMessage',
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
        //self.CifListAppSummery = function (data) {
        //    var parameters = [{
        //        Name: 'AppId',
        //        Value: data.ApplicationId
        //    }];
        //    var menuInfo = {
        //        Id: 89,
        //        Menu: 'CIF List - Application Summery',
        //        Url: '/IPDC/Operations/CifListAppSummery',
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

        self.SavePrint = function (line) {
            
            self.EntryAppId(line.ApplicationId);
            self.setUrl();

        }

        self.setUrl = function () {
            if (typeof (self.EntryAppId()) != 'undefined') {
                window.open('/IPDC/Operations/AppSummeryReportDeposit?reportTypeId=PDF&AppId=' + self.EntryAppId(), '_blank'); //+ '&cifId=' + self.EntryId(),
            }
        };

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

    var aabmvm = new AccountOpeningVm();
    var qValue = aabmvm.queryString("entryId");
    aabmvm.EntryId(qValue);
    var appId = aabmvm.queryString("AppId");
    aabmvm.ApplicationId(appId);
    ko.applyBindings(aabmvm, document.getElementById("opAccountOpening"));
});