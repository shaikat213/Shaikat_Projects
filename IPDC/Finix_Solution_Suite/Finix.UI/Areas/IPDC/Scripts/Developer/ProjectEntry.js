
$(document).ready(function () {

    var ProjectEntryViewModel = function () {
        var self = this;

        self.Id = ko.observable();

        self.DeveloperId = ko.observable();
        self.DeveloperName = ko.observable();
        self.DeveloperList = ko.observableArray([]);

        self.ProjectName = ko.observable();
        self.Area = ko.observable();
        self.TotalSaleableUnits = ko.observable();
        self.TotalSoldUnits = ko.observable();
        self.ProjectAddressId = ko.observable(false);
        self.ProjectAddress = new address();
        self.HandoverDate = ko.observable(moment());
        self.HandoverDateText = ko.observable();
        self.AsOfDate = ko.observable(moment());
        self.AsOfDateText = ko.observable();
        self.DeveloperProjectStatus = ko.observable();
        self.DeveloperProjectStatusName = ko.observable();
        self.DeveloperProjectStatusList = ko.observableArray([]);
        self.ConstructionStage = ko.observable();

        self.CountryList = ko.observableArray([]);
        self.CountryId = ko.observable();

        self.LoadCountry = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Address/GetCountries',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.CountryList(data); //Put the response in ObservableArray
                    self.ProjectAddress.CountryId(1);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadDeveloper = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Developer/GetAllDevelopers',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.DeveloperList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.LoadDeveloperProjectStatus = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Developer/GetDeveloperProjectStatus',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.DeveloperProjectStatusList(data);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }


        self.Submit = function () {
            self.HandoverDateText(moment(self.HandoverDate()).format('DD/MM/YYYY HH:mm'));
            self.AsOfDateText(moment(self.AsOfDate()).format('DD/MM/YYYY HH:mm'));
            var submitProject = {

                Id: self.Id(),

                DeveloperId: self.DeveloperId(),
                DeveloperName: self.DeveloperName(),

                ProjectName: self.ProjectName(),
                Area: self.Area(),
                TotalSaleableUnits: self.TotalSaleableUnits(),
                TotalSoldUnits: self.TotalSoldUnits(),
                ProjectAddressId: self.ProjectAddressId(),
                ProjectAddress: self.ProjectAddress,
                HandoverDate: self.HandoverDate(),
                HandoverDateText: self.HandoverDateText(),
                AsOfDate: self.AsOfDate(),
                AsOfDateText: self.AsOfDateText(),

                DeveloperProjectStatus: self.DeveloperProjectStatus(),
                DeveloperProjectStatusName: self.DeveloperProjectStatusName(),
                ConstructionStage: self.ConstructionStage()
            };

            $.ajax({
                url: '/IPDC/Developer/SaveProject',
                type: 'POST',
                contentType: 'application/json',
                data: ko.toJSON(submitProject),

                success: function (data) {
                    $('#successModal').modal('show');
                    $('#successModalText').text(data.Message);
                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        };

        self.LoadProject = function () {
            return $.ajax({
                type: "GET",
                url: '/IPDC/Developer/LoadProject?Id=' + self.Id(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    self.Id(data.Id);

                    $.when(self.LoadDeveloper()).done(function () {
                        self.DeveloperId(data.DeveloperId);
                        self.DeveloperName(data.DeveloperName);
                    });

                    self.ProjectName(data.ProjectName);
                    self.Area(data.Area);
                    self.TotalSaleableUnits(data.TotalSaleableUnits);
                    self.TotalSoldUnits(data.TotalSoldUnits);

                    self.HandoverDate(data.HandoverDate);
                    self.AsOfDate(data.AsOfDate);

                    $.when(self.LoadDeveloperProjectStatus()).done(function () {
                        self.DeveloperProjectStatus(data.DeveloperProjectStatus);
                        self.DeveloperProjectStatusName(data.DeveloperProjectStatusName);
                    });

                    $.when(self.LoadCountry()).done(function () {
                        self.ProjectAddressId(data.ProjectAddressId);
                        self.ProjectAddress.LoadAddress(data.ProjectAddress);
                    });

                    self.ConstructionStage(data.ConstructionStage);

                },
                error: function (error) {
                    alert(error.status + "<--and--> " + error.statusText);
                }
            });
        }

        self.Initializer = function () {
            if (self.Id() > 0) {
                self.LoadProject();
            } else {
                self.LoadDeveloper();
                self.LoadCountry();
                self.LoadDeveloperProjectStatus();
            }
        }

        self.queryString = function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

    };

    var deVm = new ProjectEntryViewModel();

    deVm.Id(deVm.queryString("Id"));
    deVm.Initializer();
    ko.applyBindings(deVm, document.getElementById("projectEntry")[0]);

})