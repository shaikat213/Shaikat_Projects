$(document).ready(function () {
    var urlId = 1000;
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));

    }

    function ApplicantCIFVerifications(data) {
        var self = this;

        self.ApplicationId = ko.observable();
        self.CIFPId = ko.observable();
        self.CIFOId = ko.observable();
        self.VerifierName = ko.observable(data.VerifierName);
        self.VerificationType = ko.observable(data.VerificationType);
        self.ApplicationNo = ko.observable(data.ApplicationNo);
        self.VerificationAs = ko.observable(data.VerificationAs);
        self.LatestApplicationNo = ko.observable(data.LatestApplicationNo);
        self.VerificationDate = ko.observable(data.VerificationDate);
        self.VerificationDateText = ko.observable(data.VerificationDateText);
        self.LatestVerificationDate = ko.observable(data.LatestVerificationDate);
        self.LatestVerificationDateText = ko.observable(data.LatestVerificationDateText);
        self.Count = ko.observable(data.Count);
        self.VerificationStatus = ko.observable(data.VerificationStatus);
        self.VerificationStatusName = ko.observable(data.VerificationStatusName);
        self.VerificationStatusForThisApplication = ko.observable(data.VerificationStatusForThisApplication);
        self.VerificationStatusForThisApplicationName = ko.observable(data.VerificationStatusForThisApplicationName);
        self.LatestVerificationId = ko.observable(data.LatestVerificationId);
        self.LatestForThisApplicationId = ko.observable(data.LatestForThisApplicationId);
        self.EditUrl = ko.observable(data.EditUrl);

        self.LatestDetails = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'CIFPId',
                Value: self.CIFPId()
            }, {
                Name: 'CIFOId',
                Value: self.CIFOId()
            }, {
                Name: 'Id',
                Value: self.LatestVerificationId()
            }, {
                Name: 'VerificationAs',
                Value: self.VerificationAs()
            }];
            var menuInfo = {
                Id: urlId++,
                Menu: self.VerificationType(),
                Url: self.EditUrl(),
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.Details = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'CIFPId',
                Value: self.CIFPId()
            }, {
                Name: 'CIFOId',
                Value: self.CIFOId()
            }, {
                Name: 'Id',
                Value: self.LatestForThisApplicationId()
            }, {
                Name: 'VerificationAs',
                Value: self.VerificationAs()
            }];
            var menuInfo = {
                Id: urlId++,
                Menu: self.VerificationType(),
                Url: self.EditUrl(),
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }

        self.OrgDetails = function () {
            var parameters = [{
                Name: 'AppId',
                Value: self.ApplicationId()
            }, {
                Name: 'CIFPId',
                Value: self.CIFOrgPId()
            }, {
                Name: 'CIFOId',
                Value: self.CIFOrgOId()
            }, {
                Name: 'Id',
                Value: self.LatestForThisApplicationId()
            }, {
                Name: 'VerificationAs',
                Value: self.VerificationAs()
            }];
            var menuInfo = {
                Id: urlId++,
                Menu: self.VerificationType(),
                Url: self.EditUrl(),
                Parameters: parameters
            }
            window.parent.AddTabFromExternal(menuInfo);
        }
    }

    function ApplicantCifs() {

        var self = this;
        self.VerificationAs = ko.observable()
        self.ApplicationId = ko.observable();
        self.CIFPId = ko.observable();
        self.CIFOId = ko.observable();
        self.VerificationList = ko.observableArray([]);
        self.LoadCIFVerificationHistory = function () {
            var cifType = 0;
            var cifId = 0;
            if (self.ApplicationId() > 0 && self.CIFPId() > 0) {
                cifType = 1;
                cifId = self.CIFPId();
            } else if (self.ApplicationId() > 0 && self.CIFOId() > 0) {
                cifType = 2;
                cifId = self.CIFOId();
            }

            if (cifType > 0) {

                return $.ajax({
                    type: "GET",
                    url: '/IPDC/CRM/GetCIFVerificationHistory?ApplicationId=' + self.ApplicationId() + '&CIFId=' + cifId,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        $.each(data, function (index, value) {
                            var newData = new ApplicantCIFVerifications(value);
                            newData.ApplicationId(self.ApplicationId());
                            if (cifType == 1) {
                                newData.CIFPId(self.CIFPId());
                            }
                            else if (cifType == 2) {
                                newData.CIFOId(self.CIFOId());
                            }
                            newData.VerificationAs(self.VerificationAs());
                            self.VerificationList.push(newData);
                        });
                    },
                    error: function (error) {
                        alert(error.status + "<--and--> " + error.statusText);
                    }
                });
            }
        }
    }

    var appcifs = new ApplicantCifs();
    appcifs.ApplicationId(getParameterByName('AppId'));
    appcifs.CIFPId(getParameterByName('CIFPId'));
    appcifs.CIFOId(getParameterByName('CIFOId'));
    appcifs.VerificationAs(getParameterByName('VerificationAs'));
    appcifs.LoadCIFVerificationHistory();
    ko.applyBindings(appcifs, $('#appcifs')[0]);
});