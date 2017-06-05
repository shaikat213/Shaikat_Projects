$(function () {
    //$('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY HH:mm' });
    $('#ReceivedDateTxt').datetimepicker({ format: 'DD/MM/YYYY', maxDate: moment() });
});
$(document).ready(function () {
    function CaDepositApplicationVm() {
        var self = this;
        self.Id = ko.observable();
        self.test = ko.observable('');
        self.LoadData = ko.observableArray(userInfo);
        self.EntryId = ko.observable();
        self.CurrentHolding = ko.observable();

        self.HardCopyReceived = ko.observable(false);
        self.HardCopyReceiveDateText = ko.observable();
        self.HardCopyReceiveDate = ko.observable();
        self.AmountFormatted = function (amount) {
            if (amount > 0)
                return amount.toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            return amount;
        }
        //self.SetCurrentHolding = function (line) {
        //    self.EntryId(line.Id);
        //    self.SubmitCurrentHolding();
        //}

        //self.SubmitReleseHolding = function (line) {
        //    self.EntryId(line.Id);
        //    self.SubmitRelese();
        //}

        self.SetCurrentHolding = function (line) {
            var submitData = {
                Id: line.Id,
                fromApplicationStage: 6 // Sent to Operations
            };
            $.ajax({
                url: '/IPDC/Operations/SaveCaDepositApplication',
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

        self.SubmitReleseHolding = function (line) {
            self.EntryId(line.Id);
            self.SubmitRelese();
        }

        self.SubmitRelese = function () {
            var submitData = {
                Id: self.EntryId(),
                fromApplicationStage: 7 // Sent to Operations
            };
            $.ajax({
                url: '/IPDC/Operations/SaveReleseApplication',
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

    var aabmvm = new CaDepositApplicationVm();
    var qValue = aabmvm.queryString("entryId");
    aabmvm.EntryId(qValue);
    var appId = aabmvm.queryString("AppId");
    aabmvm.Id(appId);
    ko.applyBindings(aabmvm, document.getElementById("caDepositApplication"));
});