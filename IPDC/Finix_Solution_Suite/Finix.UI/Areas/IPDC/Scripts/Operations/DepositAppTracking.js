$(document).ready(function () {
    function DepositAppTrackingVm() {
        var self = this;
        self.Id = ko.observable();
        self.ApplicationId = ko.observable();
        self.ApplicationNo = ko.observable();
        self.AccountTitle = ko.observable();
        self.DepositApplicationId = ko.observable();
        self.MaturityAmount = ko.observable();
        self.MaturityAmountFormatted = ko.pureComputed({
            read: function () {
                if (self.MaturityAmount() > 0)
                    return self.MaturityAmount().toFixed(2).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
            },
            write: function (value) {
                value = parseFloat(value.replace(/,/g, ""));
                self.MaturityAmount(isNaN(value) ? 0 : value);
            },
            owner: self
        });
        self.InstrumentDeliveryStatus = ko.observable();
        self.InstrumentDeliveryStatusName = ko.observable();
        self.InstrumentDeliveryStatusList = ko.observableArray([]);
        self.WelcomeLetterStatus = ko.observable();
        self.WelcomeLetterStatusName = ko.observable();
        self.WelcomeLetterStatusList = ko.observableArray([]);
        self.ChangeDate = ko.observable();
        self.ChangeDateText = ko.observable();

        self.SaveDepositAppTracking = function () {
            self.ChangeDateText(moment(self.ChangeDate()).format('DD/MM/YYYY'));
            var submitDepositAppTracking = {
                Id: self.Id(),
                ApplicationId: self.ApplicationId(),
                ApplicationNo: self.ApplicationNo(),
                AccountTitle: self.AccountTitle(),
                DepositApplicationId: self.DepositApplicationId(),
                MaturityAmount:self.MaturityAmount(),
                InstrumentDeliveryStatus: self.InstrumentDeliveryStatus(),
                InstrumentDeliveryStatusName:self.InstrumentDeliveryStatusName(),
                WelcomeLetterStatus: self.WelcomeLetterStatus(),
                WelcomeLetterStatusName:self.WelcomeLetterStatusName(),
                ChangeDate: self.ChangeDate(),
                ChangeDateText: self.ChangeDateText()

            };
            $.ajax({
                url: '/IPDC/Operations/SaveDepositAppTracking',
                //cache: false,
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitDepositAppTracking),
                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                    //self.Reset();
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };
        //'&DepAppId=' + self.DepositApplicationId() + '&Id=' + self.Id()
        self.LoadDepositAppTracking = function () {
            
            //if (self.ApplicationId() > 0) {
                $.getJSON("/IPDC/Operations/LoadDepositAppTrackingbyAppId/?AppId=" + self.ApplicationId() + '&DepAppId=' + self.DepositApplicationId(),
                null,
                function (data) {
                    
                    self.Id(data.Id);
                    self.ApplicationId(data.ApplicationId);
                    self.ApplicationNo(data.ApplicationNo);
                    self.AccountTitle(data.AccountTitle);
                    self.DepositApplicationId(data.DepositApplicationId);
                    self.MaturityAmount(data.MaturityAmount);
                    $.when(self.LoadInstrumentDeliveryStatus())
                            .done(function () {
                                self.InstrumentDeliveryStatus(data.InstrumentDeliveryStatus);
                            });
                    

                    $.when(self.LoadWelcomeLetterStatus())
                            .done(function () {
                                self.WelcomeLetterStatus(data.WelcomeLetterStatus);
                            });

                    self.ChangeDate(moment(data.ChangeDate));
                    self.ChangeDateText(data.ChangeDateText);
                });
            //}
        };

        self.LoadInstrumentDeliveryStatus = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/GetInstrumentStateList',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.InstrumentDeliveryStatusList(data); //Put the response in ObservableArray

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.LoadWelcomeLetterStatus = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Operations/GetWelcomeLetterStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.WelcomeLetterStatusList(data); //Put the response in ObservableArray
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.Initialize=function() {
            self.LoadInstrumentDeliveryStatus();
            self.LoadWelcomeLetterStatus();
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }

    var aabmvm = new DepositAppTrackingVm();
    aabmvm.Initialize();

    var appId = aabmvm.queryString("AppId");
    aabmvm.ApplicationId(appId);
    var depappId = aabmvm.queryString("DepAppId");
    aabmvm.DepositApplicationId(depappId);
    var selfId = aabmvm.queryString("Id");
    aabmvm.Id(selfId);
    aabmvm.LoadDepositAppTracking();
    ko.applyBindings(aabmvm, document.getElementById("depositAppTracking"));
});